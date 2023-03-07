namespace Mandara.ProductGUI.Calendars
{
    partial class SelectControlBase
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
            this.btnPrevious = new DevExpress.XtraEditors.SimpleButton();
            this.nextButton = new DevExpress.XtraEditors.SimpleButton();
            this.valueTextEdit = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrevious.Location = new System.Drawing.Point(4, 3);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(20, 20);
            this.btnPrevious.TabIndex = 21;
            this.btnPrevious.Text = "<";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Location = new System.Drawing.Point(119, 3);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(20, 20);
            this.nextButton.TabIndex = 22;
            this.nextButton.Text = ">";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // valueTextEdit
            // 
            this.valueTextEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTextEdit.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.valueTextEdit.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.valueTextEdit.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.valueTextEdit.Location = new System.Drawing.Point(26, 5);
            this.valueTextEdit.Name = "valueTextEdit";
            this.valueTextEdit.Size = new System.Drawing.Size(92, 19);
            this.valueTextEdit.TabIndex = 23;
            this.valueTextEdit.Text = "Text";
            // 
            // SelectControlBase
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.valueTextEdit);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.btnPrevious);
            this.Name = "SelectControlBase";
            this.Size = new System.Drawing.Size(145, 28);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnPrevious;
        private DevExpress.XtraEditors.SimpleButton nextButton;
        private DevExpress.XtraEditors.LabelControl valueTextEdit;
    }
}
