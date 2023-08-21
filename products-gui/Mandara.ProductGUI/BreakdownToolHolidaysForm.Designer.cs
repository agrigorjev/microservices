namespace Mandara.ProductGUI
{
    partial class BreakdownToolHolidaysForm
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
            this.lbLeg2Title = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.gcLegs = new DevExpress.XtraGrid.GridControl();
            this.gvLegs = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcLeg1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLeg2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.lbCalendarLeg2Title = new DevExpress.XtraEditors.LabelControl();
            this.lbLeg1 = new DevExpress.XtraEditors.LabelControl();
            this.lbLeg2 = new DevExpress.XtraEditors.LabelControl();
            this.lbCalendarLeg1 = new DevExpress.XtraEditors.LabelControl();
            this.lbCalendarLeg2 = new DevExpress.XtraEditors.LabelControl();
            this.lbCommonPricing = new DevExpress.XtraEditors.LabelControl();
            this.gcMonth = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLeg1Exp = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLeg2Exp = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcLegs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvLegs)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(13, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(30, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Leg 1:";
            // 
            // lbLeg2Title
            // 
            this.lbLeg2Title.Location = new System.Drawing.Point(13, 39);
            this.lbLeg2Title.Name = "lbLeg2Title";
            this.lbLeg2Title.Size = new System.Drawing.Size(30, 13);
            this.lbLeg2Title.TabIndex = 1;
            this.lbLeg2Title.Text = "Leg 2:";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(13, 65);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(79, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Common Pricing:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(305, 488);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            // 
            // gcLegs
            // 
            this.gcLegs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gcLegs.Location = new System.Drawing.Point(13, 92);
            this.gcLegs.MainView = this.gvLegs;
            this.gcLegs.Name = "gcLegs";
            this.gcLegs.Size = new System.Drawing.Size(367, 378);
            this.gcLegs.TabIndex = 4;
            this.gcLegs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvLegs});
            // 
            // gvLegs
            // 
            this.gvLegs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcLeg1,
            this.gcLeg2,
            this.gcMonth,
            this.gcLeg1Exp,
            this.gcLeg2Exp});
            this.gvLegs.GridControl = this.gcLegs;
            this.gvLegs.Name = "gvLegs";
            this.gvLegs.OptionsBehavior.Editable = false;
            this.gvLegs.OptionsView.ShowGroupPanel = false;
            // 
            // gcLeg1
            // 
            this.gcLeg1.Caption = "Leg 1";
            this.gcLeg1.FieldName = "Leg1Holiday";
            this.gcLeg1.Name = "gcLeg1";
            this.gcLeg1.Visible = true;
            this.gcLeg1.VisibleIndex = 0;
            // 
            // gcLeg2
            // 
            this.gcLeg2.Caption = "Leg 2";
            this.gcLeg2.FieldName = "Leg2Holiday";
            this.gcLeg2.Name = "gcLeg2";
            this.gcLeg2.Visible = true;
            this.gcLeg2.VisibleIndex = 1;
            // 
            // labelControl4
            // 
            this.labelControl4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl4.Location = new System.Drawing.Point(192, 12);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(47, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Calendar:";
            // 
            // lbCalendarLeg2Title
            // 
            this.lbCalendarLeg2Title.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCalendarLeg2Title.Location = new System.Drawing.Point(192, 39);
            this.lbCalendarLeg2Title.Name = "lbCalendarLeg2Title";
            this.lbCalendarLeg2Title.Size = new System.Drawing.Size(47, 13);
            this.lbCalendarLeg2Title.TabIndex = 7;
            this.lbCalendarLeg2Title.Text = "Calendar:";
            // 
            // lbLeg1
            // 
            this.lbLeg1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLeg1.Location = new System.Drawing.Point(50, 12);
            this.lbLeg1.MaximumSize = new System.Drawing.Size(120, 13);
            this.lbLeg1.Name = "lbLeg1";
            this.lbLeg1.Size = new System.Drawing.Size(0, 13);
            this.lbLeg1.TabIndex = 8;
            // 
            // lbLeg2
            // 
            this.lbLeg2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLeg2.Location = new System.Drawing.Point(50, 39);
            this.lbLeg2.MaximumSize = new System.Drawing.Size(120, 13);
            this.lbLeg2.Name = "lbLeg2";
            this.lbLeg2.Size = new System.Drawing.Size(0, 13);
            this.lbLeg2.TabIndex = 9;
            // 
            // lbCalendarLeg1
            // 
            this.lbCalendarLeg1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCalendarLeg1.Location = new System.Drawing.Point(246, 13);
            this.lbCalendarLeg1.MaximumSize = new System.Drawing.Size(120, 13);
            this.lbCalendarLeg1.Name = "lbCalendarLeg1";
            this.lbCalendarLeg1.Size = new System.Drawing.Size(0, 13);
            this.lbCalendarLeg1.TabIndex = 10;
            // 
            // lbCalendarLeg2
            // 
            this.lbCalendarLeg2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCalendarLeg2.Location = new System.Drawing.Point(246, 39);
            this.lbCalendarLeg2.MaximumSize = new System.Drawing.Size(120, 13);
            this.lbCalendarLeg2.Name = "lbCalendarLeg2";
            this.lbCalendarLeg2.Size = new System.Drawing.Size(0, 13);
            this.lbCalendarLeg2.TabIndex = 11;
            // 
            // lbCommonPricing
            // 
            this.lbCommonPricing.Location = new System.Drawing.Point(98, 65);
            this.lbCommonPricing.Name = "lbCommonPricing";
            this.lbCommonPricing.Size = new System.Drawing.Size(25, 13);
            this.lbCommonPricing.TabIndex = 12;
            this.lbCommonPricing.Text = "False";
            // 
            // gcMonth
            // 
            this.gcMonth.Caption = "Month";
            this.gcMonth.FieldName = "ExpirationMonth";
            this.gcMonth.Name = "gcMonth";
            this.gcMonth.Visible = true;
            this.gcMonth.VisibleIndex = 2;
            // 
            // gcLeg1Exp
            // 
            this.gcLeg1Exp.Caption = "Leg 1 Expiry";
            this.gcLeg1Exp.FieldName = "Leg1ExpirationDate";
            this.gcLeg1Exp.Name = "gcLeg1Exp";
            this.gcLeg1Exp.Visible = true;
            this.gcLeg1Exp.VisibleIndex = 3;
            // 
            // gcLeg2Exp
            // 
            this.gcLeg2Exp.Caption = "Leg 2 Expiry";
            this.gcLeg2Exp.FieldName = "Leg2ExpirationDate";
            this.gcLeg2Exp.Name = "gcLeg2Exp";
            this.gcLeg2Exp.Visible = true;
            this.gcLeg2Exp.VisibleIndex = 4;
            // 
            // BreakdownToolHolidaysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(392, 523);
            this.Controls.Add(this.lbCommonPricing);
            this.Controls.Add(this.lbCalendarLeg2);
            this.Controls.Add(this.lbCalendarLeg1);
            this.Controls.Add(this.lbLeg2);
            this.Controls.Add(this.lbLeg1);
            this.Controls.Add(this.lbCalendarLeg2Title);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.gcLegs);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.lbLeg2Title);
            this.Controls.Add(this.labelControl1);
            this.MinimumSize = new System.Drawing.Size(400, 550);
            this.Name = "BreakdownToolHolidaysForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Holidays";
            this.Load += new System.EventHandler(this.BreakdownToolHolidaysForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcLegs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvLegs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lbLeg2Title;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraGrid.GridControl gcLegs;
        private DevExpress.XtraGrid.Views.Grid.GridView gvLegs;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl lbCalendarLeg2Title;
        private DevExpress.XtraEditors.LabelControl lbLeg1;
        private DevExpress.XtraEditors.LabelControl lbLeg2;
        private DevExpress.XtraEditors.LabelControl lbCalendarLeg1;
        private DevExpress.XtraEditors.LabelControl lbCalendarLeg2;
        private DevExpress.XtraEditors.LabelControl lbCommonPricing;
        private DevExpress.XtraGrid.Columns.GridColumn gcLeg1;
        private DevExpress.XtraGrid.Columns.GridColumn gcLeg2;
        private DevExpress.XtraGrid.Columns.GridColumn gcMonth;
        private DevExpress.XtraGrid.Columns.GridColumn gcLeg1Exp;
        private DevExpress.XtraGrid.Columns.GridColumn gcLeg2Exp;
    }
}