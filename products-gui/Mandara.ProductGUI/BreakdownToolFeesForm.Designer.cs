namespace Mandara.ProductGUI
{
    partial class BreakdownToolFeesForm
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
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.tePrimeBroker = new DevExpress.XtraEditors.TextEdit();
            this.teExchange = new DevExpress.XtraEditors.TextEdit();
            this.teNfa = new DevExpress.XtraEditors.TextEdit();
            this.teClearing = new DevExpress.XtraEditors.TextEdit();
            this.teBlock = new DevExpress.XtraEditors.TextEdit();
            this.teCash = new DevExpress.XtraEditors.TextEdit();
            this.tePlatts = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.tePrimeBroker.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teExchange.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teNfa.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teClearing.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teBlock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCash.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tePlatts.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(140, 208);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(13, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(64, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Prime broker:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(13, 39);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(51, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Exchange:";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(12, 65);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(24, 13);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "NFA:";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(12, 91);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(43, 13);
            this.labelControl4.TabIndex = 5;
            this.labelControl4.Text = "Clearing:";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(11, 117);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(28, 13);
            this.labelControl5.TabIndex = 6;
            this.labelControl5.Text = "Block:";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(12, 143);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(28, 13);
            this.labelControl6.TabIndex = 7;
            this.labelControl6.Text = "Cash:";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(11, 169);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(31, 13);
            this.labelControl7.TabIndex = 8;
            this.labelControl7.Text = "Platts:";
            // 
            // tePrimeBroker
            // 
            this.tePrimeBroker.Location = new System.Drawing.Point(83, 10);
            this.tePrimeBroker.Name = "tePrimeBroker";
            this.tePrimeBroker.Properties.Mask.EditMask = "f4";
            this.tePrimeBroker.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.tePrimeBroker.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.tePrimeBroker.Properties.ReadOnly = true;
            this.tePrimeBroker.Size = new System.Drawing.Size(132, 20);
            this.tePrimeBroker.TabIndex = 16;
            // 
            // teExchange
            // 
            this.teExchange.Location = new System.Drawing.Point(83, 36);
            this.teExchange.Name = "teExchange";
            this.teExchange.Properties.Mask.EditMask = "f4";
            this.teExchange.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.teExchange.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.teExchange.Properties.ReadOnly = true;
            this.teExchange.Size = new System.Drawing.Size(132, 20);
            this.teExchange.TabIndex = 17;
            // 
            // teNfa
            // 
            this.teNfa.Location = new System.Drawing.Point(83, 62);
            this.teNfa.Name = "teNfa";
            this.teNfa.Properties.Mask.EditMask = "f4";
            this.teNfa.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.teNfa.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.teNfa.Properties.ReadOnly = true;
            this.teNfa.Size = new System.Drawing.Size(132, 20);
            this.teNfa.TabIndex = 18;
            // 
            // teClearing
            // 
            this.teClearing.Location = new System.Drawing.Point(83, 88);
            this.teClearing.Name = "teClearing";
            this.teClearing.Properties.Mask.EditMask = "f4";
            this.teClearing.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.teClearing.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.teClearing.Properties.ReadOnly = true;
            this.teClearing.Size = new System.Drawing.Size(132, 20);
            this.teClearing.TabIndex = 19;
            // 
            // teBlock
            // 
            this.teBlock.Location = new System.Drawing.Point(83, 114);
            this.teBlock.Name = "teBlock";
            this.teBlock.Properties.Mask.EditMask = "f4";
            this.teBlock.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.teBlock.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.teBlock.Properties.ReadOnly = true;
            this.teBlock.Size = new System.Drawing.Size(132, 20);
            this.teBlock.TabIndex = 20;
            // 
            // teCash
            // 
            this.teCash.Location = new System.Drawing.Point(83, 140);
            this.teCash.Name = "teCash";
            this.teCash.Properties.Mask.EditMask = "f4";
            this.teCash.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.teCash.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.teCash.Properties.ReadOnly = true;
            this.teCash.Size = new System.Drawing.Size(132, 20);
            this.teCash.TabIndex = 21;
            // 
            // tePlatts
            // 
            this.tePlatts.Location = new System.Drawing.Point(83, 166);
            this.tePlatts.Name = "tePlatts";
            this.tePlatts.Properties.Mask.EditMask = "f4";
            this.tePlatts.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.tePlatts.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.tePlatts.Properties.ReadOnly = true;
            this.tePlatts.Size = new System.Drawing.Size(132, 20);
            this.tePlatts.TabIndex = 22;
            // 
            // BreakdownToolFeesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(227, 243);
            this.Controls.Add(this.tePlatts);
            this.Controls.Add(this.teCash);
            this.Controls.Add(this.teBlock);
            this.Controls.Add(this.teClearing);
            this.Controls.Add(this.teNfa);
            this.Controls.Add(this.teExchange);
            this.Controls.Add(this.tePrimeBroker);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BreakdownToolFeesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Product Fees";
            this.Load += new System.EventHandler(this.BreakdownToolFeesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tePrimeBroker.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teExchange.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teNfa.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teClearing.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teBlock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCash.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tePlatts.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit tePrimeBroker;
        private DevExpress.XtraEditors.TextEdit teExchange;
        private DevExpress.XtraEditors.TextEdit teNfa;
        private DevExpress.XtraEditors.TextEdit teClearing;
        private DevExpress.XtraEditors.TextEdit teBlock;
        private DevExpress.XtraEditors.TextEdit teCash;
        private DevExpress.XtraEditors.TextEdit tePlatts;

    }
}