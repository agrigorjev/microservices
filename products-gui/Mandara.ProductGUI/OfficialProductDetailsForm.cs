using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Mandara.Business;
using Mandara.Business.Audit;
using Mandara.Business.Authorization;
using Mandara.Entities;
using Mandara.Entities.Entities;
using Mandara.Extensions.Collections;
using Optional;
using Optional.Unsafe;

namespace Mandara.ProductGUI
{
    internal partial class OfficialProductDetailsForm : XtraForm
    {
        private const string OfficialNameExistsError = "Official product with this name already exists.";
        private const string OfficialDisplayNameExistsError = "Official product with this display name already exists.";

        public int UpdatedProductId = OfficialProduct.DefaultId;

        private readonly ProductManager _products;
        private readonly BrokerManager _brokers = new BrokerManager();
        private OfficialProduct _theOfficialProduct;

        internal EventHandler<(bool, OfficialProduct)> SaveOfficialProduct;

        private TextEdit[] _plattsEdits;
        

        public bool CanShow => null != _theOfficialProduct;

        public OfficialProductDetailsForm(OfficialProduct product, ProductManager products)
        {
            _products = products;
            
            InitializeComponent();
            _plattsEdits = new []
            {
                plattsSymbolOrder, plattsSymbolName, plattsDiv, plattsMul
            };
            EditMask.SetNonZeroFloatMask(priceUnitToBblConversion);

            if (GetOfficialProduct(product))
            {
                BindCollectionFields();
                BindIndividualFields();
                symbolEnabled.Checked = _theOfficialProduct.PlattsSymbolConfig != null;
                UpdateControls();
            }
        }

        private bool GetOfficialProduct(OfficialProduct product)
        {
            if (product == null)
            {
                Option<OfficialProduct> defaultProduct = _products.GetDefaultOfficialProduct();

                if (!defaultProduct.HasValue)
                {
                    MessageBox.Show("Official products require a currency and unit, but neither is available.");
                    return false;
                }

                _theOfficialProduct = defaultProduct.ValueOr(OfficialProduct.Default);
            }
            else
            {
                _theOfficialProduct = _products.GetOfficialProduct(product.OfficialProductId);
            }

            return true;
        }

        private void BindCollectionFields()
        {
            region.Properties.DataSource = _brokers.GetRegions();
            currency.Properties.DataSource = _products.GetCurrencies();
            priceUnits.Properties.DataSource = _products.GetUnits();
            SetAvailableSettlementProducts();
        }

        private void SetAvailableSettlementProducts()
        {
            List<OfficialProduct> officialProducts =
                _products.GetOfficialProducts().OrderBy(x => x.DisplayName).ToList();

            officialProducts.Insert(0, null);
            settlementProduct.Properties.DataSource = officialProducts;

            settlementProduct.EditValue = officialProducts.FirstOrDefault(
                x => x != null && x.OfficialProductId == _theOfficialProduct.SettlementProductId);
        }

        private void BindIndividualFields()
        {
            fullName.DataBindings.Add("EditValue", _theOfficialProduct, "Name", true);
            displayName.DataBindings.Add("EditValue", _theOfficialProduct, "DisplayName", true);
            
            priceColumn.DataBindings.Add("EditValue", _theOfficialProduct, "MappingColumn", true);
            isAllowedForManualTrades.DataBindings.Add(
                "EditValue",
                _theOfficialProduct,
                "IsAllowedForManualTrades",
                true);

            region.DataBindings.Add("EditValue", _theOfficialProduct, "Region", true);
            currency.DataBindings.Add("EditValue", _theOfficialProduct, "Currency", true);

            priceUnits.Properties.DataSource = _products.GetUnits();
            priceUnits.DataBindings.Add("EditValue", _theOfficialProduct, "PriceUnit");
            priceUnitToBblConversion.DataBindings.Add(
                "EditValue",
                _theOfficialProduct,
                "UnitToBarrelConversionFactor",
                true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            if (symbolEnabled.Checked == false)
            {
                _theOfficialProduct.PlattsSymbolConfig = null;
            }

            (int idAfterSave, bool saveSuccessful) = SaveOfficialProductToDb();

            UpdatedProductId = saveSuccessful ? idAfterSave : OfficialProduct.DefaultId;
            SaveOfficialProduct?.Invoke(this, (saveSuccessful, _theOfficialProduct));
            Close();
        }

        private (int, bool) SaveOfficialProductToDb()
        {
            AuditContext auditContext = ProductListForm.CreateAuditContext("Official Product Details");

            if (settlementProduct.EditValue is OfficialProduct officialProduct)
            {
                _theOfficialProduct.SettlementProductId = officialProduct.OfficialProductId;
            }
            else
            {
                _theOfficialProduct.SettlementProductId = null;
            }

            return _products.SaveOfficialProduct(_theOfficialProduct, auditContext);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool ValidateForm()
        {
            bool isValid = ValidateRequiredString(fullName, "Full name");
            isValid &= ValidateRequiredString(displayName, "Display name");
            if (symbolEnabled.Checked)
            {
                isValid &= ValidateRequiredString(plattsSymbolName, "Platts symbol name");
            }

            return isValid;
        }

        private bool ValidateRequiredString(BaseEdit edit, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(edit.Text))
            {
                edit.ErrorText = $"'{fieldName}' is required";

                return false;
            }
            else
            {
                return true;
            }
        }

        

        private void OfficialProductDetailsForm_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.IsUserAuthorizedTo(
                ProductListForm.AuthorizedUser,
                PermissionType.ProductMgmtToolWriteAccess))
            {
                MakeEditControlsReadOnly(Controls);

                saveOfficialProduct.Visible = false;
                cancelChanges.Text = "Close";
            }
        }

        private void MakeEditControlsReadOnly(Control.ControlCollection controls)
        {
            foreach (var control in controls)
            {
                var baseEdit = control as BaseEdit;
                if (baseEdit != null)
                {
                    baseEdit.Properties.ReadOnly = true;
                }

                var groupControl = control as GroupControl;
                if (groupControl != null)
                {
                    MakeEditControlsReadOnly(groupControl.Controls);
                }
            }
        }

        private void DoesOfficialProductNameExist(object textEditor, CancelEventArgs cancelNameChange)
        {
            (Func<OfficialProduct, bool> isExistingName, string nameExistsError) = GetNameChangeValidation(textEditor);

            cancelNameChange.Cancel = ShouldCancelTextChange(isExistingName);
            ((TextEdit)textEditor).ErrorText = cancelNameChange.Cancel ? nameExistsError : string.Empty;
        }

        private (Func<OfficialProduct, bool>, string) GetNameChangeValidation(object textEditor)
        {
            return IsFullName(textEditor)
                ? (NameExists(fullName.Text), OfficialNameExistsError)
                : (DisplayNameExists(displayName.Text), OfficialDisplayNameExistsError);
        }

        private bool IsFullName(object textEditor)
        {
            return (textEditor as TextEdit) == fullName;
        }

        private Func<OfficialProduct, bool> NameExists(string newName)
        {
            return offProd => offProd.Name == newName;
        }

        private bool ShouldCancelTextChange(Func<OfficialProduct, bool> textExists)
        {
            return _products.GetOfficialProducts().Any(
                offProd => _theOfficialProduct.OfficialProductId != offProd.OfficialProductId
                           && textExists(offProd));
        }

        private Func<OfficialProduct, bool> DisplayNameExists(string newName)
        {
            return offProd => offProd.DisplayName == newName;
        }

        private void symbolEnabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            if (symbolEnabled.Checked)
            {
                EnableControls();
            }
            else
            {
                DisableControl();
            }
        }

        private void DisableControl()
        {
            _plattsEdits.ForEach(x => x.Enabled = false);
        }

        private void EnableControls()
        {
            if (_theOfficialProduct.PlattsSymbolConfig == null)
            {
                _theOfficialProduct.PlattsSymbolConfig = PlattsSymbolConfig.CreateDefaultValue(_theOfficialProduct);
            }

            _plattsEdits.ForEach(x => x.DataBindings.Clear());

            plattsSymbolName.DataBindings.Add("EditValue", _theOfficialProduct.PlattsSymbolConfig, "PlattsSymbol", true);
            plattsSymbolOrder.DataBindings.Add("EditValue", _theOfficialProduct.PlattsSymbolConfig, "Order", true);
            plattsDiv.DataBindings.Add("EditValue", _theOfficialProduct.PlattsSymbolConfig, "Div", true);
            plattsMul.DataBindings.Add("EditValue", _theOfficialProduct.PlattsSymbolConfig, "Mul", true);

            _plattsEdits.ForEach(x => x.Enabled = true);

        }
    }
}