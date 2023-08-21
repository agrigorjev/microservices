namespace Mandara.ProductGUI
{
    partial class GroupDetailsForm
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.cmbProducts = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.chkTasCheckRequired = new DevExpress.XtraEditors.CheckEdit();
            this.cmbSwapCrossProduct = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtAbbreviation = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.cmbTransferProducts = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.teTasCheckTime = new DevExpress.XtraEditors.TimeEdit();
            this.cmbSwapCrossBalmoProduct = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.gcSwapCrossPerProduct = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCategoryProduct = new DevExpress.XtraGrid.Columns.GridColumn();
            this.riCategoryProducts = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.colSwapCrossProduct = new DevExpress.XtraGrid.Columns.GridColumn();
            this.riSwapCrossProducts = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.txtDummy = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProducts.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTasCheckRequired.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSwapCrossProduct.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbbreviation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTransferProducts.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teTasCheckTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSwapCrossBalmoProduct.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcSwapCrossPerProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riCategoryProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riSwapCrossProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDummy.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(75, 18);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(63, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Group Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(144, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(236, 20);
            this.txtName.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(628, 265);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(719, 265);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbProducts
            // 
            this.cmbProducts.Location = new System.Drawing.Point(144, 70);
            this.cmbProducts.Name = "cmbProducts";
            this.cmbProducts.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbProducts.Properties.DropDownRows = 12;
            this.cmbProducts.Size = new System.Drawing.Size(236, 20);
            this.cmbProducts.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(12, 73);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(126, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Default Spreader Product:";
            // 
            // chkTasCheckRequired
            // 
            this.chkTasCheckRequired.Location = new System.Drawing.Point(142, 127);
            this.chkTasCheckRequired.Name = "chkTasCheckRequired";
            this.chkTasCheckRequired.Properties.Caption = "TAS Check Required";
            this.chkTasCheckRequired.Size = new System.Drawing.Size(129, 19);
            this.chkTasCheckRequired.TabIndex = 6;
            this.chkTasCheckRequired.CheckedChanged += new System.EventHandler(this.chkTasCheckRequired_CheckedChanged);
            // 
            // cmbSwapCrossProduct
            // 
            this.cmbSwapCrossProduct.Location = new System.Drawing.Point(150, 29);
            this.cmbSwapCrossProduct.Name = "cmbSwapCrossProduct";
            this.cmbSwapCrossProduct.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cmbSwapCrossProduct.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbSwapCrossProduct.Properties.DropDownRows = 12;
            this.cmbSwapCrossProduct.Size = new System.Drawing.Size(236, 20);
            this.cmbSwapCrossProduct.TabIndex = 5;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(44, 32);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(100, 13);
            this.labelControl4.TabIndex = 4;
            this.labelControl4.Text = "Swap Cross Product:";
            // 
            // txtAbbreviation
            // 
            this.txtAbbreviation.Location = new System.Drawing.Point(144, 41);
            this.txtAbbreviation.Name = "txtAbbreviation";
            this.txtAbbreviation.Size = new System.Drawing.Size(236, 20);
            this.txtAbbreviation.TabIndex = 1;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(73, 44);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(65, 13);
            this.labelControl5.TabIndex = 11;
            this.labelControl5.Text = "Abbreviation:";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(53, 102);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(85, 13);
            this.labelControl6.TabIndex = 12;
            this.labelControl6.Text = "Transfer Product:";
            // 
            // cmbTransferProducts
            // 
            this.cmbTransferProducts.Location = new System.Drawing.Point(144, 99);
            this.cmbTransferProducts.Name = "cmbTransferProducts";
            this.cmbTransferProducts.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cmbTransferProducts.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbTransferProducts.Properties.DropDownRows = 12;
            this.cmbTransferProducts.Size = new System.Drawing.Size(236, 20);
            this.cmbTransferProducts.TabIndex = 13;
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(58, 155);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(80, 13);
            this.labelControl7.TabIndex = 14;
            this.labelControl7.Text = "TAS Check Time:";
            // 
            // teTasCheckTime
            // 
            this.teTasCheckTime.EditValue = new System.DateTime(2013, 1, 23, 0, 0, 0, 0);
            this.teTasCheckTime.Location = new System.Drawing.Point(144, 152);
            this.teTasCheckTime.Name = "teTasCheckTime";
            this.teTasCheckTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.teTasCheckTime.Properties.Mask.EditMask = "HH:mm";
            this.teTasCheckTime.Size = new System.Drawing.Size(69, 20);
            this.teTasCheckTime.TabIndex = 15;
            // 
            // cmbSwapCrossBalmoProduct
            // 
            this.cmbSwapCrossBalmoProduct.Location = new System.Drawing.Point(150, 55);
            this.cmbSwapCrossBalmoProduct.Name = "cmbSwapCrossBalmoProduct";
            this.cmbSwapCrossBalmoProduct.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cmbSwapCrossBalmoProduct.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbSwapCrossBalmoProduct.Properties.DropDownRows = 12;
            this.cmbSwapCrossBalmoProduct.Size = new System.Drawing.Size(236, 20);
            this.cmbSwapCrossBalmoProduct.TabIndex = 16;
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(13, 58);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(131, 13);
            this.labelControl8.TabIndex = 17;
            this.labelControl8.Text = "Swap Cross Balmo Product:";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.gcSwapCrossPerProduct);
            this.groupControl1.Controls.Add(this.cmbSwapCrossProduct);
            this.groupControl1.Controls.Add(this.labelControl8);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.cmbSwapCrossBalmoProduct);
            this.groupControl1.Controls.Add(this.txtDummy);
            this.groupControl1.Location = new System.Drawing.Point(397, 15);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(397, 235);
            this.groupControl1.TabIndex = 18;
            this.groupControl1.Text = "Swap Cross";
            // 
            // gcSwapCrossPerProduct
            // 
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.Edit.Enabled = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.First.Enabled = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.First.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.Next.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.Prev.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.gcSwapCrossPerProduct.EmbeddedNavigator.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.None;
            this.gcSwapCrossPerProduct.Location = new System.Drawing.Point(13, 87);
            this.gcSwapCrossPerProduct.MainView = this.gridView1;
            this.gcSwapCrossPerProduct.Name = "gcSwapCrossPerProduct";
            this.gcSwapCrossPerProduct.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.riCategoryProducts,
            this.riSwapCrossProducts});
            this.gcSwapCrossPerProduct.Size = new System.Drawing.Size(373, 137);
            this.gcSwapCrossPerProduct.TabIndex = 18;
            this.gcSwapCrossPerProduct.UseEmbeddedNavigator = true;
            this.gcSwapCrossPerProduct.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCategoryProduct,
            this.colSwapCrossProduct});
            this.gridView1.GridControl = this.gcSwapCrossPerProduct;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // colCategoryProduct
            // 
            this.colCategoryProduct.Caption = "Category Product";
            this.colCategoryProduct.ColumnEdit = this.riCategoryProducts;
            this.colCategoryProduct.FieldName = "CategoryProduct";
            this.colCategoryProduct.Name = "colCategoryProduct";
            this.colCategoryProduct.Visible = true;
            this.colCategoryProduct.VisibleIndex = 0;
            // 
            // riCategoryProducts
            // 
            this.riCategoryProducts.AutoHeight = false;
            this.riCategoryProducts.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riCategoryProducts.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")});
            this.riCategoryProducts.DisplayMember = "Name";
            this.riCategoryProducts.Name = "riCategoryProducts";
            this.riCategoryProducts.NullText = "[Product is not specified]";
            this.riCategoryProducts.ValueMember = "Instance";
            // 
            // colSwapCrossProduct
            // 
            this.colSwapCrossProduct.Caption = "Swap Cross Balmo Product";
            this.colSwapCrossProduct.ColumnEdit = this.riSwapCrossProducts;
            this.colSwapCrossProduct.FieldName = "BalmoSwapCrossProduct";
            this.colSwapCrossProduct.Name = "colSwapCrossProduct";
            this.colSwapCrossProduct.Visible = true;
            this.colSwapCrossProduct.VisibleIndex = 1;
            // 
            // riSwapCrossProducts
            // 
            this.riSwapCrossProducts.AutoHeight = false;
            this.riSwapCrossProducts.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riSwapCrossProducts.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")});
            this.riSwapCrossProducts.DisplayMember = "Name";
            this.riSwapCrossProducts.Name = "riSwapCrossProducts";
            this.riSwapCrossProducts.NullText = "[Swap Cross Balmo Product is not specified]";
            this.riSwapCrossProducts.ValueMember = "Instance";
            // 
            // txtDummy
            // 
            this.txtDummy.Location = new System.Drawing.Point(23, 175);
            this.txtDummy.Name = "txtDummy";
            this.txtDummy.Size = new System.Drawing.Size(100, 20);
            this.txtDummy.TabIndex = 19;
            // 
            // GroupDetailsForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(810, 306);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.teTasCheckTime);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.cmbTransferProducts);
            this.Controls.Add(this.txtAbbreviation);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.chkTasCheckRequired);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.cmbProducts);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroupDetailsForm";
            this.Text = "Mandara: Group Details";
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProducts.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTasCheckRequired.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSwapCrossProduct.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbbreviation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTransferProducts.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teTasCheckTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSwapCrossBalmoProduct.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcSwapCrossPerProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riCategoryProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riSwapCrossProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDummy.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.ComboBoxEdit cmbProducts;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckEdit chkTasCheckRequired;
        private DevExpress.XtraEditors.ComboBoxEdit cmbSwapCrossProduct;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtAbbreviation;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTransferProducts;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TimeEdit teTasCheckTime;
        private DevExpress.XtraEditors.ComboBoxEdit cmbSwapCrossBalmoProduct;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gcSwapCrossPerProduct;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colCategoryProduct;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit riCategoryProducts;
        private DevExpress.XtraGrid.Columns.GridColumn colSwapCrossProduct;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit riSwapCrossProducts;
        private DevExpress.XtraEditors.TextEdit txtDummy;
    }
}