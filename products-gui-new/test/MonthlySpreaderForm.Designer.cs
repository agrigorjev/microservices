namespace Mandara.RiskMgmtTool.Spreader.MonthlySpreader
{
    partial class MonthlySpreaderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraGrid.StyleFormatCondition();
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition2 = new DevExpress.XtraGrid.StyleFormatCondition();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.manualPositions = new DevExpress.XtraGrid.GridControl();
            this.manualPositionsDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.configuredSwapQuantities = new DevExpress.XtraGrid.Columns.GridColumn();
            this.configuredFuturesQuantities = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productsAndCategoriesDisplay = new DevExpress.XtraEditors.PopupContainerControl();
            this.productsSelector = new DevExpress.XtraTreeList.TreeList();
            this.productName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.productSelected = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.portfoliosDisplay = new DevExpress.XtraEditors.PopupContainerControl();
            this.portfolioSelector = new DevExpress.XtraTreeList.TreeList();
            this.portfolioName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.calculatedSpreads = new DevExpress.XtraGrid.GridControl();
            this.calculatedSpreadsDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.futuresEquivalents = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.jalSpreads = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bookFuturesEquivalents = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.totalJalSpreads = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
            this.splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
            this.splitContainerControl1.Panel2.SuspendLayout();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.manualPositions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualPositionsDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsAndCategoriesDisplay)).BeginInit();
            this.productsAndCategoriesDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.productsSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.portfoliosDisplay)).BeginInit();
            this.portfoliosDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portfolioSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calculatedSpreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calculatedSpreadsDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            this.splitContainerControl1.Panel1.Controls.Add(this.manualPositions);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            this.splitContainerControl1.Panel2.Controls.Add(this.productsAndCategoriesDisplay);
            this.splitContainerControl1.Panel2.Controls.Add(this.portfoliosDisplay);
            this.splitContainerControl1.Panel2.Controls.Add(this.calculatedSpreads);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(999, 396);
            this.splitContainerControl1.SplitterPosition = 344;
            this.splitContainerControl1.TabIndex = 6;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // manualPositions
            // 
            this.manualPositions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manualPositions.Location = new System.Drawing.Point(0, 0);
            this.manualPositions.MainView = this.manualPositionsDisplay;
            this.manualPositions.Name = "manualPositions";
            this.manualPositions.Size = new System.Drawing.Size(344, 396);
            this.manualPositions.TabIndex = 0;
            this.manualPositions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.manualPositionsDisplay});
            // 
            // manualPositionsDisplay
            // 
            this.manualPositionsDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.configuredSwapQuantities,
            this.configuredFuturesQuantities});
            styleFormatCondition1.Appearance.ForeColor = System.Drawing.Color.Red;
            styleFormatCondition1.Appearance.Options.UseForeColor = true;
            styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Less;
            styleFormatCondition1.Value1 = "0";
            this.manualPositionsDisplay.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition1});
            this.manualPositionsDisplay.GridControl = this.manualPositions;
            this.manualPositionsDisplay.Name = "manualPositionsDisplay";
            this.manualPositionsDisplay.OptionsCustomization.AllowFilter = false;
            this.manualPositionsDisplay.OptionsCustomization.AllowGroup = false;
            this.manualPositionsDisplay.OptionsCustomization.AllowSort = false;
            this.manualPositionsDisplay.OptionsView.ShowGroupPanel = false;
            this.manualPositionsDisplay.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.manualPositionsDisplay_CellValueChanged);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Strips";
            this.gridColumn1.DisplayFormat.FormatString = "G";
            this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn1.FieldName = "Strip";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // configuredSwapQuantities
            // 
            this.configuredSwapQuantities.Caption = "Swaps";
            this.configuredSwapQuantities.DisplayFormat.FormatString = "N2";
            this.configuredSwapQuantities.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.configuredSwapQuantities.FieldName = "SwapAmount";
            this.configuredSwapQuantities.Name = "configuredSwapQuantities";
            this.configuredSwapQuantities.Visible = true;
            this.configuredSwapQuantities.VisibleIndex = 1;
            // 
            // configuredFuturesQuantities
            // 
            this.configuredFuturesQuantities.Caption = "Futures";
            this.configuredFuturesQuantities.DisplayFormat.FormatString = "N2";
            this.configuredFuturesQuantities.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.configuredFuturesQuantities.FieldName = "FuturesAmount";
            this.configuredFuturesQuantities.Name = "configuredFuturesQuantities";
            this.configuredFuturesQuantities.Visible = true;
            this.configuredFuturesQuantities.VisibleIndex = 2;
            // 
            // productsAndCategoriesDisplay
            // 
            this.productsAndCategoriesDisplay.Controls.Add(this.productsSelector);
            this.productsAndCategoriesDisplay.Location = new System.Drawing.Point(64, 15);
            this.productsAndCategoriesDisplay.Name = "productsAndCategoriesDisplay";
            this.productsAndCategoriesDisplay.Size = new System.Drawing.Size(272, 267);
            this.productsAndCategoriesDisplay.TabIndex = 104;
            // 
            // productsSelector
            // 
            this.productsSelector.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.productName});
            this.productsSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productsSelector.KeyFieldName = "Id";
            this.productsSelector.Location = new System.Drawing.Point(0, 0);
            this.productsSelector.Name = "productsSelector";
            this.productsSelector.OptionsBehavior.AutoPopulateColumns = false;
            this.productsSelector.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.productsSelector.OptionsView.ShowColumns = false;
            this.productsSelector.OptionsView.ShowHorzLines = false;
            this.productsSelector.OptionsView.ShowIndicator = false;
            this.productsSelector.OptionsView.ShowVertLines = false;
            this.productsSelector.ParentFieldName = "ParentCategoryId";
            this.productsSelector.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.productSelected});
            this.productsSelector.Size = new System.Drawing.Size(272, 267);
            this.productsSelector.TabIndex = 1;
            // 
            // productName
            // 
            this.productName.Caption = "Name";
            this.productName.FieldName = "Name";
            this.productName.MinWidth = 32;
            this.productName.Name = "productName";
            this.productName.OptionsColumn.AllowEdit = false;
            this.productName.OptionsColumn.AllowMove = false;
            this.productName.OptionsColumn.AllowSort = false;
            this.productName.Visible = true;
            this.productName.VisibleIndex = 0;
            this.productName.Width = 120;
            // 
            // productSelected
            // 
            this.productSelected.AutoHeight = false;
            this.productSelected.Name = "productSelected";
            // 
            // portfoliosDisplay
            // 
            this.portfoliosDisplay.Controls.Add(this.portfolioSelector);
            this.portfoliosDisplay.Location = new System.Drawing.Point(394, 15);
            this.portfoliosDisplay.Name = "portfoliosDisplay";
            this.portfoliosDisplay.Size = new System.Drawing.Size(272, 267);
            this.portfoliosDisplay.TabIndex = 103;
            // 
            // portfolioSelector
            // 
            this.portfolioSelector.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.portfolioName});
            this.portfolioSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portfolioSelector.KeyFieldName = "PortfolioId";
            this.portfolioSelector.Location = new System.Drawing.Point(0, 0);
            this.portfolioSelector.Name = "portfolioSelector";
            this.portfolioSelector.OptionsBehavior.AutoPopulateColumns = false;
            this.portfolioSelector.OptionsBehavior.Editable = false;
            this.portfolioSelector.OptionsView.ShowColumns = false;
            this.portfolioSelector.OptionsView.ShowHorzLines = false;
            this.portfolioSelector.OptionsView.ShowIndicator = false;
            this.portfolioSelector.OptionsView.ShowVertLines = false;
            this.portfolioSelector.ParentFieldName = "ParentPortfolioId";
            this.portfolioSelector.Size = new System.Drawing.Size(272, 267);
            this.portfolioSelector.TabIndex = 1;
            // 
            // portfolioName
            // 
            this.portfolioName.Caption = "Book Name";
            this.portfolioName.FieldName = "Name";
            this.portfolioName.Name = "portfolioName";
            this.portfolioName.OptionsColumn.AllowEdit = false;
            this.portfolioName.OptionsColumn.AllowMove = false;
            this.portfolioName.OptionsColumn.AllowSort = false;
            this.portfolioName.Visible = true;
            this.portfolioName.VisibleIndex = 0;
            // 
            // calculatedSpreads
            // 
            this.calculatedSpreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calculatedSpreads.Location = new System.Drawing.Point(0, 0);
            this.calculatedSpreads.MainView = this.calculatedSpreadsDisplay;
            this.calculatedSpreads.Name = "calculatedSpreads";
            this.calculatedSpreads.Size = new System.Drawing.Size(645, 396);
            this.calculatedSpreads.TabIndex = 1;
            this.calculatedSpreads.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.calculatedSpreadsDisplay});
            // 
            // calculatedSpreadsDisplay
            // 
            this.calculatedSpreadsDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.futuresEquivalents,
            this.gridColumn4,
            this.jalSpreads,
            this.bookFuturesEquivalents,
            this.gridColumn7,
            this.totalJalSpreads});
            styleFormatCondition2.Appearance.ForeColor = System.Drawing.Color.Red;
            styleFormatCondition2.Appearance.Options.UseForeColor = true;
            styleFormatCondition2.Condition = DevExpress.XtraGrid.FormatConditionEnum.Less;
            styleFormatCondition2.Value1 = "0";
            this.calculatedSpreadsDisplay.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition2});
            this.calculatedSpreadsDisplay.GridControl = this.calculatedSpreads;
            this.calculatedSpreadsDisplay.Name = "calculatedSpreadsDisplay";
            this.calculatedSpreadsDisplay.OptionsBehavior.Editable = false;
            this.calculatedSpreadsDisplay.OptionsCustomization.AllowFilter = false;
            this.calculatedSpreadsDisplay.OptionsCustomization.AllowGroup = false;
            this.calculatedSpreadsDisplay.OptionsCustomization.AllowSort = false;
            this.calculatedSpreadsDisplay.OptionsView.ShowGroupPanel = false;
            this.calculatedSpreadsDisplay.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.CalculatedSpreadsPositionsRowCellStyle);
            this.calculatedSpreadsDisplay.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.CalculatedSpreadsPositionsPopupMenuShowing);
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Months";
            this.gridColumn2.DisplayFormat.FormatString = "G";
            this.gridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn2.FieldName = "Month";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // futuresEquivalents
            // 
            this.futuresEquivalents.Caption = "Futures Equivalent";
            this.futuresEquivalents.DisplayFormat.FormatString = "N2";
            this.futuresEquivalents.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.futuresEquivalents.FieldName = "FuturesEquivalent";
            this.futuresEquivalents.Name = "futuresEquivalents";
            this.futuresEquivalents.Visible = true;
            this.futuresEquivalents.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "Spreads";
            this.gridColumn4.FieldName = "Spreads";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // jalSpreads
            // 
            this.jalSpreads.Caption = "JAL Spreads";
            this.jalSpreads.DisplayFormat.FormatString = "N2";
            this.jalSpreads.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.jalSpreads.FieldName = "JALSpreads";
            this.jalSpreads.Name = "jalSpreads";
            this.jalSpreads.Visible = true;
            this.jalSpreads.VisibleIndex = 3;
            // 
            // bookFuturesEquivalents
            // 
            this.bookFuturesEquivalents.Caption = "Book Futures Equivalent";
            this.bookFuturesEquivalents.DisplayFormat.FormatString = "N2";
            this.bookFuturesEquivalents.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.bookFuturesEquivalents.FieldName = "BookFuturesEquivalent";
            this.bookFuturesEquivalents.Name = "bookFuturesEquivalents";
            this.bookFuturesEquivalents.Visible = true;
            this.bookFuturesEquivalents.VisibleIndex = 4;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "Spreads";
            this.gridColumn7.FieldName = "Spreads";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 5;
            // 
            // totalJalSpreads
            // 
            this.totalJalSpreads.Caption = "Total JAL Spreads";
            this.totalJalSpreads.DisplayFormat.FormatString = "N2";
            this.totalJalSpreads.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.totalJalSpreads.FieldName = "TotalJALSpreads";
            this.totalJalSpreads.Name = "totalJalSpreads";
            this.totalJalSpreads.Visible = true;
            this.totalJalSpreads.VisibleIndex = 6;
            // 
            // MonthlySpreaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(999, 396);
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "MonthlySpreaderForm";
            this.Text = "SpreaderForm";
            this.Title = "SpreaderForm";
            this.Load += new System.EventHandler(this.MonthlySpreaderForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
            this.splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
            this.splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.manualPositions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualPositionsDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productsAndCategoriesDisplay)).EndInit();
            this.productsAndCategoriesDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.productsSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.portfoliosDisplay)).EndInit();
            this.portfoliosDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.portfolioSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calculatedSpreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calculatedSpreadsDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl manualPositions;
        private DevExpress.XtraGrid.Views.Grid.GridView manualPositionsDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn configuredSwapQuantities;
        private DevExpress.XtraGrid.Columns.GridColumn configuredFuturesQuantities;
        private DevExpress.XtraEditors.PopupContainerControl productsAndCategoriesDisplay;
        private DevExpress.XtraTreeList.TreeList productsSelector;
        private DevExpress.XtraTreeList.Columns.TreeListColumn productName;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit productSelected;
        private DevExpress.XtraEditors.PopupContainerControl portfoliosDisplay;
        private DevExpress.XtraTreeList.TreeList portfolioSelector;
        private DevExpress.XtraTreeList.Columns.TreeListColumn portfolioName;
        private DevExpress.XtraGrid.GridControl calculatedSpreads;
        private DevExpress.XtraGrid.Views.Grid.GridView calculatedSpreadsDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn futuresEquivalents;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn jalSpreads;
        private DevExpress.XtraGrid.Columns.GridColumn bookFuturesEquivalents;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn totalJalSpreads;
    }
}