using Mandara.Entities.Entities;
using Mandara.Extensions.AppSettings;
using NLog;
using System;
using System.Data.Common;

namespace Mandara.Entities
{
    using System.Data.Entity;

    [DbConfigurationType(typeof(DbTracingConfiguration))]
    public partial class MandaraEntities : DbContext
    {
        private static ILogger Logger;
        private static readonly bool LogDbInteraction;
        public const string DefaultConnStrName = "MandaraEntities";
        public string CallerId { get; }

        static MandaraEntities()
        {
            LogDbInteraction =
                Mandara.Extensions.AppSettings.AppSettings.FlagEnabled(DbTracingConfiguration.DbTracingKey);
        }

        public MandaraEntities() : this(DefaultConnStrName)
        {
        }

        public MandaraEntities(string connStrName) : base($"name = {connStrName}")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            CallerId = String.Empty;
            SetDatabaseLogger();
        }

        private void SetDatabaseLogger()
        {
            if (LogDbInteraction)
            {
                Logger = LogManager.GetLogger("DbTracing");
                Database.Log = Logger.Trace;
            }
        }

        public MandaraEntities(string connStrName, string callerId)
            : base($"name = {connStrName}")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            CallerId = callerId;
            SetDatabaseLogger();
        }

        public MandaraEntities(DbConnection conn, bool contextOwnsConnection)
            : this(conn, String.Empty, contextOwnsConnection)
        {
        }

        public MandaraEntities(DbConnection conn, string callerId, bool contextOwnsConnection)
            : base(conn, contextOwnsConnection)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            CallerId = callerId ?? String.Empty;
            SetDatabaseLogger();
        }

        public virtual DbSet<CalendarExpiryDate> CalendarExpiryDates { get; set; }
        public virtual DbSet<CalendarHoliday> CalendarHolidays { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SourceData> SourceDataSet { get; set; }
        public virtual DbSet<StockCalendar> StockCalendars { get; set; }
        public virtual DbSet<YahooMessage> YahooMessages { get; set; }
        public virtual DbSet<SourceDetail> SourceDetails { get; set; }
        public virtual DbSet<ComplexProduct> ComplexProducts { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductClass> ProductClasses { get; set; }
        public virtual DbSet<OfficialProduct> OfficialProducts { get; set; }
        public virtual DbSet<ParserWord> ParserWords { get; set; }
        public virtual DbSet<ParserDefaultProduct> ParserDefaultProducts { get; set; }
        public virtual DbSet<SourceDataMessage> SourceDataMessages { get; set; }
        public virtual DbSet<ParsedProduct> ParsedProducts { get; set; }
        public virtual DbSet<Exchange> Exchanges { get; set; }
        public virtual DbSet<ProductAlias> ProductAliases { get; set; }
        public virtual DbSet<Broker> Brokers { get; set; }
        public virtual DbSet<ParserServiceCaptcha> ParserServiceCaptchas { get; set; }
        public virtual DbSet<ReportSettings> ReportSettings { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<DeliveredProduct> DeliveredProducts { get; set; }
        public virtual DbSet<PortfolioTrade> PortfolioTrades { get; set; }
        public virtual DbSet<Portfolio> Portfolios { get; set; }
        public virtual DbSet<PortfolioClearingAccount> PortfolioClearingAccounts { get; set; }
        public virtual DbSet<PortfolioAbnAccount> PortfolioAbnAccounts { get; set; }
        public virtual DbSet<AuditMessage> AuditMessages { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TradeCapture> TradeCaptures { get; set; }
        public virtual DbSet<FixMessage> FixMessages { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<UserAlias> UserAliases { get; set; }
        public virtual DbSet<UserPortfolioPermission> UserPortfolioPermissions { get; set; }
        public virtual DbSet<TradeGroup> TradeGroups { get; set; }
        public virtual DbSet<SecurityDefinition> SecurityDefinitions { get; set; }
        public virtual DbSet<TradeTransferError> TradeTransferErrors { get; set; }
        public virtual DbSet<TradeChange> TradeChanges { get; set; }
        public virtual DbSet<PnlReport> PnlReports { get; set; }
        public virtual DbSet<PnlReportPortfolio> PnlReportPortfolios { get; set; }
        public virtual DbSet<HALProduct> HALProducts { get; set; }
        public virtual DbSet<VHALMessage> VHALMessages { get; set; }
        public virtual DbSet<SpanCalculatorService> SpanCalculatorServices { get; set; }
        public virtual DbSet<TestPnlBookpnl> TestPnlBookpnl { get; set; }
        public virtual DbSet<TestPnlSnapshot> TestPnlSnapshot { get; set; }
        public virtual DbSet<TestPnlTradepnl> TestPnlTradepnl { get; set; }
        public virtual DbSet<CompanyAlias> CompanyAlias { get; set; }
        public virtual DbSet<ProductBrokerage> ProductBrokerage { get; set; }
        public virtual DbSet<SealDetail> SealDeatails { get; set; }
        public virtual DbSet<IceProductMapping> ice_product_mappings { get; set; }
        public virtual DbSet<ABNMappings> ABNMapping { get; set; }
        public virtual DbSet<TradeScenario> TradeScenarios { get; set; }
        public virtual DbSet<AlertEmailAddress> AlertEmailAddresses { get; set; }
        public virtual DbSet<AlertPhone> AlertPhones { get; set; }
        public virtual DbSet<AdministrativeAlertGroup> AdministrativeAlertGroups { get; set; }
        public virtual DbSet<AdministrativeAlert> AdministrativeAlerts { get; set; }
        public virtual DbSet<GmiBalmoCode> GmiBalmoCodes { get; set; }
        public virtual DbSet<IceBalmoMapping> IceBalmoMappings { get; set; }
        public virtual DbSet<AlertHistory> AlertHistories { get; set; }
        public virtual DbSet<TradeSupportAlert> TradeSupportAlerts { get; set; }
        public virtual DbSet<AdminAlertVoiceCall> AdminAlertVoiceCalls { get; set; }
        public virtual DbSet<AlertVoicePhone> AlertVoicePhones { get; set; }
        public virtual DbSet<UserProductCategoryPortfolio> UserProductCategoryPortfolios { get; set; }
        public virtual DbSet<SwapCrossPerProduct> SwapCrossPerProducts { get; set; }
        public virtual DbSet<TradeTemplate> TradeTemplates { get; set; }
        public virtual DbSet<PnlReportEod> PnlReportEods { get; set; }
        public virtual DbSet<CompanyBrokerage> CompanyBrokerages { get; set; }
        public virtual DbSet<IMQMessage> IMQMessages { get; set; }
        public virtual DbSet<IMQQuote> IMQQuotes { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<PerformanceLogMessage> PerformanceLogMessages { get; set; }
        public virtual DbSet<PrecalcSdDetail> PrecalcSdDetails { get; set; }
        public virtual DbSet<PrecalcTcDetail> PrecalcTcDetails { get; set; }
        public virtual DbSet<PrecalcSourcedetailsDetail> PrecalcSourcedetailsDetails { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<FxPair> FxPairs { get; set; }
        public virtual DbSet<FxTrade> FxTrades { get; set; }

        public virtual DbSet<ForeignCurrencyPosition> ForeignCurrencyPositions { get; set; }

        public virtual DbSet<ForeignCurrencyPositionDetail> ForeignCurrencyPositionDetails { get; set; }
        public virtual DbSet<FxOfficialProductPnLMap> FxOfficialProductPnLMaps { get; set; }

        public virtual DbSet<ExchangeAccount> ExchangeAccounts { get; set; }

        public virtual DbSet<PlattsSymbolConfig> PlattsSymbolConfig { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarExpiryDate>().HasKey(t => new { t.CalendarId, t.FuturesDate });
            modelBuilder.Entity<CalendarHoliday>().HasKey(t => new { t.CalendarId, t.HolidayDate });
            modelBuilder.Entity<ProductBrokerage>().HasKey(t => new { t.ProductId, t.CompanyId });
            modelBuilder.Entity<TestPnlTradepnl>().HasKey(t => new { t.BookpnlId, t.TradeId });
            modelBuilder.Entity<TradeTransferError>().HasKey(t => new { t.EntityId, t.EntityTypeDb });
            modelBuilder.Entity<UserAlias>().HasKey(t => new { t.UserId, t.Alias });
            modelBuilder.Entity<UserPortfolioPermission>().HasKey(t => new { t.UserId, t.PortfolioId });
            modelBuilder.Entity<UserProductCategoryPortfolio>().HasKey(t => new { t.user_id, t.category_id });

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.Emails)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.AlertGroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.Phones)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.AlertGroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.VoicePhones)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.AlertGroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.adm_alerts)
                .WithOptional(e => e.AlertGroup1)
                .HasForeignKey(e => e.LevelOneGroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.adm_alerts1)
                .WithOptional(e => e.AlertGroup2)
                .HasForeignKey(e => e.LevelTwoGroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.adm_alerts2)
                .WithOptional(e => e.AlertGroup3)
                .HasForeignKey(e => e.Level3GroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.adm_alerts3)
                .WithOptional(e => e.AlertGroup4)
                .HasForeignKey(e => e.Level4GroupId);

            modelBuilder.Entity<AdministrativeAlertGroup>()
                .HasMany(e => e.Users)
                .WithMany(e => e.AlertGroups)
                .Map(m => m.ToTable("adm_alert_group_users").MapLeftKey("alert_group_id").MapRightKey("user_id"));

            modelBuilder.Entity<Broker>()
                .HasMany(e => e.IMQMessages)
                .WithRequired(e => e.Broker)
                .HasForeignKey(e => e.BrokerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Broker>()
                .HasMany(e => e.ParserWords)
                .WithOptional(e => e.Broker)
                .HasForeignKey(e => e.BrokerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Broker>()
                .HasMany(e => e.VHALMessages)
                .WithOptional(e => e.Broker)
                .HasForeignKey(e => e.BrokerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Broker>()
                .HasMany(e => e.YahooMessages)
                .WithRequired(e => e.Broker)
                .HasForeignKey(e => e.BrokerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Broker>()
                .HasOptional(e => e.ParserDefaultProduct)
                .WithRequired(e => e.Broker);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.CompanyAliases)
                .WithRequired(e => e.Company)
                .HasForeignKey(e => e.CompanyId);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.Brokerages)
                .WithRequired(e => e.Company)
                .HasForeignKey(e => e.CompanyId);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.Brokers)
                .WithOptional(e => e.Company)
                .HasForeignKey(e => e.CompanyId);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.ProductsBrokerages)
                .WithRequired(e => e.Company)
                .HasForeignKey(e => e.CompanyId);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Permissions)
                .WithMany(e => e.Groups)
                .Map(m => m.ToTable("groups_permissions").MapLeftKey("group_id").MapRightKey("permission_id"));

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Groups)
                .Map(m => m.ToTable("users_groups").MapLeftKey("group_id").MapRightKey("user_id"));

            modelBuilder.Entity<IMQMessage>()
                .HasMany(e => e.ParsedProducts)
                .WithRequired(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.HALProducts)
                .WithRequired(e => e.OfficialProduct)
                .HasForeignKey(e => e.OfficialProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.IMQQuotes)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.OfficialProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.official_products1)
                .WithOptional(e => e.official_products2)
                .HasForeignKey(e => e.SettlementProductId);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.OfficialProduct)
                .HasForeignKey(e => e.OfficialProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.MonthlyProducts)
                .WithOptional(e => e.MonthlyOfficialProduct)
                .HasForeignKey(e => e.MonthlyOfficialProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.products1)
                .WithOptional(e => e.TasOfficialProduct)
                .HasForeignKey(e => e.TasOfficialProductId);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.ParsedProducts)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.OfficialProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.ParserWords)
                .WithOptional(e => e.OfficialProduct)
                .HasForeignKey(e => e.OfficialProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OfficialProduct>()
                .HasRequired(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId);

            modelBuilder.Entity<OfficialProduct>()
                .HasOptional(e => e.PlattsSymbolConfig)
                .WithRequired(e => e.OfficialProduct);


            modelBuilder.Entity<PnlReport>()
                .HasMany(e => e.PnlReportsPortfolios)
                .WithRequired(e => e.PnlReport)
                .HasForeignKey(e => e.ReportId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.UserProductGroups)
                .WithRequired(e => e.Portfolio)
                .HasForeignKey(e => e.portfolio_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.PortfolioTrades)
                .WithRequired(e => e.Portfolio)
                .HasForeignKey(e => e.PortfolioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Portfolio)
                .HasForeignKey(e => e.DefaultPortfolioId);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.Portfolios)
                .WithOptional(e => e.ParentPortfolio)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.trade_captures)
                .WithOptional(e => e.BuyBook)
                .HasForeignKey(e => e.BuyBookID);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.trade_captures1)
                .WithOptional(e => e.Portfolio)
                .HasForeignKey(e => e.PortfolioId);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.trade_captures2)
                .WithOptional(e => e.SellBook)
                .HasForeignKey(e => e.SellBookID);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.UserPortfolioPermissions)
                .WithRequired(e => e.Portfolio)
                .HasForeignKey(e => e.PortfolioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.pnl_reports_portfolios)
                .WithRequired(e => e.Portfolio)
                .HasForeignKey(e => e.PortfolioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.adm_alerts)
                .WithOptional(e => e.Portfolio)
                .HasForeignKey(e => e.PortfolioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PortfolioClearingAccount>()
                        .HasRequired(portfolioClrAcc => portfolioClrAcc.AccountPortfolio);

            modelBuilder.Entity<PortfolioAbnAccount>()
                        .HasRequired(portfolioAbnAcc => portfolioAbnAcc.AbnPortfolio);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.adm_alerts)
                .WithOptional(e => e.ProductGroup)
                .HasForeignKey(e => e.ProductGroupId);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.UserPortfolios)
                .WithRequired(e => e.ProductCategory)
                .HasForeignKey(e => e.category_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.SwapCrossPerProducts)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Category)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.products1)
                .WithOptional(e => e.CategoryOverride)
                .HasForeignKey(e => e.CategoryOverrideId);

            modelBuilder.Entity<ProductCategory>().HasMany<ProductClass>(cat => cat.Classes)
                        .WithMany(prodClass => prodClass.Categories).Map(
                            prodCatClass =>
                            {
                                prodCatClass.MapLeftKey("category_id");
                                prodCatClass.MapRightKey("product_class_id");
                                prodCatClass.ToTable("product_category_class");
                            });

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ABNMappings)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.ComplexProduct)
                .WithRequired(e => e.Product);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ComplexParentProducts1)
                .WithOptional(e => e.ChildProduct1)
                .HasForeignKey(e => e.ChildProduct1_Id);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ComplexParentProducts2)
                .WithOptional(e => e.ChildProduct2)
                .HasForeignKey(e => e.ChildProduct2_Id);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.GmiBalmoCodes)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.IceBalmoMappings)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.IceProductsMappings)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.InternalProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Aliases)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.product_categories)
                .WithOptional(e => e.SpreaderProduct)
                .HasForeignKey(e => e.SpreaderProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.product_categories1)
                .WithOptional(e => e.SwapCrossProduct)
                .HasForeignKey(e => e.SwapCrossProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.product_categories1_1)
                .WithOptional(e => e.TransferProduct)
                .HasForeignKey(e => e.TransferProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.product_categories3)
                .WithOptional(e => e.SwapCrossBalmoProduct)
                .HasForeignKey(e => e.SwapCrossBalmoProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ComplexProductBalmos)
                .WithOptional(e => e.BalmoOnComplexProduct)
                .HasForeignKey(e => e.BalmoOnComplexProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.CrudeSwapBalmos)
                .WithOptional(e => e.BalmoOnCrudeProduct)
                .HasForeignKey(e => e.BalmoOnCrudeProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.CompaniesBrokerages)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.swap_cross_per_product)
                .WithRequired(e => e.CategoryProduct)
                .HasForeignKey(e => e.CategoryProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.swap_cross_per_product1)
                .WithRequired(e => e.BalmoSwapCrossProduct)
                .HasForeignKey(e => e.BalmoSwapCrossProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.trade_scenarios)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.product_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.security_definitions)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.product_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.adm_alerts)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.ProductID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.UnderlyingFuturesOverride)
                .WithMany(e => e.UnderlyingFuturesOverridesProducts)
                .HasForeignKey(e => e.UnderlyingFuturesOverrideId);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.Unit)
                .WithMany()
                .HasForeignKey(e => e.UnitId);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.HolidaysCalendar)
                .WithMany()
                .HasForeignKey(e => e.holidays_calendar_id);

            modelBuilder.Entity<CompanyBrokerage>()
                .HasOptional(e => e.Unit)
                .WithMany()
                .HasForeignKey(e => e.UnitId);

            modelBuilder.Entity<Region>()
                .HasMany(e => e.OfficialProducts)
                .WithOptional(e => e.Region)
                .HasForeignKey(e => e.RegionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Region>()
                .HasMany(e => e.Companies)
                .WithOptional(e => e.Region)
                .HasForeignKey(e => e.RegionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SecurityDefinition>()
                .HasMany(e => e.TradeCaptures)
                .WithRequired(e => e.SecurityDefinition)
                .HasForeignKey(e => e.SecurityDefinitionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SecurityDefinition>()
                .HasMany(e => e.PrecalcDetails)
                .WithRequired(e => e.SecurityDefinition)
                .HasForeignKey(e => e.SecurityDefinitionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SecurityDefinition>()
                .HasMany(e => e.SourceDetails)
                .WithOptional(e => e.SecurityDefinition)
                .HasForeignKey(e => e.SecurityDefinitionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SourceData>()
                .HasMany(e => e.SourceDetails)
                .WithRequired(e => e.SourceData)
                .HasForeignKey(e => e.SourceDataId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SourceData>()
                .HasMany(e => e.SealDetails)
                .WithRequired(e => e.SourceData)
                .HasForeignKey(e => e.SourceDataId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SourceDetail>()
                .HasMany(e => e.PrecalcDetails)
                .WithRequired(e => e.SourceDetail)
                .HasForeignKey(e => e.SourceDetailId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SourceDetail>()
                .HasOptional(e => e.SecurityDefinition)
                .WithMany(e => e.SourceDetails)
                .HasForeignKey(e => e.SecurityDefinitionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StockCalendar>()
                .HasMany(e => e.FuturesExpiries)
                .WithRequired(e => e.StockCalendar)
                .HasForeignKey(e => e.CalendarId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StockCalendar>()
                .HasMany(e => e.Holidays)
                .WithRequired(e => e.StockCalendar)
                .HasForeignKey(e => e.CalendarId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StockCalendar>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.ExpiryCalendar)
                .HasForeignKey(e => e.calendar_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TestPnlBookpnl>()
                .HasMany(e => e.TestPnlTradepnls)
                .WithRequired(e => e.TestPnlBookpnl)
                .HasForeignKey(e => e.BookpnlId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TestPnlSnapshot>()
                .HasMany(e => e.TestPnlBookpnls)
                .WithRequired(e => e.TestPnlSnapshot)
                .HasForeignKey(e => e.SnapshotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TradeCapture>()
                .HasMany(e => e.TradeChanges)
                .WithRequired(e => e.TradeCapture)
                .HasForeignKey(e => e.TradeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TradeCapture>()
                .HasMany(e => e.PrecalcDetails)
                .WithRequired(e => e.TradeCapture)
                .HasForeignKey(e => e.TradeCaptureId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TradeGroup>()
                .HasMany(e => e.TradeCaptures)
                .WithOptional(e => e.TradeGroup)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AuditMessages)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ProductGroupPortfolios)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserAliases)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PortfolioPermissions)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VHALMessage>()
                .HasMany(e => e.HALProducts)
                .WithRequired(e => e.VHALMessage)
                .HasForeignKey(e => e.MessageId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<YahooMessage>()
                .HasMany(e => e.ParsedProducts)
                .WithRequired(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TradeTemplate>()
                .HasOptional(e => e.Unit)
                .WithMany()
                .HasForeignKey(e => e.UnitId);

            modelBuilder.Entity<FxTrade>()
                .HasRequired(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<FxTrade>()
                .HasRequired(e => e.TradeCapture)
                .WithMany()
                .HasForeignKey(e => e.TradeCaptureId);

            modelBuilder.Entity<SourceDetail>().Property(x => x.TradePrice).HasPrecision(18, 6);
            modelBuilder.Entity<TradeCapture>().Property(x => x.Price).HasPrecision(18, 6);
            modelBuilder.Entity<TradeCapture>().Property(x => x.Quantity).HasPrecision(18, 6);
            modelBuilder.Entity<TradeCapture>().Property(x => x.AveragePx).HasPrecision(18, 6);

            modelBuilder.Entity<Product>().Property(x => x.PositionFactor).HasPrecision(18, 8);
            modelBuilder.Entity<Product>().Property(x => x.PnlFactor).HasPrecision(18, 8);
            modelBuilder.Entity<Product>().Property(x => x.PriceConversionFactorDb).HasPrecision(18, 8);

            modelBuilder.Entity<ComplexProduct>().Property(x => x.ConversionFactor1).HasPrecision(18, 8);
            modelBuilder.Entity<ComplexProduct>().Property(x => x.ConversionFactor2).HasPrecision(18, 8);

            modelBuilder.Entity<FxTrade>().Property(x => x.Rate).HasPrecision(18, 6);
            modelBuilder.Entity<FxTrade>().Property(x => x.SpotRate).HasPrecision(18, 6);
            modelBuilder.Entity<FxTrade>().Property(x => x.SpecifiedAmount).HasPrecision(18, 6);
            modelBuilder.Entity<FxTrade>().Property(x => x.AgainstAmount).HasPrecision(18, 6);

            modelBuilder.Entity<ForeignCurrencyPosition>()
                .HasMany(e => e.ForeignCurrencyPositionDetails)
                .WithRequired(e => e.ForeignCurrencyPosition)
                .HasForeignKey(e => e.ForeignCurrencyPositionId);

            modelBuilder.Entity<FxOfficialProductPnLMap>().HasKey(officialProdMap => officialProdMap.CurrencyId);

            modelBuilder.Entity<FxPair>()
                        .HasRequired(e => e.FromCurrency)
                        .WithMany()
                        .HasForeignKey(e => e.FromCurrencyId);

            modelBuilder.Entity<FxPair>()
                        .HasRequired(e => e.ToCurrency)
                        .WithMany()
                        .HasForeignKey(e => e.ToCurrencyId);
        }
    }
}
