using Mandara.Business.Model;
using Mandara.Entities;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date.Time;

namespace Mandara.Business
{
    public class DeliveredProductsManager
    {
        private readonly List<CalculationDetailModel> _livePositions;
        private readonly ILogger _log = new NLogLoggerFactory().GetCurrentClassLogger();

        public DeliveredProductsManager(List<CalculationDetailModel> livePositions)
        {
            _livePositions = livePositions;
        }

        public List<ExpiringProductWarning> GetExpiringProducts(int daysToExpire)
        {
            if (daysToExpire <= 0)
                daysToExpire = 3;

            DateTime startDate = SystemTime.Today();
            DateTime endDate = SystemTime.Today().AddDays(daysToExpire);

            // skip weekends
            if (endDate.DayOfWeek == DayOfWeek.Saturday)
                endDate = endDate.AddDays(2);
            if (endDate.DayOfWeek == DayOfWeek.Sunday)
                endDate = endDate.AddDays(1);

            List<Product> physicallySettledProducts = new List<Product>();

            using (MandaraEntities cxt = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(DeliveredProductsManager)))
            {
                physicallySettledProducts =
                    cxt.Products
                        .Include("Exchange")
                        .Include("ExpiryCalendar")
                        .Include("ExpiryCalendar.FuturesExpiries")
                        .Where(p => p.IsPhysicallySettledDb == true &&
                                    p.ExpiryCalendar.FuturesExpiries.Any(
                                        x => startDate <= x.ExpiryDate && x.ExpiryDate <= endDate))
                        .ToList();

            }

            if (physicallySettledProducts.Count == 0)
                return new List<ExpiringProductWarning>();

            List<ExpiringProductWarning> productWarnings = new List<ExpiringProductWarning>();

            foreach (Product product in physicallySettledProducts)
            {
                List<CalendarExpiryDate> expiryDates =
                    product.ExpiryCalendar.FuturesExpiries
                        .Select(x => startDate <= x.ExpiryDate && x.ExpiryDate <= endDate ? x : null)
                        .Where(x => x != null)
                        .ToList();

                foreach (CalendarExpiryDate expiryDate in expiryDates)
                {
                    productWarnings.Add(new ExpiringProductWarning
                    {
                        ExpiryDate = expiryDate.ExpiryDate,
                        ProductDescription = product.Name,
                        StripName = expiryDate.FuturesDate.ToString("MMMyy"),
                        Quantity = CalculateExpiryQuantity(product, expiryDate)
                    });
                }
            }

            return productWarnings.Where(x => x.Quantity != 0M).ToList();
        }

        private decimal CalculateExpiryQuantity(Product product, CalendarExpiryDate expiryDate)
        {
            List<CalculationDetailModel> calculationDetails =
                _livePositions.Where(x => x.CalculationDate == expiryDate.FuturesDate.Date &&
                                          x.ProductId == product.ProductId).ToList();

            if (calculationDetails.Count == 0)
                return 0M;

            decimal contractSize = product.ContractSize == 0M ? 1M : product.ContractSize;

            // TODO: Determine what the outcome of this calculation is meant to be.  Currently it's only correct if
            // the desired outcome is a number identical to what's seen on the position screen and the contract size is
            // 1000.
            return calculationDetails.Sum(x => x.AmountInner) / contractSize;
        }
    }

}
