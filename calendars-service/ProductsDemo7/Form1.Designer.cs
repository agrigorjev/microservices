namespace ProductsDemo7
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            statusStrip = new StatusStrip();
            viewState = new ToolStripStatusLabel();
            productBindingSource = new BindingSource(components);
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            colProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colName = new DevExpress.XtraGrid.Columns.GridColumn();
            colholidays_calendar_id = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsDefault = new DevExpress.XtraGrid.Columns.GridColumn();
            colcalendar_id = new DevExpress.XtraGrid.Columns.GridColumn();
            colPnlFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            colProductTypeDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colPositionFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            colCategoryId = new DevExpress.XtraGrid.Columns.GridColumn();
            colValidFrom = new DevExpress.XtraGrid.Columns.GridColumn();
            colValidTo = new DevExpress.XtraGrid.Columns.GridColumn();
            colOfficialProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colContractSize = new DevExpress.XtraGrid.Columns.GridColumn();
            colBalmoOnCrudeProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colBalmoOnComplexProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colUnderlyingFuturesProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colUnderlyingFuturesOverrideId = new DevExpress.XtraGrid.Columns.GridColumn();
            colExchangeContractCode = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeExchange = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeNfa = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeCommission = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeClearing = new DevExpress.XtraGrid.Columns.GridColumn();
            colTimezoneId = new DevExpress.XtraGrid.Columns.GridColumn();
            colRolloffTime = new DevExpress.XtraGrid.Columns.GridColumn();
            colUseRolloffSettings = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeConversionFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeCash = new DevExpress.XtraGrid.Columns.GridColumn();
            colDefinitionLink = new DevExpress.XtraGrid.Columns.GridColumn();
            colBalmoContractCode1 = new DevExpress.XtraGrid.Columns.GridColumn();
            colBalmoContractCode2 = new DevExpress.XtraGrid.Columns.GridColumn();
            colBalmoContractCode3 = new DevExpress.XtraGrid.Columns.GridColumn();
            colBalmoCodeFirstLetter = new DevExpress.XtraGrid.Columns.GridColumn();
            colExpirationTypeDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colRollingMethodDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colGivenDate = new DevExpress.XtraGrid.Columns.GridColumn();
            colExpirationMonth = new DevExpress.XtraGrid.Columns.GridColumn();
            colNumberOfDays = new DevExpress.XtraGrid.Columns.GridColumn();
            colExchangeId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeBlockTrade = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeePlattsTrade = new DevExpress.XtraGrid.Columns.GridColumn();
            colFuturesExpireTime = new DevExpress.XtraGrid.Columns.GridColumn();
            colUseExpiryCalendar = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsPhysicallySettledDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsInternalTransferProductDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colPriceConversionFactorDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsTasDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colPricingEndTime = new DevExpress.XtraGrid.Columns.GridColumn();
            colTreatTimespreadStripAsLegsDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colCalculatePnlFromLegs = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsMopsDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsMmDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colCategoryOverrideAt = new DevExpress.XtraGrid.Columns.GridColumn();
            colCategoryOverrideId = new DevExpress.XtraGrid.Columns.GridColumn();
            colTasOfficialProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsChanged = new DevExpress.XtraGrid.Columns.GridColumn();
            colMonthlyOfficialProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colDailyDiffMonthShiftDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colDailyDiffMonthShift = new DevExpress.XtraGrid.Columns.GridColumn();
            colPhysicalCode = new DevExpress.XtraGrid.Columns.GridColumn();
            colUnitId = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsAllowedForManualTradesDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colCurrency1Id = new DevExpress.XtraGrid.Columns.GridColumn();
            colCurrency2Id = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeClearingCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeCommissionCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeExchangeCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeNfaCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeCashCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeeBlockCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colFeePlattsCurrencyId = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsEnabledRiskDecompositionDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsCalendarDaySwap = new DevExpress.XtraGrid.Columns.GridColumn();
            colContractSizeMultiplierDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colIceEquivalentProductDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colIceEquivalentUnderlyingProduct = new DevExpress.XtraGrid.Columns.GridColumn();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)productBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { viewState });
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 22);
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip1";
            // 
            // viewState
            // 
            viewState.Name = "viewState";
            viewState.Size = new Size(785, 17);
            viewState.Spring = true;
            viewState.Text = "toolStripStatusLabel1";
            viewState.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // productBindingSource
            // 
            productBindingSource.DataSource = typeof(ProductsDemo.Model.Product);
            // 
            // gridView1
            // 
            gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colProductId, colName, colholidays_calendar_id, colIsDefault, colcalendar_id, colPnlFactor, colProductTypeDb, colPositionFactor, colCategoryId, colValidFrom, colValidTo, colOfficialProductId, colContractSize, colBalmoOnCrudeProductId, colBalmoOnComplexProductId, colUnderlyingFuturesProductId, colUnderlyingFuturesOverrideId, colExchangeContractCode, colFeeExchange, colFeeNfa, colFeeCommission, colFeeClearing, colTimezoneId, colRolloffTime, colUseRolloffSettings, colFeeConversionFactor, colFeeCash, colDefinitionLink, colBalmoContractCode1, colBalmoContractCode2, colBalmoContractCode3, colBalmoCodeFirstLetter, colExpirationTypeDb, colRollingMethodDb, colGivenDate, colExpirationMonth, colNumberOfDays, colExchangeId, colFeeBlockTrade, colFeePlattsTrade, colFuturesExpireTime, colUseExpiryCalendar, colIsPhysicallySettledDb, colIsInternalTransferProductDb, colPriceConversionFactorDb, colIsTasDb, colPricingEndTime, colTreatTimespreadStripAsLegsDb, colCalculatePnlFromLegs, colIsMopsDb, colIsMmDb, colCategoryOverrideAt, colCategoryOverrideId, colTasOfficialProductId, colIsChanged, colMonthlyOfficialProductId, colDailyDiffMonthShiftDb, colDailyDiffMonthShift, colPhysicalCode, colUnitId, colIsAllowedForManualTradesDb, colCurrency1Id, colCurrency2Id, colFeeClearingCurrencyId, colFeeCommissionCurrencyId, colFeeExchangeCurrencyId, colFeeNfaCurrencyId, colFeeCashCurrencyId, colFeeBlockCurrencyId, colFeePlattsCurrencyId, colIsEnabledRiskDecompositionDb, colIsCalendarDaySwap, colContractSizeMultiplierDb, colIceEquivalentProductDb, colIceEquivalentUnderlyingProduct });
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // colProductId
            // 
            colProductId.FieldName = "ProductId";
            colProductId.Name = "colProductId";
            colProductId.OptionsColumn.AllowEdit = false;
            colProductId.OptionsColumn.ReadOnly = true;
            colProductId.Visible = true;
            colProductId.VisibleIndex = 1;
            // 
            // colName
            // 
            colName.FieldName = "Name";
            colName.Name = "colName";
            colName.OptionsColumn.ReadOnly = true;
            colName.Visible = true;
            colName.VisibleIndex = 4;
            // 
            // colholidays_calendar_id
            // 
            colholidays_calendar_id.FieldName = "holidays_calendar_id";
            colholidays_calendar_id.Name = "colholidays_calendar_id";
            colholidays_calendar_id.OptionsColumn.AllowEdit = false;
            colholidays_calendar_id.OptionsColumn.ReadOnly = true;
            colholidays_calendar_id.Visible = true;
            colholidays_calendar_id.VisibleIndex = 3;
            // 
            // colIsDefault
            // 
            colIsDefault.FieldName = "IsDefault";
            colIsDefault.Name = "colIsDefault";
            colIsDefault.OptionsColumn.AllowEdit = false;
            colIsDefault.OptionsColumn.ReadOnly = true;
            colIsDefault.Visible = true;
            colIsDefault.VisibleIndex = 0;
            // 
            // colcalendar_id
            // 
            colcalendar_id.FieldName = "calendar_id";
            colcalendar_id.Name = "colcalendar_id";
            colcalendar_id.OptionsColumn.AllowEdit = false;
            colcalendar_id.OptionsColumn.ReadOnly = true;
            colcalendar_id.Visible = true;
            colcalendar_id.VisibleIndex = 2;
            // 
            // colPnlFactor
            // 
            colPnlFactor.FieldName = "PnlFactor";
            colPnlFactor.Name = "colPnlFactor";
            colPnlFactor.OptionsColumn.AllowEdit = false;
            colPnlFactor.OptionsColumn.ReadOnly = true;
            colPnlFactor.Visible = true;
            colPnlFactor.VisibleIndex = 7;
            // 
            // colProductTypeDb
            // 
            colProductTypeDb.FieldName = "ProductTypeDb";
            colProductTypeDb.Name = "colProductTypeDb";
            colProductTypeDb.OptionsColumn.AllowEdit = false;
            colProductTypeDb.OptionsColumn.ReadOnly = true;
            colProductTypeDb.Visible = true;
            colProductTypeDb.VisibleIndex = 5;
            // 
            // colPositionFactor
            // 
            colPositionFactor.FieldName = "PositionFactor";
            colPositionFactor.Name = "colPositionFactor";
            colPositionFactor.OptionsColumn.AllowEdit = false;
            colPositionFactor.OptionsColumn.ReadOnly = true;
            colPositionFactor.Visible = true;
            colPositionFactor.VisibleIndex = 6;
            // 
            // colCategoryId
            // 
            colCategoryId.FieldName = "CategoryId";
            colCategoryId.Name = "colCategoryId";
            colCategoryId.OptionsColumn.AllowEdit = false;
            colCategoryId.OptionsColumn.ReadOnly = true;
            colCategoryId.Visible = true;
            colCategoryId.VisibleIndex = 8;
            // 
            // colValidFrom
            // 
            colValidFrom.FieldName = "ValidFrom";
            colValidFrom.Name = "colValidFrom";
            colValidFrom.OptionsColumn.AllowEdit = false;
            colValidFrom.OptionsColumn.ReadOnly = true;
            colValidFrom.Visible = true;
            colValidFrom.VisibleIndex = 11;
            // 
            // colValidTo
            // 
            colValidTo.FieldName = "ValidTo";
            colValidTo.Name = "colValidTo";
            colValidTo.OptionsColumn.AllowEdit = false;
            colValidTo.OptionsColumn.ReadOnly = true;
            colValidTo.Visible = true;
            colValidTo.VisibleIndex = 12;
            // 
            // colOfficialProductId
            // 
            colOfficialProductId.FieldName = "OfficialProductId";
            colOfficialProductId.Name = "colOfficialProductId";
            colOfficialProductId.OptionsColumn.AllowEdit = false;
            colOfficialProductId.OptionsColumn.ReadOnly = true;
            colOfficialProductId.Visible = true;
            colOfficialProductId.VisibleIndex = 9;
            // 
            // colContractSize
            // 
            colContractSize.FieldName = "ContractSize";
            colContractSize.Name = "colContractSize";
            colContractSize.OptionsColumn.AllowEdit = false;
            colContractSize.OptionsColumn.ReadOnly = true;
            colContractSize.Visible = true;
            colContractSize.VisibleIndex = 10;
            // 
            // colBalmoOnCrudeProductId
            // 
            colBalmoOnCrudeProductId.FieldName = "BalmoOnCrudeProductId";
            colBalmoOnCrudeProductId.Name = "colBalmoOnCrudeProductId";
            // 
            // colBalmoOnComplexProductId
            // 
            colBalmoOnComplexProductId.FieldName = "BalmoOnComplexProductId";
            colBalmoOnComplexProductId.Name = "colBalmoOnComplexProductId";
            // 
            // colUnderlyingFuturesProductId
            // 
            colUnderlyingFuturesProductId.FieldName = "UnderlyingFuturesProductId";
            colUnderlyingFuturesProductId.Name = "colUnderlyingFuturesProductId";
            // 
            // colUnderlyingFuturesOverrideId
            // 
            colUnderlyingFuturesOverrideId.FieldName = "UnderlyingFuturesOverrideId";
            colUnderlyingFuturesOverrideId.Name = "colUnderlyingFuturesOverrideId";
            // 
            // colExchangeContractCode
            // 
            colExchangeContractCode.FieldName = "ExchangeContractCode";
            colExchangeContractCode.Name = "colExchangeContractCode";
            // 
            // colFeeExchange
            // 
            colFeeExchange.FieldName = "FeeExchange";
            colFeeExchange.Name = "colFeeExchange";
            // 
            // colFeeNfa
            // 
            colFeeNfa.FieldName = "FeeNfa";
            colFeeNfa.Name = "colFeeNfa";
            // 
            // colFeeCommission
            // 
            colFeeCommission.FieldName = "FeeCommission";
            colFeeCommission.Name = "colFeeCommission";
            // 
            // colFeeClearing
            // 
            colFeeClearing.FieldName = "FeeClearing";
            colFeeClearing.Name = "colFeeClearing";
            // 
            // colTimezoneId
            // 
            colTimezoneId.FieldName = "TimezoneId";
            colTimezoneId.Name = "colTimezoneId";
            // 
            // colRolloffTime
            // 
            colRolloffTime.FieldName = "RolloffTime";
            colRolloffTime.Name = "colRolloffTime";
            // 
            // colUseRolloffSettings
            // 
            colUseRolloffSettings.FieldName = "UseRolloffSettings";
            colUseRolloffSettings.Name = "colUseRolloffSettings";
            // 
            // colFeeConversionFactor
            // 
            colFeeConversionFactor.FieldName = "FeeConversionFactor";
            colFeeConversionFactor.Name = "colFeeConversionFactor";
            // 
            // colFeeCash
            // 
            colFeeCash.FieldName = "FeeCash";
            colFeeCash.Name = "colFeeCash";
            // 
            // colDefinitionLink
            // 
            colDefinitionLink.FieldName = "DefinitionLink";
            colDefinitionLink.Name = "colDefinitionLink";
            // 
            // colBalmoContractCode1
            // 
            colBalmoContractCode1.FieldName = "BalmoContractCode1";
            colBalmoContractCode1.Name = "colBalmoContractCode1";
            // 
            // colBalmoContractCode2
            // 
            colBalmoContractCode2.FieldName = "BalmoContractCode2";
            colBalmoContractCode2.Name = "colBalmoContractCode2";
            // 
            // colBalmoContractCode3
            // 
            colBalmoContractCode3.FieldName = "BalmoContractCode3";
            colBalmoContractCode3.Name = "colBalmoContractCode3";
            // 
            // colBalmoCodeFirstLetter
            // 
            colBalmoCodeFirstLetter.FieldName = "BalmoCodeFirstLetter";
            colBalmoCodeFirstLetter.Name = "colBalmoCodeFirstLetter";
            // 
            // colExpirationTypeDb
            // 
            colExpirationTypeDb.FieldName = "ExpirationTypeDb";
            colExpirationTypeDb.Name = "colExpirationTypeDb";
            // 
            // colRollingMethodDb
            // 
            colRollingMethodDb.FieldName = "RollingMethodDb";
            colRollingMethodDb.Name = "colRollingMethodDb";
            // 
            // colGivenDate
            // 
            colGivenDate.FieldName = "GivenDate";
            colGivenDate.Name = "colGivenDate";
            // 
            // colExpirationMonth
            // 
            colExpirationMonth.FieldName = "ExpirationMonth";
            colExpirationMonth.Name = "colExpirationMonth";
            // 
            // colNumberOfDays
            // 
            colNumberOfDays.FieldName = "NumberOfDays";
            colNumberOfDays.Name = "colNumberOfDays";
            // 
            // colExchangeId
            // 
            colExchangeId.FieldName = "ExchangeId";
            colExchangeId.Name = "colExchangeId";
            // 
            // colFeeBlockTrade
            // 
            colFeeBlockTrade.FieldName = "FeeBlockTrade";
            colFeeBlockTrade.Name = "colFeeBlockTrade";
            // 
            // colFeePlattsTrade
            // 
            colFeePlattsTrade.FieldName = "FeePlattsTrade";
            colFeePlattsTrade.Name = "colFeePlattsTrade";
            // 
            // colFuturesExpireTime
            // 
            colFuturesExpireTime.FieldName = "FuturesExpireTime";
            colFuturesExpireTime.Name = "colFuturesExpireTime";
            // 
            // colUseExpiryCalendar
            // 
            colUseExpiryCalendar.FieldName = "UseExpiryCalendar";
            colUseExpiryCalendar.Name = "colUseExpiryCalendar";
            // 
            // colIsPhysicallySettledDb
            // 
            colIsPhysicallySettledDb.FieldName = "IsPhysicallySettledDb";
            colIsPhysicallySettledDb.Name = "colIsPhysicallySettledDb";
            // 
            // colIsInternalTransferProductDb
            // 
            colIsInternalTransferProductDb.FieldName = "IsInternalTransferProductDb";
            colIsInternalTransferProductDb.Name = "colIsInternalTransferProductDb";
            // 
            // colPriceConversionFactorDb
            // 
            colPriceConversionFactorDb.FieldName = "PriceConversionFactorDb";
            colPriceConversionFactorDb.Name = "colPriceConversionFactorDb";
            // 
            // colIsTasDb
            // 
            colIsTasDb.FieldName = "IsTasDb";
            colIsTasDb.Name = "colIsTasDb";
            // 
            // colPricingEndTime
            // 
            colPricingEndTime.FieldName = "PricingEndTime";
            colPricingEndTime.Name = "colPricingEndTime";
            // 
            // colTreatTimespreadStripAsLegsDb
            // 
            colTreatTimespreadStripAsLegsDb.FieldName = "TreatTimespreadStripAsLegsDb";
            colTreatTimespreadStripAsLegsDb.Name = "colTreatTimespreadStripAsLegsDb";
            // 
            // colCalculatePnlFromLegs
            // 
            colCalculatePnlFromLegs.FieldName = "CalculatePnlFromLegs";
            colCalculatePnlFromLegs.Name = "colCalculatePnlFromLegs";
            // 
            // colIsMopsDb
            // 
            colIsMopsDb.FieldName = "IsMopsDb";
            colIsMopsDb.Name = "colIsMopsDb";
            // 
            // colIsMmDb
            // 
            colIsMmDb.FieldName = "IsMmDb";
            colIsMmDb.Name = "colIsMmDb";
            // 
            // colCategoryOverrideAt
            // 
            colCategoryOverrideAt.FieldName = "CategoryOverrideAt";
            colCategoryOverrideAt.Name = "colCategoryOverrideAt";
            // 
            // colCategoryOverrideId
            // 
            colCategoryOverrideId.FieldName = "CategoryOverrideId";
            colCategoryOverrideId.Name = "colCategoryOverrideId";
            // 
            // colTasOfficialProductId
            // 
            colTasOfficialProductId.FieldName = "TasOfficialProductId";
            colTasOfficialProductId.Name = "colTasOfficialProductId";
            // 
            // colIsChanged
            // 
            colIsChanged.FieldName = "IsChanged";
            colIsChanged.Name = "colIsChanged";
            // 
            // colMonthlyOfficialProductId
            // 
            colMonthlyOfficialProductId.FieldName = "MonthlyOfficialProductId";
            colMonthlyOfficialProductId.Name = "colMonthlyOfficialProductId";
            // 
            // colDailyDiffMonthShiftDb
            // 
            colDailyDiffMonthShiftDb.FieldName = "DailyDiffMonthShiftDb";
            colDailyDiffMonthShiftDb.Name = "colDailyDiffMonthShiftDb";
            // 
            // colDailyDiffMonthShift
            // 
            colDailyDiffMonthShift.FieldName = "DailyDiffMonthShift";
            colDailyDiffMonthShift.Name = "colDailyDiffMonthShift";
            // 
            // colPhysicalCode
            // 
            colPhysicalCode.FieldName = "PhysicalCode";
            colPhysicalCode.Name = "colPhysicalCode";
            // 
            // colUnitId
            // 
            colUnitId.FieldName = "UnitId";
            colUnitId.Name = "colUnitId";
            // 
            // colIsAllowedForManualTradesDb
            // 
            colIsAllowedForManualTradesDb.FieldName = "IsAllowedForManualTradesDb";
            colIsAllowedForManualTradesDb.Name = "colIsAllowedForManualTradesDb";
            // 
            // colCurrency1Id
            // 
            colCurrency1Id.FieldName = "Currency1Id";
            colCurrency1Id.Name = "colCurrency1Id";
            // 
            // colCurrency2Id
            // 
            colCurrency2Id.FieldName = "Currency2Id";
            colCurrency2Id.Name = "colCurrency2Id";
            // 
            // colFeeClearingCurrencyId
            // 
            colFeeClearingCurrencyId.FieldName = "FeeClearingCurrencyId";
            colFeeClearingCurrencyId.Name = "colFeeClearingCurrencyId";
            // 
            // colFeeCommissionCurrencyId
            // 
            colFeeCommissionCurrencyId.FieldName = "FeeCommissionCurrencyId";
            colFeeCommissionCurrencyId.Name = "colFeeCommissionCurrencyId";
            // 
            // colFeeExchangeCurrencyId
            // 
            colFeeExchangeCurrencyId.FieldName = "FeeExchangeCurrencyId";
            colFeeExchangeCurrencyId.Name = "colFeeExchangeCurrencyId";
            // 
            // colFeeNfaCurrencyId
            // 
            colFeeNfaCurrencyId.FieldName = "FeeNfaCurrencyId";
            colFeeNfaCurrencyId.Name = "colFeeNfaCurrencyId";
            // 
            // colFeeCashCurrencyId
            // 
            colFeeCashCurrencyId.FieldName = "FeeCashCurrencyId";
            colFeeCashCurrencyId.Name = "colFeeCashCurrencyId";
            // 
            // colFeeBlockCurrencyId
            // 
            colFeeBlockCurrencyId.FieldName = "FeeBlockCurrencyId";
            colFeeBlockCurrencyId.Name = "colFeeBlockCurrencyId";
            // 
            // colFeePlattsCurrencyId
            // 
            colFeePlattsCurrencyId.FieldName = "FeePlattsCurrencyId";
            colFeePlattsCurrencyId.Name = "colFeePlattsCurrencyId";
            // 
            // colIsEnabledRiskDecompositionDb
            // 
            colIsEnabledRiskDecompositionDb.FieldName = "IsEnabledRiskDecompositionDb";
            colIsEnabledRiskDecompositionDb.Name = "colIsEnabledRiskDecompositionDb";
            // 
            // colIsCalendarDaySwap
            // 
            colIsCalendarDaySwap.FieldName = "IsCalendarDaySwap";
            colIsCalendarDaySwap.Name = "colIsCalendarDaySwap";
            // 
            // colContractSizeMultiplierDb
            // 
            colContractSizeMultiplierDb.FieldName = "ContractSizeMultiplierDb";
            colContractSizeMultiplierDb.Name = "colContractSizeMultiplierDb";
            // 
            // colIceEquivalentProductDb
            // 
            colIceEquivalentProductDb.FieldName = "IceEquivalentProductDb";
            colIceEquivalentProductDb.Name = "colIceEquivalentProductDb";
            // 
            // colIceEquivalentUnderlyingProduct
            // 
            colIceEquivalentUnderlyingProduct.FieldName = "IceEquivalentUnderlyingProduct";
            colIceEquivalentUnderlyingProduct.Name = "colIceEquivalentUnderlyingProduct";
            // 
            // gridControl1
            // 
            gridControl1.DataSource = productBindingSource;
            gridControl1.Dock = DockStyle.Fill;
            gridControl1.Location = new Point(0, 0);
            gridControl1.MainView = gridView1;
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new Size(800, 428);
            gridControl1.TabIndex = 1;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(gridControl1);
            Controls.Add(statusStrip);
            Name = "Form1";
            Text = "ProductDemo 7";
            Load += Form1_Load;
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)productBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip;
        private ToolStripStatusLabel viewState;
        private BindingSource productBindingSource;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colholidays_calendar_id;
        private DevExpress.XtraGrid.Columns.GridColumn colIsDefault;
        private DevExpress.XtraGrid.Columns.GridColumn colcalendar_id;
        private DevExpress.XtraGrid.Columns.GridColumn colPnlFactor;
        private DevExpress.XtraGrid.Columns.GridColumn colProductTypeDb;
        private DevExpress.XtraGrid.Columns.GridColumn colPositionFactor;
        private DevExpress.XtraGrid.Columns.GridColumn colCategoryId;
        private DevExpress.XtraGrid.Columns.GridColumn colValidFrom;
        private DevExpress.XtraGrid.Columns.GridColumn colValidTo;
        private DevExpress.XtraGrid.Columns.GridColumn colOfficialProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colContractSize;
        private DevExpress.XtraGrid.Columns.GridColumn colBalmoOnCrudeProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colBalmoOnComplexProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colUnderlyingFuturesProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colUnderlyingFuturesOverrideId;
        private DevExpress.XtraGrid.Columns.GridColumn colExchangeContractCode;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeExchange;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeNfa;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeCommission;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeClearing;
        private DevExpress.XtraGrid.Columns.GridColumn colTimezoneId;
        private DevExpress.XtraGrid.Columns.GridColumn colRolloffTime;
        private DevExpress.XtraGrid.Columns.GridColumn colUseRolloffSettings;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeConversionFactor;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeCash;
        private DevExpress.XtraGrid.Columns.GridColumn colDefinitionLink;
        private DevExpress.XtraGrid.Columns.GridColumn colBalmoContractCode1;
        private DevExpress.XtraGrid.Columns.GridColumn colBalmoContractCode2;
        private DevExpress.XtraGrid.Columns.GridColumn colBalmoContractCode3;
        private DevExpress.XtraGrid.Columns.GridColumn colBalmoCodeFirstLetter;
        private DevExpress.XtraGrid.Columns.GridColumn colExpirationTypeDb;
        private DevExpress.XtraGrid.Columns.GridColumn colRollingMethodDb;
        private DevExpress.XtraGrid.Columns.GridColumn colGivenDate;
        private DevExpress.XtraGrid.Columns.GridColumn colExpirationMonth;
        private DevExpress.XtraGrid.Columns.GridColumn colNumberOfDays;
        private DevExpress.XtraGrid.Columns.GridColumn colExchangeId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeBlockTrade;
        private DevExpress.XtraGrid.Columns.GridColumn colFeePlattsTrade;
        private DevExpress.XtraGrid.Columns.GridColumn colFuturesExpireTime;
        private DevExpress.XtraGrid.Columns.GridColumn colUseExpiryCalendar;
        private DevExpress.XtraGrid.Columns.GridColumn colIsPhysicallySettledDb;
        private DevExpress.XtraGrid.Columns.GridColumn colIsInternalTransferProductDb;
        private DevExpress.XtraGrid.Columns.GridColumn colPriceConversionFactorDb;
        private DevExpress.XtraGrid.Columns.GridColumn colIsTasDb;
        private DevExpress.XtraGrid.Columns.GridColumn colPricingEndTime;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatTimespreadStripAsLegsDb;
        private DevExpress.XtraGrid.Columns.GridColumn colCalculatePnlFromLegs;
        private DevExpress.XtraGrid.Columns.GridColumn colIsMopsDb;
        private DevExpress.XtraGrid.Columns.GridColumn colIsMmDb;
        private DevExpress.XtraGrid.Columns.GridColumn colCategoryOverrideAt;
        private DevExpress.XtraGrid.Columns.GridColumn colCategoryOverrideId;
        private DevExpress.XtraGrid.Columns.GridColumn colTasOfficialProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colIsChanged;
        private DevExpress.XtraGrid.Columns.GridColumn colMonthlyOfficialProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colDailyDiffMonthShiftDb;
        private DevExpress.XtraGrid.Columns.GridColumn colDailyDiffMonthShift;
        private DevExpress.XtraGrid.Columns.GridColumn colPhysicalCode;
        private DevExpress.XtraGrid.Columns.GridColumn colUnitId;
        private DevExpress.XtraGrid.Columns.GridColumn colIsAllowedForManualTradesDb;
        private DevExpress.XtraGrid.Columns.GridColumn colCurrency1Id;
        private DevExpress.XtraGrid.Columns.GridColumn colCurrency2Id;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeClearingCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeCommissionCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeExchangeCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeNfaCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeCashCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeeBlockCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colFeePlattsCurrencyId;
        private DevExpress.XtraGrid.Columns.GridColumn colIsEnabledRiskDecompositionDb;
        private DevExpress.XtraGrid.Columns.GridColumn colIsCalendarDaySwap;
        private DevExpress.XtraGrid.Columns.GridColumn colContractSizeMultiplierDb;
        private DevExpress.XtraGrid.Columns.GridColumn colIceEquivalentProductDb;
        private DevExpress.XtraGrid.Columns.GridColumn colIceEquivalentUnderlyingProduct;
        private DevExpress.XtraGrid.GridControl gridControl1;
    }
}