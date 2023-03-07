using Mandara.Database.Query;
using Mandara.Extensions.AppSettings;
using Mandara.Extensions.Option;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NLog;

namespace Mandara.Entities
{
    public class DbTracingConfiguration : DbConfiguration
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();
        public const string DbTracingKey = "traceDb";

        public DbTracingConfiguration()
        {
            if (AppSettings.FlagEnabled(DbTracingKey))
            {
                Logger.Info("Entity log tracing enabled");
                SetDatabaseLogFormatter((context, writeAction) => new DbTracing(context, writeAction));
            }
            else
            {
                Logger.Info("Entity log tracing disabled");
            }
        }

    }

    internal class DbTracing : DatabaseLogFormatter
    {
        private static long _id = 0;
        private static readonly ConcurrentDictionary<DbCommand, (long, Stopwatch)> CmdStartTimes =
            new ConcurrentDictionary<DbCommand, (long, Stopwatch)>();

        public DbTracing(Action<string> writeAction) : base(writeAction)
        {
        }

        public DbTracing(DbContext context, Action<string> writeAction) : base(context, writeAction)
        {
        }

        public override void LogCommand<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            string callerId = (Context as MandaraEntities)?.CallerId;

            if (String.IsNullOrWhiteSpace(callerId))
            {
                DbTraceLogger.LogCommandStart(_id, GetParametersBuilder(command));
            }
            else
            {
                DbTraceLogger.LogCommandStart(callerId, _id, GetParametersBuilder(command));
            }

            Stopwatch cmdTimer = new Stopwatch();

            CmdStartTimes.TryAdd(command, (Interlocked.Increment(ref _id), cmdTimer));
            cmdTimer.Start();
        }

        private static Func<StringBuilder> GetParametersBuilder(DbCommand command)
        {
            return () => command.Parameters.Aggregate(
                new StringBuilder(),
                (paramsRpt, param) => paramsRpt.AppendLine(
                    $"{param.ParameterName} {param.DbType} = {param.Value}; "));
        }

        public override void LogParameter<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext,
            DbParameter parameter)
        {
        }

        public override void LogResult<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (CmdStartTimes.TryRemove(command, out (long, Stopwatch) cmdStart))
            {
                Stopwatch cmdTimer = cmdStart.Item2;

                cmdTimer.Stop();

                string result = null == interceptionContext.Exception ? DbTraceLogger.Success : DbTraceLogger.Failure;
                string callerId = (Context as MandaraEntities)?.CallerId;

                if (String.IsNullOrWhiteSpace(callerId))
                {
                    DbTraceLogger.LogCommandEnd(cmdStart.Item1, cmdTimer.ElapsedMilliseconds, result);
                }
                else
                {
                    DbTraceLogger.LogCommandEnd(callerId, cmdStart.Item1, cmdTimer.ElapsedMilliseconds, result);
                }
            }
        }
    }

    public static class DbParameterCollectionExt
    {
        public static T Aggregate<T>(
            this DbParameterCollection parameters,
            T collector,
            Func<T, DbParameter, T> collectParam)
        {
            IEnumerator paramWalker = parameters.GetEnumerator();

            while (paramWalker.MoveNext())
            {
                collector = collectParam(collector, (DbParameter)paramWalker.Current);
            }

            return collector;
        }
    }
}
