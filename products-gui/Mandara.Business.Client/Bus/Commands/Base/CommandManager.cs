using Mandara.Common.TaskSchedulers;
using Mandara.Entities.ErrorReporting;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Mandara.Business.Bus.Commands.Base
{
    public class CommandManager : ICommandManager
    {
        private readonly Queue<ICommand> _commandsQueue;
        private readonly object _commandsQueueLock = new object();
        private readonly BackgroundWorker _commandWorker;
        private readonly TaskScheduler _scheduler;
        private readonly ConcurrentDictionary<Guid, ICommand> _storedCommands;
        private readonly IKernel _kernel;
        private ILogger _log;

        public CommandManager()
            : this(IoC.Kernel, IoC.Get<ILoggerFactory>().GetLogger(typeof(BusClient)))
        {
        }

        public CommandManager(IKernel kernel, ILogger log)
        {
            _kernel = kernel;
            _log = log;

            _commandsQueue = new Queue<ICommand>();
            _storedCommands = new ConcurrentDictionary<Guid, ICommand>();

            _scheduler = new LimitedConcurrencyLevelTaskScheduler(10);

            _commandWorker = new BackgroundWorker();
            _commandWorker.WorkerSupportsCancellation = true;
            _commandWorker.DoWork += CommandWorker_DoWork;
            _commandWorker.RunWorkerCompleted += LogErrorAndRestart_RunWorkerCompleted;
        }

        public void Start()
        {
            if (_commandWorker != null)
                _commandWorker.RunWorkerAsync();
        }

        public void Stop()
        {
            if (_commandWorker != null)
            {
                _commandWorker.CancelAsync();

                Monitor.Enter(_commandsQueueLock);
                Monitor.Pulse(_commandsQueueLock);
                Monitor.Exit(_commandsQueueLock);
            }
        }

        public void AddCommand<T>(Action<T> commandInitializer) where T : ICommand
        {
            T command = _kernel.Get<T>();
            commandInitializer(command);
            AddCommand(command);
        }

        public void AddCommand(ICommand command)
        {
            if (command != null)
            {
                Monitor.Enter(_commandsQueueLock);
                try
                {
                    _commandsQueue.Enqueue(command);
                }
                finally
                {
                    Monitor.Pulse(_commandsQueueLock);
                    Monitor.Exit(_commandsQueueLock);
                }
            }
        }

        private void CommandWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker == null)
                return;

            while (!worker.CancellationPending)
            {
                Monitor.Enter(_commandsQueueLock);
                try
                {
                    if (_commandsQueue.Count == 0)
                    {
                        Monitor.Wait(_commandsQueueLock);
                    }

                    var command = _commandsQueue.Count > 0 ? _commandsQueue.Dequeue() : null;
                    if (command == null)
                        continue;

                    Task.Factory.StartNew(
                        () =>
                        {
                            try
                            {
                                command.CommandManager = this;
                                command.Execute();
                            }
                            catch (Exception ex)
                            {
                                ErrorReportingHelper.ReportError(
                                    "Command Manager",
                                    ErrorType.Exception,
                                    "Command encountered an error",
                                    null,
                                    ex,
                                    ErrorLevel.Critical);
                            }
                        },
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        _scheduler);
                }
                finally
                {
                    Monitor.Exit(_commandsQueueLock);
                }
            }
        }


        private void LogErrorAndRestart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            if (worker == null)
                return;

            if (e.Cancelled)
                return;

            if (e.Error != null)
            {
                _log.ErrorException("IRM Server encountered an error.", e.Error);
                worker.RunWorkerAsync();
            }
        }

        public void StoreCommand(ICommand command, Guid commandId)
        {
            if (command == null)
                return;

            if (!_storedCommands.TryAdd(commandId, command))
            {
                // todo: throw error
            }
        }

        public void RunStoredCommand(Guid snapshotId)
        {
            ICommand command;
            if (_storedCommands.TryRemove(snapshotId, out command))
            {
                AddCommand(command);
            }
        }
    }

}