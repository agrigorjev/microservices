using System;
using System.Collections.Generic;

namespace Mandara.Entities
{
    public class PricingTypeBindingHelper
    {
        public PricingType PricingType { get; set; }
        public String Name { get; set; }

        public static List<PricingTypeBindingHelper> GetAllInstances()
        {
            return new List<PricingTypeBindingHelper>
                       {
                           new PricingTypeBindingHelper {PricingType = PricingType.Standard, Name = "Standard"},
                           new PricingTypeBindingHelper {PricingType = PricingType.NonStandard, Name = "Non standard"}
                       };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}