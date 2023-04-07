using DevExpress.XtraEditors;
using MandaraDemoDTO;
using ProductsDemo.Model;
using ProductsDemo7.Model;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;

namespace ProductsDemo7
{
    public partial class Form1 : Form
    {

        ProductView pw;
        BindingSource productsSource;
        private readonly EventStoreOperationService storeService;
        public Form1()
        {
            InitializeComponent();
            pw = new ProductView(ConfigurationManager.AppSettings["productsService"] ?? "http://localhost:5162");

            this.viewState.Text = pw.StatusText;
            gridControl1.HandleCreated += GridControl1_HandleCreated;
            storeService = new EventStoreOperationService();
        }

        private void GridControl1_HandleCreated(object? sender, EventArgs e)
        {
            pw.PropertyChanged += Pw_PropertyChanged;
            pw.ProductsLoaded += Pw_ProductsLoaded;
        }

        private void Pw_ProductsLoaded(object? sender, EventArgs e)
        {
            gridControl1.BeginInvoke(new Action(() =>
            {
                gridControl1.DataSource = new BindingList<OfficialProduct>(pw.Products);
                currencyEditor.DataSource = pw.CurrencySrc;
                priceUnitEditor.DataSource = pw.PriceUnitSrc;
                regionEditor.DataSource = pw.RegionsSrc;
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void Pw_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.viewState.Text = pw.StatusText;
            if (pw.LastError != null)
            {
                this.viewState.ForeColor = Color.DarkRed;
            }
            else
            {
                this.viewState.ForeColor = Color.Black;
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {
            var victim=e.Row as OfficialProduct;

            if(victim != null && !victim.isNew) {

                pw.DoRefreshList(storeService.deleteProducts(victim));
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
                pw.DoRefreshList(storeService.updateProducts(victim));
            }
            else
            {
                pw.DoRefreshList(storeService.createProducts(victim));
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
                        victim.Region = pw.RegionsSrc.Where(c => c.Id == victim.RegionGuId).FirstOrDefault();
                    }
                }
            }
            catch { }
        }

        private void gridView1_RowDeleting(object sender, DevExpress.Data.RowDeletingEventArgs e)
        {
            Debug.Print(e.Cancel.ToString());
        }
    }
}