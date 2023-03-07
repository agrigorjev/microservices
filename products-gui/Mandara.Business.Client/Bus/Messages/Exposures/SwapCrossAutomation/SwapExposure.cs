using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mandara.Business.Client.Bus.Messages.Exposures;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class SwapExposure: Exposure
    {
        public decimal SwapAmount => Position;

        [Display(Order = -1)]
        public decimal?[] ExposureAmounts { get; set; } = new decimal?[0];

        public SwapExposure()
        {
        }

        public SwapExposure(int numMonth): base(numMonth)
        {
            ExposureAmounts = Enumerable.Repeat(default(decimal?), numMonth).ToArray();
        }

        public SwapExposure(
            int productId,
            string product,
            string category,
            int numMonths): this(numMonths)
        {
            ProductId = productId;
            Product = product;
            Category = category;
        }

        [JsonConstructor]
        public SwapExposure(
            int productId,
            string product,
            string category,
            params decimal?[] exposureAmounts)
            : base(productId, product, category, exposureAmounts.Select(pos => pos ?? 0M).ToArray())
        {
            ProductId = productId;
            Product = product;
            Category = category;

            ExposureAmounts = exposureAmounts.ToArray();
        }

        public SwapExposure(Exposure toCopy): base(toCopy.ProductId, toCopy.Product, toCopy.Category, toCopy.Positions)
        {
            ExposureAmounts = toCopy.Positions.Select(pos => (decimal?)pos).ToArray();
        }

        public void SetExposure(int monthOffset, decimal amount)
        {
            base.SetExposureAt(monthOffset, amount);
            if (ExposureAmounts.Length > monthOffset)
            {
                ExposureAmounts[monthOffset] = amount;
            }
        }

        public decimal GetExposure(int monthOffset) => ExposureAt(monthOffset);

        public bool HasExposureAtMonthOffset(int monthOffset) => HasExposureAt(monthOffset);
    }
}