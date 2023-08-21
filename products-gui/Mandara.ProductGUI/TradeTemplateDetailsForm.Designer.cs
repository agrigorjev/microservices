namespace Mandara.ProductGUI
{
    partial class TradeTemplateDetailsForm
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cmbUnits = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cmbOfficialProducts = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cmbExchange = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.leBook = new DevExpress.XtraEditors.PopupContainerEdit();
            this.popupBook = new DevExpress.XtraEditors.PopupContainerControl();
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtVolume = new DevExpress.XtraEditors.TextEdit();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbUnits.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbOfficialProducts.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExchange.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupBook)).BeginInit();
            this.popupBook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVolume.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(59, 15);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(27, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(92, 12);
            this.txtName.Name = "txtName";
            this.txtName.Properties.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(196, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TabStop = false;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(213, 154);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(63, 41);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(23, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Book";
            // 
            // cmbUnits
            // 
            this.cmbUnits.Location = new System.Drawing.Point(228, 116);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbUnits.Size = new System.Drawing.Size(60, 20);
            this.cmbUnits.TabIndex = 4;
            this.cmbUnits.SelectedIndexChanged += new System.EventHandler(this.cmbUnits_SelectedIndexChanged);
            this.cmbUnits.Validating += new System.ComponentModel.CancelEventHandler(this.cmbUnits_Validating);
            // 
            // cmbOfficialProducts
            // 
            this.cmbOfficialProducts.Location = new System.Drawing.Point(92, 90);
            this.cmbOfficialProducts.Name = "cmbOfficialProducts";
            this.cmbOfficialProducts.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbOfficialProducts.Size = new System.Drawing.Size(196, 20);
            this.cmbOfficialProducts.TabIndex = 2;
            this.cmbOfficialProducts.SelectedIndexChanged += new System.EventHandler(this.cmbOfficialProducts_SelectedIndexChanged);
            this.cmbOfficialProducts.Validating += new System.ComponentModel.CancelEventHandler(this.cmbOfficialProducts_Validating);
            // 
            // cmbExchange
            // 
            this.cmbExchange.Location = new System.Drawing.Point(92, 64);
            this.cmbExchange.Name = "cmbExchange";
            this.cmbExchange.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbExchange.Size = new System.Drawing.Size(196, 20);
            this.cmbExchange.TabIndex = 1;
            this.cmbExchange.SelectedIndexChanged += new System.EventHandler(this.cmbExchange_SelectedIndexChanged);
            this.cmbExchange.Validating += new System.ComponentModel.CancelEventHandler(this.cmbExchange_Validating);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(39, 67);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(47, 13);
            this.labelControl4.TabIndex = 9;
            this.labelControl4.Text = "Exchange";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(13, 93);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(73, 13);
            this.labelControl5.TabIndex = 10;
            this.labelControl5.Text = "Official Product";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(52, 119);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(34, 13);
            this.labelControl3.TabIndex = 12;
            this.labelControl3.Text = "Volume";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(203, 119);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(19, 13);
            this.labelControl6.TabIndex = 13;
            this.labelControl6.Text = "Unit";
            // 
            // leBook
            // 
            this.leBook.Location = new System.Drawing.Point(92, 38);
            this.leBook.Name = "leBook";
            this.leBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.leBook.Properties.PopupControl = this.popupBook;
            this.leBook.Size = new System.Drawing.Size(196, 20);
            this.leBook.TabIndex = 0;
            this.leBook.Validating += new System.ComponentModel.CancelEventHandler(this.leBook_Validating);
            // 
            // popupBook
            // 
            this.popupBook.Controls.Add(this.treeList1);
            this.popupBook.Location = new System.Drawing.Point(20, 165);
            this.popupBook.Name = "popupBook";
            this.popupBook.Size = new System.Drawing.Size(272, 267);
            this.popupBook.TabIndex = 116;
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1});
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.KeyFieldName = "PortfolioId";
            this.treeList1.Location = new System.Drawing.Point(0, 0);
            this.treeList1.Name = "treeList1";
            this.treeList1.OptionsBehavior.AutoPopulateColumns = false;
            this.treeList1.OptionsBehavior.Editable = false;
            this.treeList1.OptionsPrint.UsePrintStyles = true;
            this.treeList1.OptionsView.ShowColumns = false;
            this.treeList1.OptionsView.ShowHorzLines = false;
            this.treeList1.OptionsView.ShowIndicator = false;
            this.treeList1.OptionsView.ShowVertLines = false;
            this.treeList1.ParentFieldName = "ParentPortfolioId";
            this.treeList1.RootValue = -1;
            this.treeList1.Size = new System.Drawing.Size(272, 267);
            this.treeList1.TabIndex = 0;
            this.treeList1.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList1_FocusedNodeChanged);
            this.treeList1.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList1_CustomDrawNodeCell);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Book Name";
            this.treeListColumn1.FieldName = "Name";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.OptionsColumn.AllowMove = false;
            this.treeListColumn1.OptionsColumn.AllowSort = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(132, 154);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtVolume
            // 
            this.txtVolume.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtVolume.Location = new System.Drawing.Point(92, 116);
            this.txtVolume.Name = "txtVolume";
            this.txtVolume.Properties.Appearance.Options.UseTextOptions = true;
            this.txtVolume.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtVolume.Properties.AppearanceDisabled.Options.UseTextOptions = true;
            this.txtVolume.Properties.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtVolume.Properties.AppearanceFocused.Options.UseTextOptions = true;
            this.txtVolume.Properties.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtVolume.Properties.AppearanceReadOnly.Options.UseTextOptions = true;
            this.txtVolume.Properties.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtVolume.Properties.DisplayFormat.FormatString = "f2";
            this.txtVolume.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtVolume.Properties.EditFormat.FormatString = "f2";
            this.txtVolume.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtVolume.Properties.Mask.EditMask = "f2";
            this.txtVolume.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtVolume.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtVolume.Size = new System.Drawing.Size(97, 20);
            this.txtVolume.TabIndex = 3;
            this.txtVolume.EditValueChanged += new System.EventHandler(this.txtVolume_EditValueChanged);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // TradeTemplateDetailsForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(304, 189);
            this.Controls.Add(this.txtVolume);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.popupBook);
            this.Controls.Add(this.leBook);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.cmbExchange);
            this.Controls.Add(this.cmbOfficialProducts);
            this.Controls.Add(this.cmbUnits);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.labelControl1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(320, 227);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 227);
            this.Name = "TradeTemplateDetailsForm";
            this.Text = "Trade Template";
            this.Load += new System.EventHandler(this.TradeTemplateDetailsForm_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.TradeTemplateDetailsForm_Validating);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbUnits.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbOfficialProducts.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExchange.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupBook)).EndInit();
            this.popupBook.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVolume.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ComboBoxEdit cmbUnits;
        private DevExpress.XtraEditors.ComboBoxEdit cmbOfficialProducts;
        private DevExpress.XtraEditors.ComboBoxEdit cmbExchange;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.PopupContainerEdit leBook;
        private DevExpress.XtraEditors.PopupContainerControl popupBook;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.TextEdit txtVolume;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
    }
}