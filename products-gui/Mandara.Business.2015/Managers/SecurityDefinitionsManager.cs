using Mandara.Entities;
using Mandara.Entities.EntitiesCustomization;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Managers
{
    public class SecurityDefinitionsManager
    {
        public static int FxSecDefId = -1;

        public static Dictionary<string, SecurityDefinition> BuildSecurityDefinitionsMap(
            IEnumerable<SecurityDefinition> securityDefinitions,
            Func<SecurityDefinition, Product> getProductFromSecurityDefinition)
        {
            Dictionary<string, SecurityDefinition> secDefsByProductAndStrip =
                new Dictionary<string, SecurityDefinition>();

            securityDefinitions.Where(secDef => null != secDef.StripName
                                             && !getProductFromSecurityDefinition(secDef).IsProductDaily)
                               .ForEach(
                                   secDef =>
                                   {
                                       Strip strip = StripParser.Parse(
                                           secDef.StripName,
                                           secDef.Strip1Date ?? DateTime.MaxValue);

                                       if (!strip.Part2.IsDefault())
                                       {
                                           return;
                                       }

                                       string secDefKey = GetSecurityDefKey(secDef, strip);

                                       if (!secDefsByProductAndStrip.ContainsKey(secDefKey))
                                       {
                                           secDefsByProductAndStrip.Add(secDefKey, secDef);
                                       }
                                   });

            return secDefsByProductAndStrip;
        }

        private static string GetSecurityDefKey(SecurityDefinition secDef, Strip strip)
        {
            return $"{secDef.product_id.Value}_{(int)strip.Part1.DateType}_{strip.Part1.StartDate.ToShortDateString()}";
        }

        public static TradeChangeEntityType GetTradeChangeEntityType(int secDefId)
        {
            if (secDefId == FxSecDefId)
            {
                return TradeChangeEntityType.FxTrade;
            }

            return TradeChangeEntityType.TradeCapture;
        }
    }
}
