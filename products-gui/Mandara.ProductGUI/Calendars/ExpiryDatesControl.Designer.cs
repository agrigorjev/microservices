namespace Mandara.ProductGUI.Calendars
{
    partial class ExpiryDatesControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraGrid.GridFormatRule gridFormatRule1 = new DevExpress.XtraGrid.GridFormatRule();
            this.expiryDates = new DevExpress.XtraGrid.GridControl();
            this.expiryDatesDisplay = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.productName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.productNameValue = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.dateOffset = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dateOffsetSelector = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryDates)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryDatesDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productNameValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateOffsetSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // expiryDates
            // 
            this.expiryDates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expiryDates.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.EnabledAutoRepeat = false;
            this.expiryDates.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.First.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.expiryDates.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.expiryDates.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.expiryDates.EmbeddedNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(this.OnCalendarAddOrRemove);
            this.expiryDates.Location = new System.Drawing.Point(0, 0);
            this.expiryDates.MainView = this.expiryDatesDisplay;
            this.expiryDates.Name = "expiryDates";
            this.expiryDates.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.productNameValue,
            this.dateOffsetSelector});
            this.expiryDates.Size = new System.Drawing.Size(959, 448);
            this.expiryDates.TabIndex = 1;
            this.expiryDates.UseEmbeddedNavigator = true;
            this.expiryDates.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.expiryDatesDisplay});
            // 
            // expiryDatesDisplay
            // 
            this.expiryDatesDisplay.Appearance.FocusedRow.Options.UseBackColor = true;
            this.expiryDatesDisplay.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.expiryDatesDisplay.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.productName,
            this.dateOffset});
            gridFormatRule1.Name = "Format0";
            gridFormatRule1.Rule = null;
            this.expiryDatesDisplay.FormatRules.Add(gridFormatRule1);
            this.expiryDatesDisplay.GridControl = this.expiryDates;
            this.expiryDatesDisplay.Name = "expiryDatesDisplay";
            this.expiryDatesDisplay.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.expiryDatesDisplay.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.expiryDatesDisplay.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
            this.expiryDatesDisplay.OptionsCustomization.AllowColumnMoving = false;
            this.expiryDatesDisplay.OptionsCustomization.AllowColumnResizing = false;
            this.expiryDatesDisplay.OptionsCustomization.AllowFilter = false;
            this.expiryDatesDisplay.OptionsCustomization.AllowGroup = false;
            this.expiryDatesDisplay.OptionsCustomization.AllowQuickHideColumns = false;
            this.expiryDatesDisplay.OptionsCustomization.AllowSort = false;
            this.expiryDatesDisplay.OptionsNavigation.AutoFocusNewRow = true;
            this.expiryDatesDisplay.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.expiryDatesDisplay.OptionsSelection.EnableAppearanceHideSelection = false;
            this.expiryDatesDisplay.OptionsView.ShowGroupPanel = false;
            this.expiryDatesDisplay.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.ExpiryDatesDisplay_InitNewRow);
            this.expiryDatesDisplay.ShownEditor += new System.EventHandler(this.ExpiryDateEditorShown);
            this.expiryDatesDisplay.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.OnExpiryDateRowValidate);
            // 
            // productName
            // 
            this.productName.Caption = "Calendar";
            this.productName.ColumnEdit = this.productNameValue;
            this.productName.FieldName = "NAME";
            this.productName.Name = "productName";
            this.productName.OptionsColumn.AllowMove = false;
            this.productName.OptionsColumn.AllowShowHide = false;
            this.productName.Visible = true;
            this.productName.VisibleIndex = 0;
            this.productName.Width = 100;
            // 
            // productNameValue
            // 
            this.productNameValue.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.productNameValue.AutoHeight = false;
            this.productNameValue.Name = "productNameValue";
            this.productNameValue.Validating += new System.ComponentModel.CancelEventHandler(this.OnProductNameValidating);
            this.productNameValue.EditValueChanged += this.OnProductNameChanged;
            // 
            // dateOffset
            // 
            this.dateOffset.Caption = "Correction";
            this.dateOffset.ColumnEdit = this.dateOffsetSelector;
            this.dateOffset.FieldName = "CORRECTION";
            this.dateOffset.Name = "dateOffset";
            this.dateOffset.Visible = true;
            this.dateOffset.VisibleIndex = 1;
            this.dateOffset.Width = 40;
            // 
            // dateOffsetSelector
            // 
            this.dateOffsetSelector.AutoHeight = false;
            this.dateOffsetSelector.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dateOffsetSelector.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.dateOffsetSelector.IsFloatValue = false;
            this.dateOffsetSelector.Mask.EditMask = "N00";
            this.dateOffsetSelector.Name = "dateOffsetSelector";
            // 
            // ExpiryDatesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.expiryDates);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ExpiryDatesControl";
            this.Size = new System.Drawing.Size(959, 448);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.expiryDates)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expiryDatesDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productNameValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateOffsetSelector)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl expiryDates;
        private DevExpress.XtraGrid.Views.Grid.GridView expiryDatesDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn productName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit productNameValue;
        private DevExpress.XtraGrid.Columns.GridColumn dateOffset;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit dateOffsetSelector;
    }
}
