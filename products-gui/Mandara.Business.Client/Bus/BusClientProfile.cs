using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.ClosingPositions;
using Mandara.Business.Bus.Messages.DailyReconciliation;
using Mandara.Business.Bus.Messages.EditTrades;
using Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation;
using Mandara.Business.Bus.Messages.Historical;
using Mandara.Business.Bus.Messages.Portfolio;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Business.Bus.Messages.PricingReport;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Business.Bus.Messages.TradeImpact;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Business.TradeAdd;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Dto;
using Mandara.Entities.Import;
using Mandara.Entities.Positions;

namespace Mandara.Business.Bus
{
    public class BusClientProfile : BaseProfile
    {
        public static void InitAutomapper()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.AddProfile<BusClientProfile>();
                });
        }

        public BusClientProfile()
        {
            Configure();
        }

        protected void Configure()
        {
            CreatePortfolioTypeMaps();
            CreatePositionTypesMaps();
            MapTradesDtos();
            CreateHistoricalTypesMaps();
            CreatePricingReportTypesMaps();
            CreateTradeEditTypesMaps();
            CreateTradeImpactMessageTypesMaps();
            CreateTradeSwapExposureMessageTypesMaps();
            CreateClosingPositionsMessageTypesMaps();
            CreateLiveFeedReplayMessageTypesMaps();
            CreateTradeAddMessagesAndDataTypesMaps();
            CreateSecurityDefinitionMessageTypesMaps();
        }

        private void CreateSecurityDefinitionMessageTypesMaps()
        {
            CreateMap<SecurityDefinitionsSnapshotMessage, SecurityDefinitionsSnapshotMessageDto>();
            CreateMap<SecurityDefinitionsSnapshotMessageDto, SecurityDefinitionsSnapshotMessage>();
        }

        private void CreateTradeAddMessagesAndDataTypesMaps()
        {
            CreateMap<TradeAddPrerequisitesResponseMessage, TradeAddPrerequisitesResponseMessageDto>();
            CreateMap<TradeAddPrerequisitesResponseMessageDto, TradeAddPrerequisitesResponseMessage>()
                .ForMember(tradeAdd => tradeAdd.AvailableInstrumentsForExchange, option => option.Ignore())
                .ForMember(tradeAdd => tradeAdd.ExpiryExchanges, option => option.Ignore());

            CreateMap<TradeAddImpactRequestMessage, TradeAddImpactRequestMessageDto>()
                .AfterMap(
                    (src, dst) =>
                    {
                        NullParentPortfolio(dst.TradeAddDetails);
                    });

            CreateMap<TradeAddImpactRequestMessageDto, TradeAddImpactRequestMessage>();

            CreateMap<TradeAddImpactResponseMessage, TradeAddImpactResponseMessageDto>();
            CreateMap<TradeAddImpactResponseMessageDto, TradeAddImpactResponseMessage>();

            CreateMap<TradeAddDetails, TradeAddDetailsDto>(MemberList.Destination);
            CreateMap<TradeAddDetailsDto, TradeAddDetails>(MemberList.Source)
                .ForMember(tradeAdd => tradeAdd.AgainstAmount, option => option.Ignore());

            CreateMap<TradeAddImpact, TradeAddImpactDto>();
            CreateMap<TradeAddImpactDto, TradeAddImpact>()
                .ConstructUsing((dto) => new TradeAddImpact())
                .ForMember(tradeImpact => tradeImpact.CalcErrors, option => option.Ignore());

            CreateMap<TradeAddCreateRequestMessage, TradeAddCreateRequestMessageDto>()
                .AfterMap(
                    (src, dst) =>
                    {
                        NullParentPortfolio(dst.TradeAddDetails);
                    });

            CreateMap<TradeAddCreateRequestMessageDto, TradeAddCreateRequestMessage>();

            CreateMap<TradeAddCreateResponseMessage, TradeAddCreateResponseMessageDto>();
            CreateMap<TradeAddCreateResponseMessageDto, TradeAddCreateResponseMessage>();

            AddTwoWayMap<TradeTemplate, TradeTemplateDto>();
        }

        private static void NullParentPortfolio(TradeAddDetailsDto detail)
        {
            detail.Portfolio1.ParentPortfolio = null;
            if (detail.Portfolio2 != null)
            {
                detail.Portfolio2.ParentPortfolio = null;
            }
        }

        private void CreateLiveFeedReplayMessageTypesMaps()
        {
            CreateMap<LiveFeedReplaySnapshotMessage, LiveFeedReplaySnapshotMessageDto>();
            CreateMap<LiveFeedReplaySnapshotMessageDto, LiveFeedReplaySnapshotMessage>();
        }

        private void CreateClosingPositionsMessageTypesMaps()
        {
            CreateMap<ClosingPositionsTradesResponseMessage, ClosingPositionsTradesResponseMessageDto>();
            CreateMap<ClosingPositionsTradesResponseMessageDto, ClosingPositionsTradesResponseMessage>()
                .ForMember(positions => positions.ErrorMessage, option => option.Ignore());

            CreateMap<ClosingPositionsPlaceTradesRequestMessage, ClosingPositionsPlaceTradesRequestMessageDto>();
            CreateMap<ClosingPositionsPlaceTradesRequestMessageDto, ClosingPositionsPlaceTradesRequestMessage>();
        }

        private void CreateTradeSwapExposureMessageTypesMaps()
        {
            CreateMap<TransferTradesResponseMessage, TransferTradesResponseMessageDto>();
            CreateMap<TransferTradesResponseMessageDto, TransferTradesResponseMessage>();
        }

        private void CreateTradeImpactMessageTypesMaps()
        {
            CreateMap<TradeImpactRequestMessage, TradeImpactRequestMessageDto>();
            CreateMap<TradeImpactRequestMessageDto, TradeImpactRequestMessage>();

            CreateMap<TradeImpactResponseMessage, TradeImpactResponseMessageDto>();
            CreateMap<TradeImpactResponseMessageDto, TradeImpactResponseMessage>()
                .ForMember(tradeImpact => tradeImpact.CalcErrors, option => option.Ignore());
        }

        private void CreateTradeEditTypesMaps()
        {
            CreateMap<EditTradeParamsMessage, EditTradeParamsMessageDto>();
            CreateMap<EditTradeParamsMessageDto, EditTradeParamsMessage>();

            AddTwoWayMap<ProductAlias, ProductAliasDto>();

            AddTwoWayMap<CompanyAlias, CompanyAliasDto>();

            AddTwoWayMap<SecurityDefinition, SecurityDefinitionDto2>();

            CreateMap<TradesEditMessage, TradesEditMessageDto>();
            CreateMap<TradesEditMessageDto, TradesEditMessage>();
        }

        private void CreatePricingReportTypesMaps()
        {
            AddTwoWayMap<PricingCalculationDetail, PricingCalculationDetailDto>();

            CreateMap<TasSnapshotMessage, TasSnapshotMessageDto>();
            CreateMap<TasSnapshotMessageDto, TasSnapshotMessage>();
        }

        private void CreateHistoricalTypesMaps()
        {
            AddTwoWayMap<SourceDetail, SourceDetailDto>();

            CreateMap<ProductPriceDetail, ProductPriceDetailDto>();
            CreateMap<ProductPriceDetailDto, ProductPriceDetail>();

            AddTwoWayMap<OfficialProduct, OfficialProductDto>();

            CreateMap<CalculationDetail, CalculationDetailDto3>(MemberList.Destination)
                .ForMember(position => position.DailyDetails, option => option.Ignore())
                .AfterMap(
                    (src, dest) =>
                    {
                        dest.DailyDetails = src.DailyDetails?.ToArray().ToList() ?? new List<DailyDetail>();
                    });
            CreateMap<CalculationDetailDto3, CalculationDetail>(MemberList.Source)
                .ForMember(position => position.DailyDetails, option => option.Ignore())
                .AfterMap(
                    (src, dest) =>
                    {
                        dest.DailyDetails = new ConcurrentBag<DailyDetail>(src.DailyDetails ?? new List<DailyDetail>());
                    });

            CreateMap<HistoricalPnlSnapshotMessage, HistoricalPnlSnapshotMessageDto>();
            CreateMap<HistoricalPnlSnapshotMessageDto, HistoricalPnlSnapshotMessage>();

            CreateMap<HistoricalPositionsSnapshotMessage, HistoricalPositionsSnapshotMessageDto>();
            CreateMap<HistoricalPositionsSnapshotMessageDto, HistoricalPositionsSnapshotMessage>();
        }

        private void CreatePositionTypesMaps()
        {
            CreateMap<CalculationDetail, CalculationDetailDto>(MemberList.Destination)
                .ForMember(position => position.DailyDetails, option => option.Ignore())
                .AfterMap(
                    (src, dest) =>
                    {
                        dest.QuantityByDate = new SortedList<DateTime, DateQuantity>(src.QuantityByDate);
                        dest.DailyDetails = src.DailyDetails?.ToArray().ToList() ?? new List<DailyDetail>();
                    });

            CreateMap<CalculationDetailDto, CalculationDetail>(MemberList.Source)
                .ForMember(position => position.DailyDetails, option => option.Ignore())
                .AfterMap(
                    (src, dest) =>
                    {
                        dest.QuantityByDate = new SortedList<DateTime, DateQuantity>(
                            src.QuantityByDate ?? new SortedList<DateTime, DateQuantity>());
                        dest.DailyDetails = new ConcurrentBag<DailyDetail>(src.DailyDetails ?? new List<DailyDetail>());
                    });
            AddTwoWayMap<CalculationDetail, CalculationDetailDto2>();

            CreateMap<PositionsSnapshotMessage, PositionsSnapshotMessageDto>();
            CreateMap<PositionsSnapshotMessageDto, PositionsSnapshotMessage>();

            CreateMap<DailyReconciliationSnapshotMessage, DailyReconciliationSnapshotMessageDto>();
            CreateMap<DailyReconciliationSnapshotMessageDto, DailyReconciliationSnapshotMessage>();

            CreateMap<PositionsUpdateMessage, PositionsUpdateMessageDto>();
            CreateMap<PositionsUpdateMessageDto, PositionsUpdateMessage>();

            AddBasicTwoWayMap<PositionsWithTradesSnapshotMessage, PositionsWithTradesSnapshotMessageDto>();
        }

        private void CreatePortfolioTypeMaps()
        {
            AddTwoWayMapBlockingCircularRefs<Portfolio, PortfolioDto>();

            CreateMap<PortfolioEditMessage, PortfolioEditMessageDto>();
            CreateMap<PortfolioEditMessageDto, PortfolioEditMessage>();

            CreateMap<PortfolioUpdateMessage, PortfolioUpdateMessageDto>();
            CreateMap<PortfolioUpdateMessageDto, PortfolioUpdateMessage>();

            CreateMap<PortfolioResponseMessageDto, PortfolioResponseMessage>()
                .AfterMap(
                    (src, dest) =>
                    {
                        var sourcePortfolioMap = src.Portfolios.ToDictionary(x => x.PortfolioId, y => y);
                        var destPortfolioMap = dest.Portfolios.ToDictionary(x => x.PortfolioId, y => y);
                        foreach (var portfolio in dest.Portfolios)
                        {
                            var sourceObject = sourcePortfolioMap[portfolio.PortfolioId];
                            if (sourceObject.ParentId.HasValue)
                            {
                                portfolio.ParentId = sourceObject.ParentId.Value;
                                portfolio.ParentPortfolio = destPortfolioMap[portfolio.ParentId.Value];
                            }
                        }
                    });

            CreateMap<PortfolioResponseMessage, PortfolioResponseMessageDto>();
        }

        private void MapTradesDtos()
        {
            CreateMap<TradeCapture, TradeCaptureDto>().AfterMap(
                (src, dest) =>
                {
                    dest.TimeStamp = src.TimeStamp.HasValue
                        ? DateTime.SpecifyKind(src.TimeStamp.Value, DateTimeKind.Local).AsUtc()
                        : (DateTime?)null;
                    dest.TransactTime = src.UtcTransactTime;
                });
            CreateMap<TradeCaptureDto, TradeCapture>().AfterMap(
                (src, dest) =>
                {
                    dest.TimeStamp = src.TimeStamp.HasValue
                        ? DateTime.SpecifyKind(src.TimeStamp.Value, DateTimeKind.Utc).ToLocalTime()
                        : (DateTime?)null;
                    dest.TransactTime = src.UtcTransactTime.ToLocalTime();
                });

            AddTwoWayMap<SecurityDefinition, SecurityDefinitionDto>();

            AddTwoWayMap<TradeGroup, TradeGroupDto>();

            AddTwoWayMapBlockingCircularRefs<Portfolio, TradePortfolioDto>();

            AddTwoWayMap<Product, ProductDto>();
            AddTwoWayMap<Product, ProductDto2>();
            AddTwoWayMap<Product, ProductDto3>();

            AddTwoWayMap<ProductCategory, ProductCategoryDto>();

            AddTwoWayMap<OfficialProduct, OfficialProductDto2>();

            AddTwoWayMap<ComplexProduct, ComplexProductDto>();

            AddTwoWayMap<StockCalendar, CalendarDto>();

            AddTwoWayMap<Exchange, ExchangeDto>();

            CreateMap<TradesUpdateMessage, TradesUpdateMessageDto>();
            CreateMap<TradesUpdateMessageDto, TradesUpdateMessage>();

            CreateMap<TradesSnapshotMessage, TradesSnapshotMessageDto>();
            CreateMap<TradesSnapshotMessageDto, TradesSnapshotMessage>();

            CreateMap<FxTrade, FxTradeDto>(MemberList.Destination)
                //.IgnoreAll()
                .ForMember(target => target.AgainstAmount, member => member.MapFrom(src => src.AgainstAmount))
                .ForMember(target => target.AgainstCurrency, member => member.MapFrom(src => src.AgainstCurrency))
                .ForMember(target => target.FxProduct, member => member.MapFrom(src => src.Product))
                .ForMember(target => target.FxTradeId, member => member.MapFrom(src => src.FxTradeId))
                .ForMember(target => target.ProductType, member => member.MapFrom(src => src.ProductType))
                .ForMember(target => target.Rate, member => member.MapFrom(src => src.Rate))
                .ForMember(target => target.SpecifiedAmount, member => member.MapFrom(src => src.SpecifiedAmount))
                .ForMember(target => target.SpecifiedCurrency, member => member.MapFrom(src => src.SpecifiedCurrency))
                .ForMember(target => target.SpotRate, member => member.MapFrom(src => src.SpotRate))
                .ForMember(target => target.Tenor, member => member.MapFrom(src => src.Tenor))
                .ForMember(target => target.Trade, member => member.MapFrom(src => src.TradeCapture))
                .ForMember(target => target.ValueDate, member => member.MapFrom(src => src.ValueDate));
            CreateMap<FxTradeDto, FxTrade>(MemberList.Source)
                //.IgnoreAll()
                .ForMember(target => target.AgainstAmount, member => member.MapFrom(src => src.AgainstAmount))
                .ForMember(target => target.AgainstCurrency, member => member.MapFrom(src => src.AgainstCurrency))
                .ForMember(target => target.Product, member => member.MapFrom(src => src.FxProduct))
                .ForMember(target => target.FxTradeId, member => member.MapFrom(src => src.FxTradeId))
                .ForMember(target => target.ProductType, member => member.MapFrom(src => src.ProductType))
                .ForMember(target => target.Rate, member => member.MapFrom(src => src.Rate))
                .ForMember(target => target.SpecifiedAmount, member => member.MapFrom(src => src.SpecifiedAmount))
                .ForMember(target => target.SpecifiedCurrency, member => member.MapFrom(src => src.SpecifiedCurrency))
                .ForMember(target => target.SpotRate, member => member.MapFrom(src => src.SpotRate))
                .ForMember(target => target.Tenor, member => member.MapFrom(src => src.Tenor))
                .ForMember(target => target.TradeCapture, member => member.MapFrom(src => src.Trade))
                .ForMember(target => target.ValueDate, member => member.MapFrom(src => src.ValueDate));
        }
    }
}