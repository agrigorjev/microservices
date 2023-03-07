using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Entities
{
    public class ProductTypeBindingHelper
    {
        private static readonly List<ProductType> dailySwapProductTypes = new List<ProductType>()
        {
            ProductType.DailySwap,
            ProductType.DailyVsDaily,
            ProductType.DayVsMonthCustom,
            ProductType.DayVsMonthFullWeek
        };

        public ProductType Type { get; set; }
        public String Name { get; set; }

        public static List<ProductTypeBindingHelper> GetAllInstances()
        {
            return ConstructBindingHelpers(ProductTypeExtensions.GetNames());
        }

        private static List<ProductTypeBindingHelper> ConstructBindingHelpers(
            Dictionary<ProductType, string> prodTypeNames)
        {
            return prodTypeNames.Select(
                prodTypeName => new ProductTypeBindingHelper()
                {
                    Type = prodTypeName.Key, Name = prodTypeName.Value
                }).ToList();
        }

        public static List<ProductTypeBindingHelper> GetAllNonDailyInstances()
        {
            Dictionary<ProductType, string> prodTypeNames = ProductTypeExtensions
                  .GetNames().Where(prodTypeName => !dailySwapProductTypes.Contains(prodTypeName.Key)).ToDictionary(
                      prodTypeName => prodTypeName.Key,
                      prodTypeName => prodTypeName.Value);

            return ConstructBindingHelpers(prodTypeNames);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
