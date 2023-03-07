using Mandara.Business.TradeAdd;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Nullable;

namespace Mandara.Business
{
    public class TradeBrokerageCalculationManager : ITradeBrokerageCalculationManager
    {
        private readonly List<CompanyAlias> _companyAliases;
        private readonly List<ProductBrokerage> _productBrokerages;
        private readonly List<Company> _companies;

        public TradeBrokerageCalculationManager()
        {
            BrokerManager brokerManager = new BrokerManager();

            _companyAliases = brokerManager.GetCompanyAliases();
            _productBrokerages = brokerManager.GetProductBrokerages();
            _companies = brokerManager.GetCompanies();
        }

        public decimal CalculateTradeBrokerage(
            string executingFirm,
            int? tradeType,
            int? numOfLots,
            decimal? quantity,
            string stripName,
            DateTime? tradeStartDate,
            DateTime? tradeEndDate,
            int productId,
            Unit unit)
        {
            if (string.IsNullOrEmpty(executingFirm)
                || tradeType.IsEqualTo((int)TradeTypeControl.Transfer)
                || _companyAliases == null)
            {
                return 0M;
            }

            Company brokerageCompany = GetBrokerageCompany(executingFirm);

            if (brokerageCompany == null)
            {
                return 0M;
            }

            decimal? brokerageValue = null;
            decimal decimalNumOfLots = Client.Managers.LiveDataManager.GetDecimalNumOfLots(
                numOfLots,
                tradeType,
                quantity,
                stripName,
                tradeStartDate,
                tradeEndDate);

            // step 1: company brokerage for product override
            ProductBrokerage productBrokerage = _productBrokerages.SingleOrDefault(
                pb => pb.ProductId == productId && pb.CompanyId == brokerageCompany.CompanyId);

            if (productBrokerage != null)
            {
                brokerageValue = productBrokerage.Brokerage;
            }

            // step 2: if step 1 fails => default company brokerage
            if (brokerageValue == null)
            {
                brokerageValue = brokerageCompany.GetBrokerage(unit, decimalNumOfLots);
            }

            // step 3: if everything else fails => use 0
            if (brokerageValue == null)
            {
                brokerageValue = 0M;
            }

            return brokerageValue.Value * decimalNumOfLots;
        }

        private Company GetBrokerageCompany(string executingFirm)
        {
            CompanyAlias companyAlias = _companyAliases.SingleOrDefault(
                alias => alias.AliasName.Trim()
                              .Equals(executingFirm.Trim(), StringComparison.InvariantCultureIgnoreCase));
            Company company = _companies.SingleOrDefault(
                comp => comp.CompanyName.Trim()
                            .Equals(executingFirm.Trim(), StringComparison.InvariantCultureIgnoreCase));

            return companyAlias?.Company ?? company;
        }

        public decimal CalculateTradeBrokerage(TradeCapture trade)
        {
            return CalculateTradeBrokerage(new TradePieces(trade));
        }

        public decimal CalculateTradeBrokerage(TradePieces trade)
        {
            if (trade.Trade == null || trade.Security?.Product == null)
            {
                return 0M;
            }

            TradeCapture capture = trade.Trade;
            Product product = trade.Security.Product;

            return CalculateTradeBrokerage(
                capture.ExecutingFirm,
                capture.TradeType,
                capture.NumOfLots,
                capture.Quantity,
                trade.Security.SecurityDef.StripName,
                capture.TradeStartDate,
                capture.TradeEndDate,
                product.ProductId,
                product.Unit);
        }

        public void CalculateTradesBrokerages(List<TradeCapture> trades)
        {
            foreach (TradeCapture tradeCapture in trades)
            {
                if (!tradeCapture.Brokerage.HasValue)
                    tradeCapture.Brokerage = CalculateTradeBrokerage(tradeCapture);
            }
        }

    }
}