using DevExpress.Mvvm.Native;
using DevExpress.XtraEditors;
using MandaraDemoDTO;
using Newtonsoft.Json;
using ProductsDemo.Model;
using ProductsDemo7.Model;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;

namespace ProductsDemo7
{
    public partial class Form1 : Form
    {

        ProductView pw;
        private IDisposable pwStatusHandler;
        public Form1()
        {
            InitializeComponent();
            pw = new ProductView(ConfigurationManager.AppSettings["productsService"] ?? "http://localhost:5162", TaskScheduler.FromCurrentSynchronizationContext());
            pwStatusHandler = pw.onStatusChanged(label =>
            {
                viewState.Text = label;
            });

        }

        private void GridView1_EditFormHidden(object sender, DevExpress.XtraGrid.Views.Grid.EditFormHiddenEventArgs e)
        {
            pw.CancelUpdate();
        }
          

        private void gridView1_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {

            var victim = e.Row as OfficialProduct;

            if (victim != null && !victim.isNew)
            {

                pw.DeleteProduct(victim);
            }
        }

        private void gridControl1_EmbeddedNavigator_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
        {
            if (e.Button.ButtonType == NavigatorButtonType.Append)
            {
                gridControl1.BeginInvoke(new Action(gridView1.ShowEditForm));
            }
            if (e.Button.ButtonType == NavigatorButtonType.Remove)
            {
                if (MessageBox.Show("Do you want to delete the current row?", "Confirm deletion",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    e.Handled = true;
            }
        }



        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            var victim = e.Row as OfficialProduct;


            if (victim == null) return;
            if (!victim.isNew)
            {

                 pw.ExecUpdate(victim);
            }
            else
            {
                pw.AddProduct(victim);
            }
        }

        private void gridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                var victim = e.Row as OfficialProduct;

                if (victim != null)
                {
                    if (victim.CurrencyGuId != null && victim.CurrencyGuId != Guid.Empty)
                    {
                        victim.Currency = pw.CurrencySrc.Where(c => c.Id == victim.CurrencyGuId).FirstOrDefault();
                    }
                    else
                    {
                        e.Valid = false;
                        e.ErrorText = "Choose currency";
                    }
                    if (victim.UnitGuid != null && victim.CurrencyGuId != Guid.Empty)
                    {
                        victim.PriceUnit = pw.PriceUnitSrc.Where(c => c.Id == victim.UnitGuid).FirstOrDefault();
                    }
                    else
                    {
                        e.Valid = false;
                        e.ErrorText = "Choose price unit";
                    }
                    if (victim.RegionGuId != null)
                    {
                        victim.Region = pw.RegionSrc.Where(c => c.Id == victim.RegionGuId).FirstOrDefault();
                    }
                }

            }
            catch { }
        }



          private void gridView1_EditFormShowing(object sender, DevExpress.XtraGrid.Views.Grid.EditFormShowingEventArgs e)
        {
             pw.BeginUpdate(e.RowHandle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            currencyEditor.DataSource = pw.CurrencySrc;
            priceUnitEditor.DataSource = pw.PriceUnitSrc;
            regionEditor.DataSource = pw.RegionSrc;

            pw.Init(r =>
            {
                gridControl1.BeginInvoke(() =>
                {
                    gridControl1.DataSource = r;
                });
            });
            

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           pw?.Dispose();
           pwStatusHandler?.Dispose();
        }
    }
}