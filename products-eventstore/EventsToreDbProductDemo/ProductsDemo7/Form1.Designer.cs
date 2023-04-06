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
            statusStrip = new StatusStrip();
            viewState = new ToolStripStatusLabel();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            colId = new DevExpress.XtraGrid.Columns.GridColumn();
            colName = new DevExpress.XtraGrid.Columns.GridColumn();
            colDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            colMappingColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            colApplySignVerification = new DevExpress.XtraGrid.Columns.GridColumn();
            colApplyFractionPartVerification = new DevExpress.XtraGrid.Columns.GridColumn();
            colEpsilon = new DevExpress.XtraGrid.Columns.GridColumn();
            colApplyMissingPointVerification = new DevExpress.XtraGrid.Columns.GridColumn();
            colMissingPointAccuracy = new DevExpress.XtraGrid.Columns.GridColumn();
            colVoiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            colPublishToUms = new DevExpress.XtraGrid.Columns.GridColumn();
            colNameOnUms = new DevExpress.XtraGrid.Columns.GridColumn();
            colUnitToBarrelConversionFactor = new DevExpress.XtraGrid.Columns.GridColumn();
            colprice_expiration_period = new DevExpress.XtraGrid.Columns.GridColumn();
            colspread_price_expiration_period = new DevExpress.XtraGrid.Columns.GridColumn();
            coldesk_id = new DevExpress.XtraGrid.Columns.GridColumn();
            colSettlementProductId = new DevExpress.XtraGrid.Columns.GridColumn();
            colIsAllowedForManualTradesDb = new DevExpress.XtraGrid.Columns.GridColumn();
            colCurrencyGuId = new DevExpress.XtraGrid.Columns.GridColumn();
            colRegionGuId = new DevExpress.XtraGrid.Columns.GridColumn();
            colUnitGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            colCurrency = new DevExpress.XtraGrid.Columns.GridColumn();
            colRegion = new DevExpress.XtraGrid.Columns.GridColumn();
            colPriceUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            statusStrip.SuspendLayout();
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
            // gridView1
            // 
            gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colId, colName, colDisplayName, colMappingColumn, colApplySignVerification, colApplyFractionPartVerification, colEpsilon, colApplyMissingPointVerification, colMissingPointAccuracy, colVoiceName, colPublishToUms, colNameOnUms, colUnitToBarrelConversionFactor, colprice_expiration_period, colspread_price_expiration_period, coldesk_id, colSettlementProductId, colIsAllowedForManualTradesDb, colCurrencyGuId, colRegionGuId, colUnitGuid, colCurrency, colRegion, colPriceUnit });
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // colId
            // 
            colId.FieldName = "Id";
            colId.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            colId.Name = "colId";
            colId.OptionsColumn.ReadOnly = true;
            colId.Visible = true;
            colId.VisibleIndex = 0;
            // 
            // colName
            // 
            colName.Caption = "Name";
            colName.FieldName = "Name";
            colName.Name = "colName";
            colName.Visible = true;
            colName.VisibleIndex = 1;
            // 
            // colDisplayName
            // 
            colDisplayName.Caption = "DisplayName";
            colDisplayName.FieldName = "DisplayName";
            colDisplayName.Name = "colDisplayName";
            colDisplayName.Visible = true;
            colDisplayName.VisibleIndex = 2;
            // 
            // colMappingColumn
            // 
            colMappingColumn.FieldName = "MappingColumn";
            colMappingColumn.Name = "colMappingColumn";
            colMappingColumn.Visible = true;
            colMappingColumn.VisibleIndex = 3;
            // 
            // colApplySignVerification
            // 
            colApplySignVerification.FieldName = "ApplySignVerification";
            colApplySignVerification.Name = "colApplySignVerification";
            colApplySignVerification.Visible = true;
            colApplySignVerification.VisibleIndex = 4;
            // 
            // colApplyFractionPartVerification
            // 
            colApplyFractionPartVerification.FieldName = "ApplyFractionPartVerification";
            colApplyFractionPartVerification.Name = "colApplyFractionPartVerification";
            colApplyFractionPartVerification.Visible = true;
            colApplyFractionPartVerification.VisibleIndex = 5;
            // 
            // colEpsilon
            // 
            colEpsilon.FieldName = "Epsilon";
            colEpsilon.Name = "colEpsilon";
            colEpsilon.Visible = true;
            colEpsilon.VisibleIndex = 6;
            // 
            // colApplyMissingPointVerification
            // 
            colApplyMissingPointVerification.FieldName = "ApplyMissingPointVerification";
            colApplyMissingPointVerification.Name = "colApplyMissingPointVerification";
            colApplyMissingPointVerification.Visible = true;
            colApplyMissingPointVerification.VisibleIndex = 7;
            // 
            // colMissingPointAccuracy
            // 
            colMissingPointAccuracy.FieldName = "MissingPointAccuracy";
            colMissingPointAccuracy.Name = "colMissingPointAccuracy";
            colMissingPointAccuracy.Visible = true;
            colMissingPointAccuracy.VisibleIndex = 8;
            // 
            // colVoiceName
            // 
            colVoiceName.FieldName = "VoiceName";
            colVoiceName.Name = "colVoiceName";
            colVoiceName.Visible = true;
            colVoiceName.VisibleIndex = 9;
            // 
            // colPublishToUms
            // 
            colPublishToUms.FieldName = "PublishToUms";
            colPublishToUms.Name = "colPublishToUms";
            colPublishToUms.Visible = true;
            colPublishToUms.VisibleIndex = 10;
            // 
            // colNameOnUms
            // 
            colNameOnUms.FieldName = "NameOnUms";
            colNameOnUms.Name = "colNameOnUms";
            colNameOnUms.Visible = true;
            colNameOnUms.VisibleIndex = 11;
            // 
            // colUnitToBarrelConversionFactor
            // 
            colUnitToBarrelConversionFactor.FieldName = "UnitToBarrelConversionFactor";
            colUnitToBarrelConversionFactor.Name = "colUnitToBarrelConversionFactor";
            colUnitToBarrelConversionFactor.Visible = true;
            colUnitToBarrelConversionFactor.VisibleIndex = 12;
            // 
            // colprice_expiration_period
            // 
            colprice_expiration_period.FieldName = "price_expiration_period";
            colprice_expiration_period.Name = "colprice_expiration_period";
            colprice_expiration_period.Visible = true;
            colprice_expiration_period.VisibleIndex = 13;
            // 
            // colspread_price_expiration_period
            // 
            colspread_price_expiration_period.FieldName = "spread_price_expiration_period";
            colspread_price_expiration_period.Name = "colspread_price_expiration_period";
            colspread_price_expiration_period.Visible = true;
            colspread_price_expiration_period.VisibleIndex = 14;
            // 
            // coldesk_id
            // 
            coldesk_id.FieldName = "desk_id";
            coldesk_id.Name = "coldesk_id";
            coldesk_id.Visible = true;
            coldesk_id.VisibleIndex = 15;
            // 
            // colSettlementProductId
            // 
            colSettlementProductId.FieldName = "SettlementProductId";
            colSettlementProductId.Name = "colSettlementProductId";
            colSettlementProductId.Visible = true;
            colSettlementProductId.VisibleIndex = 16;
            // 
            // colIsAllowedForManualTradesDb
            // 
            colIsAllowedForManualTradesDb.FieldName = "IsAllowedForManualTradesDb";
            colIsAllowedForManualTradesDb.Name = "colIsAllowedForManualTradesDb";
            colIsAllowedForManualTradesDb.Visible = true;
            colIsAllowedForManualTradesDb.VisibleIndex = 17;
            // 
            // colCurrencyGuId
            // 
            colCurrencyGuId.FieldName = "CurrencyGuId";
            colCurrencyGuId.Name = "colCurrencyGuId";
            // 
            // colRegionGuId
            // 
            colRegionGuId.FieldName = "RegionGuId";
            colRegionGuId.Name = "colRegionGuId";
            // 
            // colUnitGuid
            // 
            colUnitGuid.FieldName = "UnitGuid";
            colUnitGuid.Name = "colUnitGuid";
            // 
            // colCurrency
            // 
            colCurrency.FieldName = "Currency";
            colCurrency.Name = "colCurrency";
            colCurrency.Visible = true;
            colCurrency.VisibleIndex = 18;
            // 
            // colRegion
            // 
            colRegion.FieldName = "Region";
            colRegion.Name = "colRegion";
            colRegion.Visible = true;
            colRegion.VisibleIndex = 19;
            // 
            // colPriceUnit
            // 
            colPriceUnit.FieldName = "PriceUnit";
            colPriceUnit.Name = "colPriceUnit";
            colPriceUnit.Visible = true;
            colPriceUnit.VisibleIndex = 20;
            // 
            // gridControl1
            // 
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
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip;
        private ToolStripStatusLabel viewState;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn colMappingColumn;
        private DevExpress.XtraGrid.Columns.GridColumn colApplySignVerification;
        private DevExpress.XtraGrid.Columns.GridColumn colApplyFractionPartVerification;
        private DevExpress.XtraGrid.Columns.GridColumn colEpsilon;
        private DevExpress.XtraGrid.Columns.GridColumn colApplyMissingPointVerification;
        private DevExpress.XtraGrid.Columns.GridColumn colMissingPointAccuracy;
        private DevExpress.XtraGrid.Columns.GridColumn colVoiceName;
        private DevExpress.XtraGrid.Columns.GridColumn colPublishToUms;
        private DevExpress.XtraGrid.Columns.GridColumn colNameOnUms;
        private DevExpress.XtraGrid.Columns.GridColumn colUnitToBarrelConversionFactor;
        private DevExpress.XtraGrid.Columns.GridColumn colprice_expiration_period;
        private DevExpress.XtraGrid.Columns.GridColumn colspread_price_expiration_period;
        private DevExpress.XtraGrid.Columns.GridColumn coldesk_id;
        private DevExpress.XtraGrid.Columns.GridColumn colSettlementProductId;
        private DevExpress.XtraGrid.Columns.GridColumn colIsAllowedForManualTradesDb;
        private DevExpress.XtraGrid.Columns.GridColumn colCurrencyGuId;
        private DevExpress.XtraGrid.Columns.GridColumn colRegionGuId;
        private DevExpress.XtraGrid.Columns.GridColumn colUnitGuid;
        private DevExpress.XtraGrid.Columns.GridColumn colCurrency;
        private DevExpress.XtraGrid.Columns.GridColumn colRegion;
        private DevExpress.XtraGrid.Columns.GridColumn colPriceUnit;
    }
}