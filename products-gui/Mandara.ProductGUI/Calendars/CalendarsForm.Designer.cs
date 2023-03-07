using Mandara.ProductGUI.Calendars;

namespace Mandara.ProductGUI
{
    partial class CalendarsForm
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
            this.calendarTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.expiryDatesTabPage = new DevExpress.XtraTab.XtraTabPage();
            this.expiryDatesControl = new Mandara.ProductGUI.Calendars.ExpiryDatesControl();
            this.expiryDatesTopPanel = new DevExpress.XtraEditors.PanelControl();
            this.expiryYearSelector = new Mandara.ProductGUI.Calendars.SelectYearControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.holydaysTabPage = new DevExpress.XtraTab.XtraTabPage();
            this.holidaysControl = new Mandara.ProductGUI.Calendars.HolidaysControl();
            this.holydayTopPanel = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.selectHolydatsYearMonthControl = new Mandara.ProductGUI.Calendars.SelectYearMonthControl();
            this.bottomPanel = new DevExpress.XtraEditors.PanelControl();
            this.apply = new DevExpress.XtraEditors.SimpleButton();
            this.cancel = new DevExpress.XtraEditors.SimpleButton();
            this.ok = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.calendarTabControl)).BeginInit();
            this.calendarTabControl.SuspendLayout();
            this.expiryDatesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expiryDatesTopPanel)).BeginInit();
            this.expiryDatesTopPanel.SuspendLayout();
            this.holydaysTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.holydayTopPanel)).BeginInit();
            this.holydayTopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // calendarTabControl
            // 
            this.calendarTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calendarTabControl.Location = new System.Drawing.Point(0, 1);
            this.calendarTabControl.Name = "calendarTabControl";
            this.calendarTabControl.SelectedTabPage = this.expiryDatesTabPage;
            this.calendarTabControl.Size = new System.Drawing.Size(1267, 424);
            this.calendarTabControl.TabIndex = 0;
            this.calendarTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.expiryDatesTabPage,
            this.holydaysTabPage});
            this.calendarTabControl.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.calendarTabControl_SelectedPageChanging_1);
            // 
            // expiryDatesTabPage
            // 
            this.expiryDatesTabPage.Controls.Add(this.expiryDatesControl);
            this.expiryDatesTabPage.Controls.Add(this.expiryDatesTopPanel);
            this.expiryDatesTabPage.Name = "expiryDatesTabPage";
            this.expiryDatesTabPage.Size = new System.Drawing.Size(1261, 396);
            this.expiryDatesTabPage.Text = "Expiry dates";
            // 
            // expiryDatesControl
            // 
            this.expiryDatesControl.AllowChanges = false;
            this.expiryDatesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expiryDatesControl.Location = new System.Drawing.Point(0, 50);
            this.expiryDatesControl.Margin = new System.Windows.Forms.Padding(0);
            this.expiryDatesControl.Name = "expiryDatesControl";
            this.expiryDatesControl.Size = new System.Drawing.Size(1261, 346);
            this.expiryDatesControl.TabIndex = 0;
            // 
            // expiryDatesTopPanel
            // 
            this.expiryDatesTopPanel.Controls.Add(this.expiryYearSelector);
            this.expiryDatesTopPanel.Controls.Add(this.labelControl2);
            this.expiryDatesTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.expiryDatesTopPanel.Location = new System.Drawing.Point(0, 0);
            this.expiryDatesTopPanel.Name = "expiryDatesTopPanel";
            this.expiryDatesTopPanel.Size = new System.Drawing.Size(1261, 50);
            this.expiryDatesTopPanel.TabIndex = 3;
            // 
            // expiryYearSelector
            // 
            this.expiryYearSelector.Appearance.BackColor = System.Drawing.Color.White;
            this.expiryYearSelector.Appearance.Options.UseBackColor = true;
            this.expiryYearSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.expiryYearSelector.CurrentIndex = 6;
            this.expiryYearSelector.CurrentValue = 2015;
            this.expiryYearSelector.Cycle = false;
            this.expiryYearSelector.Location = new System.Drawing.Point(60, 15);
            this.expiryYearSelector.Name = "expiryYearSelector";
            this.expiryYearSelector.SelectedYear = 2015;
            this.expiryYearSelector.Size = new System.Drawing.Size(100, 27);
            this.expiryYearSelector.TabIndex = 7;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(20, 20);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(26, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Year:";
            // 
            // holydaysTabPage
            // 
            this.holydaysTabPage.Controls.Add(this.holidaysControl);
            this.holydaysTabPage.Controls.Add(this.holydayTopPanel);
            this.holydaysTabPage.Name = "holydaysTabPage";
            this.holydaysTabPage.Size = new System.Drawing.Size(1261, 396);
            this.holydaysTabPage.Text = "Holidays";
            // 
            // holidaysControl
            // 
            this.holidaysControl.AllowChanges = false;
            this.holidaysControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.holidaysControl.Location = new System.Drawing.Point(0, 50);
            this.holidaysControl.Margin = new System.Windows.Forms.Padding(0);
            this.holidaysControl.Name = "holidaysControl";
            this.holidaysControl.Size = new System.Drawing.Size(1261, 346);
            this.holidaysControl.TabIndex = 0;
            // 
            // holydayTopPanel
            // 
            this.holydayTopPanel.Controls.Add(this.labelControl1);
            this.holydayTopPanel.Controls.Add(this.selectHolydatsYearMonthControl);
            this.holydayTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.holydayTopPanel.Location = new System.Drawing.Point(0, 0);
            this.holydayTopPanel.Name = "holydayTopPanel";
            this.holydayTopPanel.Size = new System.Drawing.Size(1261, 50);
            this.holydayTopPanel.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(20, 20);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(34, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Month:";
            // 
            // selectHolydatsYearMonthControl
            // 
            this.selectHolydatsYearMonthControl.Appearance.BackColor = System.Drawing.Color.White;
            this.selectHolydatsYearMonthControl.Appearance.Options.UseBackColor = true;
            this.selectHolydatsYearMonthControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectHolydatsYearMonthControl.Location = new System.Drawing.Point(60, 15);
            this.selectHolydatsYearMonthControl.Name = "selectHolydatsYearMonthControl";
            this.selectHolydatsYearMonthControl.Size = new System.Drawing.Size(236, 30);
            this.selectHolydatsYearMonthControl.TabIndex = 0;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.apply);
            this.bottomPanel.Controls.Add(this.cancel);
            this.bottomPanel.Controls.Add(this.ok);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 423);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1267, 60);
            this.bottomPanel.TabIndex = 1;
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(1180, 21);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 22;
            this.apply.Text = "Apply";
            this.apply.Click += new System.EventHandler(this.ApplyClicked);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.CausesValidation = false;
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(1084, 21);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 21;
            this.cancel.Text = "Cancel";
            this.cancel.Click += new System.EventHandler(this.CancelClicked);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(990, 21);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 20;
            this.ok.Text = "OK";
            this.ok.Click += new System.EventHandler(this.OkClicked);
            // 
            // CalendarsForm
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(1267, 483);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.calendarTabControl);
            this.Name = "CalendarsForm";
            this.Text = "Calendars";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalendarsForm_FormClosing);
            this.Load += new System.EventHandler(this.CalendarsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.calendarTabControl)).EndInit();
            this.calendarTabControl.ResumeLayout(false);
            this.expiryDatesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expiryDatesTopPanel)).EndInit();
            this.expiryDatesTopPanel.ResumeLayout(false);
            this.expiryDatesTopPanel.PerformLayout();
            this.holydaysTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.holydayTopPanel)).EndInit();
            this.holydayTopPanel.ResumeLayout(false);
            this.holydayTopPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl calendarTabControl;
        private DevExpress.XtraTab.XtraTabPage holydaysTabPage;
        private DevExpress.XtraTab.XtraTabPage expiryDatesTabPage;
        private HolidaysControl holidaysControl;
        private ExpiryDatesControl expiryDatesControl;
        private DevExpress.XtraEditors.PanelControl bottomPanel;
        private DevExpress.XtraEditors.SimpleButton apply;
        private DevExpress.XtraEditors.SimpleButton cancel;
        private DevExpress.XtraEditors.SimpleButton ok;
        private DevExpress.XtraEditors.PanelControl holydayTopPanel;
        private DevExpress.XtraEditors.PanelControl expiryDatesTopPanel;
        private Calendars.SelectYearControl expiryYearSelector;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private Calendars.SelectYearMonthControl selectHolydatsYearMonthControl;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}