using System;
using System.Configuration;
using NLog;

namespace Mandara.Entities
{
    public static class LoggerHelper
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static bool _isLoggingEnabled;

        static LoggerHelper()
        {
            _isLoggingEnabled = "true".Equals(ConfigurationManager.AppSettings["LogOfficialProductReferences"],
                StringComparison.InvariantCultureIgnoreCase);
        }

        public static void LogSecDef(SecurityDefinition securityDefinition)
        {
            if (!_isLoggingEnabled)
                return;

            if (securityDefinition != null && securityDefinition.Product != null &&
                securityDefinition.Product.OfficialProduct != null)
                return;

            string secDefId = "n/a", productId = "n/a", offProductId = "n/a";
            if (securityDefinition != null)
                secDefId = securityDefinition.SecurityDefinitionId.ToString();
            if (securityDefinition != null && securityDefinition.Product != null)
                productId = securityDefinition.Product.ProductId.ToString();
            if (securityDefinition != null && securityDefinition.Product != null &&
                securityDefinition.Product.OfficialProduct != null)
                offProductId = securityDefinition.Product.OfficialProduct.OfficialProductId.ToString();

            _log.Trace("SecurityDefinition set, SecDefId [{0}], ProductId [{1}], OffProductId [{2}]{3}{4}", secDefId,
                productId, offProductId, Environment.NewLine, Environment.StackTrace);
        }

        public static void LogProduct(Product product)
        {
            if (!_isLoggingEnabled)
                return;

            if (product != null && product.OfficialProduct != null)
                return;

            string productId = "n/a", offProductId = "n/a";
            if (product != null)
                productId = product.ProductId.ToString();
            if (product != null && product.OfficialProduct != null)
                offProductId = product.OfficialProduct.OfficialProductId.ToString();

            _log.Trace("Product set, ProductId [{0}], OffProductId [{1}]{2}{3}", productId, offProductId, Environment.NewLine, Environment.StackTrace);
        }

        public static void LogOffProduct(OfficialProduct officialProduct)
        {
            if (!_isLoggingEnabled)
                return;

            if (officialProduct != null)
                return;

            string offProductId = "n/a";
            if (officialProduct != null)
                offProductId = officialProduct.OfficialProductId.ToString();

            _log.Trace("OfficialProduct set, OffProductId [{0}]{1}{2}", offProductId, Environment.NewLine, Environment.StackTrace);
        }
    }
}