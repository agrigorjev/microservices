namespace Mandara.ProductGUI.Calendars
{
    partial class HolidaysControl
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
            this.Load += new System.EventHandler(HolidaysControl_Load);
            this.gcHolidays = new DevExpress.XtraGrid.GridControl();
            this.gvHolidays = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHolidays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHolidays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // gcHolidays
            // 
            this.gcHolidays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcHolidays.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.EnabledAutoRepeat = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.First.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.gcHolidays.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.gcHolidays.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.gcHolidays.EmbeddedNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(this.gcHolidays_EmbeddedNavigator_ButtonClick);
            this.gcHolidays.Location = new System.Drawing.Point(0, 0);
            this.gcHolidays.MainView = this.gvHolidays;
            this.gcHolidays.Name = "gcHolidays";
            this.gcHolidays.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1});
            this.gcHolidays.Size = new System.Drawing.Size(975, 353);
            this.gcHolidays.TabIndex = 0;
            this.gcHolidays.UseEmbeddedNavigator = true;
            this.gcHolidays.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHolidays});
            // 
            // gvHolidays
            // 
            this.gvHolidays.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gvHolidays.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcName});
            this.gvHolidays.GridControl = this.gcHolidays;
            this.gvHolidays.Name = "gvHolidays";
            this.gvHolidays.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gvHolidays.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gvHolidays.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
            this.gvHolidays.OptionsCustomization.AllowColumnMoving = false;
            this.gvHolidays.OptionsCustomization.AllowColumnResizing = false;
            this.gvHolidays.OptionsCustomization.AllowFilter = false;
            this.gvHolidays.OptionsCustomization.AllowGroup = false;
            this.gvHolidays.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvHolidays.OptionsCustomization.AllowSort = false;
            this.gvHolidays.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvHolidays.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gvHolidays.OptionsView.ShowGroupPanel = false;
            this.gvHolidays.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gvHolidays_ValidateRow);
            this.gvHolidays.DoubleClick += new System.EventHandler(this.gvHolidays_DoubleClick);
            // 
            // gcName
            // 
            this.gcName.Caption = "Calendar";
            this.gcName.ColumnEdit = this.repositoryItemTextEdit1;
            this.gcName.FieldName = "NAME";
            this.gcName.Name = "gcName";
            this.gcName.OptionsColumn.AllowMove = false;
            this.gcName.OptionsColumn.AllowShowHide = false;
            this.gcName.Visible = true;
            this.gcName.VisibleIndex = 0;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // HolidaysControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gcHolidays);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "HolidaysControl";
            this.Size = new System.Drawing.Size(975, 353);
            ((System.ComponentModel.ISupportInitialize)(this.gcHolidays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHolidays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcHolidays;
        private DevExpress.XtraGrid.Views.Grid.GridView gvHolidays;
        private DevExpress.XtraGrid.Columns.GridColumn gcolName;
        private DevExpress.XtraGrid.Columns.GridColumn gcName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
    }
}
