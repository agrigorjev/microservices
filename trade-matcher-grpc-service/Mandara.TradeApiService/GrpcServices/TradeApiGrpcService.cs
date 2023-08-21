

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcDefinitions;
using Mandara.TradeApiService.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Optional;
using Optional.Unsafe;
using System;
using System.Diagnostics.Metrics;
using static Mandara.TradeApiService.GrpcDefinitions.TradeApiGrpcService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Mandara.TradeApiService.GrpcServices;

public class TradeApiGrpcService : TradeApiGrpcServiceBase
{
    private readonly IDataStorage _cache;
    private readonly IDbContextFactory<MandaraEntities> _contextFactory;
    private readonly ITradesRepository _tradesRepository;

    private readonly DataConverters.UnitDataConverter unitConverter = new DataConverters.UnitDataConverter();
    private readonly DataConverters.ExchangeDataConverter exchangeConverter = new DataConverters.ExchangeDataConverter();
    private readonly DataConverters.TradeTemplateDataConverter tradeTemplateConverter = new DataConverters.TradeTemplateDataConverter();
    private readonly DataConverters.CompanyAliasDataConverter companyAliasConverter = new DataConverters.CompanyAliasDataConverter();
    private readonly DataConverters.PortfolioDataConverter portfolioConverter = new DataConverters.PortfolioDataConverter();
    private readonly DataConverters.OfficialProductToInstrumentDataConverter _officalProductToInstrument = new DataConverters.OfficialProductToInstrumentDataConverter();
    private readonly DataConverters.TradeCaptureToTradeAddDetailsDataConverter _tradeCaptureConverter = new DataConverters.TradeCaptureToTradeAddDetailsDataConverter();

    public TradeApiGrpcService(IDataStorage cache, IDbContextFactory<MandaraEntities> contextFactory, ITradesRepository tradesRepository)
    {
        _cache = cache;
        _contextFactory = contextFactory;
        _tradesRepository = tradesRepository;
    }

    public override Task<TradeAddPrerequisitesResponseGrpcMessage> GetTradeAddPrerequisites(TradeAddPrerequisitesRequestGrpcMessage request, ServerCallContext context)
    {
        TradeAddPrerequisitesResponseGrpcMessage responseMessage = new TradeAddPrerequisitesResponseGrpcMessage();
        List<Company> brokers;
        List<OfficialProduct> officialProducts;
        List<Exchange> exchanges;
        List<TradeTemplate> tradeTemplates;
        List<Unit> units;

        using (MandaraEntities cxt = _contextFactory.CreateDbContext())
        {
            brokers = cxt.Companies.Include(x => x.CompanyAliases).ToList();
            officialProducts = _cache.GetOfficialProducts().ToList();

            if (!request.IsMasterToolMode)
            {
                officialProducts = officialProducts.Where(it => it.IsAllowedForManualTrades).ToList();
            }

            exchanges = cxt.Exchanges
                .Include(x => x.Calendar)
                .OrderBy(x => x.Name).ToList();

            // TODO: Have something in the data layer load this.
            tradeTemplates =
                cxt.TradeTemplates
                    .Include(x => x.Exchange)
                    .Include(x => x.OfficialProduct)
                    .Include(x => x.Portfolio)
                    .Include(x => x.Unit)
                    .OrderBy(x => x.TemplateName)
                    .ToList();

            units = cxt.Units.ToList();
        }

        units.ForEach(unit => responseMessage.Units.Add(unitConverter.Convert(unit)));
        exchanges.ForEach(exchange => responseMessage.Exchanges.Add(exchangeConverter.Convert(exchange)));
        tradeTemplates.ForEach(tradeTemplate => responseMessage.TradeTemplates.Add(tradeTemplateConverter.Convert(tradeTemplate)));

        List<CompanyAliasGrpc> grpcBrokers = new List<CompanyAliasGrpc>();
        foreach (Company company in brokers)
        {
            if (company.CompanyAliases != null && company.CompanyAliases.Count > 0)
            {
                company.CompanyAliases.ToList().ForEach(companyalias => grpcBrokers.Add(companyAliasConverter.Convert(companyalias)));
            }
            else
            {
                grpcBrokers.Add(companyAliasConverter.Convert(new CompanyAlias { AliasName = company.CompanyName }));
            }
        }
        responseMessage.Brokers.AddRange(grpcBrokers.Where(x => x != null).OrderBy(x => x.AliasName));

        List<Portfolio> portfolios = _cache.GetPortfolios().ToList();
        portfolios.ForEach(portfolio => responseMessage.Portfolios.Add(portfolioConverter.Convert(portfolio)));

        var instruments = officialProducts.Select(
                x =>
                {
                    if (request.IsMasterToolMode)
                    {
                        return _officalProductToInstrument
                                    .ConvertOfficialProductToInstrumentForMasterToolMode(x);
                    }
                    else
                    {
                        return _officalProductToInstrument.Convert(x);
                    }
                })
                .Where(x => x.ExchangeUnits.Count > 0)
                .OrderBy(x => x.Name)
                .ToList();
        responseMessage.AvailableInstrumentsForExchange.AddRange(instruments);
            

        using (MandaraEntities dbContext = _contextFactory.CreateDbContext())
        {
            User? userPortfolio = dbContext.Users.Include(x => x.Portfolio)
                                          .SingleOrDefault(u => u.UserId == request.UserId);

            if (userPortfolio != null)
            {
                responseMessage.DefaultUserPortfolio = portfolioConverter.Convert(userPortfolio.Portfolio);
            }

            List<TradeCapture> tradesCaptures = GetTradeCaptures(request.TradeCaptureId);
            TradeAddDetailsGrpc? tradeAddOption = _tradeCaptureConverter.ConvertM(
                GetOfficialProductForSecurityDef(dbContext, _cache),
                GetFxTrade(dbContext),
                tradesCaptures,
                GetActionSetTradeDetailsFromParent(request.IsDuplicateMode));

            responseMessage.TradeAddDetails = tradeAddOption != null ? tradeAddOption : null;
        }

        if (request.TradeCaptureId > 0 && responseMessage.TradeAddDetails == null)
        {
            responseMessage.ErrorMessage =
                $"Unable to locate a parent trade of a time spread. Trade ID [{request.TradeCaptureId}]";
        }

        return Task.FromResult(responseMessage);
    }

    public Action<TradeAddDetailsGrpc, TradeCapture, IEnumerable<int>> GetActionSetTradeDetailsFromParent(
    bool isDuplicateMode)
    {
        if (!isDuplicateMode)
        {
            return SetNonDuplicateModeTradeAddDetails;
        }

        return (tradeDetails, parentTrade, trades) => { };
    }

    private static void SetNonDuplicateModeTradeAddDetails(
        TradeAddDetailsGrpc tradeAddDetails,
        TradeCapture firstParentTrade,
        IEnumerable<int> tradeIds)
    {
        tradeAddDetails.TradeCaptureIds.AddRange(tradeIds.ToList());
        tradeAddDetails.GroupId = firstParentTrade.GroupId;
        tradeAddDetails.TradeDate = firstParentTrade.TradeDate.toProtoTimestamp();
        tradeAddDetails.TimestampUtc = firstParentTrade.TimeStamp?.toProtoTimestamp();
        tradeAddDetails.TransactTimeUtc = firstParentTrade.TransactTime?.toProtoTimestamp();
    }

    private List<TradeCapture> GetTradeCaptures(int tradeCaptureId)
    {
        if (tradeCaptureId == 0)
        {
            return new List<TradeCapture>();
        }

        TradeCapture? trade;

        using (MandaraEntities cxt = _contextFactory.CreateDbContext())
        {
            trade = cxt.TradeCaptures
                .Include(x => x.SecurityDefinition)
                .FirstOrDefault(x => x.TradeId == tradeCaptureId);
        }

        if (trade == null)
        {
            return new List<TradeCapture>();
        }

        List<TradeCapture> tradeCaptures = new List<TradeCapture> { trade };

        //part 1: if it's an exchange trade we check for a spread group
        if (trade.TradeType == null)
        {
            List<TradeCapture> spreadGroup = _tradesRepository.GetFullSpreadTrades(trade);

            if (spreadGroup.Count > 0)
            {
                tradeCaptures.Clear();
                tradeCaptures.AddRange(spreadGroup);
            }
        }

        // part 2: if it's a manual trade we check for a trade group
        if (trade.TradeType.HasValue)
        {
            if (trade.GroupId.HasValue)
            {
                List<TradeCapture> groupTrades = _tradesRepository.GetTradesWithSameGroup(
                    trade.GroupId.Value,
                    "Filled");

                if (groupTrades.Count > 0)
                {
                    tradeCaptures.Clear();
                    tradeCaptures.AddRange(groupTrades);
                }
            }
        }

        tradeCaptures.ForEach(tc => SetTradeMasterProperties(tc));

        return tradeCaptures;
    }

    private void SetTradeSecurityDefinition(TradeCapture trade)
    {
        SecurityDefinition tradeSecDef = trade.SecurityDefinition;

        if (null == tradeSecDef)
        {
            SecurityDefinition inMemorySecDef =
                _cache.GetSecurityDefinition(trade.SecurityDefinitionId).ValueOrDefault();

            trade.SecurityDefinition = inMemorySecDef;
        }
    }

    private bool SetTradeSecurityDefinitionProduct(TradeCapture trade)
    {
        int productId = trade.SecurityDefinition.product_id.Value;
        Option<Product> product = _cache.GetProduct(productId);

        if (!product.HasValue)
        {
            //_log.Error(
            //    "SetTradeMasterProperties: Product not found in products cache, skipping trade, product id "
            //    + "[{0}], trade id [{1}]",
            //    productId,
            //    trade.TradeId);
            return false;
        }

        trade.SecurityDefinition.Product = product.ValueOrDefault();
        return true;
    }

    private void SetTradeMasterProperties(TradeCapture trade)
    {
        SetTradeSecurityDefinition(trade);

        if (!SetTradeSecurityDefinitionProduct(trade))
        {
            return;
        }

        if (trade.PortfolioId.HasValue)
        {
            Portfolio tradePortfolio = GetPortfolio(trade.PortfolioId.Value);

            trade.Portfolio = tradePortfolio.IsDefault() ? trade.Portfolio : tradePortfolio;
        }

        if (trade.SellBookID.HasValue)
        {
            Portfolio sellPortfolio = GetPortfolio(trade.SellBookID.Value);

            trade.SellBook = sellPortfolio.IsDefault() ? trade.SellBook : sellPortfolio;
        }

        if (trade.BuyBookID.HasValue)
        {
            Portfolio buyPortfolio = GetPortfolio(trade.BuyBookID.Value);

            trade.BuyBook = buyPortfolio.IsDefault() ? trade.BuyBook : buyPortfolio;
        }

        if (trade.GroupId.HasValue)
        {
            trade.TradeGroup = new TradeGroup { GroupId = trade.GroupId.Value };
        }

        Portfolio GetPortfolio(int portfolioId)
        {
            return _cache.GetPortfolio(portfolioId).ValueOr(Portfolio.Default);
        }
    }

    private Func<int, OfficialProduct?> GetOfficialProductForSecurityDef(
                MandaraEntities dbContext,
                IDataStorage store)
    {
        return (secDefId) =>
        {
            SecurityDefinition? secDef = dbContext.SecurityDefinitions.SingleOrDefault(secDef => secDef.SecurityDefinitionId == secDefId);

            Product? product = dbContext.Products.SingleOrDefault(prod => prod.ProductId == secDef.ProductId);

            return store.GetOfficialProduct(product.OfficialProductId).ValueOrDefault();
        };
    }


    private Func<int, FxTrade?> GetFxTrade(MandaraEntities dbContext)
    {
        return (tradeId) =>
        {
            FxTrade? trade = dbContext.FxTrades.SingleOrDefault(fxTrade => fxTrade.TradeCaptureId == tradeId);

            return trade;
        };
    }

    private TradeAddDetailsGrpc SetFxTradeDetails(
    FxTrade? trade,
    int firstParentTradeId,
    TradeAddDetailsGrpc tradeAddDetails)
    {
        if (trade is null)
        {
            return tradeAddDetails;
        }

        InstrumentGrpc instrument1 =
            _officalProductToInstrument.Convert(trade.Product.OfficialProduct);

        tradeAddDetails.SpecifiedAmount = trade.SpecifiedAmount;
        tradeAddDetails.FxExchangeRate = trade.Rate;
        tradeAddDetails.ForwardValueDate = trade.ValueDate.ToTimestamp();
        tradeAddDetails.FxSelectedInstrument = instrument1;
        tradeAddDetails.IsSpot = trade.ProductType == FxProductTypes.Spot;
        return tradeAddDetails;
    }

}