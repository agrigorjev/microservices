using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using Mandara.Business;
using Mandara.Business.Audit;
using Mandara.Business.Authorization;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Messages.ProductBreakdown;
using Mandara.Business.Config.Client;
using Mandara.Common;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorDetails;
using Mandara.ProductGUI.Desks;
using Mandara.ProductGUI.Desks.OfficialProducts;
using Mandara.ProductGUI.Models;
using Ninject.Infrastructure.Language;
using NLog;
using BusClient = Mandara.ProductGUI.Bus.BusClient;
using Logger = NLog.Logger;
using Timer = System.Threading.Timer;

namespace Mandara.ProductGUI
{
    internal partial class ProductListForm : XtraForm
    {
        private ProductManager _products = new ProductManager();
        private BrokerManager _brokerManager = new BrokerManager();
        private List<ProductCategory> _productGroups;
        private BindingList<Product> _groupProducts;
        private List<OfficialProduct> _officialProducts;
        private BindingList<ParserDefaultProduct> _defaultProducts;
        private List<ParserDefaultProduct> _parserDefaultProducts;
        private BindingList<Broker> _brokersBindingList;
        private BindingList<Company> _companies;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private RepositoryItemTextEdit _reCurrencyTextEdit;

        private ProductBreakdownModel ProductBreakdownModel { get; set; }

        public static BusClient BusClient { get; private set; }
        private TaskScheduler _uiSynchronizationContext;
        private Timer _serverResponseTimer;

        private bool _desksLoaded;
        private ViewMode _viewMode = ViewMode.None;
        private Dictionary<BarButtonItem, ViewMode> _viewModes;
        private BindingList<Exchange> _exchanges;
        private BindingList<Unit> _units;
        private BindingList<CurrencyModel> _currencies;

        private bool _onCloseAuditWritten;

        private IDesksRepository _desks = new DesksRepository();
        private IDeskProductsRepository _deskProducts = new DeskProductsRepository();

        public static User AuthorizedUser { get; private set; }

        public ProductListForm()
        {
            _desksLoaded = PrepareDeskProductsData();
            InitializeComponent();
            PrepareProductSearchResultsForm();
            PrepareViewModeButtons();
        }

        private bool PrepareDeskProductsData()
        {
            try
            {
                _desks.LoadDesks();
                _deskProducts.LoadProducts();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        private void PrepareProductSearchResultsForm()
        {
            productSearchResults.ProductsByDesk = _deskProducts;
        }

        private void PrepareViewModeButtons()
        {
            _viewModes = new Dictionary<BarButtonItem, ViewMode>()
            {
                { showProducts, ViewMode.Products },
                { showOfficialProducts, ViewMode.OfficialProducts },
                { showOfficialProductDefaultProducts, ViewMode.DefaultProducts },
                { showBrokers, ViewMode.Brokers },
                { showCompanies, ViewMode.Companies },
                { showExchanges, ViewMode.Exchanges },
                { showTradeTemplates, ViewMode.TradeTemplates },
                { showProductBreakdown, ViewMode.ProductBreakdown },
                { showUnits, ViewMode.Units },
                { showCurrencies, ViewMode.Currencies },
            };

            if (_desksLoaded)
            {
                _viewModes.Add(showDesks, ViewMode.Desks);
            }
            else
            {
                showDesks.Enabled = false;
            }
        }

        private void DesksConfigurator_HasSelectedDeskChanged(object sender, EventArgs args)
        {
            UpdateDeskGroupControls();
        }

        private void UpdateDeskGroupControls()
        {
            deskGroup.Visible = ViewMode.Desks == ViewMode;
            deskEdit.Enabled = ShowDeskRelatedModify(() => true);
            deskDelete.Enabled = ShowDeskRelatedModify(CanModifyDesk);

            deskOfficialProductsGroup.Visible = ViewMode.Desks == ViewMode;
            deskProductAdd.Enabled = ShowDeskRelatedAdd();
            deskProductEdit.Enabled = ShowDeskRelatedModify(CanModifyDeskProduct);
            deskProductDelete.Enabled = ShowDeskRelatedModify(CanModifyDeskProduct);

            desksConfigurator.Visible = ViewMode.Desks == ViewMode;
        }

        private bool CanModifyDesk()
        {
            return !desksConfigurator.SelectedDeskHasProducts();
        }

        private bool ShowDeskRelatedModify(Func<bool> deskProductsCheck)
        {
            return null != desksConfigurator && desksConfigurator.HasSelectedDesk && deskProductsCheck();
        }

        private bool ShowDeskRelatedAdd()
        {
            return null != desksConfigurator && desksConfigurator.HasSelectedDesk;
        }

        private bool CanModifyDeskProduct()
        {
            return desksConfigurator.SelectedDeskHasProducts() && desksConfigurator.HasSelectedProduct;
        }

        private void DesksConfigurator_HasSelectedDeskProductChanged(object sender, EventArgs args)
        {
            UpdateDeskGroupControls();
        }

        protected override void OnLoad(EventArgs emptyArgs)
        {
            if (DoAuthorization())
            {
                base.OnLoad(emptyArgs);
            }
        }

        private bool DoAuthorization()
        {
            using (CheckPasswordForm authForm = new CheckPasswordForm())
            {
                if (authForm.ShowDialog(this) != DialogResult.OK)
                {
                    CloseWithAudit("Unathorized Login Attempt");
                    return false;
                }

                AuthorizedUser = authForm.AuthorizedUser;

                if (authForm.AuthorizedUser.Locked ?? false)
                {
                    MessageBox.Show(
                        this,
                        "Your user account is locked. Please contact IT support.",
                        "Product Management Tool",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    CloseWithAudit("Unathorized Login Attempt");
                    return false;
                }

                if (!AuthorizationService.IsUserAuthorizedTo(AuthorizedUser, PermissionType.LaunchProductMgmtTool))
                {
                    MessageBox.Show(
                        "You are not authorized to access Mandara Product Management Tool.",
                        "Authorization message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);

                    CloseWithAudit("Unathorized Login Attempt");
                    return false;
                }
            }

            AuditManager.WriteAuditMessage(AuthorizedUser.UserName, null, "Product tool", "Login", "User Login");
            return true;
        }

        // This name needs to be changed.  It's not only used for logging in.  And successful login has it's own direct
        // write of an audit message.
        private void WriteLoginAudit(string auditMessage, Action postAudit)
        {
            if (!_onCloseAuditWritten)
            {
                AuditManager.WriteAuditMessage(
                    AuthorizedUser != null ? AuthorizedUser.UserName : null,
                    null,
                    "Product tool",
                    "Login",
                    auditMessage);
                _onCloseAuditWritten = true;

                postAudit();
            }
        }

        private void CloseWithAudit(string auditMessage)
        {
            WriteLoginAudit(auditMessage, Close);
        }

        private void ProductListForm_Load(object sender, EventArgs e)
        {
            viewSelectorAndActionContainer.Manager.UseAltKeyForMenu = false;
            _uiSynchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

            if (!InitializeBus())
            {
                return;
            }

            UpdateTitleBar();

            _productGroups = _products.GetProductGroups();
            _officialProducts = _products.GetOfficialProducts();
            _parserDefaultProducts = _products.GetDefaultProducts();

            List<Broker> brokers = _brokerManager.GetBrokers();

            productGroupsGrid.DataSource = _productGroups;
            officialProductsGrid.DataSource = _officialProducts;

            officialProductsRepo.DataSource = new BindingList<OfficialProduct>(_officialProducts);

            _defaultProducts =
                new BindingList<ParserDefaultProduct>(new List<ParserDefaultProduct>(_parserDefaultProducts));
            defaultProductsGrid.DataSource = _defaultProducts;
            brokersDefaultProductsRepo.DataSource = _officialProducts;

            _brokersBindingList = new BindingList<Broker>(new List<Broker>(brokers));
            brokersGrid.DataSource = _brokersBindingList;

            List<Company> companiesSource = _brokerManager.GetCompanies();

            brokersCompanyRepo.DataSource = companiesSource;

            _companies = new BindingList<Company>(new List<Company>(companiesSource));
            companiesGrid.DataSource = _companies;
            companiesRegionsRepo.DataSource = _brokerManager.GetRegions();

            SetUpDesks();

            if (!AuthorizationService.IsUserAuthorizedTo(AuthorizedUser, PermissionType.ProductMgmtToolWriteAccess))
            {
                brokersView.OptionsBehavior.Editable = false;
                saveBrokers.Enabled = false;

                companiesView.OptionsBehavior.Editable = false;
                saveCompanies.Enabled = false;

                exchangesView.OptionsBehavior.Editable = false;
                saveExchanges.Enabled = false;

                addProduct.Visibility = BarItemVisibility.Never;
                deleteProduct.Visibility = BarItemVisibility.Never;
                editProduct.Caption = "View";

                addOfficialProduct.Visibility = BarItemVisibility.Never;
                deleteOfficialProduct.Visibility = BarItemVisibility.Never;
                editOfficialProduct.Caption = "View";

                productGroupsRibbon.Visible = false;
                brokersRibbon.Visible = false;
                companiesRibbon.Visible = false;
                exchangesRibbon.Visible = false;

                productRecalculateAll.Visibility = BarItemVisibility.Never;
                productRecalculateChanged.Visibility = BarItemVisibility.Never;
                recalculateManualTrades.Visibility = BarItemVisibility.Never;

                if (!AuthorizationService.IsUserAuthorizedTo(AuthorizedUser, PermissionType.UseProductBreakdown))
                {
                    showProductBreakdown.Visibility = BarItemVisibility.Never;
                }
            }

            BindExchanges();

            InitProductBreakdown();

            if (Environment.GetCommandLineArgs().Contains("-mapsd"))
            {
                _logger.Info("Start mapping security definitions for source details");
                using (ProgressForm progressForm = new ProgressForm(
                    _products.MapSourceDetailsToSecurityDefinitions,
                    null))
                {
                    progressForm.ShowDialog();
                }
                _logger.Info("Mapping security definitions for source details done");
                _logger.Info("Start calculate positions for source details");
                RecalculateDetails(RecalcMode.NewSourceDetails);
                _logger.Info("All calculation is done");
            }

            if (Environment.GetCommandLineArgs().Contains("-autoupdate"))
            {
                RecalculateAllSilent();
            }

            ViewMode = ViewMode.Products;
        }

        private void SetUpDesks()
        {
            SetupDesksConfigurator();

            deskSelectorEditor.DataSource = _desks.DesksData;
            deskSelectorEditor.PopulateColumns();
            Enumerable.Range(1, deskSelectorEditor.Columns.Count - 1)
                      .ForEach(deskCol => deskSelectorEditor.Columns[deskCol].Visible = false);
            deskSelector.EditValue = Desk.Default.Id;
        }

        private void SetupDesksConfigurator()
        {
            //configuring it here because code dealing with custom class was deleted by designer
            desksConfigurator = new DesksConfigurator
            {
                DeskSource = _desks,
                DeskProducts = _deskProducts,
                Products = _products,
            };
            Controls.Add(desksConfigurator);
            desksConfigurator.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            desksConfigurator.Location = new System.Drawing.Point(0, 120);
            desksConfigurator.Name = "desksConfigurator";
            desksConfigurator.Size = new System.Drawing.Size(1308, 445);
            desksConfigurator.HasSelectedDeskChanged += DesksConfigurator_HasSelectedDeskChanged;
            desksConfigurator.HasSelectedDeskProductChanged += DesksConfigurator_HasSelectedDeskProductChanged;
        }

        private bool InitializeBus()
        {
            string serverPrefix = GetServerPrefix();

            try
            {
                InformaticaHelper.ServerPrefix = serverPrefix;
                BusClient = new BusClient();
                BusClient.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was an error initializing UMS context. Please make sure that UMS is installed, "
                    + "properly configured, and you are not trying to run another instance of IRM. "
                    + $"Error message:\r\n\r\n{ex.Message}");
                Close();
                return false;
            }

            return true;
        }

        private void BindExchanges()
        {
            _exchanges = new BindingList<Exchange>(_products.GetExchanges());
            _exchanges.AddingNew += ExchangesOnAddingNew;
            exchangeCalendarSelector.DataSource = _products.GetCalendars().OrderBy(x => x.Name);
            TimeZoneInfo.GetSystemTimeZones()
                        .ForEach(timeZoneInfo => { exchangeTimezoneSelector.Items.Add(new ComboBoxItem(timeZoneInfo)); });
            exchangesGrid.DataSource = _exchanges;
        }

        private void ExchangesOnAddingNew(object sender, AddingNewEventArgs args)
        {
            args.NewObject = new Exchange { Timezone = TimeZoneInfo.Utc };
        }

        private string GetServerPrefix()
        {
            ServersConfigurationSection serversSection =
                ConfigurationManager.GetSection("ServersSection") as ServersConfigurationSection;

            return serversSection?.DefaultPrefix ?? string.Empty;
        }

        private void InitProductBreakdown()
        {
            ProductBreakdownModel = new ProductBreakdownModel();

            ProductBreakdownModel.OfficialProducts.Clear();
            _officialProducts.Map(ProductBreakdownModel.OfficialProducts.Add);
            ProductBreakdownModel.OfficialProducts.ResetBindings();

            ProductBreakdownModel.ProductCategories = _productGroups;
            ProductBreakdownModel.CurrentBreakdownItem = null;

            ComboBoxItemCollection coll = contractMonthEditor.Properties.Items;
            coll.BeginUpdate();
            try
            {
                coll.AddRange(ProductBreakdownModel.ContractMonthItems);
            }
            finally
            {
                coll.EndUpdate();
            }

            // data bindings
            productBreakdownGrid.DataSource = ProductBreakdownModel.BreakdownItems;
            testTradeImpactGrid.DataSource = ProductBreakdownModel.CurrentCalculationDetails;
            officialProductSelector.Properties.DataSource = ProductBreakdownModel.OfficialProducts;
            productSelector.Properties.DataSource = ProductBreakdownModel.CurrentProducts;
            pnlEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "Pnl",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            liveSumEditor.DataBindings.Add(
                "Text",
                ProductBreakdownModel,
                "SumLive",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            overnightSumEditor.DataBindings.Add(
                "Text",
                ProductBreakdownModel,
                "SumOvernight",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            avgSettleEditor.DataBindings.Add(
                "Text",
                ProductBreakdownModel,
                "AvgSettle",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            contractMonthEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "ContractMonth",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            testTradeDateEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "PurchaseDay",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            tradeStartDateEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "StartDay",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            tradeStartDateEditor.DataBindings.Add(
                "Enabled",
                ProductBreakdownModel,
                "StartDayApplicable",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            startDateTitle.DataBindings.Add(
                "Enabled",
                ProductBreakdownModel,
                "StartDayApplicable",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            tradeEndDateEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "EndDay",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            tradeEndDateEditor.DataBindings.Add(
                "Enabled",
                ProductBreakdownModel,
                "EndDayApplicable",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            endDateTitle.DataBindings.Add(
                "Enabled",
                ProductBreakdownModel,
                "EndDayApplicable",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            quantityEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "Quantity",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
            startPriceEditor.DataBindings.Add(
                "EditValue",
                ProductBreakdownModel,
                "StartPrice",
                false,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void SetProductsListDataSource()
        {
            ProductCategory selectedGroup = productGroupsView.GetFocusedRow() as ProductCategory;

            if (selectedGroup == null)
            {
                return;
            }

            _groupProducts = new BindingList<Product>(selectedGroup.Products.ToList());
            productsGrid.DataSource = _groupProducts;
        }

        private void gvProductGroups_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            SetProductsListDataSource();
        }

        private void btnProductAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProductDetailsForm form = new ProductDetailsForm();
            form.ShowDialog();

            if (form.UpdatedProductId == null)
            {
                return;
            }

            int? updatedProductId = form.UpdatedProductId;
            Product product = _products.GetProduct(updatedProductId.Value);

            _productGroups.Single(g => g.CategoryId == product.Category.CategoryId).Products.Add(product);
            _groupProducts.Add(product);
        }

        private void btnProductEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            Product selectedProduct = GetSelectedProduct();
            if (selectedProduct == null)
            {
                return;
            }

            ProductDetailsForm form = new ProductDetailsForm(selectedProduct);
            form.ShowDialog();

            if (form.UpdatedProductId == null)
            {
                return;
            }

            int? updatedProductId = form.UpdatedProductId;
            Product product = _products.GetProduct(updatedProductId.Value);
            ProductCategory category = _productGroups.Single(g => g.CategoryId == selectedProduct.Category.CategoryId);

            category.Products.Remove(selectedProduct);
            category.Products.Add(product);
            _groupProducts.Remove(selectedProduct);
            _groupProducts.Add(product);
        }

        private Product GetSelectedProduct()
        {
            if (ViewMode == ViewMode.ProductSearch)
            {
                return _products.GetProduct(productSearchResults.CurrentProduct.ProductId);
            }

            return productsView.GetFocusedRow() as Product;
        }

        public static AuditContext CreateAuditContext(string contextName)
        {
            return new AuditContext
            {
                Source = "Product tool",
                ContextName = contextName,
                UserName = AuthorizedUser.UserName,
                UserIp = LocalIPAddress(),
            };
        }

        private static string LocalIPAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress firstOrDefault =
                host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return firstOrDefault?.ToString();
        }

        private void btnProductDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            Product selectedProduct = productsView.GetFocusedRow() as Product;

            if (selectedProduct == null)
            {
                return;
            }

            if (MessageBox.Show(
                    "Are you sure you want to delete the selected product?",
                    "Mandara Products",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            AuditContext auditContext = CreateAuditContext("Product");
            bool result;

            try
            {
                result = _products.DeleteProduct(selectedProduct, auditContext);
            }
            catch (InvalidOperationException invalidOp)
            {
                MessageBox.Show(
                    this,
                    string.Format(
                        "Unable to delete the selected product.  It may already have been deleted. ('{0}')",
                        invalidOp.Message),
                    "Delete Product",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }

            if (!result)
            {
                MessageBox.Show(
                    "Unable to delete this product because it is linked with the source data for the reporting system or/and parsed messages.",
                    "Mandara Products",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                _productGroups.Single(g => g.CategoryId == selectedProduct.Category.CategoryId)
                              .Products.Remove(selectedProduct);
                _groupProducts.Remove(selectedProduct);
            }
        }

        private void btnOfficialProductAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowOfficialProductEditor(null);
        }

        private void ShowOfficialProductEditor(OfficialProduct offProd)
        {
            OfficialProductDetailsForm offProdEditor = new OfficialProductDetailsForm(offProd, _products);

            if (!CanShowEditor())
            {
                return;
            }

            offProdEditor.SaveOfficialProduct += OnOfficialProductSaveAttemptCompleted;
            offProdEditor.ShowDialog();

            bool CanShowEditor()
            {
                return (null != offProd) || offProdEditor.CanShow;
            }
        }

        private void OnOfficialProductSaveAttemptCompleted(
            object sender,
            (bool saveSuccessful, OfficialProduct targetOffProd) saveResult)
        {
            OfficialProductDetailsForm offProdEditor = (OfficialProductDetailsForm)sender;

            offProdEditor.SaveOfficialProduct -= OnOfficialProductSaveAttemptCompleted;

            if (!saveResult.saveSuccessful)
            {
                MessageBox.Show(
                    "Error attempting to save official product.  Check the log for more information.",
                    "Official Product Save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                int updatedProductId = offProdEditor.UpdatedProductId;
                OfficialProduct product = _products.GetOfficialProduct(updatedProductId);

                _officialProducts.Remove(product);
                _officialProducts.Add(saveResult.targetOffProd);
                officialProductsGrid.RefreshDataSource();
            }
        }

        private void btnOfficialProductEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowOfficialProductEditor(officialProductsView.GetFocusedRow() as OfficialProduct);
        }

        private void btnOfficialProductDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            OfficialProduct selectedProduct = officialProductsView.GetFocusedRow() as OfficialProduct;

            if (selectedProduct == null)
            {
                return;
            }

            if (MessageBox.Show(
                    $"Are you sure you want to delete the official product '{selectedProduct.DisplayName}'?",
                    "Delete Official Product",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            AuditContext auditContext = CreateAuditContext("Official Product");
            bool wasDeleted = _products.DeleteOfficialProduct(selectedProduct, auditContext);

            if (!wasDeleted)
            {
                MessageBox.Show(
                    $"Cannot delete '{selectedProduct.DisplayName}' - it is linked to products and/or parser settings.",
                    "Delete Official Product Failure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                _officialProducts.Remove(selectedProduct);
                officialProductsGrid.RefreshDataSource();
            }
        }

        private void btnCommonViewMode_DownChanged(object sender, ItemClickEventArgs e)
        {
            if ((sender is BarButtonItem) && !(sender as BarButtonItem).Down)
            {
                return;
            }

            if (_viewModes.TryGetValue((BarButtonItem)sender, out ViewMode nextViewMode))
            {
                ViewMode = nextViewMode;
                OnViewModeChanged();
            }
        }

        private ViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                if (_viewMode != value)
                {
                    _viewMode = value;
                    OnViewModeChanged();
                }
            }
        }

        private void OnViewModeChanged()
        {
            if (AuthorizationService.IsUserAuthorizedTo(AuthorizedUser, PermissionType.ProductMgmtToolWriteAccess))
            {
                productGroupsRibbon.Visible = ViewMode == ViewMode.Products;
                brokersRibbon.Visible = ViewMode == ViewMode.Brokers;
                companiesRibbon.Visible = ViewMode == ViewMode.Companies;
                tradeTemplatesModifyGroup.Visible = ViewMode == ViewMode.TradeTemplates;
            }
            else
            {
                productGroupsRibbon.Visible = false;
                brokersRibbon.Visible = false;
                companiesRibbon.Visible = false;
                tradeTemplatesModifyGroup.Visible = false;
            }

            officialProductsGrid.Visible = ViewMode == ViewMode.OfficialProducts;

            productsViewContainer.Visible = ViewMode == ViewMode.Products;
            productsGrid.Visible = ViewMode == ViewMode.Products;
            productGroupsGrid.Visible = ViewMode == ViewMode.Products;

            defaultProductsGrid.Visible = ViewMode == ViewMode.DefaultProducts;
            brokersGrid.Visible = ViewMode == ViewMode.Brokers;
            companiesGrid.Visible = ViewMode == ViewMode.Companies;
            tradeTemplatesGrid.Visible = ViewMode == ViewMode.TradeTemplates;

            officialProductsModifyGroup.Visible = ViewMode == ViewMode.OfficialProducts;
            productsModifyGroup.Visible = ViewMode == ViewMode.Products || ViewMode == ViewMode.ProductSearch;
            defaultProductsModifyGroup.Visible = ViewMode == ViewMode.DefaultProducts;

            productSearchResults.Visible = ViewMode == ViewMode.ProductSearch;

            if (ViewMode == ViewMode.ProductSearch)
            {
                foreach (BarButtonItemLink link in viewModeGroup.ItemLinks)
                {
                    link.Item.AllowAllUp = true;
                    link.Item.Down = false;
                }
            }

            searchGroup.Visible = true;

            exchangesGrid.Visible = ViewMode == ViewMode.Exchanges;
            exchangesRibbon.Visible = ViewMode == ViewMode.Exchanges;

            unitsGrid.Visible = ViewMode == ViewMode.Units;
            unitsModifyGroup.Visible = ViewMode == ViewMode.Units;

            currenciesGrid.Visible = ViewMode == ViewMode.Currencies;
            currenciesModifyGroup.Visible = ViewMode == ViewMode.Currencies;

            productBreakdown.Visible = ViewMode == ViewMode.ProductBreakdown;

            UpdateDeskGroupControls();
        }

        private void btnGroupAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            GroupDetailsForm form = new GroupDetailsForm();
            form.ShowDialog();

            if (form.UpdatedGroupId == null)
            {
                return;
            }
            int? updatedGroupId = form.UpdatedGroupId;
            ProductCategory group = _products.GetGroup(updatedGroupId.Value);
            _productGroups.Add(@group);
            productGroupsGrid.RefreshDataSource();
        }

        private void btnGroupEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProductCategory selectedGroup = productGroupsView.GetFocusedRow() as ProductCategory;
            if (selectedGroup == null)
            {
                return;
            }

            GroupDetailsForm form = new GroupDetailsForm(selectedGroup);
            form.ShowDialog();

            if (form.UpdatedGroupId != null)
            {
                int? updatedGroupId = form.UpdatedGroupId;
                ProductCategory group = _products.GetGroup(updatedGroupId.Value);
                selectedGroup.Name = group.Name;
                productGroupsGrid.RefreshDataSource();
            }
        }

        private void btnGroupDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProductCategory selectedGroup = productGroupsView.GetFocusedRow() as ProductCategory;
            if (selectedGroup == null)
            {
                return;
            }

            if (MessageBox.Show(
                    "Are you sure you want to delete the selected group?",
                    "Mandara Products",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            AuditContext auditContext = CreateAuditContext("Product Group");

            bool result = _products.DeleteGroup(selectedGroup, auditContext);
            if (!result)
            {
                MessageBox.Show(
                    "Unable to delete this group because it has products associated with it.",
                    "Mandara Products",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                _productGroups.Remove(selectedGroup);
                productGroupsGrid.RefreshDataSource();
            }
        }

        private void btnDefaultProductsSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtDummy.Focus();
            _products.SaveDefaultProducts(_defaultProducts);
        }

        private void btnAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void btnExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }

        private bool GridValid(GridView gridView)
        {
            foreach (GridColumn column in gridView.Columns)
            {
                for (int i = 0; i < gridView.DataRowCount; i++)
                {
                    if (!IsCellValueValid(gridView, i, column))
                    {
                        gridView.FocusedRowHandle = i;
                        gridView.SetColumnError(column, $"{column.Caption} is required");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsCellValueValid(GridView gridView, int rowHandle, GridColumn column)
        {
            bool isValid = true;

            switch (column.FieldName)
            {
                case "YahooId":
                case "CompanyName":
                case "Region":
                {
                    isValid = !string.IsNullOrWhiteSpace(GetRowCellValue(gridView, rowHandle, column));
                }
                break;

                case "Name":
                {
                    if (gridView == exchangesView)
                    {
                        isValid = !string.IsNullOrWhiteSpace(GetRowCellValue(gridView, rowHandle, column));
                    }
                }
                break;
            }

            return isValid;
        }

        private static string GetRowCellValue(GridView gridView, int rowHandle, GridColumn column)
        {
            object rowCellValue = gridView.GetRowCellValue(rowHandle, column);

            return rowCellValue?.ToString();
        }

        private void btnBrokersSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtDummy.Focus();

            if (!GridValid(brokersView))
            {
                return;
            }

            AuditContext auditContext = CreateAuditContext("Brokers");

            _brokerManager.SaveBrokers(_brokersBindingList.ToList(), auditContext);
        }

        private void btnExchangesSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtDummy.Focus();

            if (!GridValid(exchangesView))
            {
                return;
            }

            AuditContext auditContext = CreateAuditContext("Exchagnes");

            _products.SaveExchanges(_exchanges.ToList(), auditContext);
        }

        private void btnCompaniesSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtDummy.Focus();

            if (!GridValid(companiesView))
            {
                return;
            }

            AuditContext auditContext = CreateAuditContext("Companies");
            _brokerManager.SaveCompanies(_companies.ToList(), auditContext, out List<string> nodeleteBrokers, out List<string> nodeleteBrokerage);

            //prepare warning message if we got companies which an not be deleted
            StringBuilder warningBuilder = new StringBuilder();
            if (nodeleteBrokers.Count > 0)
            {
                warningBuilder.Append(AddCompaniesWarning(nodeleteBrokers));
                warningBuilder.AppendLine(" not deleted. Remove brokers first.");
            }

            if (nodeleteBrokerage.Count > 0)
            {
                warningBuilder.Append(AddCompaniesWarning(nodeleteBrokerage));
                warningBuilder.Append(" not deleted. Remove product brokerage references first.");
            }

            string warning = warningBuilder.ToString();

            if (!string.IsNullOrEmpty(warning))
            {
                MessageBox.Show(
                    warning,
                    "Several companies not deleted.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            _companies = new BindingList<Company>(new List<Company>(_brokerManager.GetCompanies()));
            brokersCompanyRepo.DataSource = _companies;
            companiesGrid.DataSource = _companies;
        }

        private StringBuilder AddCompaniesWarning(List<string> companies)
        {
            StringBuilder companiesWarning = new StringBuilder(companies.Count == 1 ? "Company " : "Companies ");
            companiesWarning.Append(string.Join(", ", companies));
            return companiesWarning;
        }

        private void btnCalendar_ItemClick(object sender, ItemClickEventArgs e)
        {
            CalendarsForm calendarsForm = new CalendarsForm(AuthorizedUser);
            calendarsForm.ShowDialog();
            exchangeCalendarSelector.DataSource = _products.GetCalendars().OrderBy(x => x.Name);
        }

        private void ProductListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor = Cursors.Default;

            if (!_onCloseAuditWritten)
            {
                WriteLoginAudit("User Logout", () => { });
            }

            if (BusClient != null)
            {
                BusClient.Stop();
            }
        }

        private void btnCompanyEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            Company selectedCompany = companiesView.GetFocusedRow() as Company;
            if (selectedCompany == null)
            {
                return;
            }

            CompanyDetailsForm form = new CompanyDetailsForm(selectedCompany);
            form.ShowDialog();

            if (form.CompanyUpdated.HasValue && form.CompanyUpdated.Value)
            {
                _companies = new BindingList<Company>(new List<Company>(_brokerManager.GetCompanies()));
                companiesGrid.DataSource = _companies;
            }
        }

        private void btnSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            DoProductSearch(cleanSearchString());
        }

        private void DoProductSearch(string searchString)
        {
            int deskId = (int)deskSelector.EditValue;

            if (string.IsNullOrEmpty(searchString))
            {
                return;
            }

            productSearchResults.Search(searchString, deskId);

            ViewMode = ViewMode.ProductSearch;
        }

        private string cleanSearchString()
        {
            return ((string)searchString.EditValue ?? "").Trim();
        }

        private void gcExchanges_EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button != exchangesGrid.EmbeddedNavigator.Buttons.Remove)
            {
                return;
            }

            Exchange exchange = exchangesView.GetRow(exchangesView.FocusedRowHandle) as Exchange;
            if (exchange == null)
            {
                return;
            }

            if (!_products.CanDeleteExchange(exchange))
            {
                XtraMessageBox.Show(
                    this,
                    $"The {exchange.Name} exchange cannot be deleted because it is linked to some products.",
                    "Product Tool",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                e.Handled = true;
            }
        }

        private void btnTemplateAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            TradeTemplateDetailsForm tradeTemplateDetailsForm = new TradeTemplateDetailsForm();
            tradeTemplateDetailsForm.ShowDialog();

            RefreshTradesTemplates();
        }

        private void btnTemplateEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            TradeTemplate tradeTemplate = tradeTemplatesView.GetFocusedRow() as TradeTemplate;

            TradeTemplateDetailsForm tradeTemplateDetailsForm = new TradeTemplateDetailsForm(tradeTemplate);
            tradeTemplateDetailsForm.ShowDialog();

            RefreshTradesTemplates();
        }

        private void btnTemplateDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            TradeTemplate tradeTemplate = tradeTemplatesView.GetFocusedRow() as TradeTemplate;

            if (tradeTemplate == null)
            {
                return;
            }

            if (XtraMessageBox.Show(
                    $"Are you sure you want to delete trade template [{tradeTemplate.TemplateName}]",
                    "Delete confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new TradeTemplatesManager().Delete(tradeTemplate);
            }

            RefreshTradesTemplates();
        }

        private void btnTradeTemplates_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (showTradeTemplates.Down)
            {
                RefreshTradesTemplates();
            }
        }

        private void RefreshTradesTemplates()
        {
            tradeTemplatesGrid.DataSource = new TradeTemplatesManager().GetTradeTemplates();
        }

        private void btnProductRecalculateChanged_ItemClick(object sender, ItemClickEventArgs e)
        {
            RecalculateDetails(RecalcMode.ChangedProducts);
        }

        private void btnProductRecalculateAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            RecalculateDetails(RecalcMode.AllData);
        }

        private void RecalculateDetails(RecalcMode mode)
        {
            string warning =
                "Are you sure you want to recalculate positions for all products? This may take a few minutes.";

            switch (mode)
            {
                case RecalcMode.ChangedProducts:
                warning =
                    "Are you sure you want to recalculate positions for changed products? This may take a few minutes.";
                break;

                case RecalcMode.ManualTrades:
                warning =
                    "Are you sure you want to recalculate positions for manual trades? This may take a few minutes.";
                break;
            }

            if (mode == RecalcMode.NewSourceDetails
                || XtraMessageBox.Show(
                    warning,
                    "Recalculate confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string returnMessage;

                using (ProgressForm progressForm = new ProgressForm(
                    new PrecalcDetailRecalculator().RecalculatePrecalcPositions,
                    mode))
                {
                    progressForm.ShowDialog();
                    returnMessage = progressForm.returnMessage;
                }

                XtraMessageBox.Show($"Mode: {mode}{Environment.NewLine}{returnMessage}", "Position calculation");
            }
        }

        private void btnRecalculateManualTrades_ItemClick(object sender, ItemClickEventArgs e)
        {
            RecalculateDetails(RecalcMode.ManualTrades);
        }

        private void RecalculateAllSilent()
        {
            using (ProgressForm progressForm = new ProgressForm(
                new PrecalcDetailRecalculator().RecalculatePrecalcPositions,
                RecalcMode.AllData))
            {
                progressForm.ShowDialog();
            }

            Close();
        }

        private void btCalculate_Click(object sender, EventArgs e)
        {
            if (ProductBreakdownModel.CurrentProduct == null)
            {
                XtraMessageBox.Show(this, "Product not selected", "Product Breakdown Tool", MessageBoxButtons.OK);
                return;
            }

            Cursor = Cursors.WaitCursor;

            BusClient.GetProductBreakdown(
                new ProductBreakdownRequestMessage
                {
                    ProductId = ProductBreakdownModel.CurrentProduct.ProductId,
                    ContractMonth = ProductBreakdownModel.ContractMonth.DateTime,
                    EndDay = ProductBreakdownModel.EndDayApplicable ? ProductBreakdownModel.EndDay : (DateTime?)null,
                    Price = ProductBreakdownModel.StartPrice,
                    PurchaseDay = ProductBreakdownModel.PurchaseDay,
                    Quantity = ProductBreakdownModel.Quantity,
                    StartDay = ProductBreakdownModel.StartDayApplicable
                        ? ProductBreakdownModel.StartDay
                        : (DateTime?)null
                },
                ProductBreakdown_Callback,
                ProductBreakdown_CallbackFailure);
        }

        private void ProductBreakdown_CallbackFailure(FailureCallbackInfo info)
        {
            Invoke(
                new Action(
                    () =>
                    {
                        XtraMessageBox.Show(
                            this,
                            "Cannot get product breakdown.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Cursor = Cursors.Default;
                    }));
        }

        private void ProductBreakdown_Callback(ProductBreakdownResponseMessage message)
        {
            Invoke(
                new Action(
                    () =>
                    {
                        ProductBreakdownModel.BreakdownItems.Clear();
                        ProductBreakdownModel.CalculationDetailsAll.Clear();

                        ProductBreakdownModel.Holidays = message.Holidays;
                        ProductBreakdownModel.Fees = message.Fees;
                        ProductBreakdownModel.ErrorMessage = message.ErrorMessage;
                        foreach (ProductBreakdownItem item in message.ProductBreakdownItems)
                        {
                            BreakdownItem breakdownItem = new BreakdownItem
                            {
                                Day = item.Day,
                                LivePnl = item.LivePnl,
                                LivePrice = item.LivePrice,
                                Overnight = item.OvernightPnl,
                                SettlementPrice = item.Settlement,
                                Leg1 = item.Leg1Settlement,
                                Leg2 = item.Leg2Settlement,
                            };

                            ProductBreakdownModel.CalculationDetailsAll.Add(breakdownItem, item.Positions.ToArray());
                            ProductBreakdownModel.BreakdownItems.Add(breakdownItem);
                        }
                        ProductBreakdownModel.BreakdownItems.ResetBindings();

                        testTradeLeg1Settle.Visible = ProductBreakdownModel.BreakdownItemLegsApplicable;
                        testTradeLeg2Settle.Visible = ProductBreakdownModel.BreakdownItemLegsApplicable;

                        FeesMode.Enabled = ProductBreakdownModel.BreakdownItems != null;
                        holidayCalendarsMode.Enabled = ProductBreakdownModel.BreakdownItems != null;

                        Cursor = Cursors.Default;

                        if (!string.IsNullOrEmpty(ProductBreakdownModel.ErrorMessage))
                        {
                            XtraMessageBox.Show(
                                this,
                                ProductBreakdownModel.ErrorMessage,
                                "Product Breakdown Tool",
                                MessageBoxButtons.OK);
                        }
                    }));
        }

        private void gvProductBreakdown_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            ProductBreakdownModel.CurrentBreakdownItem = productBreakdownView.GetRow(e.FocusedRowHandle) as BreakdownItem;
        }

        private void btFees_Click(object sender, EventArgs e)
        {
            using (BreakdownToolFeesForm form = new BreakdownToolFeesForm())
            {
                form.Fees = ProductBreakdownModel.Fees;
                form.ShowDialog();
            }
        }

        private void leOfficialProduct_EditValueChanged(object sender, EventArgs e)
        {
            ProductBreakdownModel.CurrentOfficialProduct = officialProductSelector.EditValue as OfficialProduct;
        }

        private void btHolidays_Click(object sender, EventArgs e)
        {
            using (BreakdownToolHolidaysForm form = new BreakdownToolHolidaysForm())
            {
                form.Holidays = ProductBreakdownModel.Holidays;
                form.ShowDialog();
            }
        }

        private void leProduct_EditValueChanged(object sender, EventArgs e)
        {
            ProductBreakdownModel.CurrentProduct = productSelector.EditValue as Product;
        }

        private void UpdateTitleBar()
        {
            string serverName = "Primary Server";
            ServersConfigurationSection serversSection =
                ConfigurationManager.GetSection("ServersSection") as ServersConfigurationSection;

            if (serversSection != null)
            {
                foreach (ServerConfigurationElement serverDef in serversSection.Servers)
                {
                    if (serverDef.Prefix.Equals(
                        InformaticaHelper.ServerPrefix,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        serverName = serverDef.Name;
                    }
                }
            }

            Text = $"{"Mandara Product Tool"} - v.{Assembly.GetEntryAssembly().GetName().Version} - {serverName}";
        }

        private void btnExchanges_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnUnits_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!showUnits.Down)
            {
                return;
            }

            _units = new BindingList<Unit>(_products.GetUnits());

            unitsGrid.DataSource = _units;
        }

        private void gcUnits_EmbeddedNavigator_Click(object sender, EventArgs e)
        {

        }

        private void gcUnits_Click(object sender, EventArgs e)
        {

        }

        private void gcUnits_EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button != unitsGrid.EmbeddedNavigator.Buttons.Remove)
            {
                return;
            }

            Unit unit = unitsView.GetRow(unitsView.FocusedRowHandle) as Unit;

            if (unit == null)
            {
                return;
            }

            if (_products.CanDeleteUnit(unit))
            {
                return;
            }

            XtraMessageBox.Show(
                this,
                $"The {unit.Name} unit cannot be deleted because it is linked to some entities.",
                "Product Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            e.Handled = true;
        }

        private void btnUnitsSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtDummy.Focus();
            _products.SaveUnits(_units.ToList());
        }

        private void btnCurrencies_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!showCurrencies.Down)
            {
                return;
            }

            _currencies = new BindingList<CurrencyModel>(
                _products.GetCurrencies().Select(it => new CurrencyModel(it)).ToList());

            currenciesGrid.DataSource = _currencies;

            if (_reCurrencyTextEdit == null)
            {
                _reCurrencyTextEdit = new RepositoryItemTextEdit();
                PropertyInfo isoNameProp = typeof(Currency).GetProperty("IsoName");
                StringLengthAttribute stringLengthAttr =
                    isoNameProp.GetCustomAttribute(typeof(StringLengthAttribute)) as StringLengthAttribute;
                _reCurrencyTextEdit.MaxLength = stringLengthAttr?.MaximumLength ?? 3;
            }

            currencyIsoName.ColumnEdit = _reCurrencyTextEdit;
        }

        private void btnCurrenciesSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtDummy.Focus();
            try
            {
                _products.SaveCurrencies(_currencies.Select(it => it.Currency).ToList());
            }
            catch (Exception ex)
            {
                _currencies.Clear();
                _products.GetCurrencies().ForEach(it => { _currencies.Add(new CurrencyModel(it)); });

                XtraMessageBox.Show(
                    this,
                    $"An error occured during currencies saving: {ex.Message}.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void gcCurrencies_EmbeddedNavigator_Click(object sender, EventArgs e)
        {

        }

        private void gcCurrencies_EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button != currenciesGrid.EmbeddedNavigator.Buttons.Remove)
            {
                return;
            }

            Currency currency = currenciesView.GetRow(currenciesView.FocusedRowHandle) as Currency;

            if (currency == null)
            {
                return;
            }

            if (_products.CanDeleteCurrency(currency))
            {
                return;
            }

            XtraMessageBox.Show(
                this,
                $"The {currency.IsoName} currency cannot be deleted because it is linked to some official products.",
                "Product Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            e.Handled = true;
        }

        private void gvCurrencies_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            GridView view = sender as GridView;
            string isoName = (string)view.GetRowCellValue(e.RowHandle, currencyIsoName);

            if (!_products.ValidateCurrency(isoName))
            {
                e.Valid = false;
                e.ErrorText = "Currency ISO name must follow ISO-4217.";
            }
        }

        private void dePurchaseDay_DateTimeChanged(object sender, EventArgs e)
        {
            DateEdit dateEdit = sender as DateEdit;
            DateTime setValue = dateEdit.DateTime;
            int dayOffset = 0;

            dateEdit.DateTimeChanged -= dePurchaseDay_DateTimeChanged;

            if (setValue.DayOfWeek == DayOfWeek.Saturday)
            {
                dayOffset = 2;
            }
            else if (setValue.DayOfWeek == DayOfWeek.Sunday)
            {
                dayOffset = 1;
            }

            dateEdit.DateTime = setValue.AddDays(dayOffset);
            dateEdit.DateTimeChanged += dePurchaseDay_DateTimeChanged;
        }

        private void OnSearchStringChanged(object sender, EventArgs searchChanged)
        {
            DoProductSearch(cleanSearchString());
        }

        private void deskSelector_EditValueChanged(object sender, EventArgs e)
        {
            productSearchResults.Search(cleanSearchString(), (int)deskSelector.EditValue);
            ViewMode = ViewMode.ProductSearch;
        }

        private void OnAddDesk(object sender, ItemClickEventArgs clickedBtn)
        {
            desksConfigurator.NewDesk();
        }

        private void OnEditDesk(object sender, ItemClickEventArgs clickEdit)
        {
            desksConfigurator.EditDesk();
        }

        private void OnDeleteDesk(object sender, ItemClickEventArgs clickedBtn)
        {
            desksConfigurator.DeleteDesk();
        }

        private void OnAddDeskOfficialProduct(object sender, ItemClickEventArgs clickedBtn)
        {
            desksConfigurator.NewOfficialProduct();
        }

        private void OnEditDeskOfficialProduct(object sender, ItemClickEventArgs clickEdit)
        {
            desksConfigurator.EditOfficialProduct();
        }

        private void OnDeleteDeskOfficialProduct(object sender, ItemClickEventArgs clickedBtn)
        {
            desksConfigurator.DeleteOfficialProduct();
        }
    }
}