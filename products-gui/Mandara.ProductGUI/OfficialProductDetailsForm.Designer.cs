namespace Mandara.ProductGUI
{
    partial class OfficialProductDetailsForm
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
            this.fullNameLabel = new DevExpress.XtraEditors.LabelControl();
            this.fullName = new DevExpress.XtraEditors.TextEdit();
            this.displayName = new DevExpress.XtraEditors.TextEdit();
            this.displayNameLabel = new DevExpress.XtraEditors.LabelControl();
            this.priceColumn = new DevExpress.XtraEditors.TextEdit();
            this.priceColumnLabel = new DevExpress.XtraEditors.LabelControl();
            this.productPropertiesGroup = new DevExpress.XtraEditors.GroupControl();
            this.isAllowedForManualTrades = new DevExpress.XtraEditors.CheckEdit();
            this.UnitToBblConversionLabel = new DevExpress.XtraEditors.LabelControl();
            this.priceUnitLabel = new DevExpress.XtraEditors.LabelControl();
            this.currencyLabel = new DevExpress.XtraEditors.LabelControl();
            this.currency = new DevExpress.XtraEditors.LookUpEdit();
            this.priceUnits = new DevExpress.XtraEditors.LookUpEdit();
            this.priceUnitToBblConversion = new DevExpress.XtraEditors.TextEdit();
            this.settlementProductLabel = new DevExpress.XtraEditors.LabelControl();
            this.settlementProduct = new DevExpress.XtraEditors.LookUpEdit();
            this.regionLabel = new DevExpress.XtraEditors.LabelControl();
            this.region = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.symbolEnabled = new DevExpress.XtraEditors.CheckEdit();
            this.plattsSymbolOrder = new DevExpress.XtraEditors.SpinEdit();
            this.settleOrderLabel = new DevExpress.XtraEditors.LabelControl();
            this.settleSymbolLabel = new DevExpress.XtraEditors.LabelControl();
            this.plattsSymbolName = new DevExpress.XtraEditors.TextEdit();
            this.saveOfficialProduct = new DevExpress.XtraEditors.SimpleButton();
            this.cancelChanges = new DevExpress.XtraEditors.SimpleButton();
            this.plattsSymbolGroup = new DevExpress.XtraEditors.GroupControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.plattsMul = new DevExpress.XtraEditors.SpinEdit();
            this.plattsDiv = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.fullName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.displayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceColumn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productPropertiesGroup)).BeginInit();
            this.productPropertiesGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.isAllowedForManualTrades.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currency.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceUnits.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceUnitToBblConversion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settlementProduct.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.region.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbolEnabled.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsSymbolOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsSymbolName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsSymbolGroup)).BeginInit();
            this.plattsSymbolGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plattsMul.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsDiv.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // fullNameLabel
            // 
            this.fullNameLabel.Location = new System.Drawing.Point(55, 36);
            this.fullNameLabel.Name = "fullNameLabel";
            this.fullNameLabel.Size = new System.Drawing.Size(46, 13);
            this.fullNameLabel.TabIndex = 0;
            this.fullNameLabel.Text = "Full Name";
            // 
            // fullName
            // 
            this.fullName.Location = new System.Drawing.Point(114, 32);
            this.fullName.Name = "fullName";
            this.fullName.Size = new System.Drawing.Size(147, 20);
            this.fullName.TabIndex = 1;
            this.fullName.Validating += new System.ComponentModel.CancelEventHandler(this.DoesOfficialProductNameExist);
            // 
            // displayName
            // 
            this.displayName.Location = new System.Drawing.Point(114, 66);
            this.displayName.Name = "displayName";
            this.displayName.Size = new System.Drawing.Size(147, 20);
            this.displayName.TabIndex = 3;
            this.displayName.Validating += new System.ComponentModel.CancelEventHandler(this.DoesOfficialProductNameExist);
            // 
            // displayNameLabel
            // 
            this.displayNameLabel.Location = new System.Drawing.Point(37, 70);
            this.displayNameLabel.Name = "displayNameLabel";
            this.displayNameLabel.Size = new System.Drawing.Size(64, 13);
            this.displayNameLabel.TabIndex = 2;
            this.displayNameLabel.Text = "Display Name";
            // 
            // priceColumn
            // 
            this.priceColumn.Location = new System.Drawing.Point(114, 101);
            this.priceColumn.Name = "priceColumn";
            this.priceColumn.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.priceColumn.Size = new System.Drawing.Size(147, 20);
            this.priceColumn.TabIndex = 5;
            // 
            // priceColumnLabel
            // 
            this.priceColumnLabel.Location = new System.Drawing.Point(40, 105);
            this.priceColumnLabel.Name = "priceColumnLabel";
            this.priceColumnLabel.Size = new System.Drawing.Size(61, 13);
            this.priceColumnLabel.TabIndex = 4;
            this.priceColumnLabel.Text = "Price Column";
            // 
            // productPropertiesGroup
            // 
            this.productPropertiesGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productPropertiesGroup.Controls.Add(this.isAllowedForManualTrades);
            this.productPropertiesGroup.Controls.Add(this.UnitToBblConversionLabel);
            this.productPropertiesGroup.Controls.Add(this.priceUnitLabel);
            this.productPropertiesGroup.Controls.Add(this.currencyLabel);
            this.productPropertiesGroup.Controls.Add(this.currency);
            this.productPropertiesGroup.Controls.Add(this.priceUnits);
            this.productPropertiesGroup.Controls.Add(this.priceUnitToBblConversion);
            this.productPropertiesGroup.Controls.Add(this.settlementProductLabel);
            this.productPropertiesGroup.Controls.Add(this.settlementProduct);
            this.productPropertiesGroup.Controls.Add(this.regionLabel);
            this.productPropertiesGroup.Controls.Add(this.region);
            this.productPropertiesGroup.Controls.Add(this.fullNameLabel);
            this.productPropertiesGroup.Controls.Add(this.fullName);
            this.productPropertiesGroup.Controls.Add(this.displayNameLabel);
            this.productPropertiesGroup.Controls.Add(this.priceColumn);
            this.productPropertiesGroup.Controls.Add(this.displayName);
            this.productPropertiesGroup.Controls.Add(this.priceColumnLabel);
            this.productPropertiesGroup.Location = new System.Drawing.Point(4, 5);
            this.productPropertiesGroup.Name = "productPropertiesGroup";
            this.productPropertiesGroup.Size = new System.Drawing.Size(275, 347);
            this.productPropertiesGroup.TabIndex = 7;
            this.productPropertiesGroup.Text = "Product Properties";
            // 
            // isAllowedForManualTrades
            // 
            this.isAllowedForManualTrades.Location = new System.Drawing.Point(68, 318);
            this.isAllowedForManualTrades.Name = "isAllowedForManualTrades";
            this.isAllowedForManualTrades.Properties.Caption = "Allowed for manual trades";
            this.isAllowedForManualTrades.Size = new System.Drawing.Size(146, 19);
            this.isAllowedForManualTrades.TabIndex = 8;
            // 
            // UnitToBblConversionLabel
            // 
            this.UnitToBblConversionLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
            this.UnitToBblConversionLabel.Location = new System.Drawing.Point(10, 283);
            this.UnitToBblConversionLabel.Name = "UnitToBblConversionLabel";
            this.UnitToBblConversionLabel.Size = new System.Drawing.Size(108, 13);
            this.UnitToBblConversionLabel.TabIndex = 19;
            this.UnitToBblConversionLabel.Text = "Price unit -> bbl factor";
            // 
            // priceUnitLabel
            // 
            this.priceUnitLabel.Location = new System.Drawing.Point(53, 249);
            this.priceUnitLabel.Name = "priceUnitLabel";
            this.priceUnitLabel.Size = new System.Drawing.Size(45, 13);
            this.priceUnitLabel.TabIndex = 18;
            this.priceUnitLabel.Text = "Price Unit";
            // 
            // currencyLabel
            // 
            this.currencyLabel.Location = new System.Drawing.Point(57, 215);
            this.currencyLabel.Name = "currencyLabel";
            this.currencyLabel.Size = new System.Drawing.Size(44, 13);
            this.currencyLabel.TabIndex = 17;
            this.currencyLabel.Text = "Currency";
            // 
            // currency
            // 
            this.currency.Location = new System.Drawing.Point(114, 211);
            this.currency.Name = "currency";
            this.currency.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.currency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.currency.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsoName", "Currency")});
            this.currency.Properties.NullText = "[Not Specified]";
            this.currency.Properties.ValueMember = "Instance";
            this.currency.Size = new System.Drawing.Size(147, 20);
            this.currency.TabIndex = 16;
            // 
            // priceUnits
            // 
            this.priceUnits.Location = new System.Drawing.Point(114, 245);
            this.priceUnits.Name = "priceUnits";
            this.priceUnits.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.priceUnits.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.priceUnits.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Price Unit")});
            this.priceUnits.Properties.DisplayMember = "Name";
            this.priceUnits.Properties.NullText = "[Not Specified]";
            this.priceUnits.Properties.PopupFormMinSize = new System.Drawing.Size(127, 0);
            this.priceUnits.Properties.PopupSizeable = false;
            this.priceUnits.Properties.UseDropDownRowsAsMaxCount = true;
            this.priceUnits.Properties.ValueMember = "Instance";
            this.priceUnits.Size = new System.Drawing.Size(147, 20);
            this.priceUnits.TabIndex = 16;
            // 
            // priceUnitToBblConversion
            // 
            this.priceUnitToBblConversion.Location = new System.Drawing.Point(124, 279);
            this.priceUnitToBblConversion.Name = "priceUnitToBblConversion";
            this.priceUnitToBblConversion.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.priceUnitToBblConversion.Properties.Mask.EditMask = "(0\\.[0-9]*?[1-9]+[0-9]*)|([1-9]+[0-9]*?\\.[0-9]+)";
            this.priceUnitToBblConversion.Properties.Mask.IgnoreMaskBlank = false;
            this.priceUnitToBblConversion.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.priceUnitToBblConversion.Properties.Mask.ShowPlaceHolders = false;
            this.priceUnitToBblConversion.Size = new System.Drawing.Size(137, 20);
            this.priceUnitToBblConversion.TabIndex = 20;
            // 
            // settlementProductLabel
            // 
            this.settlementProductLabel.Location = new System.Drawing.Point(9, 181);
            this.settlementProductLabel.Name = "settlementProductLabel";
            this.settlementProductLabel.Size = new System.Drawing.Size(92, 13);
            this.settlementProductLabel.TabIndex = 15;
            this.settlementProductLabel.Text = "Settlement Product";
            // 
            // settlementProduct
            // 
            this.settlementProduct.Location = new System.Drawing.Point(114, 177);
            this.settlementProduct.Name = "settlementProduct";
            this.settlementProduct.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.settlementProduct.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.settlementProduct.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "Official Product")});
            this.settlementProduct.Properties.NullText = "[Not Specified]";
            this.settlementProduct.Properties.ValueMember = "Instance";
            this.settlementProduct.Size = new System.Drawing.Size(147, 20);
            this.settlementProduct.TabIndex = 14;
            // 
            // regionLabel
            // 
            this.regionLabel.Location = new System.Drawing.Point(68, 139);
            this.regionLabel.Name = "regionLabel";
            this.regionLabel.Size = new System.Drawing.Size(33, 13);
            this.regionLabel.TabIndex = 9;
            this.regionLabel.Text = "Region";
            // 
            // region
            // 
            this.region.Location = new System.Drawing.Point(114, 135);
            this.region.Name = "region";
            this.region.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.region.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.region.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Region")});
            this.region.Properties.NullText = "[Not Specified]";
            this.region.Properties.ValueMember = "Instance";
            this.region.Size = new System.Drawing.Size(147, 20);
            this.region.TabIndex = 8;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(29, 33);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(75, 13);
            this.labelControl1.TabIndex = 25;
            this.labelControl1.Text = "Symbol Enabled";
            // 
            // symbolEnabled
            // 
            this.symbolEnabled.Location = new System.Drawing.Point(177, 30);
            this.symbolEnabled.Name = "symbolEnabled";
            this.symbolEnabled.Properties.Caption = "";
            this.symbolEnabled.Size = new System.Drawing.Size(21, 19);
            this.symbolEnabled.TabIndex = 24;
            this.symbolEnabled.CheckedChanged += new System.EventHandler(this.symbolEnabled_CheckedChanged);
            // 
            // plattsSymbolOrder
            // 
            this.plattsSymbolOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.plattsSymbolOrder.Location = new System.Drawing.Point(114, 90);
            this.plattsSymbolOrder.Name = "plattsSymbolOrder";
            this.plattsSymbolOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.plattsSymbolOrder.Properties.IsFloatValue = false;
            this.plattsSymbolOrder.Properties.Mask.EditMask = "N00";
            this.plattsSymbolOrder.Size = new System.Drawing.Size(147, 20);
            this.plattsSymbolOrder.TabIndex = 23;
            // 
            // settleOrderLabel
            // 
            this.settleOrderLabel.Location = new System.Drawing.Point(37, 93);
            this.settleOrderLabel.Name = "settleOrderLabel";
            this.settleOrderLabel.Size = new System.Drawing.Size(65, 13);
            this.settleOrderLabel.TabIndex = 22;
            this.settleOrderLabel.Text = "Symbol Order";
            // 
            // settleSymbolLabel
            // 
            this.settleSymbolLabel.Location = new System.Drawing.Point(40, 61);
            this.settleSymbolLabel.Name = "settleSymbolLabel";
            this.settleSymbolLabel.Size = new System.Drawing.Size(64, 13);
            this.settleSymbolLabel.TabIndex = 13;
            this.settleSymbolLabel.Text = "Platts Symbol";
            // 
            // plattsSymbolName
            // 
            this.plattsSymbolName.Location = new System.Drawing.Point(114, 58);
            this.plattsSymbolName.Name = "plattsSymbolName";
            this.plattsSymbolName.Size = new System.Drawing.Size(147, 20);
            this.plattsSymbolName.TabIndex = 11;
            // 
            // saveOfficialProduct
            // 
            this.saveOfficialProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveOfficialProduct.Location = new System.Drawing.Point(412, 350);
            this.saveOfficialProduct.Name = "saveOfficialProduct";
            this.saveOfficialProduct.Size = new System.Drawing.Size(75, 23);
            this.saveOfficialProduct.TabIndex = 9;
            this.saveOfficialProduct.Text = "Save";
            this.saveOfficialProduct.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cancelChanges
            // 
            this.cancelChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelChanges.CausesValidation = false;
            this.cancelChanges.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelChanges.Location = new System.Drawing.Point(502, 350);
            this.cancelChanges.Name = "cancelChanges";
            this.cancelChanges.Size = new System.Drawing.Size(75, 23);
            this.cancelChanges.TabIndex = 10;
            this.cancelChanges.Text = "Cancel";
            this.cancelChanges.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // plattsSymbolGroup
            // 
            this.plattsSymbolGroup.Controls.Add(this.labelControl3);
            this.plattsSymbolGroup.Controls.Add(this.plattsMul);
            this.plattsSymbolGroup.Controls.Add(this.plattsDiv);
            this.plattsSymbolGroup.Controls.Add(this.labelControl2);
            this.plattsSymbolGroup.Controls.Add(this.labelControl1);
            this.plattsSymbolGroup.Controls.Add(this.symbolEnabled);
            this.plattsSymbolGroup.Controls.Add(this.plattsSymbolName);
            this.plattsSymbolGroup.Controls.Add(this.plattsSymbolOrder);
            this.plattsSymbolGroup.Controls.Add(this.settleSymbolLabel);
            this.plattsSymbolGroup.Controls.Add(this.settleOrderLabel);
            this.plattsSymbolGroup.Location = new System.Drawing.Point(295, 5);
            this.plattsSymbolGroup.Name = "plattsSymbolGroup";
            this.plattsSymbolGroup.Size = new System.Drawing.Size(275, 197);
            this.plattsSymbolGroup.TabIndex = 11;
            this.plattsSymbolGroup.Text = "Platts Symbol";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(60, 160);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(42, 13);
            this.labelControl3.TabIndex = 29;
            this.labelControl3.Text = "Multiplier";
            // 
            // plattsMul
            // 
            this.plattsMul.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.plattsMul.Location = new System.Drawing.Point(114, 157);
            this.plattsMul.Name = "plattsMul";
            this.plattsMul.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.plattsMul.Properties.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.plattsMul.Size = new System.Drawing.Size(147, 20);
            this.plattsMul.TabIndex = 28;
            // 
            // plattsDiv
            // 
            this.plattsDiv.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.plattsDiv.Location = new System.Drawing.Point(114, 120);
            this.plattsDiv.Name = "plattsDiv";
            this.plattsDiv.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.plattsDiv.Properties.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.plattsDiv.Size = new System.Drawing.Size(147, 20);
            this.plattsDiv.TabIndex = 27;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(69, 123);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(33, 13);
            this.labelControl2.TabIndex = 26;
            this.labelControl2.Text = "Divider";
            // 
            // OfficialProductDetailsForm
            // 
            this.AcceptButton = this.saveOfficialProduct;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelChanges;
            this.ClientSize = new System.Drawing.Size(583, 382);
            this.Controls.Add(this.plattsSymbolGroup);
            this.Controls.Add(this.cancelChanges);
            this.Controls.Add(this.saveOfficialProduct);
            this.Controls.Add(this.productPropertiesGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OfficialProductDetailsForm";
            this.Text = "Mandara: Official Product Details";
            this.Load += new System.EventHandler(this.OfficialProductDetailsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fullName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.displayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceColumn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productPropertiesGroup)).EndInit();
            this.productPropertiesGroup.ResumeLayout(false);
            this.productPropertiesGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.isAllowedForManualTrades.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currency.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceUnits.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceUnitToBblConversion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settlementProduct.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.region.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbolEnabled.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsSymbolOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsSymbolName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsSymbolGroup)).EndInit();
            this.plattsSymbolGroup.ResumeLayout(false);
            this.plattsSymbolGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plattsMul.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.plattsDiv.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl fullNameLabel;
        private DevExpress.XtraEditors.TextEdit fullName;
        private DevExpress.XtraEditors.TextEdit displayName;
        private DevExpress.XtraEditors.LabelControl displayNameLabel;
        private DevExpress.XtraEditors.TextEdit priceColumn;
        private DevExpress.XtraEditors.LabelControl priceColumnLabel;
        private DevExpress.XtraEditors.GroupControl productPropertiesGroup;
        private DevExpress.XtraEditors.SimpleButton saveOfficialProduct;
        private DevExpress.XtraEditors.SimpleButton cancelChanges;
        private DevExpress.XtraEditors.TextEdit priceUnitToBblConversion;
        private DevExpress.XtraEditors.LabelControl regionLabel;
        private DevExpress.XtraEditors.LookUpEdit region;
        private DevExpress.XtraEditors.LabelControl settleSymbolLabel;
        private DevExpress.XtraEditors.TextEdit plattsSymbolName;
        private DevExpress.XtraEditors.LabelControl settlementProductLabel;
        private DevExpress.XtraEditors.LookUpEdit settlementProduct;
        private DevExpress.XtraEditors.LabelControl currencyLabel;
        private DevExpress.XtraEditors.LookUpEdit currency;
        private DevExpress.XtraEditors.LookUpEdit priceUnits;
        private DevExpress.XtraEditors.CheckEdit isAllowedForManualTrades;
        private DevExpress.XtraEditors.LabelControl priceUnitLabel;
        private DevExpress.XtraEditors.LabelControl UnitToBblConversionLabel;
        private DevExpress.XtraEditors.SpinEdit plattsSymbolOrder;
        private DevExpress.XtraEditors.LabelControl settleOrderLabel;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit symbolEnabled;
        private DevExpress.XtraEditors.GroupControl plattsSymbolGroup;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SpinEdit plattsMul;
        private DevExpress.XtraEditors.SpinEdit plattsDiv;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}