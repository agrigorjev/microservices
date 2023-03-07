﻿namespace Mandara.ProductGUI
{
    partial class DesksConfigurator
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.desksGrid = new DevExpress.XtraGrid.GridControl();
            this.desksView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.desksProductsGrid = new DevExpress.XtraGrid.GridControl();
            this.desksProductsView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.desksGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.desksView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.desksProductsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.desksProductsView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.desksGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.desksProductsGrid);
            this.splitContainer1.Size = new System.Drawing.Size(831, 425);
            this.splitContainer1.SplitterDistance = 277;
            this.splitContainer1.TabIndex = 0;
            // 
            // desksGrid
            // 
            this.desksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            this.desksGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.First.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.desksGrid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.desksGrid.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.desksGrid.Location = new System.Drawing.Point(0, 0);
            this.desksGrid.MainView = this.desksView;
            this.desksGrid.Name = "desksGrid";
            this.desksGrid.ShowOnlyPredefinedDetails = true;
            this.desksGrid.Size = new System.Drawing.Size(277, 425);
            this.desksGrid.TabIndex = 0;
            this.desksGrid.UseEmbeddedNavigator = true;
            this.desksGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.desksView});
            // 
            // desksView
            // 
            this.desksView.ActiveFilterEnabled = false;
            this.desksView.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.desksView.Appearance.FocusedRow.Options.UseFont = true;
            this.desksView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName});
            this.desksView.GridControl = this.desksGrid;
            this.desksView.Name = "desksView";
            this.desksView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.desksView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.desksView.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.desksView.OptionsBehavior.AutoUpdateTotalSummary = false;
            this.desksView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.EditFormInplaceHideCurrentRow;
            this.desksView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
            this.desksView.OptionsCustomization.AllowGroup = false;
            this.desksView.OptionsDetail.EnableMasterViewMode = false;
            this.desksView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.desksView.OptionsView.ShowGroupPanel = false;
            this.desksView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
            this.desksView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.OnDesksFocusedRowChanged);
            this.desksView.ValidateRow += OnDeskValidateRow;
            this.desksView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.OnDeskValidatingEditor);
            // 
            // colName
            // 
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsEditForm.StartNewRow = true;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            // 
            // desksProductsGrid
            // 
            this.desksProductsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.desksProductsGrid.Location = new System.Drawing.Point(0, 0);
            this.desksProductsGrid.MainView = this.desksProductsView;
            this.desksProductsGrid.Name = "desksProductsGrid";
            this.desksProductsGrid.Size = new System.Drawing.Size(550, 425);
            this.desksProductsGrid.TabIndex = 0;
            this.desksProductsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.desksProductsView});
            // 
            // desksProductsView
            // 
            this.desksProductsView.GridControl = this.desksProductsGrid;
            this.desksProductsView.Name = "desksProductsView";
            this.desksProductsView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.desksProductsView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.desksProductsView.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.desksProductsView.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.EditFormInplaceHideCurrentRow;
            this.desksProductsView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
            this.desksProductsView.OptionsDetail.EnableMasterViewMode = false;
            this.desksProductsView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.desksProductsView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.OnProductsFocusedRowChanged);
            this.desksProductsView.EditFormPrepared += this.OnProductEditFormPrepared;
            this.desksProductsView.ValidateRow += this.OnProductValidateRow;
            this.desksProductsView.ValidatingEditor += this.OnDeskProductValidatingEditor;
            // 
            // DesksConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DesksConfigurator";
            this.Size = new System.Drawing.Size(831, 425);
            this.Load += new System.EventHandler(this.DesksConfigurator_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.desksGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.desksView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.desksProductsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.desksProductsView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private DevExpress.XtraGrid.GridControl desksGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView desksView;
        private DevExpress.XtraGrid.GridControl desksProductsGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView desksProductsView;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
    }
}
