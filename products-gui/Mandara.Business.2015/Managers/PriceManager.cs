using Mandara.Database.Query;
using Mandara.Database.Sql;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Mandara.Business.Managers
{
    public class PriceManager
    {
        public List<object> GetProductForAnalysis(
            string productName,
            DateTime fromDate,
            DateTime toDate,
            PricePrecisionType precision,
            int month,
            bool rollingMonth,
            TimeSpan fromTime,
            TimeSpan toTime,
            int holidayDisplayType)
        {
            ;
            OfficialProduct product = (new ProductManager()).GetOfficialProduct(productName);

            if (product == null)
            {
                return null;
            }

            List<CalendarHoliday> holidays = GetHolidays(holidayDisplayType);

            (object, object) result = SqlServerCommandExecution.ReadToList(
                ConnectionString.GetConnectionStringBuild(MandaraEntities.DefaultConnStrName),
                (conn) => GetLoadProductSqlCommand(
                    fromDate,
                    toDate,
                    precision,
                    month,
                    rollingMonth,
                    fromTime,
                    toTime,
                    conn),
                (reader) => GetProductDateAndPrice(reader, holidays, product)).First(res => res.Item1 != null);

            return new List<object>() { result.Item1, result.Item2 };
        }

        private static (object, object) GetProductDateAndPrice(SqlDataReader reader, List<CalendarHoliday> holidays, OfficialProduct product)
        {
            DateTime priceDate = EpochConverter.FromEpochTime((double)(int)reader["sdate"]);

            if (holidays.All(h => h.HolidayDate != priceDate.Date))
            {
                return (priceDate, Convert.ToDouble(reader[product.MappingColumn]));
            }

            return (null, null);
        }

        private static List<CalendarHoliday> GetHolidays(int holidayDisplayType)
        {
            List<CalendarHoliday> holidays = new List<CalendarHoliday>();

            if (holidayDisplayType >= 0)
            {
                using (MandaraEntities context = new MandaraEntities(
                    MandaraEntities.DefaultConnStrName,
                    nameof(PriceManager)))
                {
                    holidays = context.CalendarHolidays.Where(
                                          h => h.StockCalendar.CalendarId
                                               == (holidayDisplayType == 0 ? h.StockCalendar.CalendarId : holidayDisplayType))
                                      .ToList();
                }
            }
            return holidays;
        }

        private static SqlCommand GetLoadProductSqlCommand(
            DateTime fromDate,
            DateTime toDate,
            PricePrecisionType precision,
            int month,
            bool rollingMonth,
            TimeSpan fromTime,
            TimeSpan toTime,
            SqlConnection conn)
        {
            SqlCommand comm;

            if (precision == PricePrecisionType.Minute)
            {
                comm = new SqlCommand("[dbo].[rat_GetContinuousSeriesData]", conn);
            }
            else
            {
                comm = new SqlCommand("[dbo].[rat_GetSeriesData]", conn);
                comm.Parameters.Add(new SqlParameter("@GroupByDay", precision == PricePrecisionType.Day));
            }

            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add(new SqlParameter("@FromDate", fromDate));
            comm.Parameters.Add(new SqlParameter("@ToDate", toDate));
            comm.Parameters.Add(new SqlParameter("@FromTime", fromTime.TotalSeconds));
            comm.Parameters.Add(new SqlParameter("@ToTime", toTime.TotalSeconds));
            comm.Parameters.Add(new SqlParameter("@RollingMonth", rollingMonth));
            comm.Parameters.Add(new SqlParameter("@Month", month));

            return comm;
        }
    }
}
