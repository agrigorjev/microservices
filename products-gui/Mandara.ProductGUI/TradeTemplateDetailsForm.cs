using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using Mandara.Business;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Entities;

namespace Mandara.ProductGUI
{
    public partial class TradeTemplateDetailsForm : XtraForm
    {
        private List<Portfolio> _portfolios;
        private TradeTemplate _tradeTemplate;
        private List<Instrument> _instruments;
        private List<Unit> _units;

        public TradeTemplateDetailsForm(TradeTemplate tradeTemplate = null)
        {
            InitializeComponent();

            _tradeTemplate = tradeTemplate ?? new TradeTemplate();
            _units = new ProductManager().GetUnits();
        }

        private void TradeTemplateDetailsForm_Load(object sender, EventArgs e)
        {
            _portfolios = PortfolioManager.GetOnlyPortfolios();
            treeList1.DataSource = _portfolios;
            treeList1.ExpandAll();

            ProductManager productManager = new ProductManager();

            cmbExchange.Properties.Items.Clear();
            List<Exchange> exchanges = productManager.GetExchanges().OrderBy(x => x.Name).ToList();
            cmbExchange.Properties.Items.AddRange(exchanges);

            OfficialProductToInstrument officialProductToInstrument = new OfficialProductToInstrument();
            TradeAddHandlerConverter tradeAddHandlerConverter = new TradeAddHandlerConverter(
                new TradeAddHandlerConverterProvider(),
                officialProductToInstrument);
            Dictionary<Instrument, OfficialProduct> instrumentsProductsMap = tradeAddHandlerConverter.GetInstrumentsProductsMap(false);

            _instruments = instrumentsProductsMap.Keys.Where(x => x != null).ToList();

            // bind form
            if (_tradeTemplate.TradeTemplateId > 0)
            {
                txtName.Text = _tradeTemplate.TemplateName;
                leBook.EditValue = _portfolios.FirstOrDefault(x => x.PortfolioId == _tradeTemplate.Portfolio.PortfolioId);
                cmbExchange.SelectedItem = exchanges.FirstOrDefault(x => x.ExchangeId == _tradeTemplate.Exchange.ExchangeId);

                if (cmbExchange.SelectedItem != null)
                {
                    cmbOfficialProducts.SelectedItem =
                        _instruments.FirstOrDefault(x => x.Id == _tradeTemplate.OfficialProduct.OfficialProductId);

                    txtVolume.EditValue = _tradeTemplate.Volume;

                    cmbUnits.SelectedItem = _tradeTemplate.Unit;
                }
            }

            txtName.DataBindings.Add("EditValue", _tradeTemplate, "TemplateName", true);
        }

        private void treeList1_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            if (e.Node.HasChildren)
            {
                e.Appearance.ForeColor = Color.FromArgb(150, 150, 150);
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }

        private void treeList1_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node == null || e.Node.HasChildren)
                return;

            int portfolioId = (int)e.Node.GetValue("PortfolioId");
            Portfolio portfolio = _portfolios.FirstOrDefault(x => x.PortfolioId == portfolioId);

            leBook.EditValue = portfolio;
            leBook.ClosePopup();

            UpdateTemplateName();
        }

        private void UpdateTemplateName()
        {
            Instrument instrument = (cmbOfficialProducts.EditValue as Instrument) ?? new Instrument() {Name = ""};
            decimal volume = decimal.Parse(txtVolume.Text);
            string units = cmbUnits.Text;
            string exchange = cmbExchange.Text;

            _tradeTemplate.TemplateName = string.Format("{0} {1:.##}{2} {3}", instrument.Name, volume, units, exchange);
            txtName.Text = _tradeTemplate.TemplateName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ValidateForm();

            if (dxErrorProvider1.HasErrors)
                return;

            _tradeTemplate.TemplateName = txtName.Text;
            _tradeTemplate.Portfolio = leBook.EditValue as Portfolio;
            _tradeTemplate.Exchange = cmbExchange.SelectedItem as Exchange;
            _tradeTemplate.OfficialProduct = new OfficialProduct()
                                                 {
                                                     OfficialProductId =
                                                         (cmbOfficialProducts.SelectedItem as
                                                          Instrument).Id
                                                 };
            _tradeTemplate.Volume = decimal.Parse(txtVolume.Text);
            _tradeTemplate.Unit = (Unit) cmbUnits.SelectedItem;

            new TradeTemplatesManager().Save(_tradeTemplate);

            Close();
        }

        private void ValidateForm()
        {
            string errorText = leBook.EditValue == null ? "Book should not be blank" : null;
            dxErrorProvider1.SetError(leBook, errorText);

            errorText = cmbExchange.SelectedItem == null ? "Exchange should not be blank" : null;
            dxErrorProvider1.SetError(cmbExchange, errorText);

            errorText = cmbOfficialProducts.SelectedItem == null ? "Official product should not be blank" : null;
            dxErrorProvider1.SetError(cmbOfficialProducts, errorText);

            errorText = cmbUnits.SelectedItem == null ? "Units should not be blank" : null;
            dxErrorProvider1.SetError(cmbUnits, errorText);
        }

        private void cmbExchange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Exchange exchange = cmbExchange.SelectedItem as Exchange;
            if (exchange == null)
                return;

            List<Instrument> instruments = _instruments.Where(x => x.Exchanges.Contains(exchange.Name)).OrderBy(x => x.Name).ToList();

            cmbOfficialProducts.Properties.Items.Clear();
            cmbOfficialProducts.Properties.Items.AddRange(instruments);

            UpdateTemplateName();
        }

        private void cmbOfficialProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbUnits.Properties.Items.Clear();

            Instrument instrument = cmbOfficialProducts.SelectedItem as Instrument;
            if (instrument == null)
                return;
            
            Exchange exchange = cmbExchange.SelectedItem as Exchange;
            if (exchange == null)
                return;

            ExchangeUnits exchangeUnits;
            if (instrument.ExchangeUnits.TryGetValue(exchange.Name, out exchangeUnits))
            {
                cmbUnits.Properties.Items.AddRange(
                    (exchangeUnits.AvailableUnits ?? new List<int>()).Union(exchangeUnits.BalmoUnits ?? new List<int>())
                                                                     .Distinct()
                                                                     .OrderBy(x => x)
                                                                     .Select(x => _units.First(y => y.UnitId == x))
                                                                     .ToList());
            }

            UpdateTemplateName();
        }

        private void TradeTemplateDetailsForm_Validating(object sender, CancelEventArgs e)
        {
            ValidateForm();
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            ValidateForm();

        }

        private void leBook_Validating(object sender, CancelEventArgs e)
        {
            ValidateForm();

        }

        private void cmbExchange_Validating(object sender, CancelEventArgs e)
        {
            ValidateForm();

        }

        private void cmbOfficialProducts_Validating(object sender, CancelEventArgs e)
        {
            ValidateForm();

        }

        private void cmbUnits_Validating(object sender, CancelEventArgs e)
        {
            ValidateForm();

        }

        private void txtVolume_EditValueChanged(object sender, EventArgs e)
        {
            UpdateTemplateName();

        }

        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTemplateName();

        }
    }
}