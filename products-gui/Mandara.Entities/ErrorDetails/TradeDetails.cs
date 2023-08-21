using System.ComponentModel;
using System.Data;
using Mandara.Entities.Extensions;

namespace Mandara.Entities.ErrorDetails
{
    public class TradeDetails : ErrorDetails
    {
        // trade capture table
        [Category("Trade Capture")]
        [DisplayName("Trade Capture Id")]
        public string TradeCaptureId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Report Id")]
        public string TradeReportID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Report Transaction Type")]
        public string TradeReportTransType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Type")]
        public string TrdType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Orig Trade Id")]
        public string OrigTradeID { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Exec Id")]
        public string ExecID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Exec Type")]
        public string ExecType { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Order Status")]
        public string OrdStatus { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Symbol")]
        public string Symbol { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Security Id")]
        public string SecurityID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Security Id Source")]
        public string SecurityIDSource { get; set; }
        [Category("Trade Capture")]
        [DisplayName("CFI Code")]
        public string CFICode { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Last Qty")]
        public string LastQty { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Last Px")]
        public string LastPx { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Number of Lots")]
        public string NumOfLots { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Number of Cycles")]
        public string NumOfCycles { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Trade Date")]
        public string TradeDate { get; set; }
        [DisplayName("Trade Start Date")]
        public string TradeStartDate { get; set; }
        [DisplayName("Trade End Date")]
        public string TradeEndDate { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Side")]
        public string Side { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Order Id")]
        public string OrderID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Cl Order Id")]
        public string ClOrdID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Timestamp")]
        public string TimeStamp { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Origination trader")]
        public string OriginationTrader { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Clearing Account Id")]
        public string ClearingAccountId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Clearing Firm")]
        public string ClearingFirm { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Origination Firm")]
        public string OriginationFirm { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Transact Time")]
        public string TransactTime { get; set; }
        [Category("Trade Capture")]
        [DisplayName("UTC Transact Time")]
        public string UtcTransactTime { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Executing Firm")]
        public string ExecutingFirm { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Exchange")]
        public string Exchange { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Leg Ref Id")]
        public string LegRefID { get; set; }

        // security definition table
        [Category("Security Definition")]
        [DisplayName("Security Definition Id")]
        public string SecurityDefinitionId { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying Symbol")]
        public string UnderlyingSymbol { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying Security Id")]
        public string UnderlyingSecurityID { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying Security Id Source")]
        public string UnderlyingSecurityIDSource { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying CFI Code")]
        public string UnderlyingCFICode { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying Security Description")]
        public string UnderlyingSecurityDesc { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying Maturity Date")]
        public string UnderlyingMaturityDate { get; set; }

        [Category("Security Definition")]
        [DisplayName("Underlying Contract Multiplier")]
        public string UnderlyingContractMultiplier { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Increment Price")]
        public string IncrementPrice { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Increment Qty")]
        public string IncrementQty { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Lot Size")]
        public string LotSize { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Lot Size Multiplier")]
        public string LotSizeMultiplier { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Clearable")]
        public string Clearable { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [Category("Trade Capture")]
        [DisplayName("End Date")]
        public string EndDate { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Strip Id")]
        public string StripId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Strip Type")]
        public string StripType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Strip Name")]
        public string StripName { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Hub Id")]
        public string HubId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Hub Name")]
        public string HubName { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Hub Alias")]
        public string HubAlias { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Underlying Unit Of Measure")]
        public string UnderlyingUnitOfMeasure { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Price Denomination")]
        public string PriceDenomination { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Price Unit")]
        public string PriceUnit { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Granularity")]
        public string Granularity { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Number Of Decimal Price")]
        public string NumOfDecimalPrice { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Number Of Decimal Qty")]
        public string NumOfDecimalQty { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Product Id")]
        public string ProductId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Product Description")]
        public string ProductDescription { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Tick Value")]
        public string TickValue { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Implied Type")]
        public string ImpliedType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Primary Leg Symbol")]
        public string PrimaryLegSymbol { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Secondary Leg Symbol")]
        public string SecondaryLegSymbol { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Increment Strike")]
        public string IncrementStrike { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Min Strike")]
        public string MinStrike { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Max Strike")]
        public string MaxStrike { get; set; }

        public TradeDetails()
        {
        }

        public TradeDetails(DataRow row)
        {
            if (row != null)
            {
                #region properties
                TradeCaptureId = row["idTradeCapture"].ToDataString();
                TradeReportID = row["TradeReportID"].ToDataString();
                TradeReportTransType = row["TradeReportTransType"].ToDataString();
                TrdType = row["TrdType"].ToDataString();
                OrigTradeID = row["OrigTradeID"].ToDataString();
                ExecID = row["ExecID"].ToDataString();
                ExecType = row["ExecType"].ToDataString();
                OrdStatus = row["OrdStatus"].ToDataString();
                Symbol = row["Symbol"].ToDataString();
                SecurityID = row["SecurityID"].ToDataString();
                SecurityIDSource = row["SecurityIDSource"].ToDataString();
                CFICode = row["CFICode"].ToDataString();
                LastQty = row["LastQty"].ToDataString();
                LastPx = row["LastPx"].ToDataString();
                NumOfLots = row["NumOfLots"].ToDataString();
                NumOfCycles = row["NumOfCycles"].ToDataString();
                TradeDate = row["TradeDate"].ToDataString();
                TradeStartDate = row["TradeStartDate"].ToDataString();
                TradeEndDate = row["TradeEndDate"].ToDataString();
                Side = row["Side"].ToDataString();
                OrderID = row["OrderID"].ToDataString();
                ClOrdID = row["ClOrdID"].ToDataString();
                TimeStamp = row["TimeStamp"].ToDataString();
                OriginationTrader = row["OriginationTrader"].ToDataString();
                ClearingAccountId = row["ClearingAccountId"].ToDataString();
                ClearingFirm = row["ClearingFirm"].ToDataString();
                OriginationFirm = row["OriginationFirm"].ToDataString();
                TransactTime = row["TransactTime"].ToDataString();
                UtcTransactTime = row["UtcTransactTime"].ToDataString();
                ExecutingFirm = row["ExecutingFirm"].ToDataString();
                Exchange = row["Exchange"].ToDataString();
                LegRefID = row["LegRefID"].ToDataString();

                SecurityDefinitionId = row["idSecurityDefinition"].ToDataString();
                UnderlyingSymbol = row["UnderlyingSymbol"].ToDataString();
                UnderlyingSecurityID = row["UnderlyingSecurityID"].ToDataString();
                UnderlyingSecurityIDSource = row["UnderlyingSecurityIDSource"].ToDataString();
                UnderlyingCFICode = row["UnderlyingCFICode"].ToDataString();
                UnderlyingSecurityDesc = row["UnderlyingSecurityDesc"].ToDataString();
                UnderlyingMaturityDate = row["UnderlyingMaturityDate"].ToDataString();
                UnderlyingContractMultiplier = row["UnderlyingContractMultiplier"].ToDataString();
                IncrementPrice = row["IncrementPrice"].ToDataString();
                IncrementQty = row["IncrementQty"].ToDataString();
                LotSize = row["LotSize"].ToDataString();
                LotSizeMultiplier = row["LotSizeMultiplier"].ToDataString();
                Clearable = row["Clearable"].ToDataString();
                StartDate = row["StartDate"].ToDataString();
                EndDate = row["EndDate"].ToDataString();
                StripId = row["StripId"].ToDataString();
                StripType = row["StripType"].ToDataString();
                StripName = row["StripName"].ToDataString();
                HubId = row["HubId"].ToDataString();
                HubName = row["HubName"].ToDataString();
                HubAlias = row["HubAlias"].ToDataString();
                UnderlyingUnitOfMeasure = row["UnderlyingUnitOfMeasure"].ToDataString();
                PriceDenomination = row["PriceDenomination"].ToDataString();
                PriceUnit = row["PriceUnit"].ToDataString();
                Granularity = row["Granularity"].ToDataString();
                NumOfDecimalPrice = row["NumOfDecimalPrice"].ToDataString();
                NumOfDecimalQty = row["NumOfDecimalQty"].ToDataString();
                ProductId = row["ProductId"].ToDataString();
                ProductName = row["ProductName"].ToDataString();
                ProductDescription = row["ProductDescription"].ToDataString();
                TickValue = row["TickValue"].ToDataString();
                ImpliedType = row["ImpliedType"].ToDataString();
                PrimaryLegSymbol = row["PrimaryLegSymbol"].ToDataString();
                SecondaryLegSymbol = row["SecondaryLegSymbol"].ToDataString();
                IncrementStrike = row["IncrementStrike"].ToDataString();
                MinStrike = row["MinStrike"].ToDataString();
                MaxStrike = row["MaxStrike"].ToDataString();
                #endregion
            }
        }

        public TradeDetails(TradeCapture trade)
        {
            if (trade != null)
            {
                #region properties
                CFICode = trade.CFICode;
                ClOrdID = trade.ClOrdID;
                ClearingAccountId = trade.ClearingAccountId;
                ClearingFirm = trade.ClearingFirm;
                Exchange = trade.Exchange;
                ExecID = trade.ExecID;
                ExecType = trade.ExecType;
                ExecutingFirm = trade.ExecutingFirm;
                LegRefID = trade.LegRefID;
                NumOfCycles = trade.NumOfCycles.ToDataString();
                NumOfLots = trade.NumOfLots.ToDataString();
                OrdStatus = trade.OrdStatus;
                OrderID = trade.OrderID;
                OrigTradeID = trade.OrigTradeID;
                OriginationFirm = trade.OriginationFirm;
                OriginationTrader = trade.OriginationTrader;
                LastPx = trade.Price.ToDataString();
                LastQty = trade.Quantity.ToDataString();
                SecurityID = trade.SecurityID;
                SecurityIDSource = trade.SecurityIDSource;
                Side = trade.Side;
                Symbol = trade.Symbol;
                TimeStamp = trade.TimeStamp.ToDataString();
                TradeDate = trade.TradeDate.ToDataString();
                TradeEndDate = trade.TradeEndDate.ToDataString();
                TradeCaptureId = trade.TradeId.ToDataString();
                TradeReportID = trade.TradeReportID;
                TradeReportTransType = trade.TradeReportTransType;
                TradeStartDate = trade.TradeStartDate.ToDataString();
                TransactTime = trade.TransactTime.ToDataString();
                TrdType = trade.TrdType;

                Clearable = trade.SecurityDefinition.Clearable.ToDataString();
                EndDate = trade.SecurityDefinition.EndDate;
                Granularity = trade.SecurityDefinition.Granularity;
                HubAlias = trade.SecurityDefinition.HubAlias;
                HubId = trade.SecurityDefinition.HubId.ToDataString();
                HubName = trade.SecurityDefinition.HubName;
                ImpliedType = trade.SecurityDefinition.ImpliedType;
                IncrementPrice = trade.SecurityDefinition.IncrementPrice.ToDataString();
                IncrementQty = trade.SecurityDefinition.IncrementQty.ToDataString();
                IncrementStrike = trade.SecurityDefinition.IncrementStrike.ToDataString();
                LotSize = trade.SecurityDefinition.LotSize.ToDataString();
                LotSizeMultiplier = trade.SecurityDefinition.LotSizeMultiplier.ToDataString();
                MaxStrike = trade.SecurityDefinition.MaxStrike.ToDataString();
                MinStrike = trade.SecurityDefinition.MinStrike.ToDataString();
                NumOfDecimalPrice = trade.SecurityDefinition.NumOfDecimalPrice.ToDataString();
                NumOfDecimalQty = trade.SecurityDefinition.NumOfDecimalQty.ToDataString();
                PriceDenomination = trade.SecurityDefinition.PriceDenomination;
                PriceUnit = trade.SecurityDefinition.PriceUnit;
                PrimaryLegSymbol = trade.SecurityDefinition.PrimaryLegSymbol;
                ProductDescription = trade.SecurityDefinition.ProductDescription;
                ProductId = trade.SecurityDefinition.ProductId.ToDataString();
                ProductName = trade.SecurityDefinition.ProductName;
                SecondaryLegSymbol = trade.SecurityDefinition.SecondaryLegSymbol;
                SecurityDefinitionId = trade.SecurityDefinition.SecurityDefinitionId.ToDataString();
                StartDate = trade.SecurityDefinition.StartDate;
                StripId = trade.SecurityDefinition.StripId.ToDataString();
                StripName = trade.SecurityDefinition.StripName;
                TickValue = trade.SecurityDefinition.TickValue.ToDataString();
                UnderlyingCFICode = trade.SecurityDefinition.UnderlyingCFICode;
                UnderlyingContractMultiplier = trade.SecurityDefinition.UnderlyingContractMultiplier.ToDataString();
                UnderlyingMaturityDate = trade.SecurityDefinition.UnderlyingMaturityDate;
                UnderlyingSecurityDesc = trade.SecurityDefinition.UnderlyingSecurityDesc;
                UnderlyingSecurityID = trade.SecurityDefinition.UnderlyingSecurityID;
                UnderlyingSecurityIDSource = trade.SecurityDefinition.UnderlyingSecurityIDSource;
                UnderlyingSymbol = trade.SecurityDefinition.UnderlyingSymbol;
                UnderlyingUnitOfMeasure = trade.SecurityDefinition.UnderlyingUnitOfMeasure;
                #endregion
            }
        }

        public TradeDetails(TradeTransferErrorDetails tradeTransferErrorDetails)
        {
            Exchange = tradeTransferErrorDetails.Exchange;
            UnderlyingSecurityID = tradeTransferErrorDetails.UnderlyingSecurityID;
            UnderlyingSecurityIDSource = tradeTransferErrorDetails.UnderlyingSecurityIDSource;
        }
    }
}