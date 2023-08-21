using DevExpress.XtraEditors.Controls;

namespace Mandara.ProductGUI
{
    partial class ProductListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductListForm));
            DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition testTradeImpactLessThanZeroFormat = new DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition();
            this.testTradePositionsGrid = new DevExpress.XtraPivotGrid.PivotGridField();
            this.viewSelectorAndActionContainer = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.addProductGroup = new DevExpress.XtraBars.BarButtonItem();
            this.editProductGroup = new DevExpress.XtraBars.BarButtonItem();
            this.deleteProductGroup = new DevExpress.XtraBars.BarButtonItem();
            //this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            //this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            this.addOfficialProduct = new DevExpress.XtraBars.BarButtonItem();
            this.editOfficialProduct = new DevExpress.XtraBars.BarButtonItem();
            this.deleteOfficialProduct = new DevExpress.XtraBars.BarButtonItem();
            this.addProduct = new DevExpress.XtraBars.BarButtonItem();
            this.editProduct = new DevExpress.XtraBars.BarButtonItem();
            this.deleteProduct = new DevExpress.XtraBars.BarButtonItem();
            //this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            //this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            //this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
            //this.barButtonItem6 = new DevExpress.XtraBars.BarButtonItem();
            this.showProducts = new DevExpress.XtraBars.BarButtonItem();
            this.showOfficialProducts = new DevExpress.XtraBars.BarButtonItem();
            this.showOfficialProductDefaultProducts = new DevExpress.XtraBars.BarButtonItem();
            this.saveDefaultProducts = new DevExpress.XtraBars.BarButtonItem();
            this.aboutProductTool = new DevExpress.XtraBars.BarButtonItem();
            this.exitProductTool = new DevExpress.XtraBars.BarButtonItem();
            this.showBrokers = new DevExpress.XtraBars.BarButtonItem();
            this.saveBrokers = new DevExpress.XtraBars.BarButtonItem();
            this.showCompanies = new DevExpress.XtraBars.BarButtonItem();
            this.saveCompanies = new DevExpress.XtraBars.BarButtonItem();
            this.calendarsView = new DevExpress.XtraBars.BarButtonItem();
            this.editCompany = new DevExpress.XtraBars.BarButtonItem();
            this.searchString = new DevExpress.XtraBars.BarEditItem();
            this.productSearch = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.searchForProducts = new DevExpress.XtraBars.BarButtonItem();
            this.showExchanges = new DevExpress.XtraBars.BarButtonItem();
            this.saveExchanges = new DevExpress.XtraBars.BarButtonItem();
            this.showTradeTemplates = new DevExpress.XtraBars.BarButtonItem();
            this.addTemplate = new DevExpress.XtraBars.BarButtonItem();
            this.editTemplate = new DevExpress.XtraBars.BarButtonItem();
            this.deleteTemplate = new DevExpress.XtraBars.BarButtonItem();
            this.productRecalculateChanged = new DevExpress.XtraBars.BarButtonItem();
            this.productRecalculateAll = new DevExpress.XtraBars.BarButtonItem();
            this.recalculateManualTrades = new DevExpress.XtraBars.BarButtonItem();
            this.showProductBreakdown = new DevExpress.XtraBars.BarButtonItem();
            this.showUnits = new DevExpress.XtraBars.BarButtonItem();
            this.saveUnits = new DevExpress.XtraBars.BarButtonItem();
            this.showCurrencies = new DevExpress.XtraBars.BarButtonItem();
            this.saveCurrencies = new DevExpress.XtraBars.BarButtonItem();
            this.deskSelector = new DevExpress.XtraBars.BarEditItem();
            this.deskSelectorEditor = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.showDesks = new DevExpress.XtraBars.BarButtonItem();
            this.deskAdd = new DevExpress.XtraBars.BarButtonItem();
            this.deskEdit = new DevExpress.XtraBars.BarButtonItem();
            this.deskDelete = new DevExpress.XtraBars.BarButtonItem();
            this.deskProductAdd = new DevExpress.XtraBars.BarButtonItem();
            this.deskProductEdit = new DevExpress.XtraBars.BarButtonItem();
            this.deskProductDelete = new DevExpress.XtraBars.BarButtonItem();
            this.buttonImages = new DevExpress.Utils.ImageCollection(this.components);
            this.viewSelectorAndActionPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.viewModeGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.productGroupsRibbon = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.productsModifyGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.officialProductsModifyGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.defaultProductsModifyGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.brokersRibbon = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.companiesRibbon = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.exchangesRibbon = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.unitsModifyGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.currenciesModifyGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.tradeTemplatesModifyGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.deskGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.deskOfficialProductsGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.searchGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.miscellaneousOptions = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.productsViewContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.productGroupsGrid = new DevExpress.XtraGrid.GridControl();
            this.productGroupsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.productGroupNames = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productsGrid = new DevExpress.XtraGrid.GridControl();
            this.productsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.productNames = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productOfficialNames = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.expiryCalendar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.positionFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.pnlFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.contractSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.earliestProductDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.latestProductDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.underlyingFuturesName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.exchangeContractCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rolloffTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.altName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.officialProductsGrid = new DevExpress.XtraGrid.GridControl();
            this.officialProductsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.fullOfficialProductName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.officialProductDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.priceMappingColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.officialProductRegion = new DevExpress.XtraGrid.Columns.GridColumn();
            this.settleSymbol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.priceUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.unitToBblConversion = new DevExpress.XtraGrid.Columns.GridColumn();
            this.defaultProductsGrid = new DevExpress.XtraGrid.GridControl();
            this.defaultProductsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.userName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.officialProductName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.officialProductsRepo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.txtDummy = new DevExpress.XtraEditors.TextEdit();
            this.brokersGrid = new DevExpress.XtraGrid.GridControl();
            this.brokersView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.brokerYahooId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.brokerCompany = new DevExpress.XtraGrid.Columns.GridColumn();
            this.brokersCompanyRepo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.defaultBrokerProductName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.brokersDefaultProductsRepo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.companiesGrid = new DevExpress.XtraGrid.GridControl();
            this.companiesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.companyName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.companyRegion = new DevExpress.XtraGrid.Columns.GridColumn();
            this.companiesRegionsRepo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.companyNameAbbr = new DevExpress.XtraGrid.Columns.GridColumn();
            this.companyNameAbbrEditor = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.productSearchResults = new Mandara.ProductGUI.ProductSearchResults();
            this.exchangesGrid = new DevExpress.XtraGrid.GridControl();
            this.exchangesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.exchangeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.someMappingValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.exchangeCalendar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.exchangeCalendarSelector = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.exchangeTimezone = new DevExpress.XtraGrid.Columns.GridColumn();
            this.exchangeTimezoneSelector = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.tradeTemplatesGrid = new DevExpress.XtraGrid.GridControl();
            this.tradeTemplatesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.templateName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.portfolioName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.altExchangeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.altOfficialProductName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productVolume = new DevExpress.XtraGrid.Columns.GridColumn();
            this.altPriceUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productBreakdown = new DevExpress.XtraEditors.PanelControl();
            this.contractMonthEditor = new DevExpress.XtraEditors.ComboBoxEdit();
            this.avgSettleEditor = new DevExpress.XtraEditors.TextEdit();
            this.overnightSumEditor = new DevExpress.XtraEditors.TextEdit();
            this.liveSumEditor = new DevExpress.XtraEditors.TextEdit();
            this.pnlEditor = new DevExpress.XtraEditors.TextEdit();
            this.productSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.officialProductSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.startPriceEditor = new DevExpress.XtraEditors.TextEdit();
            this.quantityEditor = new DevExpress.XtraEditors.TextEdit();
            this.testTradeBreakdownContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.productBreakdownGrid = new DevExpress.XtraGrid.GridControl();
            this.productBreakdownView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.testTradeDay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeLivePnL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeOvernightPnL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeLivePrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeSettlement = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeLeg1Settle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeLeg2Settle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.testTradeImpactGrid = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.testTradeProductGroup = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeProduct = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeSource = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeRiskDateYear = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeRiskDateMonth = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeDetailId = new DevExpress.XtraPivotGrid.PivotGridField();
            this.tradeEndDateEditor = new DevExpress.XtraEditors.DateEdit();
            this.tradeStartDateEditor = new DevExpress.XtraEditors.DateEdit();
            this.testTradeDateEditor = new DevExpress.XtraEditors.DateEdit();
            this.testTradeAvgSettleTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeOvernightSumTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeLiveSumTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradePnLTitle = new DevExpress.XtraEditors.LabelControl();
            this.calculateTestTradeImpact = new DevExpress.XtraEditors.SimpleButton();
            this.FeesMode = new DevExpress.XtraEditors.SimpleButton();
            this.holidayCalendarsMode = new DevExpress.XtraEditors.SimpleButton();
            this.endDateTitle = new DevExpress.XtraEditors.LabelControl();
            this.startDateTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeStartPriceTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeQuantityTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeDateTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeContractMonthTitle = new DevExpress.XtraEditors.LabelControl();
            this.productTitle = new DevExpress.XtraEditors.LabelControl();
            this.officialProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.unitsGrid = new DevExpress.XtraGrid.GridControl();
            this.unitsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.unitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.defaultPositionFactorEditor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.unitPositionFactorEditor = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.monthlyContractSizeOnly = new DevExpress.XtraGrid.Columns.GridColumn();
            this.allowMonthlyContractSizeOnly = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.currenciesGrid = new DevExpress.XtraGrid.GridControl();
            this.currenciesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.currencyIsoName = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.viewSelectorAndActionContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deskSelectorEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsViewContainer)).BeginInit();
            this.productsViewContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.productGroupsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productGroupsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultProductsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultProductsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductsRepo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDummy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersCompanyRepo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersDefaultProductsRepo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companiesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companiesView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companiesRegionsRepo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companyNameAbbrEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangesView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeCalendarSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeTimezoneSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeTemplatesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeTemplatesView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBreakdown)).BeginInit();
            this.productBreakdown.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractMonthEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.avgSettleEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overnightSumEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.liveSumEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startPriceEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.quantityEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeBreakdownContainer)).BeginInit();
            this.testTradeBreakdownContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.productBreakdownGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBreakdownView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeImpactGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeEndDateEditor.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeEndDateEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeStartDateEditor.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeStartDateEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeDateEditor.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeDateEditor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitPositionFactorEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowMonthlyContractSizeOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currenciesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currenciesView)).BeginInit();
            this.SuspendLayout();
            // 
            // pgfPos
            // 
            this.testTradePositionsGrid.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
            this.testTradePositionsGrid.AreaIndex = 0;
            this.testTradePositionsGrid.Caption = "Pos";
            this.testTradePositionsGrid.CellFormat.FormatString = "#,#0.0#;(#,#0.0#);#0";
            this.testTradePositionsGrid.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.testTradePositionsGrid.FieldName = "Amount";
            this.testTradePositionsGrid.Name = "testTradePositionsGrid";
            this.testTradePositionsGrid.Width = 55;
            // 
            // ribbonControl1
            // 
            this.viewSelectorAndActionContainer.AllowMinimizeRibbon = false;
            this.viewSelectorAndActionContainer.ApplicationButtonText = null;
            this.viewSelectorAndActionContainer.ExpandCollapseItem.Id = 0;
            this.viewSelectorAndActionContainer.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.viewSelectorAndActionContainer.ExpandCollapseItem,
            this.addProductGroup,
            this.editProductGroup,
            this.deleteProductGroup,
            //this.barButtonItem1,
            //this.barButtonItem4,
            this.addOfficialProduct,
            this.editOfficialProduct,
            this.deleteOfficialProduct,
            this.addProduct,
            this.editProduct,
            this.deleteProduct,
            //this.barButtonItem2,
            //this.barButtonItem3,
            //this.barButtonItem5,
            //this.barButtonItem6,
            this.showProducts,
            this.showOfficialProducts,
            this.showOfficialProductDefaultProducts,
            this.saveDefaultProducts,
            this.aboutProductTool,
            this.exitProductTool,
            this.showBrokers,
            this.saveBrokers,
            this.showCompanies,
            this.saveCompanies,
            this.calendarsView,
            this.editCompany,
            this.searchString,
            this.searchForProducts,
            this.showExchanges,
            this.saveExchanges,
            this.showTradeTemplates,
            this.addTemplate,
            this.editTemplate,
            this.deleteTemplate,
            this.productRecalculateChanged,
            this.productRecalculateAll,
            this.recalculateManualTrades,
            this.showProductBreakdown,
            this.showUnits,
            this.saveUnits,
            this.showCurrencies,
            this.saveCurrencies,
            this.deskSelector,
            this.showDesks,
            this.deskAdd,
            this.deskEdit,
            this.deskDelete,
            this.deskProductAdd,
            this.deskProductEdit,
            this.deskProductDelete});
            this.viewSelectorAndActionContainer.LargeImages = this.buttonImages;
            this.viewSelectorAndActionContainer.Location = new System.Drawing.Point(0, 0);
            this.viewSelectorAndActionContainer.MaxItemId = 83;
            this.viewSelectorAndActionContainer.Name = "viewSelectorAndActionContainer";
            this.viewSelectorAndActionContainer.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.viewSelectorAndActionPage});
            this.viewSelectorAndActionContainer.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.productSearch,
            this.deskSelectorEditor});
            this.viewSelectorAndActionContainer.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.viewSelectorAndActionContainer.ShowCategoryInCaption = false;
            this.viewSelectorAndActionContainer.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            this.viewSelectorAndActionContainer.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            this.viewSelectorAndActionContainer.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show;
            this.viewSelectorAndActionContainer.Size = new System.Drawing.Size(1308, 116);
            this.viewSelectorAndActionContainer.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // addGroup
            // 
            this.addProductGroup.Caption = "Add";
            this.addProductGroup.Id = 1;
            this.addProductGroup.ImageOptions.LargeImageIndex = 0;
            this.addProductGroup.Name = "addProductGroup";
            this.addProductGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnGroupAdd_ItemClick);
            // 
            // editGroup
            // 
            this.editProductGroup.Caption = "Edit";
            this.editProductGroup.Id = 2;
            this.editProductGroup.ImageOptions.LargeImageIndex = 2;
            this.editProductGroup.Name = "editProductGroup";
            this.editProductGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnGroupEdit_ItemClick);
            // 
            // deleteGroup
            // 
            this.deleteProductGroup.Caption = "Delete";
            this.deleteProductGroup.Id = 3;
            this.deleteProductGroup.ImageOptions.LargeImageIndex = 1;
            this.deleteProductGroup.Name = "deleteProductGroup";
            this.deleteProductGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnGroupDelete_ItemClick);
            //// 
            //// barButtonItem1
            //// 
            //this.barButtonItem1.Caption = "barButtonItem1";
            //this.barButtonItem1.Id = 5;
            //this.barButtonItem1.Name = "barButtonItem1";
            //// 
            //// barButtonItem4
            //// 
            //this.barButtonItem4.Caption = "barButtonItem4";
            //this.barButtonItem4.Id = 6;
            //this.barButtonItem4.Name = "barButtonItem4";
            // 
            // addOfficialProduct
            // 
            this.addOfficialProduct.Caption = "Add";
            this.addOfficialProduct.Id = 11;
            this.addOfficialProduct.ImageOptions.LargeImageIndex = 3;
            this.addOfficialProduct.Name = "addOfficialProduct";
            this.addOfficialProduct.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnOfficialProductAdd_ItemClick);
            // 
            // editOfficialProduct
            // 
            this.editOfficialProduct.Caption = "Edit";
            this.editOfficialProduct.Id = 12;
            this.editOfficialProduct.ImageOptions.LargeImageIndex = 5;
            this.editOfficialProduct.Name = "editOfficialProduct";
            this.editOfficialProduct.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnOfficialProductEdit_ItemClick);
            // 
            // deleteOfficialProduct
            // 
            this.deleteOfficialProduct.Caption = "Delete";
            this.deleteOfficialProduct.Id = 13;
            this.deleteOfficialProduct.ImageOptions.LargeImageIndex = 4;
            this.deleteOfficialProduct.Name = "deleteOfficialProduct";
            this.deleteOfficialProduct.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnOfficialProductDelete_ItemClick);
            // 
            // addProduct
            // 
            this.addProduct.Caption = "Add";
            this.addProduct.Id = 14;
            this.addProduct.ImageOptions.LargeImageIndex = 6;
            this.addProduct.Name = "addProduct";
            this.addProduct.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnProductAdd_ItemClick);
            // 
            // editProduct
            // 
            this.editProduct.Caption = "Edit";
            this.editProduct.Id = 15;
            this.editProduct.ImageOptions.LargeImageIndex = 8;
            this.editProduct.Name = "editProduct";
            this.editProduct.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnProductEdit_ItemClick);
            // 
            // deleteProduct
            // 
            this.deleteProduct.Caption = "Delete";
            this.deleteProduct.Id = 16;
            this.deleteProduct.ImageOptions.LargeImageIndex = 7;
            this.deleteProduct.Name = "deleteProduct";
            this.deleteProduct.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnProductDelete_ItemClick);
            //// 
            //// barButtonItem2
            //// 
            //this.barButtonItem2.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            //this.barButtonItem2.Caption = "barButtonItem2";
            //this.barButtonItem2.Down = true;
            //this.barButtonItem2.Id = 18;
            //this.barButtonItem2.Name = "barButtonItem2";
            //// 
            //// barButtonItem3
            //// 
            //this.barButtonItem3.Caption = "barButtonItem3";
            //this.barButtonItem3.Id = 19;
            //this.barButtonItem3.Name = "barButtonItem3";
            //// 
            //// barButtonItem5
            //// 
            //this.barButtonItem5.Caption = "barButtonItem5";
            //this.barButtonItem5.Id = 22;
            //this.barButtonItem5.Name = "barButtonItem5";
            //// 
            //// barButtonItem6
            //// 
            //this.barButtonItem6.Caption = "barButtonItem6";
            //this.barButtonItem6.Id = 23;
            //this.barButtonItem6.Name = "barButtonItem6";
            // 
            // showProducts
            // 
            this.showProducts.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showProducts.Caption = "Products";
            this.showProducts.Down = true;
            this.showProducts.GroupIndex = 1;
            this.showProducts.Id = 24;
            this.showProducts.ImageOptions.LargeImageIndex = 10;
            this.showProducts.Name = "showProducts";
            this.showProducts.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // showOfficialProducts
            // 
            this.showOfficialProducts.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showOfficialProducts.Caption = "Official Products";
            this.showOfficialProducts.GroupIndex = 1;
            this.showOfficialProducts.Id = 25;
            this.showOfficialProducts.ImageOptions.LargeImageIndex = 9;
            this.showOfficialProducts.Name = "showOfficialProducts";
            this.showOfficialProducts.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // showOfficialProductDefaultProducts
            // 
            this.showOfficialProductDefaultProducts.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showOfficialProductDefaultProducts.Caption = "Default Products";
            this.showOfficialProductDefaultProducts.GroupIndex = 1;
            this.showOfficialProductDefaultProducts.Id = 26;
            this.showOfficialProductDefaultProducts.ImageOptions.LargeImageIndex = 11;
            this.showOfficialProductDefaultProducts.Name = "showOfficialProductDefaultProducts";
            this.showOfficialProductDefaultProducts.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.showOfficialProductDefaultProducts.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // saveDefaultProducts
            // 
            this.saveDefaultProducts.Caption = "Save";
            this.saveDefaultProducts.Id = 28;
            this.saveDefaultProducts.ImageOptions.LargeImageIndex = 9;
            this.saveDefaultProducts.Name = "saveDefaultProducts";
            this.saveDefaultProducts.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDefaultProductsSave_ItemClick);
            // 
            // aboutProductTool
            // 
            this.aboutProductTool.Caption = "About";
            this.aboutProductTool.Id = 29;
            this.aboutProductTool.ImageOptions.LargeImageIndex = 14;
            this.aboutProductTool.Name = "aboutProductTool";
            this.aboutProductTool.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAbout_ItemClick);
            // 
            // exitProductTool
            // 
            this.exitProductTool.Caption = "Exit";
            this.exitProductTool.Id = 30;
            this.exitProductTool.ImageOptions.LargeImageIndex = 15;
            this.exitProductTool.Name = "exitProductTool";
            this.exitProductTool.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExit_ItemClick);
            // 
            // showBrokers
            // 
            this.showBrokers.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showBrokers.Caption = "Brokers";
            this.showBrokers.GroupIndex = 1;
            this.showBrokers.Id = 31;
            this.showBrokers.ImageOptions.LargeImageIndex = 12;
            this.showBrokers.Name = "showBrokers";
            this.showBrokers.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // saveBrokers
            // 
            this.saveBrokers.Caption = "Save";
            this.saveBrokers.Id = 32;
            this.saveBrokers.ImageOptions.LargeImageIndex = 12;
            this.saveBrokers.Name = "saveBrokers";
            this.saveBrokers.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnBrokersSave_ItemClick);
            // 
            // showCompanies
            // 
            this.showCompanies.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showCompanies.Caption = "Companies";
            this.showCompanies.GroupIndex = 1;
            this.showCompanies.Id = 33;
            this.showCompanies.ImageOptions.LargeImageIndex = 13;
            this.showCompanies.Name = "showCompanies";
            this.showCompanies.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // saveCompanies
            // 
            this.saveCompanies.Caption = "Save";
            this.saveCompanies.Id = 34;
            this.saveCompanies.ImageOptions.LargeImageIndex = 13;
            this.saveCompanies.Name = "saveCompanies";
            this.saveCompanies.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCompaniesSave_ItemClick);
            // 
            // calendarsView
            // 
            this.calendarsView.Caption = "Calendars";
            this.calendarsView.Id = 35;
            this.calendarsView.ImageOptions.LargeImageIndex = 16;
            this.calendarsView.Name = "calendarsView";
            this.calendarsView.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCalendar_ItemClick);
            // 
            // editCompany
            // 
            this.editCompany.Caption = "Edit";
            this.editCompany.Id = 38;
            this.editCompany.ImageOptions.LargeImageIndex = 13;
            this.editCompany.Name = "editCompany";
            this.editCompany.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCompanyEdit_ItemClick);
            // 
            // searchString
            // 
            this.searchString.Edit = this.productSearch;
            this.searchString.EditWidth = 75;
            this.searchString.Id = 39;
            this.searchString.Name = "searchString";
            this.searchString.Edit.EditValueChangedFiringMode = EditValueChangedFiringMode.Default;
            this.searchString.EditValueChanged += OnSearchStringChanged;
            // 
            // productSearch
            // 
            this.productSearch.AutoHeight = false;
            this.productSearch.Name = "productSearch";
            // 
            // searchForProducts
            // 
            this.searchForProducts.Caption = "Search";
            this.searchForProducts.Id = 42;
            this.searchForProducts.Name = "searchForProducts";
            this.searchForProducts.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSearch_ItemClick);
            // 
            // showExchanges
            // 
            this.showExchanges.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showExchanges.Caption = "Exchanges";
            this.showExchanges.GroupIndex = 1;
            this.showExchanges.Id = 42;
            this.showExchanges.ImageOptions.LargeImageIndex = 16;
            this.showExchanges.Name = "showExchanges";
            this.showExchanges.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            this.showExchanges.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExchanges_ItemClick);
            // 
            // saveExchanges
            // 
            this.saveExchanges.Caption = "Save";
            this.saveExchanges.Id = 44;
            this.saveExchanges.ImageOptions.LargeImageIndex = 16;
            this.saveExchanges.Name = "saveExchanges";
            this.saveExchanges.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExchangesSave_ItemClick);
            // 
            // showTradeTemplates
            // 
            this.showTradeTemplates.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showTradeTemplates.Caption = "Trade Templates";
            this.showTradeTemplates.GroupIndex = 1;
            this.showTradeTemplates.Id = 45;
            this.showTradeTemplates.ImageOptions.LargeImageIndex = 18;
            this.showTradeTemplates.Name = "showTradeTemplates";
            this.showTradeTemplates.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            this.showTradeTemplates.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnTradeTemplates_ItemClick);
            // 
            // addTemplate
            // 
            this.addTemplate.Caption = "Add";
            this.addTemplate.Id = 46;
            this.addTemplate.ImageOptions.LargeImageIndex = 18;
            this.addTemplate.Name = "addTemplate";
            this.addTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnTemplateAdd_ItemClick);
            // 
            // editTemplate
            // 
            this.editTemplate.Caption = "Edit";
            this.editTemplate.Id = 47;
            this.editTemplate.ImageOptions.LargeImageIndex = 18;
            this.editTemplate.Name = "editTemplate";
            this.editTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnTemplateEdit_ItemClick);
            // 
            // deleteTemplate
            // 
            this.deleteTemplate.Caption = "Delete";
            this.deleteTemplate.Id = 48;
            this.deleteTemplate.ImageOptions.LargeImageIndex = 18;
            this.deleteTemplate.Name = "deleteTemplate";
            this.deleteTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnTemplateDelete_ItemClick);
            // 
            // productRecalculateChanged
            // 
            this.productRecalculateChanged.Caption = "Recalculate Changed";
            this.productRecalculateChanged.Id = 49;
            this.productRecalculateChanged.ImageOptions.LargeImageIndex = 19;
            this.productRecalculateChanged.Name = "productRecalculateChanged";
            this.productRecalculateChanged.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnProductRecalculateChanged_ItemClick);
            // 
            // productRecalculateAll
            // 
            this.productRecalculateAll.Caption = "Recalculate All";
            this.productRecalculateAll.Id = 50;
            this.productRecalculateAll.ImageOptions.LargeImageIndex = 20;
            this.productRecalculateAll.Name = "productRecalculateAll";
            this.productRecalculateAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnProductRecalculateAll_ItemClick);
            // 
            // recalculateManualTrades
            // 
            this.recalculateManualTrades.Caption = "Recalculate Manual Trades";
            this.recalculateManualTrades.Id = 59;
            this.recalculateManualTrades.ImageOptions.LargeImageIndex = 19;
            this.recalculateManualTrades.Name = "recalculateManualTrades";
            this.recalculateManualTrades.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRecalculateManualTrades_ItemClick);
            // 
            // showProductBreakdown
            // 
            this.showProductBreakdown.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showProductBreakdown.Caption = "Product Breakdown";
            this.showProductBreakdown.GroupIndex = 1;
            this.showProductBreakdown.Id = 61;
            this.showProductBreakdown.ImageOptions.LargeImageIndex = 16;
            this.showProductBreakdown.Name = "showProductBreakdown";
            this.showProductBreakdown.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // showUnits
            // 
            this.showUnits.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showUnits.Caption = "Units";
            this.showUnits.GroupIndex = 1;
            this.showUnits.Id = 63;
            this.showUnits.ImageOptions.LargeImageIndex = 16;
            this.showUnits.Name = "showUnits";
            this.showUnits.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            this.showUnits.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnUnits_ItemClick);
            // 
            // saveUnits
            // 
            this.saveUnits.Caption = "Save";
            this.saveUnits.Id = 65;
            this.saveUnits.Name = "saveUnits";
            this.saveUnits.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnUnitsSave_ItemClick);
            // 
            // showCurrencies
            // 
            this.showCurrencies.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showCurrencies.Caption = "Currencies";
            this.showCurrencies.GroupIndex = 1;
            this.showCurrencies.Id = 66;
            this.showCurrencies.ImageOptions.LargeImageIndex = 16;
            this.showCurrencies.Name = "showCurrencies";
            this.showCurrencies.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            this.showCurrencies.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCurrencies_ItemClick);
            // 
            // saveCurrencies
            // 
            this.saveCurrencies.Caption = "Save";
            this.saveCurrencies.Id = 67;
            this.saveCurrencies.Name = "saveCurrencies";
            this.saveCurrencies.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCurrenciesSave_ItemClick);
            // 
            // deskSelector
            // 
            this.deskSelector.Caption = "Desk";
            this.deskSelector.Edit = this.deskSelectorEditor;
            this.deskSelector.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            this.deskSelector.Id = 76;
            this.deskSelector.Name = "deskSelector";
            this.deskSelector.EditValueChanged += new System.EventHandler(this.deskSelector_EditValueChanged);
            // 
            // deskSelectorEditor
            // 
            this.deskSelectorEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deskSelectorEditor.DisplayMember = "Name";
            this.deskSelectorEditor.Name = "deskSelectorEditor";
            this.deskSelectorEditor.ValueMember = "Id";
            // 
            // showDesks
            // 
            this.showDesks.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.showDesks.Caption = "Desks";
            this.showDesks.GroupIndex = 1;
            this.showDesks.Id = 76;
            this.showDesks.ImageOptions.LargeImageIndex = 16;
            this.showDesks.Name = "showDesks";
            this.showDesks.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCommonViewMode_DownChanged);
            // 
            // deskAdd
            // 
            this.deskAdd.Caption = "Add";
            this.deskAdd.Id = 77;
            this.deskAdd.ImageOptions.LargeImageIndex = 1;
            this.deskAdd.Name = "deskAdd";
            this.deskAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddDesk);
            // 
            // deskEdit
            // 
            this.deskEdit.Caption = "Edit";
            this.deskEdit.Id = 77;
            this.deskEdit.ImageOptions.LargeImageIndex = 1;
            this.deskEdit.Name = "deskEdit";
            this.deskEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnEditDesk);
            // 
            // deskDelete
            // 
            this.deskDelete.Caption = "Delete";
            this.deskDelete.Id = 78;
            this.deskDelete.ImageOptions.LargeImageIndex = 1;
            this.deskDelete.Name = "deskDelete";
            this.deskDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnDeleteDesk);
            // 
            // deskProductAdd
            // 
            this.deskProductAdd.Caption = "Add";
            this.deskProductAdd.Id = 79;
            this.deskProductAdd.ImageOptions.LargeImageIndex = 6;
            this.deskProductAdd.Name = "deskProductAdd";
            this.deskProductAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddDeskOfficialProduct);
            // 
            // deskProductEdit
            // 
            this.deskProductEdit.Caption = "Edit";
            this.deskProductEdit.Id = 79;
            this.deskProductEdit.ImageOptions.LargeImageIndex = 6;
            this.deskProductEdit.Name = "deskProductEdit";
            this.deskProductEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnEditDeskOfficialProduct);
            // 
            // deskProductDelete
            // 
            this.deskProductDelete.Caption = "Delete";
            this.deskProductDelete.Id = 81;
            this.deskProductDelete.ImageOptions.LargeImageIndex = 7;
            this.deskProductDelete.Name = "deskProductDelete";
            this.deskProductDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnDeleteDeskOfficialProduct);
            // 
            // imageCollection1
            // 
            this.buttonImages.ImageSize = new System.Drawing.Size(32, 32);
            this.buttonImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("buttonImages.ImageStream")));
            this.buttonImages.Images.SetKeyName(0, "folder_add.png");
            this.buttonImages.Images.SetKeyName(1, "folder_delete.png");
            this.buttonImages.Images.SetKeyName(2, "folder_edit.png");
            this.buttonImages.Images.SetKeyName(3, "form_blue_add.png");
            this.buttonImages.Images.SetKeyName(4, "form_blue_delete.png");
            this.buttonImages.Images.SetKeyName(5, "form_blue_edit.png");
            this.buttonImages.Images.SetKeyName(6, "form_green_add.png");
            this.buttonImages.Images.SetKeyName(7, "form_green_delete.png");
            this.buttonImages.Images.SetKeyName(8, "form_green_edit.png");
            this.buttonImages.Images.SetKeyName(9, "form_blue.png");
            this.buttonImages.Images.SetKeyName(10, "form_green.png");
            this.buttonImages.Images.SetKeyName(11, "form_yellow.png");
            this.buttonImages.Images.SetKeyName(12, "photo_portrait.png");
            this.buttonImages.Images.SetKeyName(13, "company.png");
            this.buttonImages.Images.SetKeyName(14, "company.png");
            this.buttonImages.Images.SetKeyName(15, "about.png");
            this.buttonImages.Images.SetKeyName(16, "calendar_copy.png");
            this.buttonImages.Images.SetKeyName(17, "server_refresh.png");
            this.buttonImages.Images.SetKeyName(18, "template.png");
            this.buttonImages.Images.SetKeyName(19, "recalculate_changed_items.png");
            this.buttonImages.Images.SetKeyName(20, "recalculate_all_items1.png");
            // 
            // ribbonPage1
            // 
            this.viewSelectorAndActionPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.viewModeGroup,
            this.productGroupsRibbon,
            this.productsModifyGroup,
            this.officialProductsModifyGroup,
            this.defaultProductsModifyGroup,
            this.brokersRibbon,
            this.companiesRibbon,
            this.exchangesRibbon,
            this.unitsModifyGroup,
            this.currenciesModifyGroup,
            this.tradeTemplatesModifyGroup,
            this.deskGroup,
            this.deskOfficialProductsGroup,
            this.searchGroup,
            this.miscellaneousOptions});
            this.viewSelectorAndActionPage.Name = "viewSelectorAndActionPage";
            this.viewSelectorAndActionPage.Text = "Home";
            // 
            // viewModeGroup
            // 
            this.viewModeGroup.ItemLinks.Add(this.showProducts);
            this.viewModeGroup.ItemLinks.Add(this.showOfficialProducts);
            this.viewModeGroup.ItemLinks.Add(this.showOfficialProductDefaultProducts);
            this.viewModeGroup.ItemLinks.Add(this.showBrokers);
            this.viewModeGroup.ItemLinks.Add(this.showCompanies);
            this.viewModeGroup.ItemLinks.Add(this.calendarsView);
            this.viewModeGroup.ItemLinks.Add(this.showExchanges);
            this.viewModeGroup.ItemLinks.Add(this.showTradeTemplates);
            this.viewModeGroup.ItemLinks.Add(this.showUnits);
            this.viewModeGroup.ItemLinks.Add(this.showCurrencies);
            this.viewModeGroup.ItemLinks.Add(this.showProductBreakdown);
            this.viewModeGroup.ItemLinks.Add(this.showDesks);
            this.viewModeGroup.Name = "viewModeGroup";
            this.viewModeGroup.ShowCaptionButton = false;
            this.viewModeGroup.Text = "View Mode";
            // 
            // productGroupsRibbon
            // 
            this.productGroupsRibbon.ItemLinks.Add(this.addProductGroup);
            this.productGroupsRibbon.ItemLinks.Add(this.editProductGroup);
            this.productGroupsRibbon.ItemLinks.Add(this.deleteProductGroup);
            this.productGroupsRibbon.Name = "productGroupsRibbon";
            this.productGroupsRibbon.ShowCaptionButton = false;
            this.productGroupsRibbon.Text = "Product Groups";
            // 
            // productsModifyGroup
            // 
            this.productsModifyGroup.ItemLinks.Add(this.addProduct);
            this.productsModifyGroup.ItemLinks.Add(this.editProduct);
            this.productsModifyGroup.ItemLinks.Add(this.deleteProduct);
            this.productsModifyGroup.ItemLinks.Add(this.productRecalculateChanged);
            this.productsModifyGroup.ItemLinks.Add(this.recalculateManualTrades);
            this.productsModifyGroup.ItemLinks.Add(this.productRecalculateAll);
            this.productsModifyGroup.Name = "productsModifyGroup";
            this.productsModifyGroup.ShowCaptionButton = false;
            this.productsModifyGroup.Text = "Products";
            // 
            // officialProductsModifyGroup
            // 
            this.officialProductsModifyGroup.ItemLinks.Add(this.addOfficialProduct);
            this.officialProductsModifyGroup.ItemLinks.Add(this.editOfficialProduct);
            this.officialProductsModifyGroup.ItemLinks.Add(this.deleteOfficialProduct);
            this.officialProductsModifyGroup.Name = "officialProductsModifyGroup";
            this.officialProductsModifyGroup.ShowCaptionButton = false;
            this.officialProductsModifyGroup.Text = "Official Products";
            this.officialProductsModifyGroup.Visible = false;
            // 
            // defaultProductsModifyGroup
            // 
            this.defaultProductsModifyGroup.ItemLinks.Add(this.saveDefaultProducts);
            this.defaultProductsModifyGroup.Name = "defaultProductsModifyGroup";
            this.defaultProductsModifyGroup.ShowCaptionButton = false;
            this.defaultProductsModifyGroup.Text = "Default Products";
            this.defaultProductsModifyGroup.Visible = false;
            // 
            // brokersRibbon
            // 
            this.brokersRibbon.ItemLinks.Add(this.saveBrokers);
            this.brokersRibbon.Name = "brokersRibbon";
            this.brokersRibbon.ShowCaptionButton = false;
            this.brokersRibbon.Text = "Brokers";
            this.brokersRibbon.Visible = false;
            // 
            // companiesRibbon
            // 
            this.companiesRibbon.ItemLinks.Add(this.editCompany);
            this.companiesRibbon.ItemLinks.Add(this.saveCompanies);
            this.companiesRibbon.Name = "companiesRibbon";
            this.companiesRibbon.ShowCaptionButton = false;
            this.companiesRibbon.Text = "Companies";
            this.companiesRibbon.Visible = false;
            // 
            // exchangesRibbon
            // 
            this.exchangesRibbon.ItemLinks.Add(this.saveExchanges);
            this.exchangesRibbon.Name = "exchangesRibbon";
            this.exchangesRibbon.ShowCaptionButton = false;
            this.exchangesRibbon.Text = "Exchanges";
            this.exchangesRibbon.Visible = false;
            // 
            // unitsModifyGroup
            // 
            this.unitsModifyGroup.ItemLinks.Add(this.saveUnits);
            this.unitsModifyGroup.Name = "unitsModifyGroup";
            this.unitsModifyGroup.ShowCaptionButton = false;
            this.unitsModifyGroup.Text = "Units";
            this.unitsModifyGroup.Visible = false;
            // 
            // currenciesModifyGroup
            // 
            this.currenciesModifyGroup.ItemLinks.Add(this.saveCurrencies);
            this.currenciesModifyGroup.Name = "currenciesModifyGroup";
            this.currenciesModifyGroup.ShowCaptionButton = false;
            this.currenciesModifyGroup.Text = "Currencies";
            this.currenciesModifyGroup.Visible = false;
            // 
            // tradeTemplatesModifyGroup
            // 
            this.tradeTemplatesModifyGroup.ItemLinks.Add(this.addTemplate);
            this.tradeTemplatesModifyGroup.ItemLinks.Add(this.editTemplate);
            this.tradeTemplatesModifyGroup.ItemLinks.Add(this.deleteTemplate);
            this.tradeTemplatesModifyGroup.Name = "tradeTemplatesModifyGroup";
            this.tradeTemplatesModifyGroup.Text = "Trade Templates";
            this.tradeTemplatesModifyGroup.Visible = false;
            //
            // deskGroup
            // 
            this.deskGroup.ItemLinks.Add(this.deskAdd);
            this.deskGroup.ItemLinks.Add(this.deskEdit);
            this.deskGroup.ItemLinks.Add(this.deskDelete);
            this.deskGroup.Name = "deskGroup";
            this.deskGroup.Text = "Desks";
            // 
            // deskOfficialProductsGroup
            // 
            this.deskOfficialProductsGroup.ItemLinks.Add(this.deskProductAdd);
            this.deskOfficialProductsGroup.ItemLinks.Add(this.deskProductEdit);
            this.deskOfficialProductsGroup.ItemLinks.Add(this.deskProductDelete);
            this.deskOfficialProductsGroup.Name = "deskOfficialProductsGroup";
            this.deskOfficialProductsGroup.Text = "Official Products";
            // 
            // searchGroup
            // 
            this.searchGroup.ItemLinks.Add(this.searchString);
            this.searchGroup.ItemLinks.Add(this.searchForProducts);
            this.searchGroup.ItemLinks.Add(this.deskSelector);
            this.searchGroup.Name = "searchGroup";
            this.searchGroup.ShowCaptionButton = false;
            this.searchGroup.Text = "Search";
            // 
            // miscellaneousOptions
            // 
            this.miscellaneousOptions.ItemLinks.Add(this.aboutProductTool);
            this.miscellaneousOptions.ItemLinks.Add(this.exitProductTool);
            this.miscellaneousOptions.Name = "miscellaneousOptions";
            this.miscellaneousOptions.ShowCaptionButton = false;
            this.miscellaneousOptions.Text = "Miscellaneous";
            // 
            // productsViewContainer
            // 
            this.productsViewContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productsViewContainer.Location = new System.Drawing.Point(0, 120);
            this.productsViewContainer.Name = "productsViewContainer";
            this.productsViewContainer.Panel1.Controls.Add(this.productGroupsGrid);
            this.productsViewContainer.Panel1.Text = "Panel1";
            this.productsViewContainer.Panel2.Controls.Add(this.productsGrid);
            this.productsViewContainer.Panel2.Text = "Panel2";
            this.productsViewContainer.Size = new System.Drawing.Size(1400, 445);
            this.productsViewContainer.SplitterPosition = 239;
            this.productsViewContainer.TabIndex = 1;
            this.productsViewContainer.Text = "splitContainerControl1";
            // 
            // productGroupsGrid
            // 
            this.productGroupsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productGroupsGrid.Location = new System.Drawing.Point(0, 0);
            this.productGroupsGrid.MainView = this.productGroupsView;
            this.productGroupsGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.productGroupsGrid.Name = "productGroupsGrid";
            this.productGroupsGrid.ShowOnlyPredefinedDetails = true;
            this.productGroupsGrid.Size = new System.Drawing.Size(239, 445);
            this.productGroupsGrid.TabIndex = 0;
            this.productGroupsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.productGroupsView});
            // 
            // productGroupsView
            // 
            this.productGroupsView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.productGroupNames});
            this.productGroupsView.GridControl = this.productGroupsGrid;
            this.productGroupsView.Name = "productGroupsView";
            this.productGroupsView.OptionsBehavior.Editable = false;
            this.productGroupsView.OptionsCustomization.AllowGroup = false;
            this.productGroupsView.OptionsView.ShowGroupPanel = false;
            this.productGroupsView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.productGroupNames, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.productGroupsView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvProductGroups_FocusedRowChanged);
            // 
            // productGroupNames
            // 
            this.productGroupNames.Caption = "Product Group";
            this.productGroupNames.FieldName = "Name";
            this.productGroupNames.Name = "productGroupNames";
            this.productGroupNames.Visible = true;
            this.productGroupNames.VisibleIndex = 0;
            // 
            // productsGrid
            // 
            this.productsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productsGrid.Location = new System.Drawing.Point(0, 0);
            this.productsGrid.MainView = this.productsView;
            this.productsGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.productsGrid.Name = "productsGrid";
            this.productsGrid.ShowOnlyPredefinedDetails = true;
            this.productsGrid.Size = new System.Drawing.Size(1064, 445);
            this.productsGrid.TabIndex = 1;
            this.productsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.productsView});
            // 
            // productsView
            // 
            this.productsView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.productNames,
            this.productOfficialNames,
            this.productType,
            this.expiryCalendar,
            this.positionFactor,
            this.pnlFactor,
            this.contractSize,
            this.earliestProductDate,
            this.latestProductDate,
            this.underlyingFuturesName,
            this.exchangeContractCode,
            this.rolloffTime});
            this.productsView.GridControl = this.productsGrid;
            this.productsView.Name = "productsView";
            this.productsView.OptionsBehavior.Editable = false;
            this.productsView.OptionsCustomization.AllowGroup = false;
            this.productsView.OptionsView.ShowGroupPanel = false;
            this.productsView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.productNames, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // productNames
            // 
            this.productNames.Caption = "Name";
            this.productNames.FieldName = "Name";
            this.productNames.Name = "productNames";
            this.productNames.Visible = true;
            this.productNames.VisibleIndex = 0;
            this.productNames.Width = 114;
            // 
            // productOfficialNames
            // 
            this.productOfficialNames.Caption = "Official name";
            this.productOfficialNames.FieldName = "OfficialProduct.Name";
            this.productOfficialNames.Name = "productOfficialNames";
            this.productOfficialNames.Visible = true;
            this.productOfficialNames.VisibleIndex = 1;
            this.productOfficialNames.Width = 136;
            // 
            // productType
            // 
            this.productType.Caption = "Type";
            this.productType.FieldName = "ProductTypeDisplay";
            this.productType.Name = "productType";
            this.productType.Visible = true;
            this.productType.VisibleIndex = 2;
            this.productType.Width = 103;
            // 
            // expiryCalendar
            // 
            this.expiryCalendar.Caption = "Calendar";
            this.expiryCalendar.FieldName = "ExpiryCalendar.Name";
            this.expiryCalendar.Name = "expiryCalendar";
            this.expiryCalendar.Visible = true;
            this.expiryCalendar.VisibleIndex = 3;
            this.expiryCalendar.Width = 64;
            // 
            // positionFactor
            // 
            this.positionFactor.Caption = "Position factor";
            this.positionFactor.DisplayFormat.FormatString = "f2";
            this.positionFactor.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.positionFactor.FieldName = "PositionFactor";
            this.positionFactor.Name = "positionFactor";
            this.positionFactor.Visible = true;
            this.positionFactor.VisibleIndex = 4;
            this.positionFactor.Width = 84;
            // 
            // pnlFactor
            // 
            this.pnlFactor.Caption = "PnL factor";
            this.pnlFactor.FieldName = "PnlFactor";
            this.pnlFactor.Name = "pnlFactor";
            this.pnlFactor.Visible = true;
            this.pnlFactor.VisibleIndex = 5;
            this.pnlFactor.Width = 63;
            // 
            // contractSize
            // 
            this.contractSize.Caption = "Contract Size";
            this.contractSize.FieldName = "ContractSize";
            this.contractSize.Name = "contractSize";
            this.contractSize.Visible = true;
            this.contractSize.VisibleIndex = 6;
            this.contractSize.Width = 110;
            // 
            // earliestProductDate
            // 
            this.earliestProductDate.Caption = "Valid From";
            this.earliestProductDate.FieldName = "ValidFrom";
            this.earliestProductDate.Name = "earliestProductDate";
            this.earliestProductDate.Visible = true;
            this.earliestProductDate.VisibleIndex = 7;
            // 
            // latestProductDate
            // 
            this.latestProductDate.Caption = "Valid To";
            this.latestProductDate.FieldName = "ValidTo";
            this.latestProductDate.Name = "latestProductDate";
            this.latestProductDate.Visible = true;
            this.latestProductDate.VisibleIndex = 8;
            // 
            // underlyingFuturesName
            // 
            this.underlyingFuturesName.Caption = "Underlying product";
            this.underlyingFuturesName.FieldName = "UnderlyingFutures.Name";
            this.underlyingFuturesName.Name = "underlyingFuturesName";
            this.underlyingFuturesName.Visible = true;
            this.underlyingFuturesName.VisibleIndex = 9;
            // 
            // exchangeContractCode
            // 
            this.exchangeContractCode.Caption = "ExchangeCode";
            this.exchangeContractCode.FieldName = "ExchangeContractCode";
            this.exchangeContractCode.Name = "exchangeContractCode";
            this.exchangeContractCode.Visible = true;
            this.exchangeContractCode.VisibleIndex = 10;
            // 
            // rolloffTime
            // 
            this.rolloffTime.Caption = "Rolloff Time";
            this.rolloffTime.FieldName = "LocalRolloffTimeString";
            this.rolloffTime.Name = "rolloffTime";
            this.rolloffTime.Visible = true;
            this.rolloffTime.VisibleIndex = 11;
            // 
            // altName
            // 
            this.altName.Caption = "Name";
            this.altName.FieldName = "Name";
            this.altName.Name = "altName";
            this.altName.Visible = true;
            this.altName.VisibleIndex = 0;
            // 
            // officialProductsGrid
            // 
            this.officialProductsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.officialProductsGrid.Location = new System.Drawing.Point(0, 120);
            this.officialProductsGrid.MainView = this.officialProductsView;
            this.officialProductsGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.officialProductsGrid.Name = "officialProductsGrid";
            this.officialProductsGrid.ShowOnlyPredefinedDetails = true;
            this.officialProductsGrid.Size = new System.Drawing.Size(1308, 445);
            this.officialProductsGrid.TabIndex = 3;
            this.officialProductsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.officialProductsView});
            this.officialProductsGrid.Visible = false;
            // 
            // officialProductsView
            // 
            this.officialProductsView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.fullOfficialProductName,
            this.officialProductDisplayName,
            this.priceMappingColumn,
            this.officialProductRegion,
            this.settleSymbol,
            this.priceUnit,
            this.unitToBblConversion});
            this.officialProductsView.GridControl = this.officialProductsGrid;
            this.officialProductsView.Name = "officialProductsView";
            this.officialProductsView.OptionsBehavior.Editable = false;
            this.officialProductsView.OptionsCustomization.AllowGroup = false;
            this.officialProductsView.OptionsView.ShowGroupPanel = false;
            this.officialProductsView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.fullOfficialProductName, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // fullOfficialProductName
            // 
            this.fullOfficialProductName.Caption = "Full Name";
            this.fullOfficialProductName.FieldName = "Name";
            this.fullOfficialProductName.Name = "fullOfficialProductName";
            this.fullOfficialProductName.Visible = true;
            this.fullOfficialProductName.VisibleIndex = 0;
            this.fullOfficialProductName.Width = 366;
            // 
            // officialProductDisplayName
            // 
            this.officialProductDisplayName.Caption = "Display name";
            this.officialProductDisplayName.FieldName = "DisplayName";
            this.officialProductDisplayName.Name = "officialProductDisplayName";
            this.officialProductDisplayName.Visible = true;
            this.officialProductDisplayName.VisibleIndex = 1;
            this.officialProductDisplayName.Width = 296;
            // 
            // priceMappingColumn
            // 
            this.priceMappingColumn.Caption = "Price mapping";
            this.priceMappingColumn.FieldName = "MappingColumn";
            this.priceMappingColumn.Name = "priceMappingColumn";
            this.priceMappingColumn.Visible = true;
            this.priceMappingColumn.VisibleIndex = 2;
            this.priceMappingColumn.Width = 256;
            // 
            // officialProductRegion
            // 
            this.officialProductRegion.Caption = "Region";
            this.officialProductRegion.FieldName = "Region";
            this.officialProductRegion.Name = "officialProductRegion";
            this.officialProductRegion.Visible = true;
            this.officialProductRegion.VisibleIndex = 3;
            // 
            // settleSymbol
            // 
            this.settleSymbol.Caption = "Settle Symbol";
            this.settleSymbol.FieldName = "SettleSymbol";
            this.settleSymbol.Name = "settleSymbol";
            this.settleSymbol.Visible = true;
            this.settleSymbol.VisibleIndex = 4;
            // 
            // priceUnit
            // 
            this.priceUnit.Caption = "Price Unit";
            this.priceUnit.FieldName = "PriceUnit";
            this.priceUnit.Name = "priceUnit";
            this.priceUnit.Visible = true;
            this.priceUnit.VisibleIndex = 5;
            // 
            // unitToBblConversion
            // 
            this.unitToBblConversion.Caption = "Unit to BBL Conversion";
            this.unitToBblConversion.FieldName = "UnitToBarrelConversionFactor";
            this.unitToBblConversion.Name = "unitToBblConversion";
            this.unitToBblConversion.Visible = true;
            this.unitToBblConversion.VisibleIndex = 6;
            // 
            // defaultProductsGrid
            // 
            this.defaultProductsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.defaultProductsGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.defaultProductsGrid.Location = new System.Drawing.Point(0, 120);
            this.defaultProductsGrid.MainView = this.defaultProductsView;
            this.defaultProductsGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.defaultProductsGrid.Name = "defaultProductsGrid";
            this.defaultProductsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.officialProductsRepo});
            this.defaultProductsGrid.ShowOnlyPredefinedDetails = true;
            this.defaultProductsGrid.Size = new System.Drawing.Size(1308, 445);
            this.defaultProductsGrid.TabIndex = 4;
            this.defaultProductsGrid.UseEmbeddedNavigator = true;
            this.defaultProductsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.defaultProductsView});
            this.defaultProductsGrid.Visible = false;
            // 
            // defaultProductsView
            // 
            this.defaultProductsView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.userName,
            this.officialProductName});
            this.defaultProductsView.GridControl = this.defaultProductsGrid;
            this.defaultProductsView.Name = "defaultProductsView";
            this.defaultProductsView.OptionsCustomization.AllowGroup = false;
            this.defaultProductsView.OptionsView.ShowGroupPanel = false;
            this.defaultProductsView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.userName, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // userName
            // 
            this.userName.Caption = "Username";
            this.userName.FieldName = "UserName";
            this.userName.Name = "userName";
            this.userName.Visible = true;
            this.userName.VisibleIndex = 0;
            this.userName.Width = 200;
            // 
            // officialProductName
            // 
            this.officialProductName.Caption = "Product Name";
            this.officialProductName.ColumnEdit = this.officialProductsRepo;
            this.officialProductName.FieldName = "OfficialProduct";
            this.officialProductName.Name = "officialProductName";
            this.officialProductName.Visible = true;
            this.officialProductName.VisibleIndex = 1;
            this.officialProductName.Width = 718;
            // 
            // officialProductsRepo
            // 
            this.officialProductsRepo.AutoHeight = false;
            this.officialProductsRepo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.officialProductsRepo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Product Name")});
            this.officialProductsRepo.DisplayMember = "Name";
            this.officialProductsRepo.Name = "officialProductsRepo";
            this.officialProductsRepo.NullText = "";
            this.officialProductsRepo.ValueMember = "Instance";
            // 
            // txtDummy
            // 
            this.txtDummy.Location = new System.Drawing.Point(10000, 208);
            this.txtDummy.MenuManager = this.viewSelectorAndActionContainer;
            this.txtDummy.Name = "txtDummy";
            this.txtDummy.Size = new System.Drawing.Size(100, 20);
            this.txtDummy.TabIndex = 2;
            // 
            // brokersGrid
            // 
            this.brokersGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.brokersGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.brokersGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.brokersGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.brokersGrid.Location = new System.Drawing.Point(0, 120);
            this.brokersGrid.MainView = this.brokersView;
            this.brokersGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.brokersGrid.Name = "brokersGrid";
            this.brokersGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.brokersCompanyRepo,
            this.brokersDefaultProductsRepo});
            this.brokersGrid.ShowOnlyPredefinedDetails = true;
            this.brokersGrid.Size = new System.Drawing.Size(1308, 445);
            this.brokersGrid.TabIndex = 6;
            this.brokersGrid.UseEmbeddedNavigator = true;
            this.brokersGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.brokersView});
            this.brokersGrid.Visible = false;
            // 
            // brokersView
            // 
            this.brokersView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.brokerYahooId,
            this.brokerCompany,
            this.defaultBrokerProductName});
            this.brokersView.GridControl = this.brokersGrid;
            this.brokersView.Name = "brokersView";
            this.brokersView.OptionsCustomization.AllowGroup = false;
            this.brokersView.OptionsView.ShowGroupPanel = false;
            this.brokersView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.brokerYahooId, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // brokerYahooId
            // 
            this.brokerYahooId.Caption = "Yahoo ID";
            this.brokerYahooId.FieldName = "YahooId";
            this.brokerYahooId.Name = "brokerYahooId";
            this.brokerYahooId.Visible = true;
            this.brokerYahooId.VisibleIndex = 0;
            this.brokerYahooId.Width = 238;
            // 
            // brokerCompany
            // 
            this.brokerCompany.Caption = "Company";
            this.brokerCompany.ColumnEdit = this.brokersCompanyRepo;
            this.brokerCompany.FieldName = "Company";
            this.brokerCompany.Name = "brokerCompany";
            this.brokerCompany.Visible = true;
            this.brokerCompany.VisibleIndex = 1;
            this.brokerCompany.Width = 222;
            // 
            // brokersCompanyRepo
            // 
            this.brokersCompanyRepo.AutoHeight = false;
            this.brokersCompanyRepo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.brokersCompanyRepo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Company")});
            this.brokersCompanyRepo.Name = "brokersCompanyRepo";
            this.brokersCompanyRepo.NullText = "[Company is not specified]";
            this.brokersCompanyRepo.ValueMember = "Instance";
            // 
            // defaultBrokerProductName
            // 
            this.defaultBrokerProductName.Caption = "Default Product Name";
            this.defaultBrokerProductName.ColumnEdit = this.brokersDefaultProductsRepo;
            this.defaultBrokerProductName.FieldName = "DefaultProduct";
            this.defaultBrokerProductName.Name = "defaultBrokerProductName";
            this.defaultBrokerProductName.Visible = true;
            this.defaultBrokerProductName.VisibleIndex = 2;
            // 
            // brokersDefaultProductsRepo
            // 
            this.brokersDefaultProductsRepo.AutoHeight = false;
            this.brokersDefaultProductsRepo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.brokersDefaultProductsRepo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Product Name")});
            this.brokersDefaultProductsRepo.Name = "brokersDefaultProductsRepo";
            this.brokersDefaultProductsRepo.NullText = "[Default Product is not specified]";
            this.brokersDefaultProductsRepo.ValueMember = "Instance";
            // 
            // companiesGrid
            // 
            this.companiesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.companiesGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.companiesGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.companiesGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.companiesGrid.Location = new System.Drawing.Point(0, 120);
            this.companiesGrid.MainView = this.companiesView;
            this.companiesGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.companiesGrid.Name = "companiesGrid";
            this.companiesGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.companiesRegionsRepo,
            this.companyNameAbbrEditor});
            this.companiesGrid.ShowOnlyPredefinedDetails = true;
            this.companiesGrid.Size = new System.Drawing.Size(1308, 445);
            this.companiesGrid.TabIndex = 7;
            this.companiesGrid.UseEmbeddedNavigator = true;
            this.companiesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.companiesView});
            this.companiesGrid.Visible = false;
            // 
            // companiesView
            // 
            this.companiesView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.companyName,
            this.companyRegion,
            this.companyNameAbbr});
            this.companiesView.GridControl = this.companiesGrid;
            this.companiesView.Name = "companiesView";
            this.companiesView.OptionsCustomization.AllowGroup = false;
            this.companiesView.OptionsView.ShowGroupPanel = false;
            this.companiesView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.companyName, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // companyName
            // 
            this.companyName.Caption = "Company Voice Name";
            this.companyName.FieldName = "CompanyName";
            this.companyName.Name = "companyName";
            this.companyName.Visible = true;
            this.companyName.VisibleIndex = 0;
            this.companyName.Width = 238;
            // 
            // companyRegion
            // 
            this.companyRegion.Caption = "Region";
            this.companyRegion.ColumnEdit = this.companiesRegionsRepo;
            this.companyRegion.FieldName = "Region";
            this.companyRegion.Name = "companyRegion";
            this.companyRegion.Visible = true;
            this.companyRegion.VisibleIndex = 1;
            this.companyRegion.Width = 222;
            // 
            // companiesRegionsRepo
            // 
            this.companiesRegionsRepo.AutoHeight = false;
            this.companiesRegionsRepo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.companiesRegionsRepo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Region")});
            this.companiesRegionsRepo.Name = "companiesRegionsRepo";
            this.companiesRegionsRepo.NullText = "[Region is not specified]";
            this.companiesRegionsRepo.ValueMember = "Instance";
            // 
            // companyNameAbbr
            // 
            this.companyNameAbbr.Caption = "Abbreviation";
            this.companyNameAbbr.ColumnEdit = this.companyNameAbbrEditor;
            this.companyNameAbbr.FieldName = "AbbreviationName";
            this.companyNameAbbr.Name = "companyNameAbbr";
            this.companyNameAbbr.Visible = true;
            this.companyNameAbbr.VisibleIndex = 2;
            // 
            // companyNameAbbrEditor
            // 
            this.companyNameAbbrEditor.AutoHeight = false;
            this.companyNameAbbrEditor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.companyNameAbbrEditor.MaxLength = 3;
            this.companyNameAbbrEditor.Name = "companyNameAbbrEditor";
            // 
            // productSearchResults
            // 
            this.productSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productSearchResults.Location = new System.Drawing.Point(0, 120);
            this.productSearchResults.Name = "productSearchResults";
            this.productSearchResults.Size = new System.Drawing.Size(1308, 445);
            this.productSearchResults.TabIndex = 8;
            this.productSearchResults.Visible = false;
            // 
            // exchangesGrid
            // 
            this.exchangesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exchangesGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.exchangesGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.exchangesGrid.EmbeddedNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(this.gcExchanges_EmbeddedNavigator_ButtonClick);
            this.exchangesGrid.Location = new System.Drawing.Point(0, 120);
            this.exchangesGrid.MainView = this.exchangesView;
            this.exchangesGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.exchangesGrid.Name = "exchangesGrid";
            this.exchangesGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.exchangeCalendarSelector,
            this.exchangeTimezoneSelector});
            this.exchangesGrid.ShowOnlyPredefinedDetails = true;
            this.exchangesGrid.Size = new System.Drawing.Size(1308, 445);
            this.exchangesGrid.TabIndex = 11;
            this.exchangesGrid.UseEmbeddedNavigator = true;
            this.exchangesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.exchangesView});
            this.exchangesGrid.Visible = false;
            // 
            // exchangesView
            // 
            this.exchangesView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.exchangeName,
            this.someMappingValue,
            this.exchangeCalendar,
            this.exchangeTimezone});
            this.exchangesView.GridControl = this.exchangesGrid;
            this.exchangesView.Name = "exchangesView";
            this.exchangesView.OptionsCustomization.AllowGroup = false;
            this.exchangesView.OptionsView.ShowGroupPanel = false;
            this.exchangesView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.exchangeName, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // gridColumn28
            // 
            this.exchangeName.Caption = "Exchange Name";
            this.exchangeName.FieldName = "Name";
            this.exchangeName.Name = "exchangeName";
            this.exchangeName.Visible = true;
            this.exchangeName.VisibleIndex = 0;
            this.exchangeName.Width = 150;
            // 
            // someMappingValue
            // 
            this.someMappingValue.Caption = "Mapping Value";
            this.someMappingValue.FieldName = "MappingValue";
            this.someMappingValue.Name = "someMappingValue";
            this.someMappingValue.Visible = true;
            this.someMappingValue.VisibleIndex = 1;
            this.someMappingValue.Width = 150;
            // 
            // exchangeCalendar
            // 
            this.exchangeCalendar.Caption = "Calendar";
            this.exchangeCalendar.ColumnEdit = this.exchangeCalendarSelector;
            this.exchangeCalendar.FieldName = "Calendar";
            this.exchangeCalendar.Name = "exchangeCalendar";
            this.exchangeCalendar.Visible = true;
            this.exchangeCalendar.VisibleIndex = 2;
            this.exchangeCalendar.Width = 468;
            // 
            // exchangeCalendarSelector
            // 
            this.exchangeCalendarSelector.AutoHeight = false;
            this.exchangeCalendarSelector.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.exchangeCalendarSelector.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")});
            this.exchangeCalendarSelector.Name = "exchangeCalendarSelector";
            this.exchangeCalendarSelector.NullText = "[Calendar is not set]";
            // 
            // exchangeTimezone
            // 
            this.exchangeTimezone.Caption = "Timezone";
            this.exchangeTimezone.ColumnEdit = this.exchangeTimezoneSelector;
            this.exchangeTimezone.FieldName = "Timezone";
            this.exchangeTimezone.Name = "exchangeTimezone";
            this.exchangeTimezone.Visible = true;
            this.exchangeTimezone.VisibleIndex = 3;
            this.exchangeTimezone.Width = 466;
            // 
            // exchangeTimezoneSelector
            // 
            this.exchangeTimezoneSelector.AutoHeight = false;
            this.exchangeTimezoneSelector.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.exchangeTimezoneSelector.Name = "exchangeTimezoneSelector";
            this.exchangeTimezoneSelector.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // tradeTemplatesGrid
            // 
            this.tradeTemplatesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tradeTemplatesGrid.Location = new System.Drawing.Point(0, 120);
            this.tradeTemplatesGrid.MainView = this.tradeTemplatesView;
            this.tradeTemplatesGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.tradeTemplatesGrid.Name = "tradeTemplatesGrid";
            this.tradeTemplatesGrid.ShowOnlyPredefinedDetails = true;
            this.tradeTemplatesGrid.Size = new System.Drawing.Size(1308, 445);
            this.tradeTemplatesGrid.TabIndex = 12;
            this.tradeTemplatesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.tradeTemplatesView});
            this.tradeTemplatesGrid.Visible = false;
            // 
            // tradeTemplatesView
            // 
            this.tradeTemplatesView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.templateName,
            this.portfolioName,
            this.altExchangeName,
            this.altOfficialProductName,
            this.productVolume,
            this.altPriceUnit});
            this.tradeTemplatesView.GridControl = this.tradeTemplatesGrid;
            this.tradeTemplatesView.Name = "tradeTemplatesView";
            this.tradeTemplatesView.OptionsBehavior.Editable = false;
            this.tradeTemplatesView.OptionsCustomization.AllowGroup = false;
            this.tradeTemplatesView.OptionsView.ShowGroupPanel = false;
            // 
            // templateName
            // 
            this.templateName.Caption = "Name";
            this.templateName.FieldName = "TemplateName";
            this.templateName.Name = "templateName";
            this.templateName.Visible = true;
            this.templateName.VisibleIndex = 0;
            // 
            // portfolioName
            // 
            this.portfolioName.Caption = "Book";
            this.portfolioName.FieldName = "Portfolio.Name";
            this.portfolioName.Name = "portfolioName";
            this.portfolioName.Visible = true;
            this.portfolioName.VisibleIndex = 1;
            // 
            // altExchangeName
            // 
            this.altExchangeName.Caption = "Exchange";
            this.altExchangeName.FieldName = "Exchange.Name";
            this.altExchangeName.Name = "altExchangeName";
            this.altExchangeName.Visible = true;
            this.altExchangeName.VisibleIndex = 2;
            // 
            // altOfficialProductName
            // 
            this.altOfficialProductName.Caption = "Official Product";
            this.altOfficialProductName.FieldName = "OfficialProduct.Name";
            this.altOfficialProductName.Name = "altOfficialProductName";
            this.altOfficialProductName.Visible = true;
            this.altOfficialProductName.VisibleIndex = 3;
            // 
            // productVolume
            // 
            this.productVolume.Caption = "Volume";
            this.productVolume.DisplayFormat.FormatString = "N2";
            this.productVolume.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.productVolume.FieldName = "Volume";
            this.productVolume.Name = "productVolume";
            this.productVolume.Visible = true;
            this.productVolume.VisibleIndex = 4;
            // 
            // altPriceUnit
            // 
            this.altPriceUnit.Caption = "Units";
            this.altPriceUnit.FieldName = "PriceUnit";
            this.altPriceUnit.Name = "altPriceUnit";
            this.altPriceUnit.Visible = true;
            this.altPriceUnit.VisibleIndex = 5;
            // 
            // productBreakdown
            // 
            this.productBreakdown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productBreakdown.Controls.Add(this.contractMonthEditor);
            this.productBreakdown.Controls.Add(this.avgSettleEditor);
            this.productBreakdown.Controls.Add(this.overnightSumEditor);
            this.productBreakdown.Controls.Add(this.liveSumEditor);
            this.productBreakdown.Controls.Add(this.pnlEditor);
            this.productBreakdown.Controls.Add(this.productSelector);
            this.productBreakdown.Controls.Add(this.officialProductSelector);
            this.productBreakdown.Controls.Add(this.startPriceEditor);
            this.productBreakdown.Controls.Add(this.quantityEditor);
            this.productBreakdown.Controls.Add(this.testTradeBreakdownContainer);
            this.productBreakdown.Controls.Add(this.tradeEndDateEditor);
            this.productBreakdown.Controls.Add(this.tradeStartDateEditor);
            this.productBreakdown.Controls.Add(this.testTradeDateEditor);
            this.productBreakdown.Controls.Add(this.testTradeAvgSettleTitle);
            this.productBreakdown.Controls.Add(this.testTradeOvernightSumTitle);
            this.productBreakdown.Controls.Add(this.testTradeLiveSumTitle);
            this.productBreakdown.Controls.Add(this.testTradePnLTitle);
            this.productBreakdown.Controls.Add(this.calculateTestTradeImpact);
            this.productBreakdown.Controls.Add(this.FeesMode);
            this.productBreakdown.Controls.Add(this.holidayCalendarsMode);
            this.productBreakdown.Controls.Add(this.endDateTitle);
            this.productBreakdown.Controls.Add(this.startDateTitle);
            this.productBreakdown.Controls.Add(this.testTradeStartPriceTitle);
            this.productBreakdown.Controls.Add(this.testTradeQuantityTitle);
            this.productBreakdown.Controls.Add(this.testTradeDateTitle);
            this.productBreakdown.Controls.Add(this.testTradeContractMonthTitle);
            this.productBreakdown.Controls.Add(this.productTitle);
            this.productBreakdown.Controls.Add(this.officialProductTitle);
            this.productBreakdown.Location = new System.Drawing.Point(0, 120);
            this.productBreakdown.MinimumSize = new System.Drawing.Size(1264, 250);
            this.productBreakdown.Name = "productBreakdown";
            this.productBreakdown.Size = new System.Drawing.Size(1308, 445);
            this.productBreakdown.TabIndex = 14;
            this.productBreakdown.Visible = false;
            // 
            // contractMonthEditor
            // 
            this.contractMonthEditor.Location = new System.Drawing.Point(376, 7);
            this.contractMonthEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.contractMonthEditor.Name = "contractMonthEditor";
            this.contractMonthEditor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.contractMonthEditor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.contractMonthEditor.Size = new System.Drawing.Size(180, 20);
            this.contractMonthEditor.TabIndex = 48;
            // 
            // avgSettleEditor
            // 
            this.avgSettleEditor.Location = new System.Drawing.Point(593, 59);
            this.avgSettleEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.avgSettleEditor.Name = "avgSettleEditor";
            this.avgSettleEditor.Properties.Mask.EditMask = "f4";
            this.avgSettleEditor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.avgSettleEditor.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.avgSettleEditor.Properties.ReadOnly = true;
            this.avgSettleEditor.Size = new System.Drawing.Size(100, 20);
            this.avgSettleEditor.TabIndex = 47;
            // 
            // overnightSumEditor
            // 
            this.overnightSumEditor.Location = new System.Drawing.Point(413, 59);
            this.overnightSumEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.overnightSumEditor.Name = "overnightSumEditor";
            this.overnightSumEditor.Properties.Mask.EditMask = "f4";
            this.overnightSumEditor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.overnightSumEditor.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.overnightSumEditor.Properties.ReadOnly = true;
            this.overnightSumEditor.Size = new System.Drawing.Size(100, 20);
            this.overnightSumEditor.TabIndex = 46;
            // 
            // liveSumEditor
            // 
            this.liveSumEditor.Location = new System.Drawing.Point(212, 59);
            this.liveSumEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.liveSumEditor.Name = "liveSumEditor";
            this.liveSumEditor.Properties.Mask.EditMask = "f4";
            this.liveSumEditor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.liveSumEditor.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.liveSumEditor.Properties.ReadOnly = true;
            this.liveSumEditor.Size = new System.Drawing.Size(100, 20);
            this.liveSumEditor.TabIndex = 45;
            // 
            // pnlEditor
            // 
            this.pnlEditor.Location = new System.Drawing.Point(40, 59);
            this.pnlEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Properties.Mask.EditMask = "f4";
            this.pnlEditor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.pnlEditor.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.pnlEditor.Properties.ReadOnly = true;
            this.pnlEditor.Size = new System.Drawing.Size(100, 20);
            this.pnlEditor.TabIndex = 44;
            // 
            // productSelector
            // 
            this.productSelector.Location = new System.Drawing.Point(92, 33);
            this.productSelector.MenuManager = this.viewSelectorAndActionContainer;
            this.productSelector.Name = "productSelector";
            this.productSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.productSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.productSelector.Properties.DisplayMember = "Name";
            this.productSelector.Size = new System.Drawing.Size(180, 20);
            this.productSelector.TabIndex = 34;
            this.productSelector.EditValueChanged += new System.EventHandler(this.leProduct_EditValueChanged);
            // 
            // officialProductSelector
            // 
            this.officialProductSelector.Location = new System.Drawing.Point(92, 7);
            this.officialProductSelector.MenuManager = this.viewSelectorAndActionContainer;
            this.officialProductSelector.Name = "officialProductSelector";
            this.officialProductSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.officialProductSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.officialProductSelector.Properties.DisplayMember = "Name";
            this.officialProductSelector.Size = new System.Drawing.Size(180, 20);
            this.officialProductSelector.TabIndex = 33;
            this.officialProductSelector.EditValueChanged += new System.EventHandler(this.leOfficialProduct_EditValueChanged);
            // 
            // startPriceEditor
            // 
            this.startPriceEditor.EditValue = "0.0000";
            this.startPriceEditor.Location = new System.Drawing.Point(635, 33);
            this.startPriceEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.startPriceEditor.Name = "startPriceEditor";
            this.startPriceEditor.Properties.Mask.EditMask = "f4";
            this.startPriceEditor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.startPriceEditor.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.startPriceEditor.Size = new System.Drawing.Size(180, 20);
            this.startPriceEditor.TabIndex = 38;
            // 
            // quantityEditor
            // 
            this.quantityEditor.EditValue = "0.0000";
            this.quantityEditor.Location = new System.Drawing.Point(635, 7);
            this.quantityEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.quantityEditor.Name = "quantityEditor";
            this.quantityEditor.Properties.Mask.EditMask = "f2";
            this.quantityEditor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.quantityEditor.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.quantityEditor.Size = new System.Drawing.Size(180, 20);
            this.quantityEditor.TabIndex = 37;
            // 
            // testTradeBreakdownContainer
            // 
            this.testTradeBreakdownContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testTradeBreakdownContainer.Location = new System.Drawing.Point(12, 85);
            this.testTradeBreakdownContainer.Name = "testTradeBreakdownContainer";
            this.testTradeBreakdownContainer.Panel1.Controls.Add(this.productBreakdownGrid);
            this.testTradeBreakdownContainer.Panel1.Text = "Panel1";
            this.testTradeBreakdownContainer.Panel2.Controls.Add(this.testTradeImpactGrid);
            this.testTradeBreakdownContainer.Panel2.Text = "Panel2";
            this.testTradeBreakdownContainer.Size = new System.Drawing.Size(1284, 355);
            this.testTradeBreakdownContainer.SplitterPosition = 541;
            this.testTradeBreakdownContainer.TabIndex = 30;
            this.testTradeBreakdownContainer.Text = "splitContainerControl1";
            // 
            // productBreakdownGrid
            // 
            this.productBreakdownGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productBreakdownGrid.Location = new System.Drawing.Point(0, 0);
            this.productBreakdownGrid.MainView = this.productBreakdownView;
            this.productBreakdownGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.productBreakdownGrid.Name = "productBreakdownGrid";
            this.productBreakdownGrid.Size = new System.Drawing.Size(541, 355);
            this.productBreakdownGrid.TabIndex = 44;
            this.productBreakdownGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.productBreakdownView});
            // 
            // productBreakdownView
            // 
            this.productBreakdownView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.testTradeDay,
            this.testTradeLivePnL,
            this.testTradeOvernightPnL,
            this.testTradeLivePrice,
            this.testTradeSettlement,
            this.testTradeLeg1Settle,
            this.testTradeLeg2Settle});
            this.productBreakdownView.GridControl = this.productBreakdownGrid;
            this.productBreakdownView.Name = "productBreakdownView";
            this.productBreakdownView.OptionsBehavior.Editable = false;
            this.productBreakdownView.OptionsView.ShowGroupPanel = false;
            this.productBreakdownView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvProductBreakdown_FocusedRowChanged);
            // 
            // testTradeDay
            // 
            this.testTradeDay.Caption = "Day";
            this.testTradeDay.FieldName = "Day";
            this.testTradeDay.Name = "testTradeDay";
            this.testTradeDay.Visible = true;
            this.testTradeDay.VisibleIndex = 0;
            // 
            // testTradeLivePnL
            // 
            this.testTradeLivePnL.Caption = "Live PnL";
            this.testTradeLivePnL.FieldName = "LivePnl";
            this.testTradeLivePnL.Name = "testTradeLivePnL";
            this.testTradeLivePnL.Visible = true;
            this.testTradeLivePnL.VisibleIndex = 1;
            // 
            // testTradeOvernightPnL
            // 
            this.testTradeOvernightPnL.Caption = "Overnight";
            this.testTradeOvernightPnL.FieldName = "Overnight";
            this.testTradeOvernightPnL.Name = "testTradeOvernightPnL";
            this.testTradeOvernightPnL.Visible = true;
            this.testTradeOvernightPnL.VisibleIndex = 2;
            // 
            // testTradeLivePrice
            // 
            this.testTradeLivePrice.Caption = "Live Price";
            this.testTradeLivePrice.FieldName = "LivePrice";
            this.testTradeLivePrice.Name = "testTradeLivePrice";
            this.testTradeLivePrice.Visible = true;
            this.testTradeLivePrice.VisibleIndex = 3;
            // 
            // testTradeSettlement
            // 
            this.testTradeSettlement.Caption = "Settlement";
            this.testTradeSettlement.FieldName = "SettlementPrice";
            this.testTradeSettlement.Name = "testTradeSettlement";
            this.testTradeSettlement.Visible = true;
            this.testTradeSettlement.VisibleIndex = 4;
            // 
            // testTradeLeg1Settle
            // 
            this.testTradeLeg1Settle.Caption = "Settle Leg1";
            this.testTradeLeg1Settle.FieldName = "Leg1";
            this.testTradeLeg1Settle.Name = "testTradeLeg1Settle";
            this.testTradeLeg1Settle.Visible = true;
            this.testTradeLeg1Settle.VisibleIndex = 5;
            // 
            // testTradeLeg2Settle
            // 
            this.testTradeLeg2Settle.Caption = "Settle Leg2";
            this.testTradeLeg2Settle.FieldName = "Leg2";
            this.testTradeLeg2Settle.Name = "testTradeLeg2Settle";
            this.testTradeLeg2Settle.Visible = true;
            this.testTradeLeg2Settle.VisibleIndex = 6;
            // 
            // pgTradeImpact
            // 
            this.testTradeImpactGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTradeImpactGrid.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[] {
            this.testTradeProductGroup,
            this.testTradeProduct,
            this.testTradeSource,
            this.testTradeRiskDateYear,
            this.testTradeRiskDateMonth,
            this.testTradePositionsGrid,
            this.testTradeDetailId});
            testTradeImpactLessThanZeroFormat.Appearance.ForeColor = System.Drawing.Color.Red;
            testTradeImpactLessThanZeroFormat.Appearance.Options.UseForeColor = true;
            testTradeImpactLessThanZeroFormat.Condition = DevExpress.XtraGrid.FormatConditionEnum.Less;
            testTradeImpactLessThanZeroFormat.Field = this.testTradePositionsGrid;
            testTradeImpactLessThanZeroFormat.FieldName = "pgfPos";
            testTradeImpactLessThanZeroFormat.Value1 = "0";
            this.testTradeImpactGrid.FormatConditions.AddRange(new DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition[] {
            testTradeImpactLessThanZeroFormat});
            this.testTradeImpactGrid.Location = new System.Drawing.Point(0, 0);
            this.testTradeImpactGrid.Name = "testTradeImpactGrid";
            this.testTradeImpactGrid.OptionsChartDataSource.FieldValuesProvideMode = DevExpress.XtraPivotGrid.PivotChartFieldValuesProvideMode.DisplayText;
            this.testTradeImpactGrid.OptionsView.ShowFilterHeaders = false;
            this.testTradeImpactGrid.Size = new System.Drawing.Size(738, 355);
            this.testTradeImpactGrid.TabIndex = 45;
            this.testTradeImpactGrid.TabStop = false;
            // 
            // testTradeProductGroup
            // 
            this.testTradeProductGroup.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeProductGroup.AreaIndex = 0;
            this.testTradeProductGroup.Caption = "Product Group";
            this.testTradeProductGroup.FieldName = "ProductCategory";
            this.testTradeProductGroup.Name = "testTradeProductGroup";
            this.testTradeProductGroup.Width = 60;
            // 
            // testTradeProduct
            // 
            this.testTradeProduct.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeProduct.AreaIndex = 1;
            this.testTradeProduct.Caption = "Product";
            this.testTradeProduct.FieldName = "Product";
            this.testTradeProduct.Name = "testTradeProduct";
            this.testTradeProduct.Width = 60;
            // 
            // testTradeSource
            // 
            this.testTradeSource.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeSource.AreaIndex = 2;
            this.testTradeSource.Caption = "Source";
            this.testTradeSource.FieldName = "Source";
            this.testTradeSource.Name = "testTradeSource";
            this.testTradeSource.Width = 46;
            // 
            // testTradeRiskDateYear
            // 
            this.testTradeRiskDateYear.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
            this.testTradeRiskDateYear.AreaIndex = 0;
            this.testTradeRiskDateYear.Caption = "Year";
            this.testTradeRiskDateYear.FieldName = "CalculationDate";
            this.testTradeRiskDateYear.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateYear;
            this.testTradeRiskDateYear.Name = "testTradeRiskDateYear";
            this.testTradeRiskDateYear.UnboundFieldName = "pivotGridField4";
            this.testTradeRiskDateYear.Width = 49;
            // 
            // testTradeRiskDateMonth
            // 
            this.testTradeRiskDateMonth.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
            this.testTradeRiskDateMonth.AreaIndex = 1;
            this.testTradeRiskDateMonth.Caption = "Month";
            this.testTradeRiskDateMonth.FieldName = "CalculationDate";
            this.testTradeRiskDateMonth.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateMonth;
            this.testTradeRiskDateMonth.Name = "testTradeRiskDateMonth";
            this.testTradeRiskDateMonth.UnboundFieldName = "pivotGridField5";
            this.testTradeRiskDateMonth.Width = 60;
            // 
            // testTradeDetailId
            // 
            this.testTradeDetailId.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeDetailId.AreaIndex = 3;
            this.testTradeDetailId.FieldName = "DetailId";
            this.testTradeDetailId.Name = "testTradeDetailId";
            this.testTradeDetailId.Visible = false;
            // 
            // tradeEndDateEditor
            // 
            this.tradeEndDateEditor.EditValue = null;
            this.tradeEndDateEditor.Location = new System.Drawing.Point(890, 33);
            this.tradeEndDateEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.tradeEndDateEditor.Name = "tradeEndDateEditor";
            this.tradeEndDateEditor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tradeEndDateEditor.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.tradeEndDateEditor.Size = new System.Drawing.Size(180, 20);
            this.tradeEndDateEditor.TabIndex = 40;
            // 
            // tradeStartDateEditor
            // 
            this.tradeStartDateEditor.EditValue = null;
            this.tradeStartDateEditor.Location = new System.Drawing.Point(890, 7);
            this.tradeStartDateEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.tradeStartDateEditor.Name = "tradeStartDateEditor";
            this.tradeStartDateEditor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tradeStartDateEditor.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.tradeStartDateEditor.Size = new System.Drawing.Size(180, 20);
            this.tradeStartDateEditor.TabIndex = 39;
            // 
            // testTradeDateEditor
            // 
            this.testTradeDateEditor.EditValue = null;
            this.testTradeDateEditor.Location = new System.Drawing.Point(376, 33);
            this.testTradeDateEditor.MenuManager = this.viewSelectorAndActionContainer;
            this.testTradeDateEditor.Name = "testTradeDateEditor";
            this.testTradeDateEditor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.testTradeDateEditor.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.testTradeDateEditor.Size = new System.Drawing.Size(180, 20);
            this.testTradeDateEditor.TabIndex = 36;
            this.testTradeDateEditor.DateTimeChanged += new System.EventHandler(this.dePurchaseDay_DateTimeChanged);
            // 
            // testTradeAvgSettleTitle
            // 
            this.testTradeAvgSettleTitle.Location = new System.Drawing.Point(533, 62);
            this.testTradeAvgSettleTitle.Name = "testTradeAvgSettleTitle";
            this.testTradeAvgSettleTitle.Size = new System.Drawing.Size(54, 13);
            this.testTradeAvgSettleTitle.TabIndex = 22;
            this.testTradeAvgSettleTitle.Text = "Avg Settle:";
            // 
            // testTradeOvernightSumTitle
            // 
            this.testTradeOvernightSumTitle.Location = new System.Drawing.Point(332, 62);
            this.testTradeOvernightSumTitle.Name = "testTradeOvernightSumTitle";
            this.testTradeOvernightSumTitle.Size = new System.Drawing.Size(75, 13);
            this.testTradeOvernightSumTitle.TabIndex = 20;
            this.testTradeOvernightSumTitle.Text = "Sum Overnight:";
            // 
            // testTradeLiveSumTitle
            // 
            this.testTradeLiveSumTitle.Location = new System.Drawing.Point(160, 62);
            this.testTradeLiveSumTitle.Name = "testTradeLiveSumTitle";
            this.testTradeLiveSumTitle.Size = new System.Drawing.Size(46, 13);
            this.testTradeLiveSumTitle.TabIndex = 18;
            this.testTradeLiveSumTitle.Text = "Sum Live:";
            // 
            // testTradePnLTitle
            // 
            this.testTradePnLTitle.Location = new System.Drawing.Point(13, 62);
            this.testTradePnLTitle.Name = "testTradePnLTitle";
            this.testTradePnLTitle.Size = new System.Drawing.Size(21, 13);
            this.testTradePnLTitle.TabIndex = 16;
            this.testTradePnLTitle.Text = "PnL:";
            // 
            // calculateTestTradeImpact
            // 
            this.calculateTestTradeImpact.Location = new System.Drawing.Point(1177, 5);
            this.calculateTestTradeImpact.Name = "calculateTestTradeImpact";
            this.calculateTestTradeImpact.Size = new System.Drawing.Size(75, 23);
            this.calculateTestTradeImpact.TabIndex = 43;
            this.calculateTestTradeImpact.Text = "Calculate";
            this.calculateTestTradeImpact.Click += new System.EventHandler(this.btCalculate_Click);
            // 
            // FeesMode
            // 
            this.FeesMode.Enabled = false;
            this.FeesMode.Location = new System.Drawing.Point(1093, 33);
            this.FeesMode.Name = "FeesMode";
            this.FeesMode.Size = new System.Drawing.Size(75, 23);
            this.FeesMode.TabIndex = 42;
            this.FeesMode.Text = "Fees";
            this.FeesMode.Click += new System.EventHandler(this.btFees_Click);
            // 
            // holidayCalendarsMode
            // 
            this.holidayCalendarsMode.Enabled = false;
            this.holidayCalendarsMode.Location = new System.Drawing.Point(1093, 5);
            this.holidayCalendarsMode.Name = "holidayCalendarsMode";
            this.holidayCalendarsMode.Size = new System.Drawing.Size(75, 23);
            this.holidayCalendarsMode.TabIndex = 41;
            this.holidayCalendarsMode.Text = "Holidays";
            this.holidayCalendarsMode.Click += new System.EventHandler(this.btHolidays_Click);
            // 
            // endDateTitle
            // 
            this.endDateTitle.Location = new System.Drawing.Point(838, 36);
            this.endDateTitle.Name = "endDateTitle";
            this.endDateTitle.Size = new System.Drawing.Size(40, 13);
            this.endDateTitle.TabIndex = 9;
            this.endDateTitle.Text = "End Day";
            // 
            // startDateTitle
            // 
            this.startDateTitle.Location = new System.Drawing.Point(838, 10);
            this.startDateTitle.Name = "startDateTitle";
            this.startDateTitle.Size = new System.Drawing.Size(46, 13);
            this.startDateTitle.TabIndex = 8;
            this.startDateTitle.Text = "Start Day";
            // 
            // testTradeStartPriceTitle
            // 
            this.testTradeStartPriceTitle.Location = new System.Drawing.Point(579, 36);
            this.testTradeStartPriceTitle.Name = "testTradeStartPriceTitle";
            this.testTradeStartPriceTitle.Size = new System.Drawing.Size(50, 13);
            this.testTradeStartPriceTitle.TabIndex = 7;
            this.testTradeStartPriceTitle.Text = "Start Price";
            // 
            // testTradeQuantityTitle
            // 
            this.testTradeQuantityTitle.Location = new System.Drawing.Point(579, 10);
            this.testTradeQuantityTitle.Name = "testTradeQuantityTitle";
            this.testTradeQuantityTitle.Size = new System.Drawing.Size(42, 13);
            this.testTradeQuantityTitle.TabIndex = 6;
            this.testTradeQuantityTitle.Text = "Quantity";
            // 
            // testTradeDateTitle
            // 
            this.testTradeDateTitle.Location = new System.Drawing.Point(295, 36);
            this.testTradeDateTitle.Name = "testTradeDateTitle";
            this.testTradeDateTitle.Size = new System.Drawing.Size(65, 13);
            this.testTradeDateTitle.TabIndex = 5;
            this.testTradeDateTitle.Text = "Purchase day";
            // 
            // testTradeContractMonthTitle
            // 
            this.testTradeContractMonthTitle.Location = new System.Drawing.Point(295, 10);
            this.testTradeContractMonthTitle.Name = "testTradeContractMonthTitle";
            this.testTradeContractMonthTitle.Size = new System.Drawing.Size(75, 13);
            this.testTradeContractMonthTitle.TabIndex = 4;
            this.testTradeContractMonthTitle.Text = "Contract Month";
            // 
            // productTitle
            // 
            this.productTitle.Location = new System.Drawing.Point(13, 36);
            this.productTitle.Name = "productTitle";
            this.productTitle.Size = new System.Drawing.Size(37, 13);
            this.productTitle.TabIndex = 1;
            this.productTitle.Text = "Product";
            // 
            // officialProductTitle
            // 
            this.officialProductTitle.Location = new System.Drawing.Point(13, 10);
            this.officialProductTitle.Name = "officialProductTitle";
            this.officialProductTitle.Size = new System.Drawing.Size(73, 13);
            this.officialProductTitle.TabIndex = 0;
            this.officialProductTitle.Text = "Official Product";
            // 
            // unitsGrid
            // 
            this.unitsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.unitsGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.unitsGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.unitsGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.unitsGrid.EmbeddedNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(this.gcUnits_EmbeddedNavigator_ButtonClick);
            this.unitsGrid.EmbeddedNavigator.Click += new System.EventHandler(this.gcUnits_EmbeddedNavigator_Click);
            this.unitsGrid.Location = new System.Drawing.Point(0, 120);
            this.unitsGrid.MainView = this.unitsView;
            this.unitsGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.unitsGrid.Name = "unitsGrid";
            this.unitsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.allowMonthlyContractSizeOnly,
            this.unitPositionFactorEditor});
            this.unitsGrid.ShowOnlyPredefinedDetails = true;
            this.unitsGrid.Size = new System.Drawing.Size(1308, 445);
            this.unitsGrid.TabIndex = 15;
            this.unitsGrid.UseEmbeddedNavigator = true;
            this.unitsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.unitsView});
            this.unitsGrid.Visible = false;
            this.unitsGrid.Click += new System.EventHandler(this.gcUnits_Click);
            // 
            // unitsView
            // 
            this.unitsView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.unitName,
            this.defaultPositionFactorEditor,
            this.monthlyContractSizeOnly});
            this.unitsView.GridControl = this.unitsGrid;
            this.unitsView.Name = "unitsView";
            this.unitsView.OptionsCustomization.AllowGroup = false;
            this.unitsView.OptionsView.ShowGroupPanel = false;
            this.unitsView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.unitName, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // unitName
            // 
            this.unitName.Caption = "Unit Name";
            this.unitName.FieldName = "Name";
            this.unitName.Name = "unitName";
            this.unitName.Visible = true;
            this.unitName.VisibleIndex = 0;
            this.unitName.Width = 150;
            // 
            // defaultPositionFactorEditor
            // 
            this.defaultPositionFactorEditor.Caption = "Default Position Factor";
            this.defaultPositionFactorEditor.ColumnEdit = this.unitPositionFactorEditor;
            this.defaultPositionFactorEditor.DisplayFormat.FormatString = "f2";
            this.defaultPositionFactorEditor.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.defaultPositionFactorEditor.FieldName = "DefaultPositionFactor";
            this.defaultPositionFactorEditor.Name = "defaultPositionFactorEditor";
            this.defaultPositionFactorEditor.Visible = true;
            this.defaultPositionFactorEditor.VisibleIndex = 1;
            this.defaultPositionFactorEditor.Width = 150;
            // 
            // unitPositionFactorEditor
            // 
            this.unitPositionFactorEditor.AutoHeight = false;
            this.unitPositionFactorEditor.DisplayFormat.FormatString = "f2";
            this.unitPositionFactorEditor.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.unitPositionFactorEditor.EditFormat.FormatString = "f2";
            this.unitPositionFactorEditor.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.unitPositionFactorEditor.Name = "unitPositionFactorEditor";
            // 
            // monthlyContractSizeOnly
            // 
            this.monthlyContractSizeOnly.Caption = "Monthly Contract Size Only";
            this.monthlyContractSizeOnly.ColumnEdit = this.allowMonthlyContractSizeOnly;
            this.monthlyContractSizeOnly.FieldName = "AllowOnlyMonthlyContractSize";
            this.monthlyContractSizeOnly.Name = "monthlyContractSizeOnly";
            this.monthlyContractSizeOnly.Visible = true;
            this.monthlyContractSizeOnly.VisibleIndex = 2;
            // 
            // allowMonthlyContractSizeOnly
            // 
            this.allowMonthlyContractSizeOnly.AutoHeight = false;
            this.allowMonthlyContractSizeOnly.Name = "allowMonthlyContractSizeOnly";
            // 
            // currenciesGrid
            // 
            this.currenciesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currenciesGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.currenciesGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.currenciesGrid.EmbeddedNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(this.gcCurrencies_EmbeddedNavigator_ButtonClick);
            this.currenciesGrid.EmbeddedNavigator.Click += new System.EventHandler(this.gcCurrencies_EmbeddedNavigator_Click);
            this.currenciesGrid.Location = new System.Drawing.Point(0, 120);
            this.currenciesGrid.MainView = this.currenciesView;
            this.currenciesGrid.MenuManager = this.viewSelectorAndActionContainer;
            this.currenciesGrid.Name = "currenciesGrid";
            this.currenciesGrid.ShowOnlyPredefinedDetails = true;
            this.currenciesGrid.Size = new System.Drawing.Size(1308, 445);
            this.currenciesGrid.TabIndex = 16;
            this.currenciesGrid.UseEmbeddedNavigator = true;
            this.currenciesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.currenciesView});
            this.currenciesGrid.Visible = false;
            // 
            // currenciesView
            // 
            this.currenciesView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.currencyIsoName});
            this.currenciesView.GridControl = this.currenciesGrid;
            this.currenciesView.Name = "currenciesView";
            this.currenciesView.OptionsCustomization.AllowGroup = false;
            this.currenciesView.OptionsView.ShowGroupPanel = false;
            this.currenciesView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.currencyIsoName, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.currenciesView.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gvCurrencies_ValidateRow);
            // 
            // currencyIsoName
            // 
            this.currencyIsoName.Caption = "ISO Name";
            this.currencyIsoName.FieldName = "IsoName";
            this.currencyIsoName.Name = "currencyIsoName";
            this.currencyIsoName.Visible = true;
            this.currencyIsoName.VisibleIndex = 0;
            this.currencyIsoName.Width = 150;
            // 
            // ProductListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 565);
            this.Controls.Add(this.viewSelectorAndActionContainer);
            this.Controls.Add(this.txtDummy);
            this.Controls.Add(this.exchangesGrid);
            this.Controls.Add(this.brokersGrid);
            this.Controls.Add(this.defaultProductsGrid);
            this.Controls.Add(this.productSearchResults);
            this.Controls.Add(this.companiesGrid);
            this.Controls.Add(this.currenciesGrid);
            this.Controls.Add(this.productsViewContainer);
            this.Controls.Add(this.productBreakdown);
            this.Controls.Add(this.unitsGrid);
            this.Controls.Add(this.tradeTemplatesGrid);
            this.Controls.Add(this.officialProductsGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductListForm";
            this.Text = "Mandara Products Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProductListForm_FormClosing);
            this.Load += new System.EventHandler(this.ProductListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.viewSelectorAndActionContainer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deskSelectorEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsViewContainer)).EndInit();
            this.productsViewContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.productGroupsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productGroupsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultProductsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultProductsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductsRepo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDummy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersCompanyRepo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokersDefaultProductsRepo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companiesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companiesView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companiesRegionsRepo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companyNameAbbrEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangesView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeCalendarSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeTimezoneSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeTemplatesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeTemplatesView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBreakdown)).EndInit();
            this.productBreakdown.ResumeLayout(false);
            this.productBreakdown.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractMonthEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.avgSettleEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overnightSumEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.liveSumEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startPriceEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.quantityEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeBreakdownContainer)).EndInit();
            this.testTradeBreakdownContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.productBreakdownGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBreakdownView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeImpactGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeEndDateEditor.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeEndDateEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeStartDateEditor.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeStartDateEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeDateEditor.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeDateEditor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitPositionFactorEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowMonthlyContractSizeOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currenciesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currenciesView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl viewSelectorAndActionContainer;
        private DevExpress.XtraBars.Ribbon.RibbonPage viewSelectorAndActionPage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup productGroupsRibbon;
        private DevExpress.XtraBars.BarButtonItem addProductGroup;
        private DevExpress.XtraBars.BarButtonItem editProductGroup;
        private DevExpress.XtraBars.BarButtonItem deleteProductGroup;
        private DevExpress.Utils.ImageCollection buttonImages;
        private DevExpress.XtraEditors.SplitContainerControl productsViewContainer;
        private DevExpress.XtraGrid.GridControl productGroupsGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView productGroupsView;
        private DevExpress.XtraGrid.Columns.GridColumn productGroupNames;
        private DevExpress.XtraGrid.GridControl productsGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView productsView;
        private DevExpress.XtraGrid.Columns.GridColumn productNames;
        private DevExpress.XtraGrid.Columns.GridColumn productOfficialNames;
        private DevExpress.XtraGrid.Columns.GridColumn altName;
        private DevExpress.XtraGrid.Columns.GridColumn productType;
        private DevExpress.XtraGrid.Columns.GridColumn expiryCalendar;
        private DevExpress.XtraGrid.Columns.GridColumn positionFactor;
        private DevExpress.XtraGrid.Columns.GridColumn pnlFactor;
        private DevExpress.XtraGrid.Columns.GridColumn contractSize;
        //private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        //private DevExpress.XtraBars.BarButtonItem barButtonItem4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup viewModeGroup;
        private DevExpress.XtraBars.BarButtonItem addOfficialProduct;
        private DevExpress.XtraBars.BarButtonItem editOfficialProduct;
        private DevExpress.XtraBars.BarButtonItem deleteOfficialProduct;
        private DevExpress.XtraBars.BarButtonItem addProduct;
        private DevExpress.XtraBars.BarButtonItem editProduct;
        private DevExpress.XtraBars.BarButtonItem deleteProduct;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup officialProductsModifyGroup;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup productsModifyGroup;
        private DevExpress.XtraGrid.GridControl officialProductsGrid;
        private DesksConfigurator desksConfigurator;
        private DevExpress.XtraGrid.Views.Grid.GridView officialProductsView;
        private DevExpress.XtraGrid.Columns.GridColumn fullOfficialProductName;
        private DevExpress.XtraGrid.Columns.GridColumn officialProductDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn priceMappingColumn;
        //private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        //private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        //private DevExpress.XtraBars.BarButtonItem barButtonItem5;
        //private DevExpress.XtraBars.BarButtonItem barButtonItem6;
        private DevExpress.XtraBars.BarButtonItem showProducts;
        private DevExpress.XtraBars.BarButtonItem showOfficialProducts;
        private DevExpress.XtraBars.BarButtonItem showOfficialProductDefaultProducts;
        private DevExpress.XtraGrid.GridControl defaultProductsGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView defaultProductsView;
        private DevExpress.XtraGrid.Columns.GridColumn userName;
        private DevExpress.XtraGrid.Columns.GridColumn officialProductName;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit officialProductsRepo;
        private DevExpress.XtraBars.BarButtonItem saveDefaultProducts;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup defaultProductsModifyGroup;
        private DevExpress.XtraEditors.TextEdit txtDummy;
        private DevExpress.XtraBars.BarButtonItem aboutProductTool;
        private DevExpress.XtraBars.BarButtonItem exitProductTool;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup miscellaneousOptions;
        private DevExpress.XtraBars.BarButtonItem showBrokers;
        private DevExpress.XtraGrid.GridControl brokersGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView brokersView;
        private DevExpress.XtraGrid.Columns.GridColumn brokerYahooId;
        private DevExpress.XtraBars.BarButtonItem saveBrokers;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup brokersRibbon;
        private DevExpress.XtraGrid.Columns.GridColumn brokerCompany;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit brokersCompanyRepo;
        private DevExpress.XtraGrid.Columns.GridColumn officialProductRegion;
        private DevExpress.XtraBars.BarButtonItem showCompanies;
        private DevExpress.XtraBars.BarButtonItem saveCompanies;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup companiesRibbon;
        private DevExpress.XtraGrid.GridControl companiesGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView companiesView;
        private DevExpress.XtraGrid.Columns.GridColumn companyName;
        private DevExpress.XtraGrid.Columns.GridColumn companyRegion;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit companiesRegionsRepo;
        private DevExpress.XtraGrid.Columns.GridColumn defaultBrokerProductName;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit brokersDefaultProductsRepo;
        private DevExpress.XtraGrid.Columns.GridColumn earliestProductDate;
        private DevExpress.XtraGrid.Columns.GridColumn latestProductDate;
        private DevExpress.XtraGrid.Columns.GridColumn underlyingFuturesName;
        private DevExpress.XtraBars.BarButtonItem calendarsView;
        private DevExpress.XtraGrid.Columns.GridColumn exchangeContractCode;
        private DevExpress.XtraGrid.Columns.GridColumn settleSymbol;
        private DevExpress.XtraGrid.Columns.GridColumn priceUnit;
        private DevExpress.XtraGrid.Columns.GridColumn bblConversion;
        private DevExpress.XtraGrid.Columns.GridColumn companyNameAbbr;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit companyNameAbbrEditor;
        private DevExpress.XtraGrid.Columns.GridColumn rolloffTime;
        private DevExpress.XtraBars.BarButtonItem editCompany;
        private DevExpress.XtraBars.BarEditItem searchString;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit productSearch;
        private DevExpress.XtraBars.BarButtonItem searchForProducts;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup searchGroup;
        private ProductSearchResults productSearchResults;
        private DevExpress.XtraBars.BarButtonItem showExchanges;
        private DevExpress.XtraGrid.GridControl exchangesGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView exchangesView;
        private DevExpress.XtraGrid.Columns.GridColumn exchangeName;
        private DevExpress.XtraGrid.Columns.GridColumn someMappingValue;
        private DevExpress.XtraGrid.Columns.GridColumn exchangeCalendar;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit exchangeCalendarSelector;
        private DevExpress.XtraBars.BarButtonItem saveExchanges;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup exchangesRibbon;
        private DevExpress.XtraBars.BarButtonItem showTradeTemplates;
        private DevExpress.XtraBars.BarButtonItem addTemplate;
        private DevExpress.XtraBars.BarButtonItem editTemplate;
        private DevExpress.XtraBars.BarButtonItem deleteTemplate;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup tradeTemplatesModifyGroup;
        private DevExpress.XtraGrid.GridControl tradeTemplatesGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView tradeTemplatesView;
        private DevExpress.XtraGrid.Columns.GridColumn templateName;
        private DevExpress.XtraGrid.Columns.GridColumn portfolioName;
        private DevExpress.XtraGrid.Columns.GridColumn altExchangeName;
        private DevExpress.XtraGrid.Columns.GridColumn altOfficialProductName;
        private DevExpress.XtraGrid.Columns.GridColumn productVolume;
        private DevExpress.XtraGrid.Columns.GridColumn altPriceUnit;
        private DevExpress.XtraBars.BarButtonItem productRecalculateChanged;
        private DevExpress.XtraBars.BarButtonItem productRecalculateAll;
        private DevExpress.XtraBars.BarButtonItem recalculateManualTrades;
        private DevExpress.XtraBars.BarButtonItem showProductBreakdown;
        private DevExpress.XtraEditors.PanelControl productBreakdown;
        private DevExpress.XtraEditors.LabelControl testTradeAvgSettleTitle;
        private DevExpress.XtraEditors.LabelControl testTradeOvernightSumTitle;
        private DevExpress.XtraEditors.LabelControl testTradeLiveSumTitle;
        private DevExpress.XtraEditors.LabelControl testTradePnLTitle;
        private DevExpress.XtraEditors.SimpleButton calculateTestTradeImpact;
        private DevExpress.XtraEditors.SimpleButton FeesMode;
        private DevExpress.XtraEditors.SimpleButton holidayCalendarsMode;
        private DevExpress.XtraEditors.LabelControl endDateTitle;
        private DevExpress.XtraEditors.LabelControl startDateTitle;
        private DevExpress.XtraEditors.LabelControl testTradeStartPriceTitle;
        private DevExpress.XtraEditors.LabelControl testTradeQuantityTitle;
        private DevExpress.XtraEditors.LabelControl testTradeDateTitle;
        private DevExpress.XtraEditors.LabelControl testTradeContractMonthTitle;
        private DevExpress.XtraEditors.LabelControl productTitle;
        private DevExpress.XtraEditors.LabelControl officialProductTitle;
        private DevExpress.XtraEditors.DateEdit tradeEndDateEditor;
        private DevExpress.XtraEditors.DateEdit tradeStartDateEditor;
        private DevExpress.XtraEditors.DateEdit testTradeDateEditor;
        private DevExpress.XtraEditors.SplitContainerControl testTradeBreakdownContainer;
        private DevExpress.XtraGrid.GridControl productBreakdownGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView productBreakdownView;
        private DevExpress.XtraPivotGrid.PivotGridControl testTradeImpactGrid;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeProductGroup;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeProduct;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeSource;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeRiskDateYear;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeRiskDateMonth;
        private DevExpress.XtraPivotGrid.PivotGridField testTradePositionsGrid;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeDetailId;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeDay;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeLivePnL;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeOvernightPnL;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeSettlement;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeLeg1Settle;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeLeg2Settle;
        private DevExpress.XtraEditors.TextEdit quantityEditor;
        private DevExpress.XtraEditors.TextEdit startPriceEditor;
        private DevExpress.XtraEditors.LookUpEdit productSelector;
        private DevExpress.XtraEditors.LookUpEdit officialProductSelector;
        private DevExpress.XtraEditors.TextEdit avgSettleEditor;
        private DevExpress.XtraEditors.TextEdit overnightSumEditor;
        private DevExpress.XtraEditors.TextEdit liveSumEditor;
        private DevExpress.XtraEditors.TextEdit pnlEditor;
        private DevExpress.XtraGrid.Columns.GridColumn testTradeLivePrice;
        private DevExpress.XtraEditors.ComboBoxEdit contractMonthEditor;
        private DevExpress.XtraGrid.GridControl unitsGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView unitsView;
        private DevExpress.XtraGrid.Columns.GridColumn unitName;
        private DevExpress.XtraGrid.Columns.GridColumn defaultPositionFactorEditor;
        private DevExpress.XtraBars.BarButtonItem showUnits;
        private DevExpress.XtraBars.BarButtonItem saveUnits;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup unitsModifyGroup;
        private DevExpress.XtraGrid.GridControl currenciesGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView currenciesView;
        private DevExpress.XtraGrid.Columns.GridColumn currencyIsoName;
        private DevExpress.XtraBars.BarButtonItem showCurrencies;
        private DevExpress.XtraBars.BarButtonItem saveCurrencies;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup currenciesModifyGroup;
        private DevExpress.XtraGrid.Columns.GridColumn monthlyContractSizeOnly;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit allowMonthlyContractSizeOnly;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit unitPositionFactorEditor;
        private DevExpress.XtraGrid.Columns.GridColumn exchangeTimezone;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox exchangeTimezoneSelector;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit unitLookup;
        private DevExpress.XtraGrid.Columns.GridColumn unitToBblConversion;
        private DevExpress.XtraBars.BarEditItem deskSelector;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit deskSelectorEditor;
        private DevExpress.XtraBars.BarButtonItem showDesks;
        private DevExpress.XtraBars.BarButtonItem deskAdd;
        private DevExpress.XtraBars.BarButtonItem deskEdit;
        private DevExpress.XtraBars.BarButtonItem deskDelete;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup deskGroup;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup deskOfficialProductsGroup;
        private DevExpress.XtraBars.BarButtonItem deskProductAdd;
        private DevExpress.XtraBars.BarButtonItem deskProductEdit;
        private DevExpress.XtraBars.BarButtonItem deskProductDelete;
    }
}