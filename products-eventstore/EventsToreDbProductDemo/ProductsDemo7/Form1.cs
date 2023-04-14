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
        BindingList<OfficialProduct> _productsSource;
        private readonly EventStoreOperationService storeService;
        public Form1()
        {
            InitializeComponent();
            pw = new ProductView(ConfigurationManager.AppSettings["productsService"] ?? "http://localhost:5162");

            this.viewState.Text = pw.StatusText;
            gridControl1.HandleCreated += GridControl1_HandleCreated;
            storeService = new EventStoreOperationService();
            _productsSource = new BindingList<OfficialProduct>();
            gridControl1.DataSource = _productsSource;
            gridView1.EditFormHidden += GridView1_EditFormHidden;

            pw.StreamProducts.Subscribe(p =>
                gridControl1.BeginInvoke(new Action(() =>
                {
                    var exists = _productsSource.Select((pp, index) => new { pp, index }).FirstOrDefault(el => el.pp.Id == p.Id);
                    if (exists != null)
                    {
                        _productsSource[exists.index] = p;
                    }
                    else
                    {
                        _productsSource.Add(p);
                    }
                }))
            );

        }

        private void GridView1_EditFormHidden(object sender, DevExpress.XtraGrid.Views.Grid.EditFormHiddenEventArgs e)
        {
            _inEdit = null;
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
                //_productsSource.Clear();
                //pw.Products.ForEach(p => _productsSource.Add(p));
                currencyEditor.DataSource = pw.CurrencySrc;
                priceUnitEditor.DataSource = pw.PriceUnitSrc;
                regionEditor.DataSource = pw.RegionsSrc;
            }));
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


        private void gridView1_RowDeleted(object sender, DevExpress.Data.RowDeletedEventArgs e)
        {
            var victim = e.Row as OfficialProduct;

            if (victim != null && !victim.isNew)
            {

                storeService.deleteProducts(victim).Subscribe(wr => Debug.Print("Delete done {0}", wr.LogPosition));
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
                if (!String.IsNullOrEmpty(_inEdit))
                {
                    storeService.updateProducts(victim,JsonConvert.DeserializeObject<OfficialProduct>(_inEdit)).Subscribe(wr => Debug.Print("Update done {0}", wr.LogPosition));
                }
            }
            else
            {
                storeService.createProducts(victim).Subscribe(wr => Debug.Print("Create done {0}", wr.LogPosition));
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

       

        private String _inEdit;

       private void gridView1_EditFormShowing(object sender, DevExpress.XtraGrid.Views.Grid.EditFormShowingEventArgs e)
        {
            try
            {
                _inEdit = JsonConvert.SerializeObject(_productsSource[e.RowHandle]);
 
            }
            catch { }
        }
    }
}