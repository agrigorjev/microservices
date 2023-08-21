using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.EditForm.Helpers;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Mandara.Business;
using Mandara.Business.Managers.Products;
using Mandara.Entities;
using Mandara.ProductGUI.Desks;
using Mandara.ProductGUI.Desks.OfficialProducts;

namespace Mandara.ProductGUI
{
    public partial class DesksConfigurator : XtraUserControl
    {
        internal IDesksRepository DeskSource { get; set; }
        internal IDeskProductsRepository DeskProducts { get; set; }
        public bool HasSelectedDesk { get; private set; }
        public bool HasSelectedProduct { get; private set; }
        public ProductManager Products { get; set; }

        private bool _canConfigureDesks;

        private const string EmptyDeskErrorMsg = "Desk name cannot be empty.";
        private SortedList<string, OfficialProduct> _officialProductsForSelection;

        public event EventHandler HasSelectedDeskChanged;
        public event EventHandler HasSelectedDeskProductChanged;

        public DesksConfigurator()
        {
            InitializeComponent();
        }

        private void DesksConfigurator_Load(object sender, EventArgs e)
        {
            SetUpProductsManager();
            desksGrid.DataSource = new BindingList<Desk>(DeskSource.Desks);

            if (!DeskSource.CanAccessDesks())
            {
                return;
            }

            desksView.FocusInvalidRow();
            _officialProductsForSelection = GetOfficialProductsForSelection(Products.GetOfficialProducts());

            if (DeskSource.Desks.Any())
            {
                SetUpDeskProductsView();
            }
        }

        private void SetUpProductsManager()
        {
            Products.RegisterForAddedOfficialProduct(OnOfficialProductAdded);
            Products.RegisterForDeletedOfficialProduct(OnOfficialProductRemoved);
            Products.RegisterForChangedOfficialProduct(OnOfficialProductChanged);
        }

        private void SetUpDeskProductsView()
        {
            desksProductsView.Columns[nameof(OfficialProductForDesk.DeskId)].Visible = false;
            desksProductsView.Columns[nameof(OfficialProductForDesk.AlertGroupId)].Visible = false;
            desksProductsView.Columns[nameof(OfficialProductForDesk.Key)].Visible = false;
            desksProductsView.Columns[nameof(OfficialProductForDesk.OfficialProductId)].ColumnEdit =
                SetUpOfficialProductNameDisplay();
        }

        private RepositoryItemLookUpEdit SetUpOfficialProductNameDisplay()
        {
            RepositoryItemLookUpEdit productCombo = new RepositoryItemLookUpEdit
            {
                DataSource = _officialProductsForSelection.Values
            };

            SetProductData(productCombo);
            return productCombo;
        }

        private SortedList<string, OfficialProduct> GetOfficialProductsForSelection(
            List<OfficialProduct> allOfficialProducts)
        {
            return allOfficialProducts.Aggregate(new SortedList<string, OfficialProduct>(), (sortedOffProds, offProd) =>
            {
                sortedOffProds.Add(offProd.Name, offProd);
                return sortedOffProds;
            });
        }

        private static void SetProductData(RepositoryItemLookUpEdit productCombo)
        {
            productCombo.DisplayMember = nameof(OfficialProduct.Name);
            productCombo.ValueMember = nameof(OfficialProduct.OfficialProductId);
            productCombo.PopulateColumns();

            Enumerable.Range(0, productCombo.Columns.Count).Reverse().ForEach(col =>
                {
                    if (productCombo.Columns[col].FieldName != nameof(OfficialProduct.Name))
                    {
                        productCombo.Columns.Remove(productCombo.Columns[col]);
                    }
                });
        }

        private void OnDeskValidateRow(object sender, ValidateRowEventArgs changedRow)
        {
            if (!(changedRow.Row is Desk))
            {
                return;
            }

            Desk desk = changedRow.Row as Desk;

            if (desk.IsDefault)
            {
                changedRow.Valid = false;
                changedRow.ErrorText = EmptyDeskErrorMsg;
                return;
            }

            if (changedRow.RowHandle == GridControl.NewItemRowHandle)
            {
                SaveDesk(desk, DeskSource.Add, changedRow);
            }
            else
            {
                SaveDesk(desk, DeskSource.Update, changedRow);
            }
        }

        private void SaveDesk(Desk desk, Func<Desk, DatabaseActionResult> saveAction, ValidateRowEventArgs changedRow)
        {
            DatabaseActionResult outcome = saveAction(desk);

            if (!outcome.Success)
            {
                changedRow.Valid = false;
                changedRow.ErrorText = outcome.Message;
            }
        }

        private void OnDeskValidatingEditor(object sender, BaseContainerValidateEditorEventArgs editor)
        {
            string deskName = ((editor.Value as string) ?? string.Empty).Trim();

            if (deskName.Length == 0)
            {
                editor.Valid = false;
                editor.ErrorText = "Desk name cannot be empty.";
                return;
            }

            Desk duplicate = DeskSource.GetDesk(deskName);

            if (!duplicate.IsDefault)
            {
                editor.Valid = false;
                editor.ErrorText = "Desk with this name already exists.";
            }
        }

        private void OnDesksFocusedRowChanged(object sender, FocusedRowChangedEventArgs focusedRow)
        {
            HasSelectedDesk = focusedRow.FocusedRowHandle != GridControl.InvalidRowHandle
                              && focusedRow.FocusedRowHandle != GridControl.NewItemRowHandle;

            HasSelectedDeskChanged?.Invoke(this, EventArgs.Empty);
            SelectConfigurationsForCurrentDesk();
        }

        private void SelectConfigurationsForCurrentDesk()
        {
            desksProductsGrid.DataSource =
                new BindingList<OfficialProductForDesk>(DeskProducts.OfficialProductsForDesk(SelectedDesk().Id));
            desksProductsGrid.Enabled = HasSelectedDesk;
        }

        private Desk SelectedDesk()
        {
            int elementSelected = desksView.ViewRowHandleToDataSourceIndex(desksView.FocusedRowHandle);

            if (elementSelected >= 0 && elementSelected < DeskSource.TotalDesks())
            {
                return DeskSource.Desks.ElementAt(elementSelected);
            }

            return Desk.Default;
        }

        private void OnProductsFocusedRowChanged(object sender, FocusedRowChangedEventArgs focusedRow)
        {
            HasSelectedProduct = focusedRow.FocusedRowHandle != GridControl.InvalidRowHandle
                              && focusedRow.FocusedRowHandle != GridControl.NewItemRowHandle;

            HasSelectedDeskProductChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnProductValidateRow(object sender, ValidateRowEventArgs changedRow)
        {
            if (!(changedRow.Row is OfficialProductForDesk))
            {
                return;
            }

            OfficialProductForDesk officialProduct = changedRow.Row as OfficialProductForDesk;

            if (changedRow.RowHandle == GridControl.NewItemRowHandle)
            {
                SaveProduct(officialProduct, DeskProducts.Add, changedRow);
            }
            else
            {
                SaveProduct(officialProduct, DeskProducts.Update, changedRow);
            }
        }

        private void SaveProduct(
            OfficialProductForDesk officialProduct,
            Func<OfficialProductForDesk, DatabaseActionResult> saveAction,
            ValidateRowEventArgs changedRow)
        {
            DatabaseActionResult outcome = saveAction(officialProduct);

            if (!outcome.Success)
            {
                changedRow.Valid = false;
                changedRow.ErrorText = outcome.Message;
            }
        }

        private void OnDeskProductValidatingEditor(
            object sender,
            BaseContainerValidateEditorEventArgs editFormValidation)
        {
            EditFormValidateEditorEventArgs editorValidation = editFormValidation as EditFormValidateEditorEventArgs;
            string changedField = editorValidation.Column.FieldName;
            OfficialProductForDesk currentProduct = SelectedProduct();
            (bool isValid, string invalidMsg) =
                ValidateOfficialProductKey(changedField, currentProduct, editorValidation);

            if (isValid)
            {
                (isValid, invalidMsg) = ValidateOfficialProductFields(changedField, editorValidation);
            }

            if (!isValid)
            {
                editorValidation.Valid = false;
                editorValidation.ErrorText = invalidMsg;
            }
        }

        private OfficialProductForDesk SelectedProduct()
        {
            int elementSelected = desksProductsView.ViewRowHandleToDataSourceIndex(desksProductsView.FocusedRowHandle);
            int numProductsForDesk = DeskProducts.TotalProducts();

            if (elementSelected >= 0 && elementSelected < numProductsForDesk)
            {
                return ((BindingList<OfficialProductForDesk>)(desksProductsView.DataSource))[elementSelected];
            }

            return OfficialProductForDesk.Default;
        }

        private (bool, string) ValidateOfficialProductKey(
            string changedField,
            OfficialProductForDesk currentProduct,
            EditFormValidateEditorEventArgs editorValidation)
        {
            if (HasProductKeyFieldChanged(changedField)
                && IsDuplicateProduct(currentProduct, changedField, (int)editorValidation.Value))
            {
                return (false,
                    $"This combination of product and strip type already exists for the {SelectedDesk().Name} desk.");
            }

            return (true, string.Empty);
        }

        private bool HasProductKeyFieldChanged(string changedField)
        {
            bool productChanged = nameof(OfficialProductForDesk.OfficialProductId) == changedField;
            bool productTypeChanged = nameof(OfficialProductForDesk.ProductType) == changedField;

            return productChanged || productTypeChanged;
        }

        private bool IsDuplicateProduct(OfficialProductForDesk currentProduct, string changedField, int newValue)
        {
            OfficialProductForDesk prodCopy = currentProduct.Copy();

            switch (changedField)
            {
                case nameof(OfficialProductForDesk.OfficialProductId):
                {
                    prodCopy.OfficialProductId = newValue;
                }
                break;


                case nameof(OfficialProductForDesk.ProductType):
                {
                    prodCopy.ProductType = (StripStructure)newValue;
                }
                break;
            }

            return prodCopy.IsKeyChanged() && DeskProducts.ProductExists(prodCopy);
        }

        private (bool, string) ValidateOfficialProductFields(
            string changedField,
            EditFormValidateEditorEventArgs editorValidation)
        {
            bool isValid = true;
            string invalidMsg = string.Empty;

            switch (changedField)
            {
                case nameof(OfficialProductForDesk.PricePosition):
                {
                    if (null == editorValidation.Value || !IsValidInteger(0, int.MaxValue, (int)editorValidation.Value))
                    {
                        isValid = false;
                        invalidMsg = "Price position must be positive or zero.";
                    }
                }
                break;

                case nameof(OfficialProductForDesk.PriceFactor):
                {
                    if (null == editorValidation.Value
                        || !IsValidDecimal(0.01M, decimal.MaxValue, (decimal)editorValidation.Value))
                    {
                        isValid = false;
                        invalidMsg = "Price factor must be positive.";
                    }
                }
                break;

                case nameof(OfficialProductForDesk.CacheLifeTimeMinutes):
                {
                    if (null == editorValidation.Value || !IsValidInteger(1, int.MaxValue, (int)editorValidation.Value))
                    {
                        isValid = false;
                        invalidMsg = "The cache lifetime must be at least 1 minute.";
                    }
                }
                break;

                case nameof(OfficialProductForDesk.ShowPriceAsAged):
                {
                    if (null == editorValidation.Value || !IsValidInteger(1, int.MaxValue, (int)editorValidation.Value))
                    {
                        isValid = false;
                        invalidMsg = "The trade must be allowed to be at least 1 minute old before displaying as aged.";
                    }
                }
                break;

                // This is not actually a sufficient check for ordering - order values must be unique for a desk unless
                // all are zero.  Confirm this in AVM.
                case nameof(OfficialProductForDesk.Order):
                {
                    if (null == editorValidation.Value || !IsValidInteger(0, int.MaxValue, (int)editorValidation.Value))
                    {
                        isValid = false;
                        invalidMsg = "The order must be positive or zero.";
                    }
                }
                break;
            }

            return (isValid, invalidMsg);
        }

        private bool IsValidInteger(int minValue, int maxValue, int currentValue)
        {
            return currentValue >= minValue && currentValue <= maxValue;
        }

        private bool IsValidDecimal(decimal minValue, decimal maxValue, decimal currentValue)
        {
            return currentValue >= minValue && currentValue <= maxValue;
        }

        public bool SelectedDeskHasProducts()
        {
            return DeskProducts.OfficialProductsForDesk(SelectedDesk().Id).Count > 0;
        }

        public void NewDesk()
        {
            desksView.AddNewRow();
            desksView.SelectRow(desksView.RowCount - 1);
            desksView.FocusedRowHandle = GridControl.NewItemRowHandle;

            desksView.ShowEditor();
        }

        public void EditDesk()
        {
            desksView.ShowEditForm();
        }

        public void DeleteDesk()
        {
            DatabaseActionResult outcome = DeskSource.Remove(SelectedDesk());

            if (!outcome.Success)
            {
                MessageBox.Show(outcome.Message, "Delete Desk", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                desksView.DeleteRow(desksView.FocusedRowHandle);
            }
        }

        public void NewOfficialProduct()
        {
            desksProductsView.AddNewRow();
            desksProductsView.SelectRow(desksProductsView.RowCount - 1);
            desksProductsView.FocusedRowHandle = GridControl.NewItemRowHandle;
            desksProductsView.ShowEditor();
        }

        public void EditOfficialProduct()
        {
            desksProductsView.Focus();
            desksProductsView.SelectRow(desksProductsView.FocusedRowHandle);
            desksProductsView.ShowEditForm();
        }

        public void DeleteOfficialProduct()
        {
            DatabaseActionResult outcome = DeskProducts.Delete(SelectedProduct());

            if (!outcome.Success)
            {
                MessageBox.Show(outcome.Message, "Delete Official Product", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SelectConfigurationsForCurrentDesk();
        }

        private void OnProductEditFormPrepared(object sender, EditFormPreparedEventArgs productEditForm)
        {
            GridView productsView = (GridView)sender;
            OfficialProductForDesk newProduct = productsView.GetFocusedRow() as OfficialProductForDesk;
            LookUpEdit offProdSelector = GetOfficialProductIdSelector(productEditForm.BindableControls);

            offProdSelector.Properties.DataSource = null;
            offProdSelector.Properties.DataSource = ((RepositoryItemLookUpEdit)desksProductsView
                                                                               .Columns[nameof(OfficialProductForDesk
                                                                                   .OfficialProductId)].ColumnEdit)
                .DataSource;
            SetNewOfficialProductDeskAndProductId(newProduct, offProdSelector);
            offProdSelector.Refresh();
        }

        private LookUpEdit GetOfficialProductIdSelector(EditFormBindableControlsCollection offProdEditFormControls)
        {
            return offProdEditFormControls.First(
                ctrl => (ctrl.Tag as string).Contains(nameof(OfficialProductForDesk.OfficialProductId))) as LookUpEdit;
        }

        private void SetNewOfficialProductDeskAndProductId(OfficialProductForDesk newProduct, LookUpEdit offProdSelector)
        {
            newProduct.DeskId = Desk.DefaultId == newProduct.DeskId ? SelectedDesk().Id : newProduct.DeskId;
            newProduct.OfficialProductId = OfficialProductForDesk.DefaultProductId == newProduct.OfficialProductId
                ? GetFirstOfficialProductId(offProdSelector)
                : newProduct.OfficialProductId;
        }

        private int GetFirstOfficialProductId(LookUpEdit offProdSelector)
        {
            offProdSelector.ItemIndex = 0;
            return ((OfficialProduct)offProdSelector.GetSelectedDataRow()).OfficialProductId;
        }

        private void OnOfficialProductAdded(object sender, OfficialProductChangeEventArgs changedOffProd)
        {
            _officialProductsForSelection.Add(changedOffProd.Changed.Name, changedOffProd.Changed);
            SetUpDeskProductsView();
        }

        private void OnOfficialProductRemoved(object sender, OfficialProductChangeEventArgs changedOffProd)
        {
            OfficialProduct changed = changedOffProd.Changed;

            _officialProductsForSelection.Remove(changed.Name);
            RemoveOfficialProductForDesks(changed);
            SetUpDeskProductsView();
        }

        private void RemoveOfficialProductForDesks(OfficialProduct removed)
        {
            DatabaseActionResult deleteResult = DeskProducts.Delete(removed.OfficialProductId);

            if (!deleteResult.Success)
            {
                MessageBox.Show(
                    String.Format(
                        "Failed to remove all Desk Official Products for '{0}' - {1}. Manual update is necessary",
                        removed.Name,
                        deleteResult.Message),
                    "Remove For Desks",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void OnOfficialProductChanged(object sender, OfficialProductChangeEventArgs changedOffProd)
        {
            _officialProductsForSelection.Remove(changedOffProd.PreviousName);
            _officialProductsForSelection.Add(changedOffProd.Changed.Name, changedOffProd.Changed);
            SetUpDeskProductsView();
        }
    }
}
