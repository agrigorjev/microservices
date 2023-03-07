using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Mandara.Entities.Enums;

namespace Mandara.ProductGUI
{
    public partial class ProgressForm : Form
    {
        BackgroundWorker worker = new BackgroundWorker();
        object argument;
        bool allowClose = false;
        public string returnMessage = "";

        public ProgressForm(DoWorkEventHandler work, object param)
        {
            InitializeComponent();
            worker.DoWork += work;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            argument = param;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var exception = e.Error;
                string errorText = "\r\n\r\nError Details:\r\n";

                while (exception != null)
                {
                    errorText += exception.Message + "\r\n";
                    exception = exception.InnerException;
                }

                XtraMessageBox.Show("Product Management Tool encounter an error and will be closed." + errorText,
                    "Product Management Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Application.Exit();
            }

            allowClose = true;
            Close();
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressBar.Value = e.ProgressPercentage;
            label1.Text = e.UserState.ToString();
            returnMessage = label1.Text;
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !allowClose;
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            worker.RunWorkerAsync(argument);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            label1.Text = "Cancelling operation";
            worker.CancelAsync();
        }
    }
}
