using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using JetBrains.Annotations;
using Mandara.Database.Query;
using Ninject.Extensions.Logging;
using Optional.Collections;

namespace Mandara.ProductGUI.Desks
{
    public class DesksRepository : IDesksRepository, INotifyPropertyChanged
    {
        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();
        private bool _canAccessDesks;

        private ObservableCollection<Desk> _desksData;
        public ObservableCollection<Desk> DesksData
        {
            get => _desksData ?? new ObservableCollection<Desk>();
            private set
            {
                if (null == value || value.Equals(_desksData))
                {
                    return;
                }

                _desksData = value;
                OnPropertyChanged(nameof(DesksData));
            }
        }

        private List<Desk> _actualDesks;
        private readonly object _databaseInteractionLock = new object();

        public DesksRepository()
        {
            DesksData = new ObservableCollection<Desk>(new List<Desk>());
        }

        public bool LoadDesks()
        {
            try
            {
                List<Desk> desksInDb = SqlServerCommandExecution.ReadToList(
                    DesksConfiguration.ConnectionStringName,
                    "SELECT Id, Name FROM dbo.Desk",
                    reader => new Desk((string)reader.GetValue(1), (int)reader.GetValue(0)));

                DesksData = new ObservableCollection<Desk>(desksInDb);
                DesksData.Insert(0, Desk.Default);
                _canAccessDesks = true;
            }
            catch (Exception)
            {
                DesksData = new ObservableCollection<Desk>();
                _canAccessDesks = false;
            }

            return _canAccessDesks;
        }

        public bool CanAccessDesks()
        {
            return _canAccessDesks;
        }

        public DatabaseActionResult Add(Desk newDesk)
        {
            if (newDesk.IsDefault)
            {
                return ReportFailureToSaveDefaultDesk(newDesk, "adding");
            }

            try
            {
                return AddDeskToDb(newDesk);
            }
            catch (SqlException insertError)
            {
                Logger.Error(insertError, "Failed to insert Desk with name '{0}'\n", newDesk.Name);
                return new DatabaseActionResult(false, insertError.Message);
            }
        }

        private static DatabaseActionResult ReportFailureToSaveDefaultDesk(Desk deskToSave, string attemptedAction)
        {
            return new DatabaseActionResult(false, $"Desk '{deskToSave.Name}' (ID {deskToSave.Id}) is invalid for {attemptedAction}.");
        }

        private DatabaseActionResult AddDeskToDb(Desk newDesk)
        {
            lock (_databaseInteractionLock)
            {
                if (DeskExists(newDesk.Name))
                {
                    string existingDesk = $"Desk '{newDesk.Name}' already exists.";

                    Logger.Info(existingDesk);
                    return new DatabaseActionResult(false, existingDesk);
                }

                // For some reason the last key added is returned as a decimal.
                // Note that this implementation is trusting that the database is low usage with few inserts.  Otherwise
                // the returned @@IDENTITY value could belong to some other inserted row if the script isn't executed in
                // a transaction.
                int newId = (int)(decimal)SqlServerCommandExecution.ExecuteScalarQuery(
                    DesksConfiguration.ConnectionStringName,
                    $"insert into Desk (Name) values ('{newDesk.Name}'); select @@IDENTITY");

                Logger.Info("Inserted Desk with name '{0}' and ID '{1}'", newId);

                Desk insertedDesk = new Desk(newDesk.Name, newId);
                DesksData.Add(insertedDesk);
                newDesk.Id = newId;
                return new DatabaseActionResult(true, string.Empty);
            }
        }

        private bool DeskExists(string deskName)
        {
            return DesksData.Any(desk => deskName.Equals(desk.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public DatabaseActionResult Update(Desk changedDesk)
        {
            if (changedDesk.IsDefault)
            {
                return ReportFailureToSaveDefaultDesk(changedDesk, "updating");
            }

            try
            {
                lock (_databaseInteractionLock)
                {
                    if (!DeskExists(changedDesk.Id))
                    {
                        return ReportNonExistentDesk(changedDesk);
                    }

                    return UpdateToDb(changedDesk);
                }
            }
            catch (Exception updError)
            {
                Logger.Error(updError, "Failed to update Desk {0} name to '{1}'\n", changedDesk.Id, changedDesk.Name);
                return new DatabaseActionResult(false, updError.Message);
            }
        }

        private bool DeskExists(int deskId)
        {
            return DesksData.Any(desk => deskId == desk.Id);
        }

        private static DatabaseActionResult ReportNonExistentDesk(Desk desk)
        {
            string nonExistentDesk = string.Format("Cannot update desk with ID {0} - no such desk exists.", desk.Id);

            Logger.Info(nonExistentDesk);
            return new DatabaseActionResult(false, nonExistentDesk);
        }

        private DatabaseActionResult UpdateToDb(Desk changedDesk)
        {
            int rowsChanged = SqlServerCommandExecution.ExecuteNonQuery(
                DesksConfiguration.ConnectionStringName,
                $"update Desk set name = '{changedDesk.Name}' where id = {changedDesk.Id}");

            if (rowsChanged == 1)
            {
                DesksData.First(desk => changedDesk.Id == desk.Id).Name = changedDesk.Name;
                OnPropertyChanged(nameof(DesksData));
                return new DatabaseActionResult(true, string.Empty);
            }

            return new DatabaseActionResult(
                false,
                $"Unknown error saving desk '{changedDesk.Name}' (ID {changedDesk.Id}).");
        }

        public DatabaseActionResult Remove(Desk deskToDelete)
        {
            if (deskToDelete.IsDefault)
            {
                return ReportFailureToSaveDefaultDesk(deskToDelete, "deleting");
            }

            try
            {
                return RemoveDeskFromDb(deskToDelete);
            }
            catch (SqlException deleteError)
            {
                Logger.Error(
                    deleteError,
                    "Failed to delete Desk with name '{0}' and ID {1}\n",
                    deskToDelete.Name,
                    deskToDelete.Id);
                return new DatabaseActionResult(false, deleteError.Message);
            }
        }

        private DatabaseActionResult RemoveDeskFromDb(Desk deskToDelete)
        {
            lock (_databaseInteractionLock)
            {
                if (!DeskExists(deskToDelete.Id))
                {
                    ReportNonExistentDesk(deskToDelete);
                }

                SqlServerCommandExecution.ExecuteNonQuery(
                    DesksConfiguration.ConnectionStringName,
                    $"delete from Desk where Id = {deskToDelete.Id}");

                DesksData.Remove(DesksData.First(desk => deskToDelete.Id == desk.Id));
                return new DatabaseActionResult(true, string.Empty);
            }
        }

        public List<Desk> Desks => _actualDesks ?? CreateActualDeskList();

        private List<Desk> CreateActualDeskList()
        {
            return _actualDesks = DesksData.Where(desk => !desk.IsDefault).Select(desk => new Desk(desk.Name, desk.Id))
                                           .ToList();
        }

        public Desk GetDesk(string name)
        {
            return DesksData.FirstOrNone(desk => string.Compare(desk.Name, name, true) == 0).ValueOr(Desk.Default);
        }

        public int TotalDesks()
        {
            lock (_databaseInteractionLock)

            {
                return DesksData.Count - 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            CreateActualDeskList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}