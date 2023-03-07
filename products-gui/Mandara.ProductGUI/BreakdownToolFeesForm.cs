using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mandara.Business.Bus.Messages.ProductBreakdown;

namespace Mandara.ProductGUI
{
    public partial class BreakdownToolFeesForm : DevExpress.XtraEditors.XtraForm
    {
        public BreakdownToolFeesForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }

        public Fees Fees { get; set; }

        private void BreakdownToolFeesForm_Load(object sender, EventArgs e)
        {
            tePrimeBroker.DataBindings.Clear();
            teExchange.DataBindings.Clear();
            teNfa.DataBindings.Clear();
            teClearing.DataBindings.Clear();
            teBlock.DataBindings.Clear();
            teCash.DataBindings.Clear();
            tePlatts.DataBindings.Clear();

            if (Fees != null)
            {
                tePrimeBroker.DataBindings.Add("EditValue", Fees, "Commission", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                teExchange.DataBindings.Add("EditValue", Fees, "Exchange", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                teNfa.DataBindings.Add("EditValue", Fees, "Nfa", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                teClearing.DataBindings.Add("EditValue", Fees, "Clearing", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                teBlock.DataBindings.Add("EditValue", Fees, "Block", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                teCash.DataBindings.Add("EditValue", Fees, "Cash", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                tePlatts.DataBindings.Add("EditValue", Fees, "Platts", false,
                    DataSourceUpdateMode.OnPropertyChanged);
            }
        }
    }
}
