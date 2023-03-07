using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Mandara.Business.Client.Bus.Messages.Exposures
{
    public class Exposure
    {
        public string Category { get; protected set; }
        [Display(Order = -1)]
        public int ProductId { get; protected set; }
        public string Product { get; protected set; }

        public decimal Position => Positions.Select(exposure => exposure).Sum();

        [Display(Order = -1)]
        public int NumExposureMonths => Positions.Length;

        [Display(Order = -1)]
        public decimal[] Positions { get; set; } = new decimal[0];

        public Exposure()
        {
        }

        public Exposure(int numMonths)
        {
            Positions = Enumerable.Repeat(0M, numMonths).ToArray();
        }

        public Exposure(
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
        public Exposure(
            int productId,
            string product,
            string category,
            params decimal[] positions)
        {
            ProductId = productId;
            Product = product;
            Category = category;

            Positions = positions.ToArray();
        }

        public void SetExposureAt(int monthOffset, decimal amount)
        {
            if (Positions.Length > monthOffset)
            {
                Positions[monthOffset] = amount;
            }
        }

        public decimal ExposureAt(int monthOffset) =>
            Positions.Length > monthOffset ? Positions[monthOffset] : 0M;

        public bool HasExposureAt(int monthOffset) => ExposureAt(monthOffset) != 0M;

        public string Key => GetKey(Category, Product, ProductId);

        public static string GetKey(string category, string product, int productId) =>
            $"{category}_{product}_{productId}";

        public string ExposedTo() => $"{Category}/{Product}";
    }
}