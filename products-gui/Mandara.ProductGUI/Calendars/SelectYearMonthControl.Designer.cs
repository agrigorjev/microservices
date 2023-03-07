namespace Mandara.ProductGUI.Calendars
{
    partial class SelectYearMonthControl
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
            this.selectYearControl = new Mandara.ProductGUI.Calendars.SelectYearControl();
            this.selectMonthControl = new Mandara.ProductGUI.Calendars.SelectMonthControl();
            this.SuspendLayout();
            // 
            // selectYearControl
            // 
            this.selectYearControl.Appearance.BackColor = System.Drawing.Color.White;
            this.selectYearControl.Appearance.Options.UseBackColor = true;
            this.selectYearControl.Cycle = false;
            this.selectYearControl.Location = new System.Drawing.Point(0, 0);
            this.selectYearControl.Name = "selectYearControl";
            this.selectYearControl.Size = new System.Drawing.Size(100, 27);
            this.selectYearControl.TabIndex = 11;
            // 
            // selectMonthControl
            // 
            this.selectMonthControl.Appearance.BackColor = System.Drawing.Color.White;
            this.selectMonthControl.Appearance.Options.UseBackColor = true;
            this.selectMonthControl.Cycle = true;
            this.selectMonthControl.Location = new System.Drawing.Point(103, 0);
            this.selectMonthControl.Name = "selectMonthControl";
            this.selectMonthControl.Size = new System.Drawing.Size(130, 27);
            this.selectMonthControl.TabIndex = 10;
            // 
            // SelectYearMonthControl
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.selectYearControl);
            this.Controls.Add(this.selectMonthControl);
            this.Name = "SelectYearMonthControl";
            this.Size = new System.Drawing.Size(233, 26);
            this.ResumeLayout(false);

        }

        #endregion

        private SelectYearControl selectYearControl;
        private SelectMonthControl selectMonthControl;
    }
}
