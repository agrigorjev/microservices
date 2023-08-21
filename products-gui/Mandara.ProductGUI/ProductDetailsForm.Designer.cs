using System;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Mandara.ProductGUI
{
    partial class ProductDetailsForm
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
            DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition pivotGridStyleFormatCondition1 = new DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition();
            this.pgfPos = new DevExpress.XtraPivotGrid.PivotGridField();
            this.productNameTitle = new DevExpress.XtraEditors.LabelControl();
            this.productName = new DevExpress.XtraEditors.TextEdit();
            this.productTypeTitle = new DevExpress.XtraEditors.LabelControl();
            this.expiryCalendarTitle = new DevExpress.XtraEditors.LabelControl();
            this.officialProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.productGroupTitle = new DevExpress.XtraEditors.LabelControl();
            this.positionConversionFactorTitle = new DevExpress.XtraEditors.LabelControl();
            this.pnlConversionFactorTitle = new DevExpress.XtraEditors.LabelControl();
            this.contractSizeTitle = new DevExpress.XtraEditors.LabelControl();
            this.expiryCalendar = new DevExpress.XtraEditors.LookUpEdit();
            this.productCategorySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.officialProductSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.complexProductSection = new DevExpress.XtraEditors.GroupControl();
            this.calculatePnlFromLegs = new DevExpress.XtraEditors.CheckEdit();
            this.leg2PnlFactor = new DevExpress.XtraEditors.TextEdit();
            this.leg1PnlFactor = new DevExpress.XtraEditors.TextEdit();
            this.pnlFactorsTitle = new DevExpress.XtraEditors.LabelControl();
            this.positionFactorsTitle = new DevExpress.XtraEditors.LabelControl();
            this.treatTimeSpreadStripAsLegs = new DevExpress.XtraEditors.CheckEdit();
            this.useCommonPricing = new DevExpress.XtraEditors.CheckEdit();
            this.leg2PositionFactor = new DevExpress.XtraEditors.TextEdit();
            this.leg1PositionFactor = new DevExpress.XtraEditors.TextEdit();
            this.legt1FactorsTitle = new DevExpress.XtraEditors.LabelControl();
            this.leg2ProductSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.leg2ProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.leg1FactorsTitle = new DevExpress.XtraEditors.LabelControl();
            this.leg1ProductSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.leg1ProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.commonProductPropertiesSection = new DevExpress.XtraEditors.GroupControl();
            this.contractSizeMultiplier = new DevExpress.XtraEditors.ComboBoxEdit();
            this.currency2Selector = new DevExpress.XtraEditors.LookUpEdit();
            this.currency2Title = new DevExpress.XtraEditors.LabelControl();
            this.currency1Selector = new DevExpress.XtraEditors.LookUpEdit();
            this.currency1Title = new DevExpress.XtraEditors.LabelControl();
            this.holidayCalendarTitle = new DevExpress.XtraEditors.LabelControl();
            this.holidaysCalendar = new DevExpress.XtraEditors.LookUpEdit();
            this.unitsSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.dailyDiffMonthShift = new DevExpress.XtraEditors.LookUpEdit();
            this.dailyDiffMonthShiftTitle = new DevExpress.XtraEditors.LabelControl();
            this.monthlyOfficialProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.monthlyOfficialProductSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.tasOfficialProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.tasOfficialProductSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.priceConversionFactor = new DevExpress.XtraEditors.TextEdit();
            this.priceConversionFactorTitle = new DevExpress.XtraEditors.LabelControl();
            this.underlyingFuturesSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.exchangeTitle = new DevExpress.XtraEditors.LabelControl();
            this.exchangeSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.definitionLinkTitle = new DevExpress.XtraEditors.LabelControl();
            this.underlyingFuturesTitle = new DevExpress.XtraEditors.LabelControl();
            this.productTypeSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.exchangeContractCode = new DevExpress.XtraEditors.TextEdit();
            this.positionFactor = new DevExpress.XtraEditors.TextEdit();
            this.pnlFactor = new DevExpress.XtraEditors.TextEdit();
            this.contractSize = new DevExpress.XtraEditors.TextEdit();
            this.exchContractCodeTitle = new DevExpress.XtraEditors.LabelControl();
            this.definitionLink = new DevExpress.XtraEditors.HyperLinkEdit();
            this.useExpiryCalendar = new DevExpress.XtraEditors.CheckEdit();
            this.validTo = new DevExpress.XtraEditors.DateEdit();
            this.validToTitle = new DevExpress.XtraEditors.LabelControl();
            this.validFrom = new DevExpress.XtraEditors.DateEdit();
            this.validFromTitle = new DevExpress.XtraEditors.LabelControl();
            this.save = new DevExpress.XtraEditors.SimpleButton();
            this.cancel = new DevExpress.XtraEditors.SimpleButton();
            this.productAliasesSection = new DevExpress.XtraEditors.GroupControl();
            this.productAliases = new DevExpress.XtraGrid.GridControl();
            this.productAliasesDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.alias = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rolloffSettings = new DevExpress.XtraEditors.GroupControl();
            this.mocActivationTimezoneSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.mocActivationTime = new DevExpress.XtraEditors.TimeEdit();
            this.mocActivationTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.isCalendarDaySwap = new DevExpress.XtraEditors.CheckEdit();
            this.useRolloffSettings = new DevExpress.XtraEditors.CheckEdit();
            this.localRollOffTime = new DevExpress.XtraEditors.LabelControl();
            this.localRollOffTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.rolloffTime = new DevExpress.XtraEditors.TimeEdit();
            this.rollOffTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.rolloffTimezoneSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.timeZoneTitle = new DevExpress.XtraEditors.LabelControl();
            this.futuresExpirationSettings = new DevExpress.XtraEditors.GroupControl();
            this.tasTimezoneSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.tasTimeZoneTitle = new DevExpress.XtraEditors.LabelControl();
            this.pricingEndTime = new DevExpress.XtraEditors.TimeEdit();
            this.pricingEndTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.futuresExpiryTime = new DevExpress.XtraEditors.TimeEdit();
            this.futuresExpiryTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.useFuturesExpirationSettings = new DevExpress.XtraEditors.CheckEdit();
            this.tasActivationTime = new DevExpress.XtraEditors.TimeEdit();
            this.tasActivationTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.productBrokerageByCompanySection = new DevExpress.XtraEditors.GroupControl();
            this.companiesBrokerage = new DevExpress.XtraGrid.GridControl();
            this.companyBrokerageDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.companyName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.companySelector = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.brokerage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.futuresExpirationDate = new DevExpress.XtraEditors.GroupControl();
            this.expireTimeTitle = new DevExpress.XtraEditors.LabelControl();
            this.numberOfDaysSection = new DevExpress.XtraEditors.PanelControl();
            this.numberOfDaysTitle = new DevExpress.XtraEditors.LabelControl();
            this.numberOfDaysSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.expirationMonthSection = new DevExpress.XtraEditors.PanelControl();
            this.expirationMonthTitle = new DevExpress.XtraEditors.LabelControl();
            this.expirationMonthSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.isRollingBackward = new DevExpress.XtraEditors.CheckEdit();
            this.isRollingForward = new DevExpress.XtraEditors.CheckEdit();
            this.rollingMethodTitle = new DevExpress.XtraEditors.LabelControl();
            this.expTypeNumOfDaysSelector = new DevExpress.XtraEditors.CheckEdit();
            this.expiryTypeGivenDateSelector = new DevExpress.XtraEditors.CheckEdit();
            this.expiryTypeCalendarSelector = new DevExpress.XtraEditors.CheckEdit();
            this.expiryTypeTitle = new DevExpress.XtraEditors.LabelControl();
            this.givenDateSection = new DevExpress.XtraEditors.PanelControl();
            this.givenDate = new DevExpress.XtraEditors.DateEdit();
            this.givenDateTitle = new DevExpress.XtraEditors.LabelControl();
            this.productTabs = new DevExpress.XtraTab.XtraTabControl();
            this.basicSettings = new DevExpress.XtraTab.XtraTabPage();
            this.basicSettingsSection = new DevExpress.XtraEditors.PanelControl();
            this.productValidityRangeSection = new DevExpress.XtraEditors.GroupControl();
            this.testTradeImpactSection = new DevExpress.XtraEditors.GroupControl();
            this.testTradeStartDateTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradePnL = new DevExpress.XtraEditors.TextEdit();
            this.testTradePnLTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradePrice = new DevExpress.XtraEditors.TextEdit();
            this.testTradeImpact = new DevExpress.XtraPivotGrid.PivotGridControl();
            this.testTradeImpactProductGroupSort = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeImpactProductSort = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeImpactSourceSort = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeImpactCalcYearSort = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeImpactMonthSort = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeImpactCalcIdSort = new DevExpress.XtraPivotGrid.PivotGridField();
            this.testTradeStripSelector = new DevExpress.XtraEditors.ComboBoxEdit();
            this.testTradePriceTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeLivePrice = new DevExpress.XtraEditors.TextEdit();
            this.testTradeVolume = new DevExpress.XtraEditors.TextEdit();
            this.testTradeLivePriceTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeVolumeTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeStripTitle = new DevExpress.XtraEditors.LabelControl();
            this.testTradeStartDate = new DevExpress.XtraEditors.DateEdit();
            this.productOptionsSection = new DevExpress.XtraEditors.GroupControl();
            this.enableRiskDecomposition = new DevExpress.XtraEditors.CheckEdit();
            this.allowedForManualTrades = new DevExpress.XtraEditors.CheckEdit();
            this.tasTypeSelectors = new DevExpress.XtraEditors.PanelControl();
            this.isMoc = new DevExpress.XtraEditors.CheckEdit();
            this.isPlain = new DevExpress.XtraEditors.CheckEdit();
            this.isTas = new DevExpress.XtraEditors.CheckEdit();
            this.isMinuteMarker = new DevExpress.XtraEditors.CheckEdit();
            this.isMops = new DevExpress.XtraEditors.CheckEdit();
            this.isInternalTransferProduct = new DevExpress.XtraEditors.CheckEdit();
            this.isPhysicallySettled = new DevExpress.XtraEditors.CheckEdit();
            this.alsoIsTas = new DevExpress.XtraEditors.CheckEdit();
            this.advancedSettings = new DevExpress.XtraTab.XtraTabPage();
            this.balmoMappingsSection = new DevExpress.XtraEditors.PanelControl();
            this.iceBalmoMappingsSection = new DevExpress.XtraEditors.GroupControl();
            this.iceBalmoMappings = new DevExpress.XtraGrid.GridControl();
            this.iceBalmoMappingsDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.iceBalmoPrefix = new DevExpress.XtraGrid.Columns.GridColumn();
            this.iceBalmoPrefixValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.iceBalmoStartChar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.iceBalmoCharValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.iceBalmoEndChar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.iceBalmoStartDay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.iceBalmoStartDayValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.companyNameSelector = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.categoryOverrideSection = new DevExpress.XtraEditors.GroupControl();
            this.underlyingFuturesOverride = new DevExpress.XtraEditors.LookUpEdit();
            this.categoryOverride = new DevExpress.XtraEditors.LookUpEdit();
            this.categoryOverrideDate = new DevExpress.XtraEditors.DateEdit();
            this.categoryOverrideDateTitle = new DevExpress.XtraEditors.LabelControl();
            this.overrideCategorySelectorTitle = new DevExpress.XtraEditors.LabelControl();
            this.iceProductAliasesSection = new DevExpress.XtraEditors.GroupControl();
            this.iceAliases = new DevExpress.XtraGrid.GridControl();
            this.iceAliasesDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.iceAliasProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.iceProductAliasIdValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.abnMappingsSection = new DevExpress.XtraEditors.GroupControl();
            this.abnMappings = new DevExpress.XtraGrid.GridControl();
            this.abnMappingsDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.abnExchangeContractCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.abnProductCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.brokerageCompanyIdValue = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.balmoComplexProductsSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.balmoComplexProductTitle = new DevExpress.XtraEditors.LabelControl();
            this.balmoCrudeSwapsSelector = new DevExpress.XtraEditors.LookUpEdit();
            this.balmoCrudeSwapTitle = new DevExpress.XtraEditors.LabelControl();
            this.balmoContractCodesSection = new DevExpress.XtraEditors.GroupControl();
            this.contractCodeOneFirstLetter = new DevExpress.XtraEditors.TextEdit();
            this.contractCodeThree = new DevExpress.XtraEditors.TextEdit();
            this.contractCodeOneFirstLetterTitle = new DevExpress.XtraEditors.LabelControl();
            this.contractCodeThreeTitle = new DevExpress.XtraEditors.LabelControl();
            this.contractCodeTwo = new DevExpress.XtraEditors.TextEdit();
            this.contractCodeTwoTitle = new DevExpress.XtraEditors.LabelControl();
            this.contractCodeOne = new DevExpress.XtraEditors.TextEdit();
            this.contractCodeOneTitle = new DevExpress.XtraEditors.LabelControl();
            this.productFeesSection = new DevExpress.XtraEditors.GroupControl();
            this.plattsCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.cashCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.blockCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.clearingCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.nfaCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.exchangeCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.primeBrokerCurrencySelector = new DevExpress.XtraEditors.LookUpEdit();
            this.exchangeFee = new DevExpress.XtraEditors.TextEdit();
            this.exchangeFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.plattsFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.plattsFee = new DevExpress.XtraEditors.TextEdit();
            this.blockFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.blockFee = new DevExpress.XtraEditors.TextEdit();
            this.cashFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.cashFee = new DevExpress.XtraEditors.TextEdit();
            this.clearingFee = new DevExpress.XtraEditors.TextEdit();
            this.clearingFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.commisionFee = new DevExpress.XtraEditors.TextEdit();
            this.primerBrokerFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.nfaFee = new DevExpress.XtraEditors.TextEdit();
            this.nfaFeeTitle = new DevExpress.XtraEditors.LabelControl();
            this.gmiBalmoCodesSection = new DevExpress.XtraEditors.GroupControl();
            this.gmiBalmoCodes = new DevExpress.XtraGrid.GridControl();
            this.gmiBalmoCodesDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gmiExchangeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gmiPrefix = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gmiPrefixValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gmiStartChar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gmiStartCharValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gmiEndChar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gmiPricingDay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gmiPricingDayValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gmiCompanyCodeSelector = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.availableCurrency1 = new System.Windows.Forms.BindingSource(this.components);
            this.errorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.availableCurrency2 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.productName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryCalendar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productCategorySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.complexProductSection)).BeginInit();
            this.complexProductSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calculatePnlFromLegs.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg2PnlFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg1PnlFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treatTimeSpreadStripAsLegs.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.useCommonPricing.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg2PositionFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg1PositionFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg2ProductSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg1ProductSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.commonProductPropertiesSection)).BeginInit();
            this.commonProductPropertiesSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractSizeMultiplier.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currency2Selector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currency1Selector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.holidaysCalendar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitsSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dailyDiffMonthShift.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.monthlyOfficialProductSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasOfficialProductSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceConversionFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.underlyingFuturesSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productTypeSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeContractCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractSize.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.definitionLink.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.useExpiryCalendar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productAliasesSection)).BeginInit();
            this.productAliasesSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.productAliases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productAliasesDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rolloffSettings)).BeginInit();
            this.rolloffSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mocActivationTimezoneSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mocActivationTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isCalendarDaySwap.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.useRolloffSettings.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rolloffTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rolloffTimezoneSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.futuresExpirationSettings)).BeginInit();
            this.futuresExpirationSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tasTimezoneSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pricingEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.futuresExpiryTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.useFuturesExpirationSettings.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasActivationTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBrokerageByCompanySection)).BeginInit();
            this.productBrokerageByCompanySection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.companiesBrokerage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companyBrokerageDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companySelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.futuresExpirationDate)).BeginInit();
            this.futuresExpirationDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfDaysSection)).BeginInit();
            this.numberOfDaysSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfDaysSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expirationMonthSection)).BeginInit();
            this.expirationMonthSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expirationMonthSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isRollingBackward.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isRollingForward.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTypeNumOfDaysSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryTypeGivenDateSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryTypeCalendarSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.givenDateSection)).BeginInit();
            this.givenDateSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.givenDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.givenDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productTabs)).BeginInit();
            this.productTabs.SuspendLayout();
            this.basicSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.basicSettingsSection)).BeginInit();
            this.basicSettingsSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.productValidityRangeSection)).BeginInit();
            this.productValidityRangeSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeImpactSection)).BeginInit();
            this.testTradeImpactSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testTradePnL.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradePrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeImpact)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeStripSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeLivePrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeVolume.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeStartDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeStartDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productOptionsSection)).BeginInit();
            this.productOptionsSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.enableRiskDecomposition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowedForManualTrades.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasTypeSelectors)).BeginInit();
            this.tasTypeSelectors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.isMoc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isPlain.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isTas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isMinuteMarker.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isMops.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isInternalTransferProduct.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isPhysicallySettled.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alsoIsTas.Properties)).BeginInit();
            this.advancedSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.balmoMappingsSection)).BeginInit();
            this.balmoMappingsSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoMappingsSection)).BeginInit();
            this.iceBalmoMappingsSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoMappings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoMappingsDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoPrefixValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoCharValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoStartDayValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.companyNameSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverrideSection)).BeginInit();
            this.categoryOverrideSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.underlyingFuturesOverride.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverride.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverrideDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverrideDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceProductAliasesSection)).BeginInit();
            this.iceProductAliasesSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iceAliases)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceAliasesDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceProductAliasIdValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.abnMappingsSection)).BeginInit();
            this.abnMappingsSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.abnMappings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.abnMappingsDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokerageCompanyIdValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.balmoComplexProductsSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.balmoCrudeSwapsSelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.balmoContractCodesSection)).BeginInit();
            this.balmoContractCodesSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeOneFirstLetter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeThree.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeTwo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeOne.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productFeesSection)).BeginInit();
            this.productFeesSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plattsCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blockCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clearingCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nfaCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.primeBrokerCurrencySelector.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blockFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clearingFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.commisionFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nfaFee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiBalmoCodesSection)).BeginInit();
            this.gmiBalmoCodesSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gmiBalmoCodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiBalmoCodesDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiPrefixValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiStartCharValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiPricingDayValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiCompanyCodeSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableCurrency1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableCurrency2)).BeginInit();
            this.SuspendLayout();
            // 
            // pgfPos
            // 
            this.pgfPos.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
            this.pgfPos.AreaIndex = 0;
            this.pgfPos.Caption = "Pos";
            this.pgfPos.CellFormat.FormatString = "#,#0.0#;(#,#0.0#);#0";
            this.pgfPos.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.pgfPos.FieldName = "Amount";
            this.pgfPos.Name = "pgfPos";
            this.pgfPos.Width = 55;
            // 
            // productNameTitle
            // 
            this.productNameTitle.Location = new System.Drawing.Point(64, 27);
            this.productNameTitle.Name = "productNameTitle";
            this.productNameTitle.Size = new System.Drawing.Size(31, 13);
            this.productNameTitle.TabIndex = 0;
            this.productNameTitle.Text = "Name:";
            // 
            // productName
            // 
            this.productName.Location = new System.Drawing.Point(102, 24);
            this.productName.Name = "productName";
            this.productName.Size = new System.Drawing.Size(165, 20);
            this.productName.TabIndex = 0;
            // 
            // productTypeTitle
            // 
            this.productTypeTitle.Location = new System.Drawing.Point(347, 27);
            this.productTypeTitle.Name = "productTypeTitle";
            this.productTypeTitle.Size = new System.Drawing.Size(28, 13);
            this.productTypeTitle.TabIndex = 2;
            this.productTypeTitle.Text = "Type:";
            // 
            // expiryCalendarTitle
            // 
            this.expiryCalendarTitle.Location = new System.Drawing.Point(298, 79);
            this.expiryCalendarTitle.Name = "expiryCalendarTitle";
            this.expiryCalendarTitle.Size = new System.Drawing.Size(78, 13);
            this.expiryCalendarTitle.TabIndex = 3;
            this.expiryCalendarTitle.Text = "Expiry calendar:";
            // 
            // officialProductTitle
            // 
            this.officialProductTitle.Location = new System.Drawing.Point(18, 53);
            this.officialProductTitle.Name = "officialProductTitle";
            this.officialProductTitle.Size = new System.Drawing.Size(77, 13);
            this.officialProductTitle.TabIndex = 4;
            this.officialProductTitle.Text = "Official product:";
            // 
            // productGroupTitle
            // 
            this.productGroupTitle.Location = new System.Drawing.Point(62, 79);
            this.productGroupTitle.Name = "productGroupTitle";
            this.productGroupTitle.Size = new System.Drawing.Size(33, 13);
            this.productGroupTitle.TabIndex = 5;
            this.productGroupTitle.Text = "Group:";
            // 
            // positionConversionFactorTitle
            // 
            this.positionConversionFactorTitle.Location = new System.Drawing.Point(28, 157);
            this.positionConversionFactorTitle.Name = "positionConversionFactorTitle";
            this.positionConversionFactorTitle.Size = new System.Drawing.Size(128, 13);
            this.positionConversionFactorTitle.TabIndex = 6;
            this.positionConversionFactorTitle.Text = "Position conversion factor:";
            // 
            // pnlConversionFactorTitle
            // 
            this.pnlConversionFactorTitle.Location = new System.Drawing.Point(48, 131);
            this.pnlConversionFactorTitle.Name = "pnlConversionFactorTitle";
            this.pnlConversionFactorTitle.Size = new System.Drawing.Size(108, 13);
            this.pnlConversionFactorTitle.TabIndex = 7;
            this.pnlConversionFactorTitle.Text = "PnL conversion factor:";
            // 
            // contractSizeTitle
            // 
            this.contractSizeTitle.Location = new System.Drawing.Point(28, 105);
            this.contractSizeTitle.Name = "contractSizeTitle";
            this.contractSizeTitle.Size = new System.Drawing.Size(67, 13);
            this.contractSizeTitle.TabIndex = 8;
            this.contractSizeTitle.Text = "Contract size:";
            // 
            // expiryCalendar
            // 
            this.expiryCalendar.Location = new System.Drawing.Point(382, 76);
            this.expiryCalendar.Name = "expiryCalendar";
            this.expiryCalendar.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.expiryCalendar.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Calendar", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.expiryCalendar.Properties.NullText = "[Select expiry calendar]";
            this.expiryCalendar.Size = new System.Drawing.Size(165, 20);
            this.expiryCalendar.TabIndex = 5;
            this.expiryCalendar.EditValueChanged += new System.EventHandler(this.ExpiryCalendarChanged);
            // 
            // productCategorySelector
            // 
            this.productCategorySelector.Location = new System.Drawing.Point(102, 76);
            this.productCategorySelector.Name = "productCategorySelector";
            this.productCategorySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.productCategorySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Group", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.productCategorySelector.Properties.NullText = "[Select group]";
            this.productCategorySelector.Size = new System.Drawing.Size(165, 20);
            this.productCategorySelector.TabIndex = 4;
            // 
            // officialProductSelector
            // 
            this.officialProductSelector.Location = new System.Drawing.Point(102, 50);
            this.officialProductSelector.Name = "officialProductSelector";
            this.officialProductSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.officialProductSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Official Product", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.officialProductSelector.Properties.NullText = "[Select official product]";
            this.officialProductSelector.Size = new System.Drawing.Size(165, 20);
            this.officialProductSelector.TabIndex = 2;
            // 
            // complexProductSection
            // 
            this.complexProductSection.Controls.Add(this.calculatePnlFromLegs);
            this.complexProductSection.Controls.Add(this.leg2PnlFactor);
            this.complexProductSection.Controls.Add(this.leg1PnlFactor);
            this.complexProductSection.Controls.Add(this.pnlFactorsTitle);
            this.complexProductSection.Controls.Add(this.positionFactorsTitle);
            this.complexProductSection.Controls.Add(this.treatTimeSpreadStripAsLegs);
            this.complexProductSection.Controls.Add(this.useCommonPricing);
            this.complexProductSection.Controls.Add(this.leg2PositionFactor);
            this.complexProductSection.Controls.Add(this.leg1PositionFactor);
            this.complexProductSection.Controls.Add(this.legt1FactorsTitle);
            this.complexProductSection.Controls.Add(this.leg2ProductSelector);
            this.complexProductSection.Controls.Add(this.leg2ProductTitle);
            this.complexProductSection.Controls.Add(this.leg1FactorsTitle);
            this.complexProductSection.Controls.Add(this.leg1ProductSelector);
            this.complexProductSection.Controls.Add(this.leg1ProductTitle);
            this.complexProductSection.Location = new System.Drawing.Point(5, 273);
            this.complexProductSection.Name = "complexProductSection";
            this.complexProductSection.Size = new System.Drawing.Size(559, 129);
            this.complexProductSection.TabIndex = 1;
            this.complexProductSection.Text = "Complex Product";
            // 
            // calculatePnlFromLegs
            // 
            this.calculatePnlFromLegs.Location = new System.Drawing.Point(10, 101);
            this.calculatePnlFromLegs.Name = "calculatePnlFromLegs";
            this.calculatePnlFromLegs.Properties.Caption = "Calculate PnL from Legs";
            this.calculatePnlFromLegs.Size = new System.Drawing.Size(146, 19);
            this.calculatePnlFromLegs.TabIndex = 27;
            // 
            // leg2PnlFactor
            // 
            this.leg2PnlFactor.Location = new System.Drawing.Point(475, 73);
            this.leg2PnlFactor.Name = "leg2PnlFactor";
            this.leg2PnlFactor.Size = new System.Drawing.Size(72, 20);
            this.leg2PnlFactor.TabIndex = 8;
            // 
            // leg1PnlFactor
            // 
            this.leg1PnlFactor.Location = new System.Drawing.Point(475, 49);
            this.leg1PnlFactor.Name = "leg1PnlFactor";
            this.leg1PnlFactor.Size = new System.Drawing.Size(72, 20);
            this.leg1PnlFactor.TabIndex = 4;
            // 
            // pnlFactorsTitle
            // 
            this.pnlFactorsTitle.Location = new System.Drawing.Point(475, 27);
            this.pnlFactorsTitle.Name = "pnlFactorsTitle";
            this.pnlFactorsTitle.Size = new System.Drawing.Size(56, 13);
            this.pnlFactorsTitle.TabIndex = 1;
            this.pnlFactorsTitle.Text = "PnL Factors";
            // 
            // positionFactorsTitle
            // 
            this.positionFactorsTitle.Location = new System.Drawing.Point(379, 27);
            this.positionFactorsTitle.Name = "positionFactorsTitle";
            this.positionFactorsTitle.Size = new System.Drawing.Size(76, 13);
            this.positionFactorsTitle.TabIndex = 26;
            this.positionFactorsTitle.Text = "Position Factors";
            // 
            // treatTimeSpreadStripAsLegs
            // 
            this.treatTimeSpreadStripAsLegs.Location = new System.Drawing.Point(111, 24);
            this.treatTimeSpreadStripAsLegs.Name = "treatTimeSpreadStripAsLegs";
            this.treatTimeSpreadStripAsLegs.Properties.Caption = "Treat timespread strip as legs";
            this.treatTimeSpreadStripAsLegs.Size = new System.Drawing.Size(166, 19);
            this.treatTimeSpreadStripAsLegs.TabIndex = 0;
            // 
            // useCommonPricing
            // 
            this.useCommonPricing.Location = new System.Drawing.Point(10, 24);
            this.useCommonPricing.Name = "useCommonPricing";
            this.useCommonPricing.Properties.Caption = "Common pricing";
            this.useCommonPricing.Size = new System.Drawing.Size(130, 19);
            this.useCommonPricing.TabIndex = 0;
            // 
            // leg2PositionFactor
            // 
            this.leg2PositionFactor.Location = new System.Drawing.Point(379, 73);
            this.leg2PositionFactor.Name = "leg2PositionFactor";
            this.leg2PositionFactor.Size = new System.Drawing.Size(72, 20);
            this.leg2PositionFactor.TabIndex = 7;
            this.leg2PositionFactor.EditValueChanged += new System.EventHandler(this.txtLeg2Factor_EditValueChanged);
            // 
            // leg1PositionFactor
            // 
            this.leg1PositionFactor.Location = new System.Drawing.Point(379, 49);
            this.leg1PositionFactor.Name = "leg1PositionFactor";
            this.leg1PositionFactor.Size = new System.Drawing.Size(72, 20);
            this.leg1PositionFactor.TabIndex = 3;
            this.leg1PositionFactor.EditValueChanged += new System.EventHandler(this.txtLeg1Factor_EditValueChanged);
            // 
            // legt1FactorsTitle
            // 
            this.legt1FactorsTitle.Location = new System.Drawing.Point(343, 76);
            this.legt1FactorsTitle.Name = "legt1FactorsTitle";
            this.legt1FactorsTitle.Size = new System.Drawing.Size(30, 13);
            this.legt1FactorsTitle.TabIndex = 22;
            this.legt1FactorsTitle.Text = "Leg 2:";
            // 
            // leg2ProductSelector
            // 
            this.leg2ProductSelector.Location = new System.Drawing.Point(102, 75);
            this.leg2ProductSelector.Name = "leg2ProductSelector";
            this.leg2ProductSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.leg2ProductSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Product")});
            this.leg2ProductSelector.Properties.NullText = "";
            this.leg2ProductSelector.Size = new System.Drawing.Size(225, 20);
            this.leg2ProductSelector.TabIndex = 5;
            this.leg2ProductSelector.EditValueChanged += new System.EventHandler(this.leProductLeg2_EditValueChanged);
            // 
            // leg2ProductTitle
            // 
            this.leg2ProductTitle.Location = new System.Drawing.Point(26, 78);
            this.leg2ProductTitle.Name = "leg2ProductTitle";
            this.leg2ProductTitle.Size = new System.Drawing.Size(70, 13);
            this.leg2ProductTitle.TabIndex = 20;
            this.leg2ProductTitle.Text = "Leg 2 Product:";
            // 
            // leg1FactorsTitle
            // 
            this.leg1FactorsTitle.Location = new System.Drawing.Point(343, 52);
            this.leg1FactorsTitle.Name = "leg1FactorsTitle";
            this.leg1FactorsTitle.Size = new System.Drawing.Size(30, 13);
            this.leg1FactorsTitle.TabIndex = 18;
            this.leg1FactorsTitle.Text = "Leg 1:";
            // 
            // leg1ProductSelector
            // 
            this.leg1ProductSelector.Location = new System.Drawing.Point(102, 49);
            this.leg1ProductSelector.Name = "leg1ProductSelector";
            this.leg1ProductSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.leg1ProductSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Product")});
            this.leg1ProductSelector.Properties.NullText = "";
            this.leg1ProductSelector.Size = new System.Drawing.Size(226, 20);
            this.leg1ProductSelector.TabIndex = 2;
            this.leg1ProductSelector.EditValueChanged += new System.EventHandler(this.leProductLeg1_EditValueChanged);
            // 
            // leg1ProductTitle
            // 
            this.leg1ProductTitle.Location = new System.Drawing.Point(26, 52);
            this.leg1ProductTitle.Name = "leg1ProductTitle";
            this.leg1ProductTitle.Size = new System.Drawing.Size(70, 13);
            this.leg1ProductTitle.TabIndex = 8;
            this.leg1ProductTitle.Text = "Leg 1 Product:";
            // 
            // commonProductPropertiesSection
            // 
            this.commonProductPropertiesSection.Controls.Add(this.contractSizeMultiplier);
            this.commonProductPropertiesSection.Controls.Add(this.currency2Selector);
            this.commonProductPropertiesSection.Controls.Add(this.currency2Title);
            this.commonProductPropertiesSection.Controls.Add(this.currency1Selector);
            this.commonProductPropertiesSection.Controls.Add(this.currency1Title);
            this.commonProductPropertiesSection.Controls.Add(this.holidayCalendarTitle);
            this.commonProductPropertiesSection.Controls.Add(this.holidaysCalendar);
            this.commonProductPropertiesSection.Controls.Add(this.unitsSelector);
            this.commonProductPropertiesSection.Controls.Add(this.dailyDiffMonthShift);
            this.commonProductPropertiesSection.Controls.Add(this.dailyDiffMonthShiftTitle);
            this.commonProductPropertiesSection.Controls.Add(this.monthlyOfficialProductTitle);
            this.commonProductPropertiesSection.Controls.Add(this.monthlyOfficialProductSelector);
            this.commonProductPropertiesSection.Controls.Add(this.tasOfficialProductTitle);
            this.commonProductPropertiesSection.Controls.Add(this.tasOfficialProductSelector);
            this.commonProductPropertiesSection.Controls.Add(this.priceConversionFactor);
            this.commonProductPropertiesSection.Controls.Add(this.priceConversionFactorTitle);
            this.commonProductPropertiesSection.Controls.Add(this.underlyingFuturesSelector);
            this.commonProductPropertiesSection.Controls.Add(this.exchangeTitle);
            this.commonProductPropertiesSection.Controls.Add(this.exchangeSelector);
            this.commonProductPropertiesSection.Controls.Add(this.definitionLinkTitle);
            this.commonProductPropertiesSection.Controls.Add(this.underlyingFuturesTitle);
            this.commonProductPropertiesSection.Controls.Add(this.productTypeSelector);
            this.commonProductPropertiesSection.Controls.Add(this.exchangeContractCode);
            this.commonProductPropertiesSection.Controls.Add(this.positionFactor);
            this.commonProductPropertiesSection.Controls.Add(this.pnlFactor);
            this.commonProductPropertiesSection.Controls.Add(this.contractSize);
            this.commonProductPropertiesSection.Controls.Add(this.productNameTitle);
            this.commonProductPropertiesSection.Controls.Add(this.productName);
            this.commonProductPropertiesSection.Controls.Add(this.productTypeTitle);
            this.commonProductPropertiesSection.Controls.Add(this.expiryCalendarTitle);
            this.commonProductPropertiesSection.Controls.Add(this.officialProductTitle);
            this.commonProductPropertiesSection.Controls.Add(this.officialProductSelector);
            this.commonProductPropertiesSection.Controls.Add(this.productGroupTitle);
            this.commonProductPropertiesSection.Controls.Add(this.productCategorySelector);
            this.commonProductPropertiesSection.Controls.Add(this.exchContractCodeTitle);
            this.commonProductPropertiesSection.Controls.Add(this.positionConversionFactorTitle);
            this.commonProductPropertiesSection.Controls.Add(this.pnlConversionFactorTitle);
            this.commonProductPropertiesSection.Controls.Add(this.expiryCalendar);
            this.commonProductPropertiesSection.Controls.Add(this.contractSizeTitle);
            this.commonProductPropertiesSection.Controls.Add(this.definitionLink);
            this.commonProductPropertiesSection.Location = new System.Drawing.Point(5, 5);
            this.commonProductPropertiesSection.Name = "commonProductPropertiesSection";
            this.commonProductPropertiesSection.Size = new System.Drawing.Size(559, 262);
            this.commonProductPropertiesSection.TabIndex = 0;
            this.commonProductPropertiesSection.Text = "Product Properties";
            // 
            // contractSizeMultiplier
            // 
            this.contractSizeMultiplier.Location = new System.Drawing.Point(161, 102);
            this.contractSizeMultiplier.Name = "contractSizeMultiplier";
            this.contractSizeMultiplier.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.contractSizeMultiplier.Size = new System.Drawing.Size(60, 20);
            this.contractSizeMultiplier.TabIndex = 7;
            // 
            // currency2Selector
            // 
            this.currency2Selector.Location = new System.Drawing.Point(242, 180);
            this.currency2Selector.Name = "currency2Selector";
            this.currency2Selector.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.currency2Selector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.currency2Selector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.currency2Selector.Properties.NullText = "Non";
            this.currency2Selector.Size = new System.Drawing.Size(60, 20);
            this.currency2Selector.TabIndex = 15;
            this.currency2Selector.EditValueChanged += new System.EventHandler(this.leCurrency_EditValueChanged);
            // 
            // currency2Title
            // 
            this.currency2Title.Location = new System.Drawing.Point(182, 183);
            this.currency2Title.Name = "currency2Title";
            this.currency2Title.Size = new System.Drawing.Size(54, 13);
            this.currency2Title.TabIndex = 43;
            this.currency2Title.Text = "Currency2:";
            // 
            // currency1Selector
            // 
            this.currency1Selector.Location = new System.Drawing.Point(101, 180);
            this.currency1Selector.Name = "currency1Selector";
            this.currency1Selector.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.currency1Selector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.currency1Selector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.currency1Selector.Properties.NullText = "Non";
            this.currency1Selector.Size = new System.Drawing.Size(60, 20);
            this.currency1Selector.TabIndex = 14;
            this.currency1Selector.EditValueChanged += new System.EventHandler(this.leCurrency_EditValueChanged);
            // 
            // currency1Title
            // 
            this.currency1Title.Location = new System.Drawing.Point(41, 183);
            this.currency1Title.Name = "currency1Title";
            this.currency1Title.Size = new System.Drawing.Size(54, 13);
            this.currency1Title.TabIndex = 41;
            this.currency1Title.Text = "Currency1:";
            // 
            // holidayCalendarTitle
            // 
            this.holidayCalendarTitle.Location = new System.Drawing.Point(288, 105);
            this.holidayCalendarTitle.Name = "holidayCalendarTitle";
            this.holidayCalendarTitle.Size = new System.Drawing.Size(88, 13);
            this.holidayCalendarTitle.TabIndex = 39;
            this.holidayCalendarTitle.Text = "Holidays calendar:";
            // 
            // holidaysCalendar
            // 
            this.holidaysCalendar.Location = new System.Drawing.Point(382, 102);
            this.holidaysCalendar.Name = "holidaysCalendar";
            this.holidaysCalendar.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.holidaysCalendar.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Calendar", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.holidaysCalendar.Properties.NullText = "[Select holidays calendar]";
            this.holidaysCalendar.Size = new System.Drawing.Size(165, 20);
            this.holidaysCalendar.TabIndex = 9;
            // 
            // unitsSelector
            // 
            this.unitsSelector.Location = new System.Drawing.Point(226, 102);
            this.unitsSelector.Name = "unitsSelector";
            this.unitsSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.unitsSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "PriceUnit", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.unitsSelector.Properties.NullText = "";
            this.unitsSelector.Size = new System.Drawing.Size(41, 20);
            this.unitsSelector.TabIndex = 8;
            this.unitsSelector.EditValueChanged += new System.EventHandler(this.UnitsChanged);
            // 
            // dailyDiffMonthShift
            // 
            this.dailyDiffMonthShift.Location = new System.Drawing.Point(442, 232);
            this.dailyDiffMonthShift.Name = "dailyDiffMonthShift";
            this.dailyDiffMonthShift.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dailyDiffMonthShift.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dailyDiffMonthShift.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Daily diff month shift")});
            this.dailyDiffMonthShift.Properties.NullText = "[Not set]";
            this.dailyDiffMonthShift.Properties.ValueMember = "Value";
            this.dailyDiffMonthShift.Size = new System.Drawing.Size(105, 20);
            this.dailyDiffMonthShift.TabIndex = 21;
            // 
            // dailyDiffMonthShiftTitle
            // 
            this.dailyDiffMonthShiftTitle.Location = new System.Drawing.Point(332, 235);
            this.dailyDiffMonthShiftTitle.Name = "dailyDiffMonthShiftTitle";
            this.dailyDiffMonthShiftTitle.Size = new System.Drawing.Size(103, 13);
            this.dailyDiffMonthShiftTitle.TabIndex = 19;
            this.dailyDiffMonthShiftTitle.Text = "Daily diff month shift:";
            // 
            // monthlyOfficialProductTitle
            // 
            this.monthlyOfficialProductTitle.Location = new System.Drawing.Point(273, 235);
            this.monthlyOfficialProductTitle.Name = "monthlyOfficialProductTitle";
            this.monthlyOfficialProductTitle.Size = new System.Drawing.Size(103, 13);
            this.monthlyOfficialProductTitle.TabIndex = 34;
            this.monthlyOfficialProductTitle.Text = "Monthly off. product:";
            // 
            // monthlyOfficialProductSelector
            // 
            this.monthlyOfficialProductSelector.Location = new System.Drawing.Point(382, 232);
            this.monthlyOfficialProductSelector.Name = "monthlyOfficialProductSelector";
            this.monthlyOfficialProductSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.monthlyOfficialProductSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Official Product", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.monthlyOfficialProductSelector.Properties.NullText = "[Select official product]";
            this.monthlyOfficialProductSelector.Size = new System.Drawing.Size(165, 20);
            this.monthlyOfficialProductSelector.TabIndex = 20;
            // 
            // tasOfficialProductTitle
            // 
            this.tasOfficialProductTitle.Location = new System.Drawing.Point(11, 235);
            this.tasOfficialProductTitle.Name = "tasOfficialProductTitle";
            this.tasOfficialProductTitle.Size = new System.Drawing.Size(84, 13);
            this.tasOfficialProductTitle.TabIndex = 32;
            this.tasOfficialProductTitle.Text = "TAS off. product:";
            // 
            // tasOfficialProductSelector
            // 
            this.tasOfficialProductSelector.Location = new System.Drawing.Point(102, 232);
            this.tasOfficialProductSelector.Name = "tasOfficialProductSelector";
            this.tasOfficialProductSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tasOfficialProductSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Official Product", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.tasOfficialProductSelector.Properties.NullText = "[Select official product]";
            this.tasOfficialProductSelector.Size = new System.Drawing.Size(165, 20);
            this.tasOfficialProductSelector.TabIndex = 18;
            // 
            // priceConversionFactor
            // 
            this.priceConversionFactor.Location = new System.Drawing.Point(475, 180);
            this.priceConversionFactor.Name = "priceConversionFactor";
            this.priceConversionFactor.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.priceConversionFactor.Size = new System.Drawing.Size(72, 20);
            this.priceConversionFactor.TabIndex = 16;
            // 
            // priceConversionFactorTitle
            // 
            this.priceConversionFactorTitle.Location = new System.Drawing.Point(353, 183);
            this.priceConversionFactorTitle.Name = "priceConversionFactorTitle";
            this.priceConversionFactorTitle.Size = new System.Drawing.Size(114, 13);
            this.priceConversionFactorTitle.TabIndex = 29;
            this.priceConversionFactorTitle.Text = "Price conversion factor:";
            // 
            // underlyingFuturesSelector
            // 
            this.underlyingFuturesSelector.Location = new System.Drawing.Point(382, 50);
            this.underlyingFuturesSelector.Name = "underlyingFuturesSelector";
            this.underlyingFuturesSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.underlyingFuturesSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Official Product", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.underlyingFuturesSelector.Properties.NullText = "[Select unerlying product]";
            this.underlyingFuturesSelector.Size = new System.Drawing.Size(165, 20);
            this.underlyingFuturesSelector.TabIndex = 3;
            // 
            // exchangeTitle
            // 
            this.exchangeTitle.Location = new System.Drawing.Point(324, 131);
            this.exchangeTitle.Name = "exchangeTitle";
            this.exchangeTitle.Size = new System.Drawing.Size(51, 13);
            this.exchangeTitle.TabIndex = 28;
            this.exchangeTitle.Text = "Exchange:";
            // 
            // exchangeSelector
            // 
            this.exchangeSelector.Location = new System.Drawing.Point(382, 128);
            this.exchangeSelector.Name = "exchangeSelector";
            this.exchangeSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.exchangeSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Exchange", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.exchangeSelector.Properties.NullText = "[Select exchange]";
            this.exchangeSelector.Size = new System.Drawing.Size(165, 20);
            this.exchangeSelector.TabIndex = 11;
            this.exchangeSelector.EditValueChanged += new System.EventHandler(this.leExchange_EditValueChanged);
            // 
            // definitionLinkTitle
            // 
            this.definitionLinkTitle.Location = new System.Drawing.Point(25, 209);
            this.definitionLinkTitle.Name = "definitionLinkTitle";
            this.definitionLinkTitle.Size = new System.Drawing.Size(70, 13);
            this.definitionLinkTitle.TabIndex = 26;
            this.definitionLinkTitle.Text = "Definition Link:";
            // 
            // underlyingFuturesTitle
            // 
            this.underlyingFuturesTitle.Location = new System.Drawing.Point(275, 53);
            this.underlyingFuturesTitle.Name = "underlyingFuturesTitle";
            this.underlyingFuturesTitle.Size = new System.Drawing.Size(100, 13);
            this.underlyingFuturesTitle.TabIndex = 24;
            this.underlyingFuturesTitle.Text = "Underlying products:";
            // 
            // productTypeSelector
            // 
            this.productTypeSelector.Location = new System.Drawing.Point(382, 24);
            this.productTypeSelector.Name = "productTypeSelector";
            this.productTypeSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.productTypeSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.productTypeSelector.Properties.DisplayMember = "Name";
            this.productTypeSelector.Properties.DropDownRows = 10;
            this.productTypeSelector.Properties.NullText = "[Select product type]";
            this.productTypeSelector.Properties.ValueMember = "Type";
            this.productTypeSelector.Size = new System.Drawing.Size(165, 20);
            this.productTypeSelector.TabIndex = 1;
            this.productTypeSelector.EditValueChanged += new System.EventHandler(this.ProductTypeChanged);
            // 
            // exchangeContractCode
            // 
            this.exchangeContractCode.Location = new System.Drawing.Point(475, 154);
            this.exchangeContractCode.Name = "exchangeContractCode";
            this.exchangeContractCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.exchangeContractCode.Size = new System.Drawing.Size(72, 20);
            this.exchangeContractCode.TabIndex = 13;
            // 
            // positionFactor
            // 
            this.positionFactor.Location = new System.Drawing.Point(162, 154);
            this.positionFactor.Name = "positionFactor";
            this.positionFactor.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.positionFactor.Properties.DisplayFormat.FormatString = "f2";
            this.positionFactor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.positionFactor.Properties.EditFormat.FormatString = "f2";
            this.positionFactor.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.positionFactor.Size = new System.Drawing.Size(105, 20);
            this.positionFactor.TabIndex = 12;
            this.positionFactor.EditValueChanged += new System.EventHandler(this.txtPositionFactor_EditValueChanged);
            // 
            // pnlFactor
            // 
            this.pnlFactor.Location = new System.Drawing.Point(162, 128);
            this.pnlFactor.Name = "pnlFactor";
            this.pnlFactor.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.pnlFactor.Size = new System.Drawing.Size(105, 20);
            this.pnlFactor.TabIndex = 10;
            this.pnlFactor.EditValueChanged += new System.EventHandler(this.txtPnlFactor_EditValueChanged);
            // 
            // contractSize
            // 
            this.contractSize.Location = new System.Drawing.Point(102, 102);
            this.contractSize.Name = "contractSize";
            this.contractSize.Size = new System.Drawing.Size(54, 20);
            this.contractSize.TabIndex = 6;
            this.contractSize.EditValueChanged += new System.EventHandler(this.txtContractSize_EditValueChanged);
            // 
            // exchContractCodeTitle
            // 
            this.exchContractCodeTitle.Location = new System.Drawing.Point(343, 157);
            this.exchContractCodeTitle.Name = "exchContractCodeTitle";
            this.exchContractCodeTitle.Size = new System.Drawing.Size(124, 13);
            this.exchContractCodeTitle.TabIndex = 6;
            this.exchContractCodeTitle.Text = "Exchange Contract Code:";
            // 
            // definitionLink
            // 
            this.definitionLink.Location = new System.Drawing.Point(102, 206);
            this.definitionLink.Name = "definitionLink";
            this.definitionLink.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.definitionLink.Properties.SingleClick = false;
            this.definitionLink.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.definitionLink.Size = new System.Drawing.Size(445, 20);
            this.definitionLink.TabIndex = 17;
            // 
            // useExpiryCalendar
            // 
            this.useExpiryCalendar.Location = new System.Drawing.Point(10, 24);
            this.useExpiryCalendar.Name = "useExpiryCalendar";
            this.useExpiryCalendar.Properties.Caption = "Use expiry calendar";
            this.useExpiryCalendar.Size = new System.Drawing.Size(130, 19);
            this.useExpiryCalendar.TabIndex = 0;
            // 
            // validTo
            // 
            this.validTo.EditValue = null;
            this.validTo.Location = new System.Drawing.Point(351, 24);
            this.validTo.Name = "validTo";
            this.validTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.validTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.validTo.Size = new System.Drawing.Size(196, 20);
            this.validTo.TabIndex = 2;
            // 
            // validToTitle
            // 
            this.validToTitle.Location = new System.Drawing.Point(306, 27);
            this.validToTitle.Name = "validToTitle";
            this.validToTitle.Size = new System.Drawing.Size(39, 13);
            this.validToTitle.TabIndex = 3;
            this.validToTitle.Text = "Valid to:";
            // 
            // validFrom
            // 
            this.validFrom.EditValue = null;
            this.validFrom.Location = new System.Drawing.Point(102, 24);
            this.validFrom.Name = "validFrom";
            this.validFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.validFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.validFrom.Size = new System.Drawing.Size(175, 20);
            this.validFrom.TabIndex = 1;
            // 
            // validFromTitle
            // 
            this.validFromTitle.Location = new System.Drawing.Point(44, 27);
            this.validFromTitle.Name = "validFromTitle";
            this.validFromTitle.Size = new System.Drawing.Size(51, 13);
            this.validFromTitle.TabIndex = 0;
            this.validFromTitle.Text = "Valid from:";
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.save.Location = new System.Drawing.Point(398, 895);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 1;
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.onSave);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.CausesValidation = false;
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(492, 895);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // productAliasesSection
            // 
            this.productAliasesSection.Controls.Add(this.productAliases);
            this.productAliasesSection.Location = new System.Drawing.Point(5, 594);
            this.productAliasesSection.Name = "productAliasesSection";
            this.productAliasesSection.Size = new System.Drawing.Size(277, 139);
            this.productAliasesSection.TabIndex = 5;
            this.productAliasesSection.Text = "Product Aliases";
            // 
            // productAliases
            // 
            this.productAliases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productAliases.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.First.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.productAliases.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.productAliases.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.productAliases.Location = new System.Drawing.Point(2, 20);
            this.productAliases.MainView = this.productAliasesDisplay;
            this.productAliases.Name = "productAliases";
            this.productAliases.Size = new System.Drawing.Size(273, 117);
            this.productAliases.TabIndex = 0;
            this.productAliases.UseEmbeddedNavigator = true;
            this.productAliases.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.productAliasesDisplay});
            // 
            // productAliasesDisplay
            // 
            this.productAliasesDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.alias});
            this.productAliasesDisplay.GridControl = this.productAliases;
            this.productAliasesDisplay.Name = "productAliasesDisplay";
            this.productAliasesDisplay.OptionsCustomization.AllowGroup = false;
            this.productAliasesDisplay.OptionsView.ShowGroupPanel = false;
            this.productAliasesDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gvAliases_InitNewRow);
            // 
            // alias
            // 
            this.alias.Caption = "Alias";
            this.alias.FieldName = "Name";
            this.alias.Name = "alias";
            this.alias.Visible = true;
            this.alias.VisibleIndex = 0;
            // 
            // rolloffSettings
            // 
            this.rolloffSettings.Controls.Add(this.mocActivationTimezoneSelector);
            this.rolloffSettings.Controls.Add(this.mocActivationTime);
            this.rolloffSettings.Controls.Add(this.mocActivationTimeTitle);
            this.rolloffSettings.Controls.Add(this.isCalendarDaySwap);
            this.rolloffSettings.Controls.Add(this.useRolloffSettings);
            this.rolloffSettings.Controls.Add(this.localRollOffTime);
            this.rolloffSettings.Controls.Add(this.localRollOffTimeTitle);
            this.rolloffSettings.Controls.Add(this.rolloffTime);
            this.rolloffSettings.Controls.Add(this.rollOffTimeTitle);
            this.rolloffSettings.Controls.Add(this.rolloffTimezoneSelector);
            this.rolloffSettings.Controls.Add(this.timeZoneTitle);
            this.rolloffSettings.Location = new System.Drawing.Point(288, 408);
            this.rolloffSettings.Name = "rolloffSettings";
            this.rolloffSettings.Size = new System.Drawing.Size(276, 131);
            this.rolloffSettings.TabIndex = 3;
            this.rolloffSettings.Text = "Roll Off Settings";
            // 
            // mocActivationTimezoneSelector
            // 
            this.mocActivationTimezoneSelector.Enabled = false;
            this.mocActivationTimezoneSelector.Location = new System.Drawing.Point(82, 75);
            this.mocActivationTimezoneSelector.Name = "mocActivationTimezoneSelector";
            this.mocActivationTimezoneSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.mocActivationTimezoneSelector.Size = new System.Drawing.Size(178, 20);
            this.mocActivationTimezoneSelector.TabIndex = 2;
            this.mocActivationTimezoneSelector.Visible = false;
            // 
            // mocActivationTime
            // 
            this.mocActivationTime.EditValue = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.mocActivationTime.Location = new System.Drawing.Point(115, 101);
            this.mocActivationTime.Name = "mocActivationTime";
            this.mocActivationTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.mocActivationTime.Size = new System.Drawing.Size(84, 20);
            this.mocActivationTime.TabIndex = 26;
            // 
            // mocActivationTimeTitle
            // 
            this.mocActivationTimeTitle.Location = new System.Drawing.Point(9, 104);
            this.mocActivationTimeTitle.Name = "mocActivationTimeTitle";
            this.mocActivationTimeTitle.Size = new System.Drawing.Size(100, 13);
            this.mocActivationTimeTitle.TabIndex = 25;
            this.mocActivationTimeTitle.Text = "MOC activation time:";
            // 
            // isCalendarDaySwap
            // 
            this.isCalendarDaySwap.Location = new System.Drawing.Point(139, 24);
            this.isCalendarDaySwap.Name = "isCalendarDaySwap";
            this.isCalendarDaySwap.Properties.Caption = "Calendar day swap";
            this.isCalendarDaySwap.Size = new System.Drawing.Size(132, 19);
            this.isCalendarDaySwap.TabIndex = 24;
            // 
            // useRolloffSettings
            // 
            this.useRolloffSettings.Location = new System.Drawing.Point(9, 24);
            this.useRolloffSettings.Name = "useRolloffSettings";
            this.useRolloffSettings.Properties.Caption = "Use Roll Off Settings";
            this.useRolloffSettings.Size = new System.Drawing.Size(146, 19);
            this.useRolloffSettings.TabIndex = 0;
            this.useRolloffSettings.CheckedChanged += new System.EventHandler(this.OnUseRollOffSettingsChanged);
            // 
            // localRollOffTime
            // 
            this.localRollOffTime.Location = new System.Drawing.Point(203, 52);
            this.localRollOffTime.Name = "localRollOffTime";
            this.localRollOffTime.Size = new System.Drawing.Size(28, 13);
            this.localRollOffTime.TabIndex = 4;
            this.localRollOffTime.Text = "00:00";
            this.localRollOffTime.Visible = false;
            // 
            // localRollOffTimeTitle
            // 
            this.localRollOffTimeTitle.Location = new System.Drawing.Point(169, 52);
            this.localRollOffTimeTitle.Name = "localRollOffTimeTitle";
            this.localRollOffTimeTitle.Size = new System.Drawing.Size(28, 13);
            this.localRollOffTimeTitle.TabIndex = 3;
            this.localRollOffTimeTitle.Text = "Local:";
            this.localRollOffTimeTitle.Visible = false;
            // 
            // rolloffTime
            // 
            this.rolloffTime.EditValue = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.rolloffTime.Enabled = false;
            this.rolloffTime.Location = new System.Drawing.Point(82, 49);
            this.rolloffTime.Name = "rolloffTime";
            this.rolloffTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.rolloffTime.Size = new System.Drawing.Size(73, 20);
            this.rolloffTime.TabIndex = 0;
            // 
            // rollOffTimeTitle
            // 
            this.rollOffTimeTitle.Location = new System.Drawing.Point(11, 52);
            this.rollOffTimeTitle.Name = "rollOffTimeTitle";
            this.rollOffTimeTitle.Size = new System.Drawing.Size(65, 13);
            this.rollOffTimeTitle.TabIndex = 23;
            this.rollOffTimeTitle.Text = "Roll Off Time:";
            // 
            // rolloffTimezoneSelector
            // 
            this.rolloffTimezoneSelector.Enabled = false;
            this.rolloffTimezoneSelector.Location = new System.Drawing.Point(82, 75);
            this.rolloffTimezoneSelector.Name = "rolloffTimezoneSelector";
            this.rolloffTimezoneSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.rolloffTimezoneSelector.Size = new System.Drawing.Size(178, 20);
            this.rolloffTimezoneSelector.TabIndex = 2;
            // 
            // timeZoneTitle
            // 
            this.timeZoneTitle.Location = new System.Drawing.Point(27, 78);
            this.timeZoneTitle.Name = "timeZoneTitle";
            this.timeZoneTitle.Size = new System.Drawing.Size(49, 13);
            this.timeZoneTitle.TabIndex = 1;
            this.timeZoneTitle.Text = "Timezone:";
            // 
            // futuresExpirationSettings
            // 
            this.futuresExpirationSettings.Controls.Add(this.tasTimezoneSelector);
            this.futuresExpirationSettings.Controls.Add(this.tasTimeZoneTitle);
            this.futuresExpirationSettings.Controls.Add(this.pricingEndTime);
            this.futuresExpirationSettings.Controls.Add(this.pricingEndTimeTitle);
            this.futuresExpirationSettings.Controls.Add(this.futuresExpiryTime);
            this.futuresExpirationSettings.Controls.Add(this.futuresExpiryTimeTitle);
            this.futuresExpirationSettings.Controls.Add(this.useFuturesExpirationSettings);
            this.futuresExpirationSettings.Controls.Add(this.tasActivationTime);
            this.futuresExpirationSettings.Controls.Add(this.tasActivationTimeTitle);
            this.futuresExpirationSettings.Location = new System.Drawing.Point(288, 408);
            this.futuresExpirationSettings.Name = "futuresExpirationSettings";
            this.futuresExpirationSettings.Size = new System.Drawing.Size(276, 131);
            this.futuresExpirationSettings.TabIndex = 29;
            this.futuresExpirationSettings.Text = "Futures Expiration Settings";
            // 
            // tasTimezoneSelector
            // 
            this.tasTimezoneSelector.Location = new System.Drawing.Point(119, 105);
            this.tasTimezoneSelector.Name = "tasTimezoneSelector";
            this.tasTimezoneSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tasTimezoneSelector.Size = new System.Drawing.Size(152, 20);
            this.tasTimezoneSelector.TabIndex = 8;
            // 
            // tasTimeZoneTitle
            // 
            this.tasTimeZoneTitle.Location = new System.Drawing.Point(119, 88);
            this.tasTimeZoneTitle.Name = "tasTimeZoneTitle";
            this.tasTimeZoneTitle.Size = new System.Drawing.Size(49, 13);
            this.tasTimeZoneTitle.TabIndex = 6;
            this.tasTimeZoneTitle.Text = "Timezone:";
            // 
            // pricingEndTime
            // 
            this.pricingEndTime.EditValue = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.pricingEndTime.Enabled = false;
            this.pricingEndTime.Location = new System.Drawing.Point(119, 63);
            this.pricingEndTime.Name = "pricingEndTime";
            this.pricingEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.pricingEndTime.Size = new System.Drawing.Size(84, 20);
            this.pricingEndTime.TabIndex = 4;
            // 
            // pricingEndTimeTitle
            // 
            this.pricingEndTimeTitle.Location = new System.Drawing.Point(119, 46);
            this.pricingEndTimeTitle.Name = "pricingEndTimeTitle";
            this.pricingEndTimeTitle.Size = new System.Drawing.Size(81, 13);
            this.pricingEndTimeTitle.TabIndex = 2;
            this.pricingEndTimeTitle.Text = "Pricing End Time:";
            // 
            // futuresExpiryTime
            // 
            this.futuresExpiryTime.EditValue = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.futuresExpiryTime.Enabled = false;
            this.futuresExpiryTime.Location = new System.Drawing.Point(23, 63);
            this.futuresExpiryTime.Name = "futuresExpiryTime";
            this.futuresExpiryTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.futuresExpiryTime.Size = new System.Drawing.Size(84, 20);
            this.futuresExpiryTime.TabIndex = 3;
            // 
            // futuresExpiryTimeTitle
            // 
            this.futuresExpiryTimeTitle.Location = new System.Drawing.Point(8, 46);
            this.futuresExpiryTimeTitle.Name = "futuresExpiryTimeTitle";
            this.futuresExpiryTimeTitle.Size = new System.Drawing.Size(99, 13);
            this.futuresExpiryTimeTitle.TabIndex = 1;
            this.futuresExpiryTimeTitle.Text = "Futures Expiry Time:";
            // 
            // useFuturesExpirationSettings
            // 
            this.useFuturesExpirationSettings.Location = new System.Drawing.Point(9, 24);
            this.useFuturesExpirationSettings.Name = "useFuturesExpirationSettings";
            this.useFuturesExpirationSettings.Properties.Caption = "Use Settings";
            this.useFuturesExpirationSettings.Size = new System.Drawing.Size(146, 19);
            this.useFuturesExpirationSettings.TabIndex = 0;
            this.useFuturesExpirationSettings.CheckedChanged += new System.EventHandler(this.chkUseFuturesExpirationSettings_CheckedChanged);
            // 
            // tasActivationTime
            // 
            this.tasActivationTime.EditValue = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.tasActivationTime.Location = new System.Drawing.Point(23, 105);
            this.tasActivationTime.Name = "tasActivationTime";
            this.tasActivationTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.tasActivationTime.Size = new System.Drawing.Size(84, 20);
            this.tasActivationTime.TabIndex = 7;
            // 
            // tasActivationTimeTitle
            // 
            this.tasActivationTimeTitle.Location = new System.Drawing.Point(11, 88);
            this.tasActivationTimeTitle.Name = "tasActivationTimeTitle";
            this.tasActivationTimeTitle.Size = new System.Drawing.Size(96, 13);
            this.tasActivationTimeTitle.TabIndex = 5;
            this.tasActivationTimeTitle.Text = "TAS activation time:";
            // 
            // productBrokerageByCompanySection
            // 
            this.productBrokerageByCompanySection.Controls.Add(this.companiesBrokerage);
            this.productBrokerageByCompanySection.Location = new System.Drawing.Point(288, 663);
            this.productBrokerageByCompanySection.Name = "productBrokerageByCompanySection";
            this.productBrokerageByCompanySection.Size = new System.Drawing.Size(276, 186);
            this.productBrokerageByCompanySection.TabIndex = 6;
            this.productBrokerageByCompanySection.Text = "Product Brokerage by Company";
            // 
            // companiesBrokerage
            // 
            this.companiesBrokerage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.First.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.companiesBrokerage.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.companiesBrokerage.Location = new System.Drawing.Point(2, 20);
            this.companiesBrokerage.MainView = this.companyBrokerageDisplay;
            this.companiesBrokerage.Name = "companiesBrokerage";
            this.companiesBrokerage.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.companySelector});
            this.companiesBrokerage.Size = new System.Drawing.Size(272, 164);
            this.companiesBrokerage.TabIndex = 0;
            this.companiesBrokerage.UseEmbeddedNavigator = true;
            this.companiesBrokerage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.companyBrokerageDisplay});
            // 
            // companyBrokerageDisplay
            // 
            this.companyBrokerageDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.companyName,
            this.brokerage});
            this.companyBrokerageDisplay.GridControl = this.companiesBrokerage;
            this.companyBrokerageDisplay.Name = "companyBrokerageDisplay";
            this.companyBrokerageDisplay.OptionsCustomization.AllowGroup = false;
            this.companyBrokerageDisplay.OptionsView.ShowGroupPanel = false;
            this.companyBrokerageDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gridView1_InitNewRow);
            // 
            // companyName
            // 
            this.companyName.Caption = "Company";
            this.companyName.ColumnEdit = this.companySelector;
            this.companyName.FieldName = "CompanyId";
            this.companyName.Name = "companyName";
            this.companyName.Visible = true;
            this.companyName.VisibleIndex = 0;
            // 
            // companySelector
            // 
            this.companySelector.AutoHeight = false;
            this.companySelector.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.companySelector.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Company")});
            this.companySelector.DropDownRows = 12;
            this.companySelector.Name = "companySelector";
            this.companySelector.ValueMember = "CompanyId";
            // 
            // brokerage
            // 
            this.brokerage.Caption = "Brokerage";
            this.brokerage.DisplayFormat.FormatString = "F2";
            this.brokerage.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.brokerage.FieldName = "Brokerage";
            this.brokerage.Name = "brokerage";
            this.brokerage.Visible = true;
            this.brokerage.VisibleIndex = 1;
            // 
            // futuresExpirationDate
            // 
            this.futuresExpirationDate.Controls.Add(this.expireTimeTitle);
            this.futuresExpirationDate.Controls.Add(this.numberOfDaysSection);
            this.futuresExpirationDate.Controls.Add(this.expirationMonthSection);
            this.futuresExpirationDate.Controls.Add(this.isRollingBackward);
            this.futuresExpirationDate.Controls.Add(this.isRollingForward);
            this.futuresExpirationDate.Controls.Add(this.rollingMethodTitle);
            this.futuresExpirationDate.Controls.Add(this.expTypeNumOfDaysSelector);
            this.futuresExpirationDate.Controls.Add(this.expiryTypeGivenDateSelector);
            this.futuresExpirationDate.Controls.Add(this.expiryTypeCalendarSelector);
            this.futuresExpirationDate.Controls.Add(this.expiryTypeTitle);
            this.futuresExpirationDate.Controls.Add(this.givenDateSection);
            this.futuresExpirationDate.Location = new System.Drawing.Point(525, 114);
            this.futuresExpirationDate.Name = "futuresExpirationDate";
            this.futuresExpirationDate.Size = new System.Drawing.Size(361, 143);
            this.futuresExpirationDate.TabIndex = 12;
            this.futuresExpirationDate.Text = "Futures Expiration Date Settings";
            this.futuresExpirationDate.Visible = false;
            // 
            // expireTimeTitle
            // 
            this.expireTimeTitle.Location = new System.Drawing.Point(209, 89);
            this.expireTimeTitle.Name = "expireTimeTitle";
            this.expireTimeTitle.Size = new System.Drawing.Size(59, 13);
            this.expireTimeTitle.TabIndex = 25;
            this.expireTimeTitle.Text = "Expire Time:";
            // 
            // numberOfDaysSection
            // 
            this.numberOfDaysSection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.numberOfDaysSection.Controls.Add(this.numberOfDaysTitle);
            this.numberOfDaysSection.Controls.Add(this.numberOfDaysSelector);
            this.numberOfDaysSection.Location = new System.Drawing.Point(3, 110);
            this.numberOfDaysSection.Name = "numberOfDaysSection";
            this.numberOfDaysSection.Size = new System.Drawing.Size(200, 26);
            this.numberOfDaysSection.TabIndex = 22;
            // 
            // numberOfDaysTitle
            // 
            this.numberOfDaysTitle.Location = new System.Drawing.Point(9, 5);
            this.numberOfDaysTitle.Name = "numberOfDaysTitle";
            this.numberOfDaysTitle.Size = new System.Drawing.Size(81, 13);
            this.numberOfDaysTitle.TabIndex = 19;
            this.numberOfDaysTitle.Text = "Number of Days:";
            // 
            // numberOfDaysSelector
            // 
            this.numberOfDaysSelector.EditValue = "";
            this.numberOfDaysSelector.Location = new System.Drawing.Point(96, 2);
            this.numberOfDaysSelector.Name = "numberOfDaysSelector";
            this.numberOfDaysSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numberOfDaysSelector.Properties.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31"});
            this.numberOfDaysSelector.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.numberOfDaysSelector.Size = new System.Drawing.Size(54, 20);
            this.numberOfDaysSelector.TabIndex = 20;
            // 
            // expirationMonthSection
            // 
            this.expirationMonthSection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.expirationMonthSection.Controls.Add(this.expirationMonthTitle);
            this.expirationMonthSection.Controls.Add(this.expirationMonthSelector);
            this.expirationMonthSection.Location = new System.Drawing.Point(3, 83);
            this.expirationMonthSection.Name = "expirationMonthSection";
            this.expirationMonthSection.Size = new System.Drawing.Size(200, 26);
            this.expirationMonthSection.TabIndex = 21;
            // 
            // expirationMonthTitle
            // 
            this.expirationMonthTitle.Location = new System.Drawing.Point(5, 6);
            this.expirationMonthTitle.Name = "expirationMonthTitle";
            this.expirationMonthTitle.Size = new System.Drawing.Size(85, 13);
            this.expirationMonthTitle.TabIndex = 19;
            this.expirationMonthTitle.Text = "Expiration Month:";
            // 
            // expirationMonthSelector
            // 
            this.expirationMonthSelector.Location = new System.Drawing.Point(96, 3);
            this.expirationMonthSelector.Name = "expirationMonthSelector";
            this.expirationMonthSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.expirationMonthSelector.Properties.Items.AddRange(new object[] {
            "-2",
            "-1",
            "0",
            "+1",
            "+2"});
            this.expirationMonthSelector.Properties.NullText = "-1";
            this.expirationMonthSelector.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.expirationMonthSelector.Size = new System.Drawing.Size(54, 20);
            this.expirationMonthSelector.TabIndex = 20;
            // 
            // isRollingBackward
            // 
            this.isRollingBackward.Location = new System.Drawing.Point(170, 60);
            this.isRollingBackward.Name = "isRollingBackward";
            this.isRollingBackward.Properties.Caption = "Backward";
            this.isRollingBackward.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isRollingBackward.Properties.RadioGroupIndex = 20;
            this.isRollingBackward.Size = new System.Drawing.Size(75, 19);
            this.isRollingBackward.TabIndex = 18;
            this.isRollingBackward.TabStop = false;
            this.isRollingBackward.CheckedChanged += new System.EventHandler(this.rbRollingMethod_CheckedChanged);
            // 
            // isRollingForward
            // 
            this.isRollingForward.EditValue = true;
            this.isRollingForward.Location = new System.Drawing.Point(99, 60);
            this.isRollingForward.Name = "isRollingForward";
            this.isRollingForward.Properties.Caption = "Forward";
            this.isRollingForward.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isRollingForward.Properties.RadioGroupIndex = 20;
            this.isRollingForward.Size = new System.Drawing.Size(65, 19);
            this.isRollingForward.TabIndex = 17;
            this.isRollingForward.CheckedChanged += new System.EventHandler(this.rbRollingMethod_CheckedChanged);
            // 
            // rollingMethodTitle
            // 
            this.rollingMethodTitle.Location = new System.Drawing.Point(19, 63);
            this.rollingMethodTitle.Name = "rollingMethodTitle";
            this.rollingMethodTitle.Size = new System.Drawing.Size(74, 13);
            this.rollingMethodTitle.TabIndex = 16;
            this.rollingMethodTitle.Text = "Rolling Method:";
            // 
            // expTypeNumOfDaysSelector
            // 
            this.expTypeNumOfDaysSelector.Location = new System.Drawing.Point(251, 31);
            this.expTypeNumOfDaysSelector.Name = "expTypeNumOfDaysSelector";
            this.expTypeNumOfDaysSelector.Properties.Caption = "Number of Days";
            this.expTypeNumOfDaysSelector.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.expTypeNumOfDaysSelector.Properties.RadioGroupIndex = 10;
            this.expTypeNumOfDaysSelector.Size = new System.Drawing.Size(104, 19);
            this.expTypeNumOfDaysSelector.TabIndex = 15;
            this.expTypeNumOfDaysSelector.TabStop = false;
            this.expTypeNumOfDaysSelector.CheckedChanged += new System.EventHandler(this.rbExpirationType_CheckedChanged);
            // 
            // expiryTypeGivenDateSelector
            // 
            this.expiryTypeGivenDateSelector.Location = new System.Drawing.Point(170, 31);
            this.expiryTypeGivenDateSelector.Name = "expiryTypeGivenDateSelector";
            this.expiryTypeGivenDateSelector.Properties.Caption = "Given Date";
            this.expiryTypeGivenDateSelector.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.expiryTypeGivenDateSelector.Properties.RadioGroupIndex = 10;
            this.expiryTypeGivenDateSelector.Size = new System.Drawing.Size(75, 19);
            this.expiryTypeGivenDateSelector.TabIndex = 14;
            this.expiryTypeGivenDateSelector.TabStop = false;
            this.expiryTypeGivenDateSelector.CheckedChanged += new System.EventHandler(this.rbExpirationType_CheckedChanged);
            // 
            // expiryTypeCalendarSelector
            // 
            this.expiryTypeCalendarSelector.EditValue = true;
            this.expiryTypeCalendarSelector.Location = new System.Drawing.Point(99, 31);
            this.expiryTypeCalendarSelector.Name = "expiryTypeCalendarSelector";
            this.expiryTypeCalendarSelector.Properties.Caption = "Calendar";
            this.expiryTypeCalendarSelector.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.expiryTypeCalendarSelector.Properties.RadioGroupIndex = 10;
            this.expiryTypeCalendarSelector.Size = new System.Drawing.Size(65, 19);
            this.expiryTypeCalendarSelector.TabIndex = 13;
            this.expiryTypeCalendarSelector.CheckedChanged += new System.EventHandler(this.rbExpirationType_CheckedChanged);
            // 
            // expiryTypeTitle
            // 
            this.expiryTypeTitle.Location = new System.Drawing.Point(14, 34);
            this.expiryTypeTitle.Name = "expiryTypeTitle";
            this.expiryTypeTitle.Size = new System.Drawing.Size(79, 13);
            this.expiryTypeTitle.TabIndex = 12;
            this.expiryTypeTitle.Text = "Expiration Type:";
            // 
            // givenDateSection
            // 
            this.givenDateSection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.givenDateSection.Controls.Add(this.givenDate);
            this.givenDateSection.Controls.Add(this.givenDateTitle);
            this.givenDateSection.Location = new System.Drawing.Point(3, 83);
            this.givenDateSection.Name = "givenDateSection";
            this.givenDateSection.Size = new System.Drawing.Size(200, 26);
            this.givenDateSection.TabIndex = 22;
            // 
            // givenDate
            // 
            this.givenDate.EditValue = null;
            this.givenDate.Location = new System.Drawing.Point(96, 3);
            this.givenDate.Name = "givenDate";
            this.givenDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.givenDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.givenDate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.givenDate.Size = new System.Drawing.Size(100, 20);
            this.givenDate.TabIndex = 20;
            // 
            // givenDateTitle
            // 
            this.givenDateTitle.Location = new System.Drawing.Point(33, 6);
            this.givenDateTitle.Name = "givenDateTitle";
            this.givenDateTitle.Size = new System.Drawing.Size(57, 13);
            this.givenDateTitle.TabIndex = 19;
            this.givenDateTitle.Text = "Given Date:";
            // 
            // productTabs
            // 
            this.productTabs.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.productTabs.Appearance.Options.UseBackColor = true;
            this.productTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.productTabs.Location = new System.Drawing.Point(0, 0);
            this.productTabs.MultiLine = DevExpress.Utils.DefaultBoolean.False;
            this.productTabs.Name = "productTabs";
            this.productTabs.PaintStyleName = "Skin";
            this.productTabs.SelectedTabPage = this.basicSettings;
            this.productTabs.Size = new System.Drawing.Size(575, 884);
            this.productTabs.TabIndex = 0;
            this.productTabs.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.basicSettings,
            this.advancedSettings});
            // 
            // basicSettings
            // 
            this.basicSettings.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.basicSettings.Appearance.PageClient.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.basicSettings.Appearance.PageClient.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.basicSettings.Appearance.PageClient.Options.UseBackColor = true;
            this.basicSettings.Appearance.PageClient.Options.UseBorderColor = true;
            this.basicSettings.Controls.Add(this.basicSettingsSection);
            this.basicSettings.Controls.Add(this.futuresExpirationDate);
            this.basicSettings.Name = "basicSettings";
            this.basicSettings.Size = new System.Drawing.Size(569, 856);
            this.basicSettings.Text = "Basic";
            // 
            // basicSettingsSection
            // 
            this.basicSettingsSection.Controls.Add(this.productValidityRangeSection);
            this.basicSettingsSection.Controls.Add(this.testTradeImpactSection);
            this.basicSettingsSection.Controls.Add(this.productOptionsSection);
            this.basicSettingsSection.Controls.Add(this.commonProductPropertiesSection);
            this.basicSettingsSection.Controls.Add(this.complexProductSection);
            this.basicSettingsSection.Controls.Add(this.futuresExpirationSettings);
            this.basicSettingsSection.Controls.Add(this.rolloffSettings);
            this.basicSettingsSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basicSettingsSection.Location = new System.Drawing.Point(0, 0);
            this.basicSettingsSection.Name = "basicSettingsSection";
            this.basicSettingsSection.Size = new System.Drawing.Size(569, 856);
            this.basicSettingsSection.TabIndex = 2;
            // 
            // productValidityRangeSection
            // 
            this.productValidityRangeSection.Controls.Add(this.validFromTitle);
            this.productValidityRangeSection.Controls.Add(this.validFrom);
            this.productValidityRangeSection.Controls.Add(this.validToTitle);
            this.productValidityRangeSection.Controls.Add(this.validTo);
            this.productValidityRangeSection.Location = new System.Drawing.Point(5, 792);
            this.productValidityRangeSection.Name = "productValidityRangeSection";
            this.productValidityRangeSection.Size = new System.Drawing.Size(559, 56);
            this.productValidityRangeSection.TabIndex = 5;
            this.productValidityRangeSection.Text = "Validity";
            // 
            // testTradeImpactSection
            // 
            this.testTradeImpactSection.Controls.Add(this.testTradeStartDateTitle);
            this.testTradeImpactSection.Controls.Add(this.testTradePnL);
            this.testTradeImpactSection.Controls.Add(this.testTradePnLTitle);
            this.testTradeImpactSection.Controls.Add(this.testTradePrice);
            this.testTradeImpactSection.Controls.Add(this.testTradeImpact);
            this.testTradeImpactSection.Controls.Add(this.testTradeStripSelector);
            this.testTradeImpactSection.Controls.Add(this.testTradePriceTitle);
            this.testTradeImpactSection.Controls.Add(this.testTradeLivePrice);
            this.testTradeImpactSection.Controls.Add(this.testTradeVolume);
            this.testTradeImpactSection.Controls.Add(this.testTradeLivePriceTitle);
            this.testTradeImpactSection.Controls.Add(this.testTradeVolumeTitle);
            this.testTradeImpactSection.Controls.Add(this.testTradeStripTitle);
            this.testTradeImpactSection.Controls.Add(this.testTradeStartDate);
            this.testTradeImpactSection.Location = new System.Drawing.Point(5, 545);
            this.testTradeImpactSection.Name = "testTradeImpactSection";
            this.testTradeImpactSection.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.testTradeImpactSection.Size = new System.Drawing.Size(559, 233);
            this.testTradeImpactSection.TabIndex = 4;
            this.testTradeImpactSection.Text = "Trade Impact";
            // 
            // testTradeStartDateTitle
            // 
            this.testTradeStartDateTitle.Location = new System.Drawing.Point(90, 27);
            this.testTradeStartDateTitle.Name = "testTradeStartDateTitle";
            this.testTradeStartDateTitle.Size = new System.Drawing.Size(54, 13);
            this.testTradeStartDateTitle.TabIndex = 2;
            this.testTradeStartDateTitle.Text = "Start Date:";
            this.testTradeStartDateTitle.Visible = false;
            // 
            // testTradePnL
            // 
            this.testTradePnL.Location = new System.Drawing.Point(490, 24);
            this.testTradePnL.Name = "testTradePnL";
            this.testTradePnL.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.testTradePnL.Properties.DisplayFormat.FormatString = "f4";
            this.testTradePnL.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.testTradePnL.Properties.EditFormat.FormatString = "f4";
            this.testTradePnL.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.testTradePnL.Properties.ReadOnly = true;
            this.testTradePnL.Size = new System.Drawing.Size(64, 20);
            this.testTradePnL.TabIndex = 10;
            // 
            // testTradePnLTitle
            // 
            this.testTradePnLTitle.Location = new System.Drawing.Point(463, 27);
            this.testTradePnLTitle.Name = "testTradePnLTitle";
            this.testTradePnLTitle.Size = new System.Drawing.Size(21, 13);
            this.testTradePnLTitle.TabIndex = 9;
            this.testTradePnLTitle.Text = "PnL:";
            // 
            // testTradePrice
            // 
            this.testTradePrice.Location = new System.Drawing.Point(289, 24);
            this.testTradePrice.Name = "testTradePrice";
            this.testTradePrice.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.testTradePrice.Properties.Mask.EditMask = "f4";
            this.testTradePrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.testTradePrice.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.testTradePrice.Size = new System.Drawing.Size(54, 20);
            this.testTradePrice.TabIndex = 6;
            this.testTradePrice.EditValueChanged += new System.EventHandler(this.txtPrice_EditValueChanged);
            // 
            // testTradeImpact
            // 
            this.testTradeImpact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTradeImpact.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[] {
            this.testTradeImpactProductGroupSort,
            this.testTradeImpactProductSort,
            this.testTradeImpactSourceSort,
            this.testTradeImpactCalcYearSort,
            this.testTradeImpactMonthSort,
            this.pgfPos,
            this.testTradeImpactCalcIdSort});
            pivotGridStyleFormatCondition1.Appearance.ForeColor = System.Drawing.Color.Red;
            pivotGridStyleFormatCondition1.Appearance.Options.UseForeColor = true;
            pivotGridStyleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Less;
            pivotGridStyleFormatCondition1.Field = this.pgfPos;
            pivotGridStyleFormatCondition1.FieldName = "pgfPos";
            pivotGridStyleFormatCondition1.Value1 = "0";
            this.testTradeImpact.FormatConditions.AddRange(new DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition[] {
            pivotGridStyleFormatCondition1});
            this.testTradeImpact.Location = new System.Drawing.Point(2, 50);
            this.testTradeImpact.Name = "testTradeImpact";
            this.testTradeImpact.OptionsChartDataSource.FieldValuesProvideMode = DevExpress.XtraPivotGrid.PivotChartFieldValuesProvideMode.DisplayText;
            this.testTradeImpact.OptionsView.ShowFilterHeaders = false;
            this.testTradeImpact.Size = new System.Drawing.Size(555, 181);
            this.testTradeImpact.TabIndex = 11;
            this.testTradeImpact.TabStop = false;
            // 
            // testTradeImpactProductGroupSort
            // 
            this.testTradeImpactProductGroupSort.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeImpactProductGroupSort.AreaIndex = 0;
            this.testTradeImpactProductGroupSort.Caption = "Product Group";
            this.testTradeImpactProductGroupSort.FieldName = "ProductCategory";
            this.testTradeImpactProductGroupSort.Name = "testTradeImpactProductGroupSort";
            this.testTradeImpactProductGroupSort.Width = 60;
            // 
            // testTradeImpactProductSort
            // 
            this.testTradeImpactProductSort.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeImpactProductSort.AreaIndex = 1;
            this.testTradeImpactProductSort.Caption = "Product";
            this.testTradeImpactProductSort.FieldName = "Product";
            this.testTradeImpactProductSort.Name = "testTradeImpactProductSort";
            this.testTradeImpactProductSort.Width = 60;
            // 
            // testTradeImpactSourceSort
            // 
            this.testTradeImpactSourceSort.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeImpactSourceSort.AreaIndex = 2;
            this.testTradeImpactSourceSort.Caption = "Source";
            this.testTradeImpactSourceSort.FieldName = "Source";
            this.testTradeImpactSourceSort.Name = "testTradeImpactSourceSort";
            this.testTradeImpactSourceSort.Width = 46;
            // 
            // testTradeImpactCalcYearSort
            // 
            this.testTradeImpactCalcYearSort.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
            this.testTradeImpactCalcYearSort.AreaIndex = 0;
            this.testTradeImpactCalcYearSort.Caption = "Year";
            this.testTradeImpactCalcYearSort.FieldName = "CalculationDate";
            this.testTradeImpactCalcYearSort.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateYear;
            this.testTradeImpactCalcYearSort.Name = "testTradeImpactCalcYearSort";
            this.testTradeImpactCalcYearSort.UnboundFieldName = "pivotGridField4";
            this.testTradeImpactCalcYearSort.Width = 49;
            // 
            // testTradeImpactMonthSort
            // 
            this.testTradeImpactMonthSort.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
            this.testTradeImpactMonthSort.AreaIndex = 1;
            this.testTradeImpactMonthSort.Caption = "Month";
            this.testTradeImpactMonthSort.FieldName = "CalculationDate";
            this.testTradeImpactMonthSort.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateMonth;
            this.testTradeImpactMonthSort.Name = "testTradeImpactMonthSort";
            this.testTradeImpactMonthSort.UnboundFieldName = "pivotGridField5";
            this.testTradeImpactMonthSort.Width = 60;
            // 
            // testTradeImpactCalcIdSort
            // 
            this.testTradeImpactCalcIdSort.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.testTradeImpactCalcIdSort.AreaIndex = 3;
            this.testTradeImpactCalcIdSort.FieldName = "DetailId";
            this.testTradeImpactCalcIdSort.Name = "testTradeImpactCalcIdSort";
            this.testTradeImpactCalcIdSort.Visible = false;
            // 
            // testTradeStripSelector
            // 
            this.testTradeStripSelector.Location = new System.Drawing.Point(172, 24);
            this.testTradeStripSelector.Name = "testTradeStripSelector";
            this.testTradeStripSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.testTradeStripSelector.Size = new System.Drawing.Size(78, 20);
            this.testTradeStripSelector.TabIndex = 4;
            this.testTradeStripSelector.SelectedIndexChanged += new System.EventHandler(this.cmbStrip_SelectedIndexChanged);
            // 
            // testTradePriceTitle
            // 
            this.testTradePriceTitle.Location = new System.Drawing.Point(256, 27);
            this.testTradePriceTitle.Name = "testTradePriceTitle";
            this.testTradePriceTitle.Size = new System.Drawing.Size(27, 13);
            this.testTradePriceTitle.TabIndex = 5;
            this.testTradePriceTitle.Text = "Price:";
            // 
            // testTradeLivePrice
            // 
            this.testTradeLivePrice.Location = new System.Drawing.Point(405, 24);
            this.testTradeLivePrice.Name = "testTradeLivePrice";
            this.testTradeLivePrice.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.testTradeLivePrice.Properties.DisplayFormat.FormatString = "f4";
            this.testTradeLivePrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.testTradeLivePrice.Properties.EditFormat.FormatString = "f4";
            this.testTradeLivePrice.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.testTradeLivePrice.Properties.ReadOnly = true;
            this.testTradeLivePrice.Size = new System.Drawing.Size(54, 20);
            this.testTradeLivePrice.TabIndex = 8;
            // 
            // testTradeVolume
            // 
            this.testTradeVolume.Location = new System.Drawing.Point(31, 24);
            this.testTradeVolume.Name = "testTradeVolume";
            this.testTradeVolume.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.testTradeVolume.Properties.Mask.EditMask = "f2";
            this.testTradeVolume.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.testTradeVolume.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.testTradeVolume.Size = new System.Drawing.Size(54, 20);
            this.testTradeVolume.TabIndex = 1;
            this.testTradeVolume.EditValueChanged += new System.EventHandler(this.txtVol_EditValueChanged);
            // 
            // testTradeLivePriceTitle
            // 
            this.testTradeLivePriceTitle.Location = new System.Drawing.Point(350, 27);
            this.testTradeLivePriceTitle.Name = "testTradeLivePriceTitle";
            this.testTradeLivePriceTitle.Size = new System.Drawing.Size(49, 13);
            this.testTradeLivePriceTitle.TabIndex = 7;
            this.testTradeLivePriceTitle.Text = "Live Price:";
            // 
            // testTradeVolumeTitle
            // 
            this.testTradeVolumeTitle.Location = new System.Drawing.Point(7, 27);
            this.testTradeVolumeTitle.Name = "testTradeVolumeTitle";
            this.testTradeVolumeTitle.Size = new System.Drawing.Size(18, 13);
            this.testTradeVolumeTitle.TabIndex = 0;
            this.testTradeVolumeTitle.Text = "Vol:";
            // 
            // testTradeStripTitle
            // 
            this.testTradeStripTitle.Location = new System.Drawing.Point(140, 27);
            this.testTradeStripTitle.Name = "testTradeStripTitle";
            this.testTradeStripTitle.Size = new System.Drawing.Size(26, 13);
            this.testTradeStripTitle.TabIndex = 3;
            this.testTradeStripTitle.Text = "Strip:";
            // 
            // testTradeStartDate
            // 
            this.testTradeStartDate.EditValue = null;
            this.testTradeStartDate.Location = new System.Drawing.Point(150, 24);
            this.testTradeStartDate.Name = "testTradeStartDate";
            this.testTradeStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.testTradeStartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.testTradeStartDate.Size = new System.Drawing.Size(100, 20);
            this.testTradeStartDate.TabIndex = 2;
            this.testTradeStartDate.Visible = false;
            this.testTradeStartDate.EditValueChanged += new System.EventHandler(this.deStartDate_EditValueChanged);
            // 
            // productOptionsSection
            // 
            this.productOptionsSection.Controls.Add(this.enableRiskDecomposition);
            this.productOptionsSection.Controls.Add(this.allowedForManualTrades);
            this.productOptionsSection.Controls.Add(this.tasTypeSelectors);
            this.productOptionsSection.Controls.Add(this.isInternalTransferProduct);
            this.productOptionsSection.Controls.Add(this.isPhysicallySettled);
            this.productOptionsSection.Controls.Add(this.useExpiryCalendar);
            this.productOptionsSection.Controls.Add(this.alsoIsTas);
            this.productOptionsSection.Location = new System.Drawing.Point(5, 408);
            this.productOptionsSection.Name = "productOptionsSection";
            this.productOptionsSection.Size = new System.Drawing.Size(277, 131);
            this.productOptionsSection.TabIndex = 2;
            this.productOptionsSection.Text = "Product Options";
            // 
            // enableRiskDecomposition
            // 
            this.enableRiskDecomposition.Location = new System.Drawing.Point(129, 49);
            this.enableRiskDecomposition.Name = "enableRiskDecomposition";
            this.enableRiskDecomposition.Properties.Caption = "Enable risk decomposition";
            this.enableRiskDecomposition.Size = new System.Drawing.Size(146, 19);
            this.enableRiskDecomposition.TabIndex = 3;
            // 
            // allowedForManualTrades
            // 
            this.allowedForManualTrades.Location = new System.Drawing.Point(129, 24);
            this.allowedForManualTrades.Name = "allowedForManualTrades";
            this.allowedForManualTrades.Properties.Caption = "Allowed for manual trades";
            this.allowedForManualTrades.Size = new System.Drawing.Size(146, 19);
            this.allowedForManualTrades.TabIndex = 1;
            // 
            // tasTypeSelectors
            // 
            this.tasTypeSelectors.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tasTypeSelectors.Controls.Add(this.isMoc);
            this.tasTypeSelectors.Controls.Add(this.isPlain);
            this.tasTypeSelectors.Controls.Add(this.isTas);
            this.tasTypeSelectors.Controls.Add(this.isMinuteMarker);
            this.tasTypeSelectors.Controls.Add(this.isMops);
            this.tasTypeSelectors.Location = new System.Drawing.Point(2, 93);
            this.tasTypeSelectors.Name = "tasTypeSelectors";
            this.tasTypeSelectors.Size = new System.Drawing.Size(265, 27);
            this.tasTypeSelectors.TabIndex = 4;
            // 
            // isMoc
            // 
            this.isMoc.Location = new System.Drawing.Point(216, 5);
            this.isMoc.Name = "isMoc";
            this.isMoc.Properties.Caption = "MOC";
            this.isMoc.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isMoc.Properties.RadioGroupIndex = 1;
            this.isMoc.Size = new System.Drawing.Size(46, 19);
            this.isMoc.TabIndex = 4;
            this.isMoc.TabStop = false;
            this.isMoc.CheckedChanged += new System.EventHandler(this.ChangeContractSubType);
            // 
            // isPlain
            // 
            this.isPlain.Location = new System.Drawing.Point(165, 5);
            this.isPlain.Name = "isPlain";
            this.isPlain.Properties.Caption = "Plain";
            this.isPlain.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isPlain.Properties.RadioGroupIndex = 1;
            this.isPlain.Size = new System.Drawing.Size(50, 19);
            this.isPlain.TabIndex = 3;
            this.isPlain.TabStop = false;
            this.isPlain.CheckedChanged += new System.EventHandler(this.ChangeContractSubType);
            // 
            // isTas
            // 
            this.isTas.Location = new System.Drawing.Point(8, 5);
            this.isTas.Name = "isTas";
            this.isTas.Properties.Caption = "TAS";
            this.isTas.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isTas.Properties.RadioGroupIndex = 1;
            this.isTas.Size = new System.Drawing.Size(44, 19);
            this.isTas.TabIndex = 0;
            this.isTas.TabStop = false;
            this.isTas.CheckedChanged += new System.EventHandler(this.ChangeContractSubType);
            // 
            // isMinuteMarker
            // 
            this.isMinuteMarker.Location = new System.Drawing.Point(118, 5);
            this.isMinuteMarker.Name = "isMinuteMarker";
            this.isMinuteMarker.Properties.Caption = "MM";
            this.isMinuteMarker.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isMinuteMarker.Properties.RadioGroupIndex = 1;
            this.isMinuteMarker.Size = new System.Drawing.Size(41, 19);
            this.isMinuteMarker.TabIndex = 2;
            this.isMinuteMarker.TabStop = false;
            this.isMinuteMarker.CheckedChanged += new System.EventHandler(this.ChangeContractSubType);
            // 
            // isMops
            // 
            this.isMops.Location = new System.Drawing.Point(58, 5);
            this.isMops.Name = "isMops";
            this.isMops.Properties.Caption = "MOPS";
            this.isMops.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.isMops.Properties.RadioGroupIndex = 1;
            this.isMops.Size = new System.Drawing.Size(55, 19);
            this.isMops.TabIndex = 1;
            this.isMops.TabStop = false;
            this.isMops.CheckedChanged += new System.EventHandler(this.ChangeContractSubType);
            // 
            // isInternalTransferProduct
            // 
            this.isInternalTransferProduct.Location = new System.Drawing.Point(10, 74);
            this.isInternalTransferProduct.Name = "isInternalTransferProduct";
            this.isInternalTransferProduct.Properties.Caption = "Internal Transfer Product";
            this.isInternalTransferProduct.Size = new System.Drawing.Size(146, 19);
            this.isInternalTransferProduct.TabIndex = 4;
            // 
            // isPhysicallySettled
            // 
            this.isPhysicallySettled.Location = new System.Drawing.Point(10, 49);
            this.isPhysicallySettled.Name = "isPhysicallySettled";
            this.isPhysicallySettled.Properties.Caption = "Physically Settled";
            this.isPhysicallySettled.Size = new System.Drawing.Size(112, 19);
            this.isPhysicallySettled.TabIndex = 2;
            // 
            // alsoIsTas
            // 
            this.alsoIsTas.Location = new System.Drawing.Point(10, 98);
            this.alsoIsTas.Name = "alsoIsTas";
            this.alsoIsTas.Properties.Caption = "TAS";
            this.alsoIsTas.Size = new System.Drawing.Size(44, 19);
            this.alsoIsTas.TabIndex = 5;
            // 
            // advancedSettings
            // 
            this.advancedSettings.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.advancedSettings.Appearance.PageClient.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.advancedSettings.Appearance.PageClient.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.advancedSettings.Appearance.PageClient.Options.UseBackColor = true;
            this.advancedSettings.Appearance.PageClient.Options.UseBorderColor = true;
            this.advancedSettings.Controls.Add(this.balmoMappingsSection);
            this.advancedSettings.Name = "advancedSettings";
            this.advancedSettings.Size = new System.Drawing.Size(569, 856);
            this.advancedSettings.Text = "Advanced";
            // 
            // balmoMappingsSection
            // 
            this.balmoMappingsSection.Controls.Add(this.iceBalmoMappingsSection);
            this.balmoMappingsSection.Controls.Add(this.categoryOverrideSection);
            this.balmoMappingsSection.Controls.Add(this.iceProductAliasesSection);
            this.balmoMappingsSection.Controls.Add(this.productAliasesSection);
            this.balmoMappingsSection.Controls.Add(this.abnMappingsSection);
            this.balmoMappingsSection.Controls.Add(this.balmoComplexProductsSelector);
            this.balmoMappingsSection.Controls.Add(this.balmoComplexProductTitle);
            this.balmoMappingsSection.Controls.Add(this.balmoCrudeSwapsSelector);
            this.balmoMappingsSection.Controls.Add(this.balmoCrudeSwapTitle);
            this.balmoMappingsSection.Controls.Add(this.balmoContractCodesSection);
            this.balmoMappingsSection.Controls.Add(this.productFeesSection);
            this.balmoMappingsSection.Controls.Add(this.gmiBalmoCodesSection);
            this.balmoMappingsSection.Controls.Add(this.productBrokerageByCompanySection);
            this.balmoMappingsSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.balmoMappingsSection.Location = new System.Drawing.Point(0, 0);
            this.balmoMappingsSection.Name = "balmoMappingsSection";
            this.balmoMappingsSection.Size = new System.Drawing.Size(569, 856);
            this.balmoMappingsSection.TabIndex = 12;
            // 
            // iceBalmoMappingsSection
            // 
            this.iceBalmoMappingsSection.Controls.Add(this.iceBalmoMappings);
            this.iceBalmoMappingsSection.Location = new System.Drawing.Point(5, 208);
            this.iceBalmoMappingsSection.Name = "iceBalmoMappingsSection";
            this.iceBalmoMappingsSection.Size = new System.Drawing.Size(559, 118);
            this.iceBalmoMappingsSection.TabIndex = 3;
            this.iceBalmoMappingsSection.Text = "ICE Balmo Mappings";
            // 
            // iceBalmoMappings
            // 
            this.iceBalmoMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.First.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.iceBalmoMappings.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.iceBalmoMappings.Location = new System.Drawing.Point(2, 20);
            this.iceBalmoMappings.MainView = this.iceBalmoMappingsDisplay;
            this.iceBalmoMappings.Name = "iceBalmoMappings";
            this.iceBalmoMappings.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.companyNameSelector,
            this.iceBalmoStartDayValue,
            this.iceBalmoCharValue,
            this.iceBalmoPrefixValue});
            this.iceBalmoMappings.Size = new System.Drawing.Size(555, 96);
            this.iceBalmoMappings.TabIndex = 0;
            this.iceBalmoMappings.UseEmbeddedNavigator = true;
            this.iceBalmoMappings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.iceBalmoMappingsDisplay});
            // 
            // iceBalmoMappingsDisplay
            // 
            this.iceBalmoMappingsDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.iceBalmoPrefix,
            this.iceBalmoStartChar,
            this.iceBalmoEndChar,
            this.iceBalmoStartDay});
            this.iceBalmoMappingsDisplay.GridControl = this.iceBalmoMappings;
            this.iceBalmoMappingsDisplay.Name = "iceBalmoMappingsDisplay";
            this.iceBalmoMappingsDisplay.OptionsCustomization.AllowGroup = false;
            this.iceBalmoMappingsDisplay.OptionsView.ShowGroupPanel = false;
            this.iceBalmoMappingsDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gvIceBalmoMappings_InitNewRow);
            // 
            // iceBalmoPrefix
            // 
            this.iceBalmoPrefix.Caption = "Prefix";
            this.iceBalmoPrefix.ColumnEdit = this.iceBalmoPrefixValue;
            this.iceBalmoPrefix.FieldName = "PrefixChar";
            this.iceBalmoPrefix.Name = "iceBalmoPrefix";
            this.iceBalmoPrefix.Visible = true;
            this.iceBalmoPrefix.VisibleIndex = 0;
            // 
            // iceBalmoPrefixValue
            // 
            this.iceBalmoPrefixValue.AutoHeight = false;
            this.iceBalmoPrefixValue.MaxLength = 10;
            this.iceBalmoPrefixValue.Name = "iceBalmoPrefixValue";
            // 
            // iceBalmoStartChar
            // 
            this.iceBalmoStartChar.Caption = "Start Char";
            this.iceBalmoStartChar.ColumnEdit = this.iceBalmoCharValue;
            this.iceBalmoStartChar.FieldName = "StartChar";
            this.iceBalmoStartChar.Name = "iceBalmoStartChar";
            this.iceBalmoStartChar.Visible = true;
            this.iceBalmoStartChar.VisibleIndex = 1;
            // 
            // iceBalmoCharValue
            // 
            this.iceBalmoCharValue.AutoHeight = false;
            this.iceBalmoCharValue.MaxLength = 1;
            this.iceBalmoCharValue.Name = "iceBalmoCharValue";
            // 
            // iceBalmoEndChar
            // 
            this.iceBalmoEndChar.Caption = "End Char";
            this.iceBalmoEndChar.ColumnEdit = this.iceBalmoCharValue;
            this.iceBalmoEndChar.FieldName = "EndChar";
            this.iceBalmoEndChar.Name = "iceBalmoEndChar";
            this.iceBalmoEndChar.Visible = true;
            this.iceBalmoEndChar.VisibleIndex = 2;
            // 
            // iceBalmoStartDay
            // 
            this.iceBalmoStartDay.Caption = "Start Day";
            this.iceBalmoStartDay.ColumnEdit = this.iceBalmoStartDayValue;
            this.iceBalmoStartDay.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.iceBalmoStartDay.FieldName = "StartDay";
            this.iceBalmoStartDay.Name = "iceBalmoStartDay";
            this.iceBalmoStartDay.Visible = true;
            this.iceBalmoStartDay.VisibleIndex = 3;
            // 
            // iceBalmoStartDayValue
            // 
            this.iceBalmoStartDayValue.AutoHeight = false;
            this.iceBalmoStartDayValue.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.iceBalmoStartDayValue.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.iceBalmoStartDayValue.Name = "iceBalmoStartDayValue";
            // 
            // companyNameSelector
            // 
            this.companyNameSelector.AutoHeight = false;
            this.companyNameSelector.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.companyNameSelector.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Company")});
            this.companyNameSelector.DropDownRows = 12;
            this.companyNameSelector.Name = "companyNameSelector";
            this.companyNameSelector.ValueMember = "CompanyId";
            // 
            // categoryOverrideSection
            // 
            this.categoryOverrideSection.Controls.Add(this.underlyingFuturesOverride);
            this.categoryOverrideSection.Controls.Add(this.categoryOverride);
            this.categoryOverrideSection.Controls.Add(this.categoryOverrideDate);
            this.categoryOverrideSection.Controls.Add(this.categoryOverrideDateTitle);
            this.categoryOverrideSection.Controls.Add(this.overrideCategorySelectorTitle);
            this.categoryOverrideSection.Location = new System.Drawing.Point(288, 547);
            this.categoryOverrideSection.Name = "categoryOverrideSection";
            this.categoryOverrideSection.Size = new System.Drawing.Size(276, 110);
            this.categoryOverrideSection.TabIndex = 19;
            this.categoryOverrideSection.Text = "Category Override";
            // 
            // underlyingFuturesOverride
            // 
            this.underlyingFuturesOverride.Location = new System.Drawing.Point(104, 81);
            this.underlyingFuturesOverride.Name = "underlyingFuturesOverride";
            this.underlyingFuturesOverride.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.underlyingFuturesOverride.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Official Product")});
            this.underlyingFuturesOverride.Properties.NullText = "[Select underlying futures]";
            this.underlyingFuturesOverride.Size = new System.Drawing.Size(162, 20);
            this.underlyingFuturesOverride.TabIndex = 35;
            // 
            // categoryOverride
            // 
            this.categoryOverride.Location = new System.Drawing.Point(104, 55);
            this.categoryOverride.Name = "categoryOverride";
            this.categoryOverride.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.categoryOverride.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Group", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.categoryOverride.Properties.NullText = "[Select group]";
            this.categoryOverride.Size = new System.Drawing.Size(162, 20);
            this.categoryOverride.TabIndex = 33;
            // 
            // categoryOverrideDate
            // 
            this.categoryOverrideDate.EditValue = null;
            this.categoryOverrideDate.Location = new System.Drawing.Point(104, 29);
            this.categoryOverrideDate.Name = "categoryOverrideDate";
            this.categoryOverrideDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.categoryOverrideDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.categoryOverrideDate.Size = new System.Drawing.Size(162, 20);
            this.categoryOverrideDate.TabIndex = 22;
            // 
            // categoryOverrideDateTitle
            // 
            this.categoryOverrideDateTitle.Location = new System.Drawing.Point(70, 32);
            this.categoryOverrideDateTitle.Name = "categoryOverrideDateTitle";
            this.categoryOverrideDateTitle.Size = new System.Drawing.Size(28, 13);
            this.categoryOverrideDateTitle.TabIndex = 21;
            this.categoryOverrideDateTitle.Text = "From:";
            // 
            // overrideCategorySelectorTitle
            // 
            this.overrideCategorySelectorTitle.Location = new System.Drawing.Point(49, 58);
            this.overrideCategorySelectorTitle.Name = "overrideCategorySelectorTitle";
            this.overrideCategorySelectorTitle.Size = new System.Drawing.Size(49, 13);
            this.overrideCategorySelectorTitle.TabIndex = 20;
            this.overrideCategorySelectorTitle.Text = "Category:";
            // 
            // iceProductAliasesSection
            // 
            this.iceProductAliasesSection.Controls.Add(this.iceAliases);
            this.iceProductAliasesSection.Location = new System.Drawing.Point(5, 739);
            this.iceProductAliasesSection.Name = "iceProductAliasesSection";
            this.iceProductAliasesSection.Size = new System.Drawing.Size(277, 112);
            this.iceProductAliasesSection.TabIndex = 6;
            this.iceProductAliasesSection.Text = "ICE Product Aliases";
            // 
            // iceAliases
            // 
            this.iceAliases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iceAliases.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.First.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.iceAliases.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.iceAliases.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.iceAliases.Location = new System.Drawing.Point(2, 20);
            this.iceAliases.MainView = this.iceAliasesDisplay;
            this.iceAliases.Name = "iceAliases";
            this.iceAliases.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.iceProductAliasIdValue});
            this.iceAliases.Size = new System.Drawing.Size(273, 90);
            this.iceAliases.TabIndex = 0;
            this.iceAliases.UseEmbeddedNavigator = true;
            this.iceAliases.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.iceAliasesDisplay});
            // 
            // iceAliasesDisplay
            // 
            this.iceAliasesDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.iceAliasProductId});
            this.iceAliasesDisplay.GridControl = this.iceAliases;
            this.iceAliasesDisplay.Name = "iceAliasesDisplay";
            this.iceAliasesDisplay.OptionsCustomization.AllowGroup = false;
            this.iceAliasesDisplay.OptionsView.ShowGroupPanel = false;
            this.iceAliasesDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gridView2_InitNewRow);
            // 
            // iceAliasProductId
            // 
            this.iceAliasProductId.Caption = "ICE Product Id";
            this.iceAliasProductId.ColumnEdit = this.iceProductAliasIdValue;
            this.iceAliasProductId.DisplayFormat.FormatString = "f0";
            this.iceAliasProductId.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.iceAliasProductId.FieldName = "IceProductId";
            this.iceAliasProductId.Name = "iceAliasProductId";
            this.iceAliasProductId.Visible = true;
            this.iceAliasProductId.VisibleIndex = 0;
            // 
            // iceProductAliasIdValue
            // 
            this.iceProductAliasIdValue.AutoHeight = false;
            this.iceProductAliasIdValue.Mask.EditMask = "f0";
            this.iceProductAliasIdValue.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.iceProductAliasIdValue.Mask.UseMaskAsDisplayFormat = true;
            this.iceProductAliasIdValue.Name = "iceProductAliasIdValue";
            // 
            // abnMappingsSection
            // 
            this.abnMappingsSection.Controls.Add(this.abnMappings);
            this.abnMappingsSection.Location = new System.Drawing.Point(5, 470);
            this.abnMappingsSection.Name = "abnMappingsSection";
            this.abnMappingsSection.Size = new System.Drawing.Size(277, 118);
            this.abnMappingsSection.TabIndex = 7;
            this.abnMappingsSection.Text = "ABN Mappings";
            // 
            // abnMappings
            // 
            this.abnMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.abnMappings.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.First.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.abnMappings.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.abnMappings.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.abnMappings.Location = new System.Drawing.Point(2, 20);
            this.abnMappings.MainView = this.abnMappingsDisplay;
            this.abnMappings.Name = "abnMappings";
            this.abnMappings.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.brokerageCompanyIdValue});
            this.abnMappings.Size = new System.Drawing.Size(273, 96);
            this.abnMappings.TabIndex = 1;
            this.abnMappings.UseEmbeddedNavigator = true;
            this.abnMappings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.abnMappingsDisplay});
            // 
            // abnMappingsDisplay
            // 
            this.abnMappingsDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.abnExchangeContractCode,
            this.abnProductCode});
            this.abnMappingsDisplay.GridControl = this.abnMappings;
            this.abnMappingsDisplay.Name = "abnMappingsDisplay";
            this.abnMappingsDisplay.OptionsCustomization.AllowGroup = false;
            this.abnMappingsDisplay.OptionsView.ShowGroupPanel = false;
            this.abnMappingsDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gvMappings_InitNewRow);
            // 
            // abnExchangeContractCode
            // 
            this.abnExchangeContractCode.Caption = "Exchange Code";
            this.abnExchangeContractCode.FieldName = "ExchangeCode";
            this.abnExchangeContractCode.Name = "abnExchangeContractCode";
            this.abnExchangeContractCode.Visible = true;
            this.abnExchangeContractCode.VisibleIndex = 0;
            // 
            // abnProductCode
            // 
            this.abnProductCode.Caption = "Product Code";
            this.abnProductCode.FieldName = "ProductCode";
            this.abnProductCode.Name = "abnProductCode";
            this.abnProductCode.Visible = true;
            this.abnProductCode.VisibleIndex = 1;
            // 
            // brokerageCompanyIdValue
            // 
            this.brokerageCompanyIdValue.AutoHeight = false;
            this.brokerageCompanyIdValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.brokerageCompanyIdValue.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Company")});
            this.brokerageCompanyIdValue.DropDownRows = 12;
            this.brokerageCompanyIdValue.Name = "brokerageCompanyIdValue";
            this.brokerageCompanyIdValue.ValueMember = "CompanyId";
            // 
            // balmoComplexProductsSelector
            // 
            this.balmoComplexProductsSelector.Enabled = false;
            this.balmoComplexProductsSelector.Location = new System.Drawing.Point(137, 43);
            this.balmoComplexProductsSelector.Name = "balmoComplexProductsSelector";
            this.balmoComplexProductsSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.balmoComplexProductsSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Product", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.balmoComplexProductsSelector.Properties.NullText = "";
            this.balmoComplexProductsSelector.Size = new System.Drawing.Size(235, 20);
            this.balmoComplexProductsSelector.TabIndex = 1;
            this.balmoComplexProductsSelector.EditValueChanged += new System.EventHandler(this.leBalmoComplexProducts_EditValueChanged);
            // 
            // balmoComplexProductTitle
            // 
            this.balmoComplexProductTitle.Location = new System.Drawing.Point(11, 46);
            this.balmoComplexProductTitle.Name = "balmoComplexProductTitle";
            this.balmoComplexProductTitle.Size = new System.Drawing.Size(120, 13);
            this.balmoComplexProductTitle.TabIndex = 18;
            this.balmoComplexProductTitle.Text = "Balmo on Complex Swap:";
            // 
            // balmoCrudeSwapsSelector
            // 
            this.balmoCrudeSwapsSelector.Enabled = false;
            this.balmoCrudeSwapsSelector.Location = new System.Drawing.Point(137, 17);
            this.balmoCrudeSwapsSelector.Name = "balmoCrudeSwapsSelector";
            this.balmoCrudeSwapsSelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.balmoCrudeSwapsSelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Product", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.balmoCrudeSwapsSelector.Properties.NullText = "";
            this.balmoCrudeSwapsSelector.Size = new System.Drawing.Size(235, 20);
            this.balmoCrudeSwapsSelector.TabIndex = 0;
            this.balmoCrudeSwapsSelector.EditValueChanged += new System.EventHandler(this.leBalmoCrudeSwaps_EditValueChanged);
            // 
            // balmoCrudeSwapTitle
            // 
            this.balmoCrudeSwapTitle.Location = new System.Drawing.Point(23, 20);
            this.balmoCrudeSwapTitle.Name = "balmoCrudeSwapTitle";
            this.balmoCrudeSwapTitle.Size = new System.Drawing.Size(108, 13);
            this.balmoCrudeSwapTitle.TabIndex = 18;
            this.balmoCrudeSwapTitle.Text = "Balmo on Crude Swap:";
            // 
            // balmoContractCodesSection
            // 
            this.balmoContractCodesSection.Controls.Add(this.contractCodeOneFirstLetter);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeThree);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeOneFirstLetterTitle);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeThreeTitle);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeTwo);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeTwoTitle);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeOne);
            this.balmoContractCodesSection.Controls.Add(this.contractCodeOneTitle);
            this.balmoContractCodesSection.Location = new System.Drawing.Point(5, 332);
            this.balmoContractCodesSection.Name = "balmoContractCodesSection";
            this.balmoContractCodesSection.Size = new System.Drawing.Size(277, 132);
            this.balmoContractCodesSection.TabIndex = 3;
            this.balmoContractCodesSection.Text = "Balmo Contract Codes";
            // 
            // contractCodeOneFirstLetter
            // 
            this.contractCodeOneFirstLetter.Location = new System.Drawing.Point(221, 102);
            this.contractCodeOneFirstLetter.Name = "contractCodeOneFirstLetter";
            this.contractCodeOneFirstLetter.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.contractCodeOneFirstLetter.Properties.Mask.BeepOnError = true;
            this.contractCodeOneFirstLetter.Properties.Mask.EditMask = "[A-Z]{1}";
            this.contractCodeOneFirstLetter.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
            this.contractCodeOneFirstLetter.Properties.Mask.SaveLiteral = false;
            this.contractCodeOneFirstLetter.Properties.Mask.ShowPlaceHolders = false;
            this.contractCodeOneFirstLetter.Properties.MaxLength = 1;
            this.contractCodeOneFirstLetter.Size = new System.Drawing.Size(46, 20);
            this.contractCodeOneFirstLetter.TabIndex = 3;
            // 
            // contractCodeThree
            // 
            this.contractCodeThree.Location = new System.Drawing.Point(221, 76);
            this.contractCodeThree.Name = "contractCodeThree";
            this.contractCodeThree.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.contractCodeThree.Properties.Mask.BeepOnError = true;
            this.contractCodeThree.Properties.Mask.EditMask = "[A-Z]{2}";
            this.contractCodeThree.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
            this.contractCodeThree.Properties.Mask.SaveLiteral = false;
            this.contractCodeThree.Properties.Mask.ShowPlaceHolders = false;
            this.contractCodeThree.Properties.MaxLength = 2;
            this.contractCodeThree.Size = new System.Drawing.Size(46, 20);
            this.contractCodeThree.TabIndex = 2;
            // 
            // contractCodeOneFirstLetterTitle
            // 
            this.contractCodeOneFirstLetterTitle.Location = new System.Drawing.Point(76, 105);
            this.contractCodeOneFirstLetterTitle.Name = "contractCodeOneFirstLetterTitle";
            this.contractCodeOneFirstLetterTitle.Size = new System.Drawing.Size(139, 13);
            this.contractCodeOneFirstLetterTitle.TabIndex = 22;
            this.contractCodeOneFirstLetterTitle.Text = "Contract Code 1 First Letter:";
            // 
            // contractCodeThreeTitle
            // 
            this.contractCodeThreeTitle.Location = new System.Drawing.Point(124, 79);
            this.contractCodeThreeTitle.Name = "contractCodeThreeTitle";
            this.contractCodeThreeTitle.Size = new System.Drawing.Size(91, 13);
            this.contractCodeThreeTitle.TabIndex = 22;
            this.contractCodeThreeTitle.Text = "Contract Code #3:";
            // 
            // contractCodeTwo
            // 
            this.contractCodeTwo.Location = new System.Drawing.Point(221, 50);
            this.contractCodeTwo.Name = "contractCodeTwo";
            this.contractCodeTwo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.contractCodeTwo.Properties.Mask.BeepOnError = true;
            this.contractCodeTwo.Properties.Mask.EditMask = "[A-Z]{2}";
            this.contractCodeTwo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
            this.contractCodeTwo.Properties.Mask.SaveLiteral = false;
            this.contractCodeTwo.Properties.Mask.ShowPlaceHolders = false;
            this.contractCodeTwo.Properties.MaxLength = 2;
            this.contractCodeTwo.ShowToolTips = false;
            this.contractCodeTwo.Size = new System.Drawing.Size(46, 20);
            this.contractCodeTwo.TabIndex = 1;
            // 
            // contractCodeTwoTitle
            // 
            this.contractCodeTwoTitle.Location = new System.Drawing.Point(124, 53);
            this.contractCodeTwoTitle.Name = "contractCodeTwoTitle";
            this.contractCodeTwoTitle.Size = new System.Drawing.Size(91, 13);
            this.contractCodeTwoTitle.TabIndex = 20;
            this.contractCodeTwoTitle.Text = "Contract Code #2:";
            // 
            // contractCodeOne
            // 
            this.contractCodeOne.Location = new System.Drawing.Point(221, 24);
            this.contractCodeOne.Name = "contractCodeOne";
            this.contractCodeOne.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.contractCodeOne.Properties.Mask.BeepOnError = true;
            this.contractCodeOne.Properties.Mask.EditMask = "[A-Z]{2}";
            this.contractCodeOne.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
            this.contractCodeOne.Properties.Mask.SaveLiteral = false;
            this.contractCodeOne.Properties.Mask.ShowPlaceHolders = false;
            this.contractCodeOne.Properties.MaxLength = 2;
            this.contractCodeOne.Size = new System.Drawing.Size(46, 20);
            this.contractCodeOne.TabIndex = 0;
            // 
            // contractCodeOneTitle
            // 
            this.contractCodeOneTitle.Location = new System.Drawing.Point(124, 27);
            this.contractCodeOneTitle.Name = "contractCodeOneTitle";
            this.contractCodeOneTitle.Size = new System.Drawing.Size(91, 13);
            this.contractCodeOneTitle.TabIndex = 18;
            this.contractCodeOneTitle.Text = "Contract Code #1:";
            // 
            // productFeesSection
            // 
            this.productFeesSection.Controls.Add(this.plattsCurrencySelector);
            this.productFeesSection.Controls.Add(this.cashCurrencySelector);
            this.productFeesSection.Controls.Add(this.blockCurrencySelector);
            this.productFeesSection.Controls.Add(this.clearingCurrencySelector);
            this.productFeesSection.Controls.Add(this.nfaCurrencySelector);
            this.productFeesSection.Controls.Add(this.exchangeCurrencySelector);
            this.productFeesSection.Controls.Add(this.primeBrokerCurrencySelector);
            this.productFeesSection.Controls.Add(this.exchangeFee);
            this.productFeesSection.Controls.Add(this.exchangeFeeTitle);
            this.productFeesSection.Controls.Add(this.plattsFeeTitle);
            this.productFeesSection.Controls.Add(this.plattsFee);
            this.productFeesSection.Controls.Add(this.blockFeeTitle);
            this.productFeesSection.Controls.Add(this.blockFee);
            this.productFeesSection.Controls.Add(this.cashFeeTitle);
            this.productFeesSection.Controls.Add(this.cashFee);
            this.productFeesSection.Controls.Add(this.clearingFee);
            this.productFeesSection.Controls.Add(this.clearingFeeTitle);
            this.productFeesSection.Controls.Add(this.commisionFee);
            this.productFeesSection.Controls.Add(this.primerBrokerFeeTitle);
            this.productFeesSection.Controls.Add(this.nfaFee);
            this.productFeesSection.Controls.Add(this.nfaFeeTitle);
            this.productFeesSection.Location = new System.Drawing.Point(288, 332);
            this.productFeesSection.Name = "productFeesSection";
            this.productFeesSection.Size = new System.Drawing.Size(276, 209);
            this.productFeesSection.TabIndex = 4;
            this.productFeesSection.Text = "Product Fees";
            // 
            // plattsCurrencySelector
            // 
            this.plattsCurrencySelector.Location = new System.Drawing.Point(212, 180);
            this.plattsCurrencySelector.Name = "plattsCurrencySelector";
            this.plattsCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.plattsCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.plattsCurrencySelector.Properties.NullText = "";
            this.plattsCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.plattsCurrencySelector.TabIndex = 26;
            // 
            // cashCurrencySelector
            // 
            this.cashCurrencySelector.Location = new System.Drawing.Point(212, 154);
            this.cashCurrencySelector.Name = "cashCurrencySelector";
            this.cashCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cashCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.cashCurrencySelector.Properties.NullText = "";
            this.cashCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.cashCurrencySelector.TabIndex = 25;
            // 
            // blockCurrencySelector
            // 
            this.blockCurrencySelector.Location = new System.Drawing.Point(212, 128);
            this.blockCurrencySelector.Name = "blockCurrencySelector";
            this.blockCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.blockCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.blockCurrencySelector.Properties.NullText = "";
            this.blockCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.blockCurrencySelector.TabIndex = 24;
            // 
            // clearingCurrencySelector
            // 
            this.clearingCurrencySelector.Location = new System.Drawing.Point(212, 102);
            this.clearingCurrencySelector.Name = "clearingCurrencySelector";
            this.clearingCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.clearingCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.clearingCurrencySelector.Properties.NullText = "";
            this.clearingCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.clearingCurrencySelector.TabIndex = 23;
            // 
            // nfaCurrencySelector
            // 
            this.nfaCurrencySelector.Location = new System.Drawing.Point(212, 76);
            this.nfaCurrencySelector.Name = "nfaCurrencySelector";
            this.nfaCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.nfaCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.nfaCurrencySelector.Properties.NullText = "";
            this.nfaCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.nfaCurrencySelector.TabIndex = 22;
            // 
            // exchangeCurrencySelector
            // 
            this.exchangeCurrencySelector.Location = new System.Drawing.Point(212, 50);
            this.exchangeCurrencySelector.Name = "exchangeCurrencySelector";
            this.exchangeCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.exchangeCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.exchangeCurrencySelector.Properties.NullText = "";
            this.exchangeCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.exchangeCurrencySelector.TabIndex = 21;
            // 
            // primeBrokerCurrencySelector
            // 
            this.primeBrokerCurrencySelector.Location = new System.Drawing.Point(212, 24);
            this.primeBrokerCurrencySelector.Name = "primeBrokerCurrencySelector";
            this.primeBrokerCurrencySelector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.primeBrokerCurrencySelector.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.primeBrokerCurrencySelector.Properties.NullText = "";
            this.primeBrokerCurrencySelector.Size = new System.Drawing.Size(59, 20);
            this.primeBrokerCurrencySelector.TabIndex = 20;
            // 
            // exchangeFee
            // 
            this.exchangeFee.Location = new System.Drawing.Point(87, 50);
            this.exchangeFee.Name = "exchangeFee";
            this.exchangeFee.Size = new System.Drawing.Size(122, 20);
            this.exchangeFee.TabIndex = 16;
            // 
            // exchangeFeeTitle
            // 
            this.exchangeFeeTitle.Location = new System.Drawing.Point(33, 53);
            this.exchangeFeeTitle.Name = "exchangeFeeTitle";
            this.exchangeFeeTitle.Size = new System.Drawing.Size(51, 13);
            this.exchangeFeeTitle.TabIndex = 17;
            this.exchangeFeeTitle.Text = "Exchange:";
            // 
            // plattsFeeTitle
            // 
            this.plattsFeeTitle.Location = new System.Drawing.Point(53, 183);
            this.plattsFeeTitle.Name = "plattsFeeTitle";
            this.plattsFeeTitle.Size = new System.Drawing.Size(31, 13);
            this.plattsFeeTitle.TabIndex = 15;
            this.plattsFeeTitle.Text = "Platts:";
            // 
            // plattsFee
            // 
            this.plattsFee.Location = new System.Drawing.Point(87, 180);
            this.plattsFee.Name = "plattsFee";
            this.plattsFee.Size = new System.Drawing.Size(122, 20);
            this.plattsFee.TabIndex = 5;
            // 
            // blockFeeTitle
            // 
            this.blockFeeTitle.Location = new System.Drawing.Point(56, 131);
            this.blockFeeTitle.Name = "blockFeeTitle";
            this.blockFeeTitle.Size = new System.Drawing.Size(28, 13);
            this.blockFeeTitle.TabIndex = 13;
            this.blockFeeTitle.Text = "Block:";
            // 
            // blockFee
            // 
            this.blockFee.Location = new System.Drawing.Point(87, 128);
            this.blockFee.Name = "blockFee";
            this.blockFee.Size = new System.Drawing.Size(122, 20);
            this.blockFee.TabIndex = 3;
            // 
            // cashFeeTitle
            // 
            this.cashFeeTitle.Location = new System.Drawing.Point(56, 157);
            this.cashFeeTitle.Name = "cashFeeTitle";
            this.cashFeeTitle.Size = new System.Drawing.Size(28, 13);
            this.cashFeeTitle.TabIndex = 11;
            this.cashFeeTitle.Text = "Cash:";
            // 
            // cashFee
            // 
            this.cashFee.Location = new System.Drawing.Point(87, 154);
            this.cashFee.Name = "cashFee";
            this.cashFee.Size = new System.Drawing.Size(122, 20);
            this.cashFee.TabIndex = 4;
            // 
            // clearingFee
            // 
            this.clearingFee.Location = new System.Drawing.Point(87, 102);
            this.clearingFee.Name = "clearingFee";
            this.clearingFee.Size = new System.Drawing.Size(122, 20);
            this.clearingFee.TabIndex = 2;
            // 
            // clearingFeeTitle
            // 
            this.clearingFeeTitle.Location = new System.Drawing.Point(41, 105);
            this.clearingFeeTitle.Name = "clearingFeeTitle";
            this.clearingFeeTitle.Size = new System.Drawing.Size(43, 13);
            this.clearingFeeTitle.TabIndex = 4;
            this.clearingFeeTitle.Text = "Clearing:";
            // 
            // commisionFee
            // 
            this.commisionFee.Location = new System.Drawing.Point(87, 24);
            this.commisionFee.Name = "commisionFee";
            this.commisionFee.Size = new System.Drawing.Size(122, 20);
            this.commisionFee.TabIndex = 0;
            // 
            // primerBrokerFeeTitle
            // 
            this.primerBrokerFeeTitle.Location = new System.Drawing.Point(20, 27);
            this.primerBrokerFeeTitle.Name = "primerBrokerFeeTitle";
            this.primerBrokerFeeTitle.Size = new System.Drawing.Size(64, 13);
            this.primerBrokerFeeTitle.TabIndex = 0;
            this.primerBrokerFeeTitle.Text = "Prime Broker:";
            // 
            // nfaFee
            // 
            this.nfaFee.Location = new System.Drawing.Point(87, 76);
            this.nfaFee.Name = "nfaFee";
            this.nfaFee.Size = new System.Drawing.Size(122, 20);
            this.nfaFee.TabIndex = 1;
            // 
            // nfaFeeTitle
            // 
            this.nfaFeeTitle.Location = new System.Drawing.Point(60, 79);
            this.nfaFeeTitle.Name = "nfaFeeTitle";
            this.nfaFeeTitle.Size = new System.Drawing.Size(24, 13);
            this.nfaFeeTitle.TabIndex = 2;
            this.nfaFeeTitle.Text = "NFA:";
            // 
            // gmiBalmoCodesSection
            // 
            this.gmiBalmoCodesSection.Controls.Add(this.gmiBalmoCodes);
            this.gmiBalmoCodesSection.Location = new System.Drawing.Point(5, 80);
            this.gmiBalmoCodesSection.Name = "gmiBalmoCodesSection";
            this.gmiBalmoCodesSection.Size = new System.Drawing.Size(559, 122);
            this.gmiBalmoCodesSection.TabIndex = 2;
            this.gmiBalmoCodesSection.Text = "GMI Balmo Codes";
            // 
            // gmiBalmoCodes
            // 
            this.gmiBalmoCodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.First.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.gmiBalmoCodes.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.gmiBalmoCodes.Location = new System.Drawing.Point(2, 20);
            this.gmiBalmoCodes.MainView = this.gmiBalmoCodesDisplay;
            this.gmiBalmoCodes.Name = "gmiBalmoCodes";
            this.gmiBalmoCodes.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.gmiCompanyCodeSelector,
            this.gmiPricingDayValue,
            this.gmiStartCharValue,
            this.gmiPrefixValue});
            this.gmiBalmoCodes.Size = new System.Drawing.Size(555, 100);
            this.gmiBalmoCodes.TabIndex = 0;
            this.gmiBalmoCodes.UseEmbeddedNavigator = true;
            this.gmiBalmoCodes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gmiBalmoCodesDisplay});
            // 
            // gmiBalmoCodesDisplay
            // 
            this.gmiBalmoCodesDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gmiExchangeCode,
            this.gmiPrefix,
            this.gmiStartChar,
            this.gmiEndChar,
            this.gmiPricingDay});
            this.gmiBalmoCodesDisplay.GridControl = this.gmiBalmoCodes;
            this.gmiBalmoCodesDisplay.Name = "gmiBalmoCodesDisplay";
            this.gmiBalmoCodesDisplay.OptionsCustomization.AllowGroup = false;
            this.gmiBalmoCodesDisplay.OptionsView.ShowGroupPanel = false;
            this.gmiBalmoCodesDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gvGmiBalmoCodes_InitNewRow);
            // 
            // gmiExchangeCode
            // 
            this.gmiExchangeCode.Caption = "Exchange Code";
            this.gmiExchangeCode.FieldName = "ExchangeCode";
            this.gmiExchangeCode.Name = "gmiExchangeCode";
            this.gmiExchangeCode.Visible = true;
            this.gmiExchangeCode.VisibleIndex = 0;
            // 
            // gmiPrefix
            // 
            this.gmiPrefix.Caption = "Prefix";
            this.gmiPrefix.ColumnEdit = this.gmiPrefixValue;
            this.gmiPrefix.FieldName = "PrefixChar";
            this.gmiPrefix.Name = "gmiPrefix";
            this.gmiPrefix.Visible = true;
            this.gmiPrefix.VisibleIndex = 1;
            // 
            // gmiPrefixValue
            // 
            this.gmiPrefixValue.AutoHeight = false;
            this.gmiPrefixValue.MaxLength = 10;
            this.gmiPrefixValue.Name = "gmiPrefixValue";
            // 
            // gmiStartChar
            // 
            this.gmiStartChar.Caption = "Start Char";
            this.gmiStartChar.ColumnEdit = this.gmiStartCharValue;
            this.gmiStartChar.FieldName = "StartChar";
            this.gmiStartChar.Name = "gmiStartChar";
            this.gmiStartChar.Visible = true;
            this.gmiStartChar.VisibleIndex = 2;
            // 
            // gmiStartCharValue
            // 
            this.gmiStartCharValue.AutoHeight = false;
            this.gmiStartCharValue.MaxLength = 1;
            this.gmiStartCharValue.Name = "gmiStartCharValue";
            // 
            // gmiEndChar
            // 
            this.gmiEndChar.Caption = "End Char";
            this.gmiEndChar.ColumnEdit = this.gmiStartCharValue;
            this.gmiEndChar.FieldName = "EndChar";
            this.gmiEndChar.Name = "gmiEndChar";
            this.gmiEndChar.Visible = true;
            this.gmiEndChar.VisibleIndex = 3;
            // 
            // gmiPricingDay
            // 
            this.gmiPricingDay.Caption = "Pricing Day";
            this.gmiPricingDay.ColumnEdit = this.gmiPricingDayValue;
            this.gmiPricingDay.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gmiPricingDay.FieldName = "PricingDay";
            this.gmiPricingDay.Name = "gmiPricingDay";
            this.gmiPricingDay.Visible = true;
            this.gmiPricingDay.VisibleIndex = 4;
            // 
            // gmiPricingDayValue
            // 
            this.gmiPricingDayValue.AutoHeight = false;
            this.gmiPricingDayValue.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gmiPricingDayValue.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gmiPricingDayValue.Name = "gmiPricingDayValue";
            // 
            // gmiCompanyCodeSelector
            // 
            this.gmiCompanyCodeSelector.AutoHeight = false;
            this.gmiCompanyCodeSelector.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gmiCompanyCodeSelector.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Company")});
            this.gmiCompanyCodeSelector.DropDownRows = 12;
            this.gmiCompanyCodeSelector.Name = "gmiCompanyCodeSelector";
            this.gmiCompanyCodeSelector.ValueMember = "CompanyId";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ProductDetailsForm
            // 
            this.AcceptButton = this.save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(575, 933);
            this.Controls.Add(this.productTabs);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductDetailsForm";
            this.Text = "Mandara: Product Details";
            this.Load += new System.EventHandler(this.ProductDetailsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.productName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryCalendar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productCategorySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officialProductSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.complexProductSection)).EndInit();
            this.complexProductSection.ResumeLayout(false);
            this.complexProductSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calculatePnlFromLegs.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg2PnlFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg1PnlFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treatTimeSpreadStripAsLegs.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.useCommonPricing.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg2PositionFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg1PositionFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg2ProductSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leg1ProductSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.commonProductPropertiesSection)).EndInit();
            this.commonProductPropertiesSection.ResumeLayout(false);
            this.commonProductPropertiesSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractSizeMultiplier.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currency2Selector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currency1Selector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.holidaysCalendar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unitsSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dailyDiffMonthShift.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.monthlyOfficialProductSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasOfficialProductSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceConversionFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.underlyingFuturesSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productTypeSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeContractCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractSize.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.definitionLink.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.useExpiryCalendar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productAliasesSection)).EndInit();
            this.productAliasesSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.productAliases)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productAliasesDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rolloffSettings)).EndInit();
            this.rolloffSettings.ResumeLayout(false);
            this.rolloffSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mocActivationTimezoneSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mocActivationTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isCalendarDaySwap.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.useRolloffSettings.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rolloffTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rolloffTimezoneSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.futuresExpirationSettings)).EndInit();
            this.futuresExpirationSettings.ResumeLayout(false);
            this.futuresExpirationSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tasTimezoneSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pricingEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.futuresExpiryTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.useFuturesExpirationSettings.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasActivationTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBrokerageByCompanySection)).EndInit();
            this.productBrokerageByCompanySection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.companiesBrokerage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companyBrokerageDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companySelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.futuresExpirationDate)).EndInit();
            this.futuresExpirationDate.ResumeLayout(false);
            this.futuresExpirationDate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfDaysSection)).EndInit();
            this.numberOfDaysSection.ResumeLayout(false);
            this.numberOfDaysSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfDaysSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expirationMonthSection)).EndInit();
            this.expirationMonthSection.ResumeLayout(false);
            this.expirationMonthSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expirationMonthSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isRollingBackward.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isRollingForward.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTypeNumOfDaysSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryTypeGivenDateSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryTypeCalendarSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.givenDateSection)).EndInit();
            this.givenDateSection.ResumeLayout(false);
            this.givenDateSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.givenDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.givenDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productTabs)).EndInit();
            this.productTabs.ResumeLayout(false);
            this.basicSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.basicSettingsSection)).EndInit();
            this.basicSettingsSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.productValidityRangeSection)).EndInit();
            this.productValidityRangeSection.ResumeLayout(false);
            this.productValidityRangeSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeImpactSection)).EndInit();
            this.testTradeImpactSection.ResumeLayout(false);
            this.testTradeImpactSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testTradePnL.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradePrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeImpact)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeStripSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeLivePrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeVolume.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeStartDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testTradeStartDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productOptionsSection)).EndInit();
            this.productOptionsSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.enableRiskDecomposition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowedForManualTrades.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tasTypeSelectors)).EndInit();
            this.tasTypeSelectors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.isMoc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isPlain.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isTas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isMinuteMarker.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isMops.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isInternalTransferProduct.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isPhysicallySettled.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alsoIsTas.Properties)).EndInit();
            this.advancedSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.balmoMappingsSection)).EndInit();
            this.balmoMappingsSection.ResumeLayout(false);
            this.balmoMappingsSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoMappingsSection)).EndInit();
            this.iceBalmoMappingsSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoMappings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoMappingsDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoPrefixValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoCharValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceBalmoStartDayValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.companyNameSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverrideSection)).EndInit();
            this.categoryOverrideSection.ResumeLayout(false);
            this.categoryOverrideSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.underlyingFuturesOverride.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverride.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverrideDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryOverrideDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceProductAliasesSection)).EndInit();
            this.iceProductAliasesSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iceAliases)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceAliasesDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iceProductAliasIdValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.abnMappingsSection)).EndInit();
            this.abnMappingsSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.abnMappings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.abnMappingsDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brokerageCompanyIdValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.balmoComplexProductsSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.balmoCrudeSwapsSelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.balmoContractCodesSection)).EndInit();
            this.balmoContractCodesSection.ResumeLayout(false);
            this.balmoContractCodesSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeOneFirstLetter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeThree.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeTwo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contractCodeOne.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productFeesSection)).EndInit();
            this.productFeesSection.ResumeLayout(false);
            this.productFeesSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plattsCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blockCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clearingCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nfaCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.primeBrokerCurrencySelector.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exchangeFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blockFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clearingFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.commisionFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nfaFee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiBalmoCodesSection)).EndInit();
            this.gmiBalmoCodesSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gmiBalmoCodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiBalmoCodesDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiPrefixValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiStartCharValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiPricingDayValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gmiCompanyCodeSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableCurrency1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableCurrency2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl productNameTitle;
        private DevExpress.XtraEditors.TextEdit productName;
        private DevExpress.XtraEditors.LabelControl productTypeTitle;
        private DevExpress.XtraEditors.LabelControl expiryCalendarTitle;
        private DevExpress.XtraEditors.LabelControl officialProductTitle;
        private DevExpress.XtraEditors.LabelControl productGroupTitle;
        private DevExpress.XtraEditors.LabelControl positionConversionFactorTitle;
        private DevExpress.XtraEditors.LabelControl pnlConversionFactorTitle;
        private DevExpress.XtraEditors.LabelControl contractSizeTitle;
        private DevExpress.XtraEditors.LookUpEdit expiryCalendar;
        private DevExpress.XtraEditors.LookUpEdit productCategorySelector;
        private DevExpress.XtraEditors.LookUpEdit officialProductSelector;
        private DevExpress.XtraEditors.GroupControl complexProductSection;
        private DevExpress.XtraEditors.LabelControl legt1FactorsTitle;
        private DevExpress.XtraEditors.LookUpEdit leg2ProductSelector;
        private DevExpress.XtraEditors.LabelControl leg2ProductTitle;
        private DevExpress.XtraEditors.LabelControl leg1FactorsTitle;
        private DevExpress.XtraEditors.LookUpEdit leg1ProductSelector;
        private DevExpress.XtraEditors.LabelControl leg1ProductTitle;
        private DevExpress.XtraEditors.GroupControl commonProductPropertiesSection;
        private DevExpress.XtraEditors.SimpleButton save;
        private DevExpress.XtraEditors.SimpleButton cancel;
        private DevExpress.XtraEditors.TextEdit positionFactor;
        private DevExpress.XtraEditors.TextEdit pnlFactor;
        private DevExpress.XtraEditors.TextEdit contractSize;
        private DevExpress.XtraEditors.LookUpEdit productTypeSelector;
        private DevExpress.XtraEditors.TextEdit leg2PositionFactor;
        private DevExpress.XtraEditors.TextEdit leg1PositionFactor;
        private DevExpress.XtraEditors.GroupControl productAliasesSection;
        private DevExpress.XtraGrid.GridControl productAliases;
        private DevExpress.XtraGrid.Views.Grid.GridView productAliasesDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn alias;
        private DevExpress.XtraEditors.DateEdit validTo;
        private DevExpress.XtraEditors.LabelControl validToTitle;
        private DevExpress.XtraEditors.DateEdit validFrom;
        private DevExpress.XtraEditors.LabelControl validFromTitle;
        private DevExpress.XtraEditors.LabelControl underlyingFuturesTitle;
        private DevExpress.XtraEditors.TextEdit exchangeContractCode;
        private DevExpress.XtraEditors.LabelControl exchContractCodeTitle;
        private DevExpress.XtraEditors.GroupControl rolloffSettings;
        private DevExpress.XtraEditors.TimeEdit rolloffTime;
        private DevExpress.XtraEditors.LabelControl rollOffTimeTitle;
        private DevExpress.XtraEditors.ComboBoxEdit rolloffTimezoneSelector;
        private DevExpress.XtraEditors.LabelControl timeZoneTitle;
        private DevExpress.XtraEditors.LabelControl localRollOffTime;
        private DevExpress.XtraEditors.LabelControl localRollOffTimeTitle;
        private DevExpress.XtraEditors.CheckEdit useRolloffSettings;
        private DevExpress.XtraEditors.GroupControl productBrokerageByCompanySection;
        private DevExpress.XtraGrid.GridControl companiesBrokerage;
        private DevExpress.XtraGrid.Views.Grid.GridView companyBrokerageDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn companyName;
        private DevExpress.XtraGrid.Columns.GridColumn brokerage;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit companySelector;
        private DevExpress.XtraEditors.LabelControl definitionLinkTitle;
        private DevExpress.XtraEditors.HyperLinkEdit definitionLink;
        private DevExpress.XtraEditors.GroupControl futuresExpirationDate;
        private DevExpress.XtraEditors.PanelControl givenDateSection;
        private DevExpress.XtraEditors.DateEdit givenDate;
        private DevExpress.XtraEditors.LabelControl givenDateTitle;
        private DevExpress.XtraEditors.PanelControl numberOfDaysSection;
        private DevExpress.XtraEditors.LabelControl numberOfDaysTitle;
        private DevExpress.XtraEditors.ComboBoxEdit numberOfDaysSelector;
        private DevExpress.XtraEditors.PanelControl expirationMonthSection;
        private DevExpress.XtraEditors.LabelControl expirationMonthTitle;
        private DevExpress.XtraEditors.ComboBoxEdit expirationMonthSelector;
        private DevExpress.XtraEditors.CheckEdit isRollingBackward;
        private DevExpress.XtraEditors.CheckEdit isRollingForward;
        private DevExpress.XtraEditors.LabelControl rollingMethodTitle;
        private DevExpress.XtraEditors.CheckEdit expTypeNumOfDaysSelector;
        private DevExpress.XtraEditors.CheckEdit expiryTypeGivenDateSelector;
        private DevExpress.XtraEditors.CheckEdit expiryTypeCalendarSelector;
        private DevExpress.XtraEditors.LabelControl expiryTypeTitle;
        private DevExpress.XtraTab.XtraTabControl productTabs;
        private DevExpress.XtraTab.XtraTabPage advancedSettings;
        private DevExpress.XtraEditors.GroupControl abnMappingsSection;
        private DevExpress.XtraTab.XtraTabPage basicSettings;
        private DevExpress.XtraGrid.GridControl abnMappings;
        private DevExpress.XtraGrid.Views.Grid.GridView abnMappingsDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn abnExchangeContractCode;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit brokerageCompanyIdValue;
        private DevExpress.XtraGrid.Columns.GridColumn abnProductCode;
        private DevExpress.XtraEditors.PanelControl balmoMappingsSection;
        private DevExpress.XtraEditors.PanelControl basicSettingsSection;
        private DevExpress.XtraEditors.GroupControl gmiBalmoCodesSection;
        private DevExpress.XtraGrid.GridControl gmiBalmoCodes;
        private DevExpress.XtraGrid.Views.Grid.GridView gmiBalmoCodesDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn gmiExchangeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gmiPrefix;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit gmiCompanyCodeSelector;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit gmiStartCharValue;
        private DevExpress.XtraGrid.Columns.GridColumn gmiStartChar;
        private DevExpress.XtraGrid.Columns.GridColumn gmiEndChar;
        private DevExpress.XtraGrid.Columns.GridColumn gmiPricingDay;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit gmiPricingDayValue;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit gmiPrefixValue;
        private DevExpress.XtraEditors.LabelControl exchangeTitle;
        private DevExpress.XtraEditors.LookUpEdit exchangeSelector;
        private DevExpress.XtraEditors.LabelControl expireTimeTitle;
        private DevExpress.XtraEditors.CheckEdit useExpiryCalendar;
        private DevExpress.XtraEditors.CheckEdit isPhysicallySettled;
        private DevExpress.XtraEditors.CheckEdit useCommonPricing;
        private DevExpress.XtraEditors.LookUpEdit balmoComplexProductsSelector;
        private DevExpress.XtraEditors.LabelControl balmoComplexProductTitle;
        private DevExpress.XtraEditors.LookUpEdit balmoCrudeSwapsSelector;
        private DevExpress.XtraEditors.LabelControl balmoCrudeSwapTitle;
        private DevExpress.XtraEditors.GroupControl balmoContractCodesSection;
        private DevExpress.XtraEditors.TextEdit contractCodeOneFirstLetter;
        private DevExpress.XtraEditors.TextEdit contractCodeThree;
        private DevExpress.XtraEditors.LabelControl contractCodeOneFirstLetterTitle;
        private DevExpress.XtraEditors.LabelControl contractCodeThreeTitle;
        private DevExpress.XtraEditors.TextEdit contractCodeTwo;
        private DevExpress.XtraEditors.LabelControl contractCodeTwoTitle;
        private DevExpress.XtraEditors.TextEdit contractCodeOne;
        private DevExpress.XtraEditors.LabelControl contractCodeOneTitle;
        private DevExpress.XtraEditors.GroupControl productFeesSection;
        private DevExpress.XtraEditors.LabelControl plattsFeeTitle;
        private DevExpress.XtraEditors.TextEdit plattsFee;
        private DevExpress.XtraEditors.LabelControl blockFeeTitle;
        private DevExpress.XtraEditors.TextEdit blockFee;
        private DevExpress.XtraEditors.LabelControl cashFeeTitle;
        private DevExpress.XtraEditors.TextEdit cashFee;
        private DevExpress.XtraEditors.TextEdit clearingFee;
        private DevExpress.XtraEditors.LabelControl clearingFeeTitle;
        private DevExpress.XtraEditors.TextEdit commisionFee;
        private DevExpress.XtraEditors.LabelControl primerBrokerFeeTitle;
        private DevExpress.XtraEditors.TextEdit nfaFee;
        private DevExpress.XtraEditors.LabelControl nfaFeeTitle;
        private DevExpress.XtraEditors.GroupControl productValidityRangeSection;
        private DevExpress.XtraEditors.GroupControl testTradeImpactSection;
        private DevExpress.XtraEditors.GroupControl productOptionsSection;
        private DevExpress.XtraEditors.CheckEdit isInternalTransferProduct;
        private DevExpress.XtraEditors.LookUpEdit underlyingFuturesSelector;
        private DevExpress.XtraEditors.TextEdit testTradeVolume;
        private DevExpress.XtraEditors.LabelControl testTradeVolumeTitle;
        private DevExpress.XtraEditors.TextEdit testTradePrice;
        private DevExpress.XtraEditors.LabelControl testTradePriceTitle;
        private DevExpress.XtraEditors.TextEdit testTradeLivePrice;
        private DevExpress.XtraEditors.LabelControl testTradeLivePriceTitle;
        private DevExpress.XtraEditors.LabelControl testTradeStripTitle;
        private DevExpress.XtraEditors.ComboBoxEdit testTradeStripSelector;
        private DevExpress.XtraPivotGrid.PivotGridControl testTradeImpact;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeImpactProductGroupSort;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeImpactProductSort;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeImpactSourceSort;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeImpactCalcYearSort;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeImpactMonthSort;
        private DevExpress.XtraPivotGrid.PivotGridField pgfPos;
        private DevExpress.XtraPivotGrid.PivotGridField testTradeImpactCalcIdSort;
        private DevExpress.XtraEditors.TextEdit testTradePnL;
        private DevExpress.XtraEditors.LabelControl testTradePnLTitle;
        private DevExpress.XtraEditors.DateEdit testTradeStartDate;
        private DevExpress.XtraEditors.LabelControl testTradeStartDateTitle;
        private DevExpress.XtraEditors.GroupControl iceProductAliasesSection;
        private DevExpress.XtraGrid.GridControl iceAliases;
        private DevExpress.XtraGrid.Views.Grid.GridView iceAliasesDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn iceAliasProductId;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit iceProductAliasIdValue;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider errorProvider;
        private DevExpress.XtraEditors.TextEdit priceConversionFactor;
        private DevExpress.XtraEditors.LabelControl priceConversionFactorTitle;
        private DevExpress.XtraEditors.TextEdit exchangeFee;
        private DevExpress.XtraEditors.LabelControl exchangeFeeTitle;
        private DevExpress.XtraEditors.GroupControl futuresExpirationSettings;
        private DevExpress.XtraEditors.TimeEdit pricingEndTime;
        private DevExpress.XtraEditors.LabelControl pricingEndTimeTitle;
        private DevExpress.XtraEditors.TimeEdit futuresExpiryTime;
        private DevExpress.XtraEditors.LabelControl futuresExpiryTimeTitle;
        private DevExpress.XtraEditors.CheckEdit useFuturesExpirationSettings;
        private DevExpress.XtraEditors.TimeEdit tasActivationTime;
        private DevExpress.XtraEditors.LabelControl tasActivationTimeTitle;
        private DevExpress.XtraEditors.ComboBoxEdit tasTimezoneSelector;
        private DevExpress.XtraEditors.LabelControl tasTimeZoneTitle;
        private DevExpress.XtraEditors.CheckEdit isTas;
        private DevExpress.XtraEditors.CheckEdit isMinuteMarker;
        private DevExpress.XtraEditors.CheckEdit isMops;
        private DevExpress.XtraEditors.CheckEdit alsoIsTas;
        private DevExpress.XtraEditors.PanelControl tasTypeSelectors;
        private DevExpress.XtraEditors.CheckEdit isPlain;
        private DevExpress.XtraEditors.LabelControl tasOfficialProductTitle;
        private DevExpress.XtraEditors.LookUpEdit tasOfficialProductSelector;
        private DevExpress.XtraEditors.CheckEdit treatTimeSpreadStripAsLegs;
        private DevExpress.XtraEditors.GroupControl categoryOverrideSection;
        private DevExpress.XtraEditors.LookUpEdit categoryOverride;
        private DevExpress.XtraEditors.DateEdit categoryOverrideDate;
        private DevExpress.XtraEditors.LabelControl categoryOverrideDateTitle;
        private DevExpress.XtraEditors.LabelControl overrideCategorySelectorTitle;
        private DevExpress.XtraEditors.LookUpEdit underlyingFuturesOverride;
        private DevExpress.XtraEditors.TextEdit leg2PnlFactor;
        private DevExpress.XtraEditors.TextEdit leg1PnlFactor;
        private DevExpress.XtraEditors.LabelControl pnlFactorsTitle;
        private DevExpress.XtraEditors.LabelControl positionFactorsTitle;
        private DevExpress.XtraEditors.LabelControl monthlyOfficialProductTitle;
        private DevExpress.XtraEditors.LookUpEdit monthlyOfficialProductSelector;
        private DevExpress.XtraEditors.LabelControl dailyDiffMonthShiftTitle;
        private DevExpress.XtraEditors.LookUpEdit dailyDiffMonthShift;
        private DevExpress.XtraEditors.LookUpEdit unitsSelector;
        private DevExpress.XtraEditors.LabelControl holidayCalendarTitle;
        private DevExpress.XtraEditors.LookUpEdit holidaysCalendar;
        private DevExpress.XtraEditors.GroupControl iceBalmoMappingsSection;
        private DevExpress.XtraGrid.GridControl iceBalmoMappings;
        private DevExpress.XtraGrid.Views.Grid.GridView iceBalmoMappingsDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn iceBalmoPrefix;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit iceBalmoPrefixValue;
        private DevExpress.XtraGrid.Columns.GridColumn iceBalmoStartChar;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit iceBalmoCharValue;
        private DevExpress.XtraGrid.Columns.GridColumn iceBalmoEndChar;
        private DevExpress.XtraGrid.Columns.GridColumn iceBalmoStartDay;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit iceBalmoStartDayValue;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit companyNameSelector;
        private DevExpress.XtraEditors.CheckEdit allowedForManualTrades;
        private DevExpress.XtraEditors.LabelControl currency2Title;
        private DevExpress.XtraEditors.LookUpEdit currency1Selector;
        private DevExpress.XtraEditors.LabelControl currency1Title;
        private DevExpress.XtraEditors.LookUpEdit currency2Selector;
        private DevExpress.XtraEditors.CheckEdit enableRiskDecomposition;
        private ComboBoxEdit contractSizeMultiplier;
        private CheckEdit isCalendarDaySwap;
        private LookUpEdit primeBrokerCurrencySelector;
        private System.Windows.Forms.BindingSource availableCurrency2;
        private System.Windows.Forms.BindingSource availableCurrency1;
        private LookUpEdit plattsCurrencySelector;
        private LookUpEdit cashCurrencySelector;
        private LookUpEdit blockCurrencySelector;
        private LookUpEdit clearingCurrencySelector;
        private LookUpEdit nfaCurrencySelector;
        private LookUpEdit exchangeCurrencySelector;
        private CheckEdit calculatePnlFromLegs;
        private CheckEdit isMoc;
        private ComboBoxEdit mocActivationTimezoneSelector;
        private TimeEdit mocActivationTime;
        private LabelControl mocActivationTimeTitle;
    }
}
