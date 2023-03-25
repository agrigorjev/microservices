using ProductsDemo.Model;
using System.ComponentModel;
using System.Configuration;

namespace ProductsDemo7
{
    public partial class Form1 : Form
    {

        ProductView pw;
        public Form1()
        {
            InitializeComponent();
            pw = new ProductView(ConfigurationManager.AppSettings["productsService"] ?? "http://localhost:5162");
            pw.PropertyChanged += Pw_PropertyChanged;
            pw.ProductsLoaded += Pw_ProductsLoaded;
            this.viewState.Text = pw.StatusText;


        }

        private void Pw_ProductsLoaded(object? sender, EventArgs e)
        {
            gridControl1.BeginInvoke(new Action(() =>
            {
                gridControl1.DataSource = new BindingList<Product>(pw.Products);
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

    }
}