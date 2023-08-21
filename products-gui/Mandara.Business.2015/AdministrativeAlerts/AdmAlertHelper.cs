using Mandara.Business.Audit;
using Mandara.Date.Time;
using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AdminAlertVoiceCall = Mandara.Entities.AdminAlertVoiceCall;
using AdministrativeAlert = Mandara.Entities.AdministrativeAlert;
using AlertHistory = Mandara.Entities.AlertHistory;
using Portfolio = Mandara.Entities.Portfolio;
using User = Mandara.Entities.User;

namespace Mandara.Business.AdministrativeAlerts
{
    /// <summary>
    /// Provides static db interaction functionality fo administrative alerts entities
    /// </summary>
    public abstract class AdmAlertHelper
    {
        /// <summary>
        /// Returns list of current alerts registered in the system
        /// </summary>
        /// <param name="isFeatureEnabled"></param>
        /// <returns>BindingList of AdministrativeAlerts</returns>
        public static BindingList<AdministrativeAlert> GetAlerts(bool loadTransferErrorsAlerts)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                DbQuery<AdministrativeAlert> query = ctx.AdministrativeAlerts;

                if (!loadTransferErrorsAlerts)
                {
                    int transferServiceErrorsType = (int)AdministrativeAlert.AdmAlertType.TransferServiceErrors;

                    query = (DbQuery<AdministrativeAlert>)query.Where(x => x.AlertType != transferServiceErrorsType);
                }

                return new BindingList<AdministrativeAlert>(query.ToList());
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(AdmAlertHelper));
        }

        /// <summary>
        /// Returns list of current active alerts registered in the system
        /// </summary>
        /// <returns>BindingList of AdministrativeAlerts</returns>
        public static BindingList<AdministrativeAlert> GetActiveAlerts()
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                return new BindingList<AdministrativeAlert>(
                    ctx.AdministrativeAlerts
                       .Include("Portfolio").Include("Product").Include("ProductGroup")
                       .Include("AlertGroup1").Include("AlertGroup1.Phones")
                       .Include("AlertGroup1.Users").Include("AlertGroup1.Emails")
                       .Include("AlertGroup1.VoicePhones")
                       .Include("AlertGroup2").Include("AlertGroup2.Phones")
                       .Include("AlertGroup2.VoicePhones")
                       .Include("AlertGroup2.Users").Include("AlertGroup2.Emails")
                       .Include("AlertGroup3").Include("AlertGroup3.Phones")
                       .Include("AlertGroup3.Users").Include("AlertGroup3.Emails")
                       .Include("AlertGroup3.VoicePhones")
                       .Include("AlertGroup4").Include("AlertGroup4.Phones")
                       .Include("AlertGroup4.Users").Include("AlertGroup4.Emails")
                       .Include("AlertGroup4.VoicePhones")
                       .Where(a => a.Active)
                       .ToList());
            }
        }

        /// <summary>
        /// Saves alert to db
        /// </summary>
        /// <param name="alert">Alert to save</param>
        /// <param name="auditContext">Context for audit messages</param>
        public static void SaveAlert(AdministrativeAlert alert, AuditContext auditContext)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            using (var transaction = ctx.Database.BeginTransaction())
            {
                AdministrativeAlert originalAlert = ctx.AdministrativeAlerts.Include("Portfolio")
                                                       .Include("AlertGroup1")
                                                       .Include("AlertGroup2")
                                                       .Include("AlertGroup3")
                                                       .Include("AlertGroup4")
                                                       .Include("Product")
                                                       .Include("ProductGroup")
                                                       .SingleOrDefault(a => a.AlertId == alert.AlertId);

                if (originalAlert == null)
                {
                    originalAlert = new AdministrativeAlert();
                    ctx.AdministrativeAlerts.Add(originalAlert);
                }

                originalAlert.ConditionCheckCount = alert.ConditionCheckCount;

                originalAlert.Escalation = alert.Escalation;

                originalAlert.Level1Escalation = alert.Level1Escalation;
                originalAlert.Level2Escalation = alert.Level2Escalation;
                originalAlert.Level3Escalation = alert.Level3Escalation;
                originalAlert.Level4Escalation = alert.Level4Escalation;

                originalAlert.Title = alert.Title;
                originalAlert.TypeOfAlert = alert.TypeOfAlert;
                originalAlert.Boundary = alert.Boundary;

                originalAlert.Level1Subject = alert.Level1Subject;
                originalAlert.Level2Subject = alert.Level2Subject;
                originalAlert.Level3Subject = alert.Level3Subject;
                originalAlert.Level4Subject = alert.Level4Subject;

                originalAlert.Level1Message = alert.Level1Message;
                originalAlert.Level2Message = alert.Level2Message;
                originalAlert.Level3Message = alert.Level3Message;
                originalAlert.Level4Message = alert.Level4Message;

                originalAlert.EmailSubjectTemplate = alert.EmailSubjectTemplate;
                originalAlert.EmailMessageTemplate = alert.EmailMessageTemplate;

                originalAlert.Product = alert.Product != null ? ctx.Products.SingleOrDefault(p => p.ProductId == alert.Product.ProductId) : null;

                originalAlert.ProductGroup = alert.ProductGroup != null ? ctx.ProductCategories.SingleOrDefault(p => p.CategoryId == alert.ProductGroup.CategoryId) : null;

                if (alert.Portfolio != null)
                {
                    Portfolio mapPortfolio = ctx.Portfolios.SingleOrDefault(p => p.PortfolioId == alert.Portfolio.PortfolioId);
                    originalAlert.Portfolio = mapPortfolio;
                }

                if (alert.TypeOfAlert == AdministrativeAlert.AdmAlertType.Trade_Time)
                {
                    if (alert.TypeOfBoundary != AdministrativeAlert.BoundaryType.InRange)
                        originalAlert.TradeTime = alert.TradeTime;
                    else
                        originalAlert.TradeTimeRange = alert.TradeTimeRange;
                }
                else
                {
                    originalAlert.ThresholdValue = alert.ThresholdValue;
                }

                originalAlert.Active = alert.Active;

                List<AdministrativeAlertGroup> groups = ctx.AdministrativeAlertGroups.ToList();
                AdministrativeAlertGroup sGroup;

                if (alert.AlertGroup1 != null)
                {
                    sGroup = groups.SingleOrDefault(ag => ag.GroupId == alert.AlertGroup1.GroupId);
                    originalAlert.AlertGroup1 = sGroup;
                }

                if (alert.AlertGroup2 != null)
                {
                    sGroup = groups.SingleOrDefault(ag => ag.GroupId == alert.AlertGroup2.GroupId);
                    originalAlert.AlertGroup2 = sGroup;
                }

                if (alert.AlertGroup3 != null)
                {
                    sGroup = groups.SingleOrDefault(ag => ag.GroupId == alert.AlertGroup3.GroupId);
                    originalAlert.AlertGroup3 = sGroup;
                }

                if (alert.AlertGroup4 != null)
                {
                    sGroup = groups.SingleOrDefault(ag => ag.GroupId == alert.AlertGroup4.GroupId);
                    originalAlert.AlertGroup4 = sGroup;
                }

                originalAlert.IsLevel1Active = alert.IsLevel1Active;
                originalAlert.IsLevel2Active = alert.IsLevel2Active;
                originalAlert.IsLevel3Active = alert.IsLevel3Active;
                originalAlert.IsLevel4Active = alert.IsLevel4Active;

                originalAlert.DoNotTriggerOnWeekends = alert.DoNotTriggerOnWeekends;

                originalAlert.StartTime = alert.StartTime;
                originalAlert.CustomPropertiesJson = alert.CustomPropertiesJson;

                ctx.SaveChanges();
                transaction.Commit();
            }
        }
        /// <summary>
        /// Get single alert with all needed linked entities
        /// </summary>
        /// <param name="alertId">Ifd of alert to return</param>
        /// <returns>AdministrativeAlert</returns>
        public static AdministrativeAlert GetAlert(int alertId)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                return ctx.AdministrativeAlerts.Include("Portfolio")
                          .Include("AlertGroup1")
                          .Include("AlertGroup2")
                          .Include("AlertGroup3")
                          .Include("AlertGroup4")
                          .Include("Product")
                          .Include("ProductGroup")
                          .SingleOrDefault(a => a.AlertId == alertId);
            }
        }

        /// <summary>
        /// Delete administrative alert
        /// </summary>
        /// <param name="alertId">id of alert to delete</param>
        /// <param name="context">Context for audit messages</param>
        public static void DeleteAlert(int alertId, AuditContext context)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                var alert = ctx.AdministrativeAlerts.SingleOrDefault(a => a.AlertId == alertId);
                if (alert != null)
                {
                    ctx.AdministrativeAlerts.Remove(alert);
                    ctx.SaveChanges();
                }

            }
        }


        private static BindingList<AdministrativeAlertGroup> _alertGroups;

        /// <summary>
        ///  Static list represented of registered alert groups
        /// </summary>
        public static BindingList<AdministrativeAlertGroup> AlertGroups
        {
            get
            {
                if (_alertGroups == null)
                {
                    using (var ctx = CreateMandaraProductsDbContext())
                    {
                        _alertGroups = new BindingList<AdministrativeAlertGroup>(ctx.AdministrativeAlertGroups.ToList());
                    }
                }

                return _alertGroups;
            }

        }
        /// <summary>
        /// Saves given administrative alert group to db
        /// </summary>
        /// <param name="group">Group to save</param>
        /// <param name="auditContext">Context for audit messages</param>
        public static void SaveGroup(AdministrativeAlertGroup group, AuditContext auditContext)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            using (var transaction = ctx.Database.BeginTransaction())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;

                AdministrativeAlertGroup originalGroup =
                    ctx.AdministrativeAlertGroups.Include("Emails")
                    .Include("Phones")
                    .Include("Users")
                    .Include("VoicePhones")
                    .SingleOrDefault(g => g.GroupId == group.GroupId);

                List<User> _users = ctx.Users.ToList();
                if (originalGroup == null)
                {
                    originalGroup = new AdministrativeAlertGroup();
                    ctx.AdministrativeAlertGroups.Add(originalGroup);
                }

                //                    if (auditContext != null)
                //                        originalGroup;//.StartTrackingAll();

                originalGroup.Title = group.Title;
                originalGroup.Phones.Clear();
                foreach (var p in group.Phones.ToList())
                {
                    if (!originalGroup.Phones.Any(pp => pp.Phone == p.Phone))
                    {
                        originalGroup.Phones.Add(p);
                    }
                }
                originalGroup.VoicePhones.Clear();
                foreach (var p in group.VoicePhones.ToList())
                {
                    if (!originalGroup.VoicePhones.Any(pp => pp.Phone == p.Phone))
                    {
                        originalGroup.VoicePhones.Add(p);
                    }
                }
                originalGroup.Emails.Clear();
                foreach (var e in group.Emails.ToList())
                {
                    if (!originalGroup.Emails.Any(pp => pp.Email == e.Email))
                    {
                        originalGroup.Emails.Add(e);
                    }
                }
                //                if (auditContext != null)
                //                        originalGroup.StopTrackingAll();
                originalGroup.Users.Clear();
                foreach (var u in group.Users)
                {
                    originalGroup.Users.Add(_users.FirstOrDefault(uu => uu.UserId == u.UserId));
                }


                ctx.ChangeTracker.DetectChanges();
                ctx.SaveChanges();
                transaction.Commit();
                _alertGroups = new BindingList<AdministrativeAlertGroup>(ctx.AdministrativeAlertGroups.ToList());
            }
        }
        /// <summary>
        /// Returns grup with all needed linked entities
        /// </summary>
        /// <param name="groupId">id of group to return</param>
        /// <returns>AdministrativeAlertGroup</returns>
        public static AdministrativeAlertGroup GetGroup(int groupId)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                return
                     ctx.AdministrativeAlertGroups.Include("Emails")
                     .Include("Phones")
                     .Include("VoicePhones")
                     .Include("Users").SingleOrDefault(g => g.GroupId == groupId);
            }
        }
        /// <summary>
        /// Try to delete AdministrativeAlertGroup
        /// </summary>
        /// <param name="groupId">GroupId to delete</param>
        /// <param name="context">Context for audit messages</param>
        /// <returns>True if successful delete, false if group assigned to any administrative alert</returns>
        public static bool DeleteGroup(int groupId, AuditContext context)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                var group = ctx.AdministrativeAlertGroups.Include("Emails")
                    .Include("Phones")
                    .Include("Users")
                    .Include("adm_alerts")
                    .Include("adm_alerts1").SingleOrDefault(g => g.GroupId == groupId);

                if (group == null)
                    return true;

                if (group.adm_alerts1.Count + group.adm_alerts.Count > 0)
                {
                    return false;
                }

                using (var transaction = ctx.Database.BeginTransaction())
                {
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    group.VoicePhones.Clear();
                    group.Phones.Clear();
                    group.Emails.Clear();
                    group.Users.Clear();
                    ctx.AdministrativeAlertGroups.Remove(group);

                    ctx.ChangeTracker.DetectChanges();
                    ctx.SaveChanges();
                    transaction.Commit();
                }

                _alertGroups = new BindingList<AdministrativeAlertGroup>(ctx.AdministrativeAlertGroups.ToList());
                return true;
            }
        }

        /// <summary>
        /// Returns list of portfolios in system
        /// </summary>
        /// <returns></returns>
        public static List<Portfolio> GetPortfolios()
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                return ctx.Portfolios.ToList();
            }
        }

        public static List<AlertHistory> GetActiveHistoryEntries()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.AlertHistories.Where(x => x.IsActive).ToList();
            }
        }

        public static List<AlertHistory> GetTodayTradeTimeHistoryEntries()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                DateTime dateTime = SystemTime.Now().Date;

                return cxt.AlertHistories.Where(
                    x =>
                    x.AcknowledgedIp != null && x.AlertType.Equals("Trade Time") &&
                    x.TriggeredAt >= dateTime).OrderByDescending(x => x.TriggeredAt).ToList();
            }
        }

        public static void UpdateHistoryEntry(AlertHistory entry)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                AlertHistory oldEntry =
                    cxt.AlertHistories
                       .SingleOrDefault(x => x.IsActive && x.TriggerKey == entry.TriggerKey);

                if (oldEntry != null)
                {
                    oldEntry.IsActive = false;
                }

                cxt.AlertHistories.Add(entry);

                cxt.SaveChanges();
            }
        }
        /// <summary>
        /// Update history. May returns identity for updated unit
        /// </summary>
        /// <param name="entry">Entry to update</param>
        /// <param name="histroyId">Out identity value</param>
        public static void UpdateHistoryEntry(AlertHistory entry, out int histroyId)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                AlertHistory oldEntry =
                    cxt.AlertHistories
                       .SingleOrDefault(x => x.IsActive && x.TriggerKey == entry.TriggerKey);

                if (oldEntry != null)
                {
                    oldEntry.IsActive = false;
                }

                cxt.AlertHistories.Add(entry);

                cxt.SaveChanges();
                histroyId = entry.AlertHistoryId;
            }
        }
        /// <summary>
        /// Find history id by trigger
        /// </summary>
        /// <returns>Alert history id</returns>
        public static int IdentifyHistory(string triggerKey, int level)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                var entry =
                    cxt.AlertHistories
                       .FirstOrDefault(x => x.IsActive && x.TriggerKey == triggerKey);
                return entry == null ? 0 : entry.AlertHistoryId;
            }
        }

        public static void DeactivateHistoryEntry(string triggerKey, string acknowledgedBy, string acknowledgedIp)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            using (var transaction = cxt.Database.BeginTransaction())
            {
                AlertHistory activeEntry =
                    cxt.AlertHistories
                       .SingleOrDefault(x => x.IsActive && x.TriggerKey == triggerKey);

                if (activeEntry != null)
                {
                    activeEntry.IsActive = false;
                    activeEntry.AcknowledgedBy = acknowledgedBy;
                    activeEntry.AcknowledgedIp = acknowledgedIp;

                    cxt.SaveChanges();
                    transaction.Commit();
                }
            }
        }

        public static List<AlertHistory> GetHistoryEntriesAt(DateTime date)
        {
            DateTime startDate = date.Date;
            DateTime endDate = date.Date.AddDays(1);

            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.AlertHistories
                          .Where(ah => startDate <= ah.TriggeredAt && ah.TriggeredAt < endDate).ToList();
            }
        }

        /// <summary>
        /// Add voice call record to db. For use from AdminAlertService. Initiate new call.
        /// </summary>
        /// <param name="alertHistory">Referencer alert</param>
        /// <param name="phone">Phone number  string</param>
        /// <param name="message">Message </param>
        public static void SubmitVoiceCall(string triggerKey, int alertHistory, string phone, string message, string actualValue)
        {
            //Cut message to max symbols allowed
            if (message.Length > 500) message = message.Substring(0, 500);
            //Format phone number
            if (!phone.StartsWith("+")) phone = "+" + phone;
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                if (cxt.AdminAlertVoiceCalls.Count(vc => vc.Phone == phone && vc.AlertHistoryId == alertHistory && vc.StatusDb != (int)AdminAlertVoiceCall.CallState.Served) == 0)
                {
                    cxt.AdminAlertVoiceCalls.Add(new AdminAlertVoiceCall()
                    {
                        TriggerKey = triggerKey,
                        Phone = phone,
                        AlertHistoryId = alertHistory,
                        Message = message,
                        Status = AdminAlertVoiceCall.CallState.Queued,
                        ActualValue = actualValue
                    });
                    cxt.SaveChanges();
                }
            }
        }

        public static void UpdateVoiceCall(AdminAlertVoiceCall call)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                AdminAlertVoiceCall current = cxt.AdminAlertVoiceCalls.SingleOrDefault(c => c.CallId == call.CallId);
                if (current != null)
                {
                    current.Result = call.Result;
                    current.Status = call.Status;
                    current.DialedAt = call.DialedAt;
                    current.RetryNum = call.RetryNum;
                    cxt.SaveChanges();
                }
            }
        }

        public static List<AdminAlertVoiceCall> ActualAlerts(int lastId)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.AdminAlertVoiceCalls.Where(vc => vc.CallId > lastId
                    && vc.StatusDb != (int)AdminAlertVoiceCall.CallState.Served && vc.StatusDb != (int)AdminAlertVoiceCall.CallState.Interrupted).ToList();

            }
        }

        public static List<int> AcknowledgedOrInactiveTriggersAt(DateTime lastCheckDate)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.AlertHistories
                          .Where(ah => (!string.IsNullOrEmpty(ah.AcknowledgedIp)) && ah.TriggeredAt > lastCheckDate).Select(ah => ah.AlertHistoryId).ToList();

            }
        }
        /// <summary>
        /// Set state of administrative alert
        /// </summary>
        /// <param name="alertId">Id of admAlert</param>
        /// <param name="active">Active status of alert to set</param>
        /// <param name="auditContext">Context for audit messages</param>
        public static void SetAlertActive(int alertId, bool active, AuditContext auditContext)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                var dbAlert = cxt.AdministrativeAlerts.FirstOrDefault(a => a.AlertId == alertId);

                if (dbAlert != null)
                {
                    dbAlert.Active = active;
                    cxt.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Get if alert history record is subject for acknowledge
        /// </summary>
        /// <param name="historyID">id of history record</param>
        /// <returns>True if alert history record is active and not acknowledged </returns>
        public static bool IfAlertToAcknowledge(int historyID)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                AlertHistory history = cxt.AlertHistories.FirstOrDefault(ah => ah.AlertHistoryId == historyID);
                if (history != null)
                {
                    return !history.IsAcknowledged && history.IsActive;
                }

            }
            return false;
        }
    }
}
