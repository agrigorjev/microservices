using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Trades;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Mandara.Business
{
    /// <summary>
    /// Process trade support alerts. Performs database and cache operations
    /// </summary>
    public class TradeSupportManager
    {
        private readonly object _lockSupport = new object();
        private readonly ConcurrentDictionary<Guid, TradeSupportAlert> _tradeSupportLog;

        public event EventHandler<TradeSupportAlertEventArgs> AlertAdded;
        public event EventHandler<AcknowledgeAlertEventArgs> AlertAcknowledged;
        private bool isReadOnly=false;

        /// <summary>
        /// Constructor
        /// </summary>
        public TradeSupportManager()
        {
            _tradeSupportLog = new ConcurrentDictionary<Guid, TradeSupportAlert>();
            isReadOnly = ConfigurationManager.AppSettings["ReadOnlyDb"] == "true";
        }

        public void UpdateTradeSupportLogMessages()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                lock (_lockSupport)
                {
                    cxt.TradeSupportAlerts
                        .Where(a => a.Submitted >= Since)
                        .ToList()
                        .ForEach(a =>
                        {
                            _tradeSupportLog.AddOrUpdate(a.Id, a, (key, val) => a);
                        });
                }
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(TradeSupportManager));
        }

        /// <summary>
        /// Read config parameter for how old entries to retrieve from db
        /// </summary>
        private static DateTime Since
        {
            get
            {
                string period =
                    ConfigurationManager.AppSettings["RetrieveSupportLogPeriod"] ?? "2";

                int dates;
                if (!int.TryParse(period, out dates))
                    dates = 2;

                return SystemTime.Now().AddDays(-dates);
            }
        }

        /// <summary>
        /// Adds entry to database.
        /// </summary>
        /// <param name="alert">Entry to add</param>
        /// <returns>Entry to add into collection</returns>
        private TradeSupportAlert AddAlert(TradeSupportAlert alert)
        {
            if (alert.Id.Equals(Guid.Empty))
            {
                alert.Id = Guid.NewGuid();
            }

            if (isReadOnly) return alert;

            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                if (cxt.TradeSupportAlerts.SingleOrDefault(a => a.Id == alert.Id) == null)
                {
                    cxt.TradeSupportAlerts.Add(alert);
                }

                cxt.SaveChanges();

                _tradeSupportLog.TryAdd(alert.Id, alert);
            }

            return alert;
        }

        /// <summary>
        /// Update database and internal collection with new acknowledged status of alert
        /// </summary>
        /// <param name="id">Id of alert</param>
        /// <param name="user">Name of user acknowledged</param>
        /// <param name="ip">Ip address of user acknowledged</param>
        /// <returns>true if succeeds, false if fails</returns>
        private bool AcknowledgeAlert(Guid id, string user, string ip)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                TradeSupportAlert originalAlert =
                    cxt.TradeSupportAlerts.SingleOrDefault(a => a.Id == id);

                if (originalAlert != null)
                {
                    originalAlert.IsAcknowledged = true;
                    originalAlert.AcknowledgedAt = SystemTime.Now();
                    originalAlert.AcknowledgedBy = user;
                    originalAlert.AcknowledgedIP = ip;
                    cxt.SaveChanges();

                    lock (_lockSupport)
                    {
                        TradeSupportAlert comparison;
                        _tradeSupportLog.TryGetValue(id, out comparison);
                        _tradeSupportLog.TryUpdate(id, originalAlert, comparison);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get all current not acknowledged alerts (Used for snapshot request)
        /// </summary>
        /// <returns>Not acknowledged alerts</returns>
        public List<TradeSupportAlert> GetActual()
        {
            lock (_lockSupport)
            {
                return _tradeSupportLog.Values
                    .Where(a => !a.IsAcknowledged)
                    .OrderBy(a => a.Submitted)
                    .ToList();
            }
        }

        /// <summary>
        /// Get all not acknowledged alerts in past several dates 
        /// (Used for snapshot request wit huser specified date parameter)
        /// <param name="datesOffset">Number of dates fto retrieve support alerts for</param>
        /// </summary>
        /// <returns>Not acknowledged alerts</returns>
        public List<TradeSupportAlert> GetActualByPeriod(int datesOffset)
        {
            try
            {
                DateTime getAfter = SystemTime.Now().Date.AddDays(-datesOffset);
                if (getAfter >= Since)
                {
                    lock (_lockSupport)
                    {
                        return _tradeSupportLog.Values
                            .Where(a => !a.IsAcknowledged && a.Submitted >= getAfter)
                            .OrderBy(a => a.Submitted)
                            .ToList();
                    }
                }
                else
                {
                    using (MandaraEntities cxt = CreateMandaraProductsDbContext())
                    {
                        lock (_lockSupport)
                        {
                            return cxt.TradeSupportAlerts
                                .Where(a => a.Submitted >= getAfter)
                                .ToList();
                        }
                    }
                }
            }
            catch
            {
                return GetActual();
            }
        }

        /// <summary>
        /// Get acknowledged alerts
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public List<Guid> GetAcknowledged()
        {
            lock (_lockSupport)
            {
                return _tradeSupportLog.Values
                    .Where(a => a.IsAcknowledged)
                    .Select(a => a.Id)
                    .ToList();
            }
        }

        /// <summary>
        /// Initiate db and collection update raise event if succeeds
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="ip"></param>
        public void Acknowledge(Guid id, string user, string ip)
        {
            if (AcknowledgeAlert(id, user, ip) && AlertAcknowledged != null)
            {
                AlertAcknowledged(null, new AcknowledgeAlertEventArgs() { AcknowledgedID = id });
            }
        }

        /// <summary>
        /// Try add alert
        /// </summary>
        /// <param name="alert"></param>
        public void AddSupportAlert(TradeSupportAlert alert)
        {
            if (alert == null)
                return;

            if (alert.Submitted == DateTime.MinValue)
                alert.Submitted = SystemTime.Now();

            alert = AddAlert(alert);

            SendTradeSupportAlertToEventLog(alert);

            if (AlertAdded != null)
            {
                AlertAdded(null, new TradeSupportAlertEventArgs() { Alert = alert });
            }
        }

        private void SendTradeSupportAlertToEventLog(TradeSupportAlert alert)
        {
            // mirror trade support alert to the event log
            ErrorType errorType = ErrorType.Information;
            switch (alert.AType)
            {
                case TradeSupportAlert.AlertType.Dummies:
                errorType = ErrorType.Information;
                break;
                case TradeSupportAlert.AlertType.Expiring_products:
                errorType = ErrorType.Information;
                break;
                case TradeSupportAlert.AlertType.Import:
                errorType = ErrorType.ImportError;
                break;
                case TradeSupportAlert.AlertType.Product_Definition:
                errorType = ErrorType.TradeError;
                break;
                case TradeSupportAlert.AlertType.RollOff:
                errorType = ErrorType.Information;
                break;
                case TradeSupportAlert.AlertType.TAS:
                errorType = ErrorType.DataError;
                break;
                case TradeSupportAlert.AlertType.TradeOnHolidayDate:
                errorType = ErrorType.Information;
                break;
            }

            Error error = new Error
            {
                Source = "Trade Support",
                Type = errorType,
                Message = alert.Message,
                Date = alert.Submitted,
                Level = ErrorLevel.Normal,
                UserName = alert.Username,
            };

            if (alert.AType == TradeSupportAlert.AlertType.TAS)
            {
                List<TasCheckDetail> tasCheckDetail = alert.Parameters as List<TasCheckDetail>;
                if (tasCheckDetail != null)
                {
                    error.PortfolioIds = tasCheckDetail.SelectMany(d => d.PortfolioIds).Distinct().ToList();
                }
            }

            ErrorReportingHelper.GlobalQueue.Enqueue(error);
        }
    }

    /// <summary>
    /// Class for ad alert event. Contains added alert
    /// </summary>
    public class TradeSupportAlertEventArgs : EventArgs
    {
        public TradeSupportAlert Alert { get; set; }
    }

    /// <summary>
    /// Class for acknowledged event. Contains ID of acknowledged entry
    /// </summary>
    public class AcknowledgeAlertEventArgs : EventArgs
    {
        public Guid AcknowledgedID { get; set; }
    }
}
