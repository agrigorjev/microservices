using Mandara.Business.DataInterface;
using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Data
{
    public class PnlEodTimesDataProvider : ReplaceableDbContext, IPnlEodTimesDataProvider
    {
        public Dictionary<DateTime, DateTime> GetPnlEodTimes(int numberOfDaysToLoad)
        {
            using (MandaraEntities cxt = _dbContextCreator())
            {
                List<PnlReportEod> pnlReportEods =
                    cxt.PnlReportEods.OrderByDescending(x => x.Day).Take(numberOfDaysToLoad).ToList();

                return pnlReportEods.ToDictionary(x => x.Day.Date, x => x.SnapshotDatetime);
            }
        }

        public void SetPnlEodTime(DateTime snapshotDatetime)
        {
            DateTime day = snapshotDatetime.Date;

            using (MandaraEntities cxt = _dbContextCreator())
            {
                PnlReportEod val = cxt.PnlReportEods.FirstOrDefault(x => x.Day == day);

                if (val == null)
                {
                    val = new PnlReportEod { Day = day };
                    cxt.PnlReportEods.Add(val);
                }

                val.EndOfDayDb = snapshotDatetime;

                cxt.SaveChanges();
            }
        }
    }
}