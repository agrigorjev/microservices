using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Mandara.Business;
using Mandara.Entities;

namespace Mandara.ProductGUI
{
    public partial class GroupDetailsForm : DevExpress.XtraEditors.XtraForm
    {
        public Int32? UpdatedGroupId;

        private ProductManager productManager = new ProductManager();
        private ProductCategory GroupToEdit;
        private BindingList<SwapCrossPerProduct> _swapCrossPerProducts;

        public GroupDetailsForm(ProductCategory group = null)
        {
            InitializeComponent();

            if (group == null)
            {
                GroupToEdit = new ProductCategory
                {
                    Name = "New category"
                };
            }
            else
            {
                GroupToEdit = productManager.GetGroup(group.CategoryId);

                riCategoryProducts.DataSource =
                    GroupToEdit.Products
                               .Where(p => p.Type == ProductType.Swap ||
                                           p.Type == ProductType.FuturesBasedSwap)
                               .OrderBy(x => x.Name);

                riSwapCrossProducts.DataSource =
                    productManager.GetProducts()
                                  .Where(p => p.Type == ProductType.Balmo)
                                  .OrderBy(p => p.Name)
                                  .ToList();

                _swapCrossPerProducts = new BindingList<SwapCrossPerProduct>(GroupToEdit.SwapCrossPerProducts.ToList());
                gcSwapCrossPerProduct.DataSource = _swapCrossPerProducts;

                cmbProducts.Properties.Items.AddRange(GroupToEdit.Products.ToList());

                List<Product> swapCrossProducts = GroupToEdit.Products.Where(p => p.Type == ProductType.Futures).ToList();

                if (swapCrossProducts.Count > 0)
                {
                    cmbSwapCrossProduct.Properties.Items.Add(DBNull.Value);
                    cmbSwapCrossProduct.Properties.Items.AddRange(swapCrossProducts);
                }

                List<Product> swapCrossBalmoProducts =
                    GroupToEdit.Products.Where(p => p.Type == ProductType.Swap || p.Type == ProductType.Balmo).ToList();

                if (swapCrossBalmoProducts.Count > 0)
                {
                    cmbSwapCrossBalmoProduct.Properties.Items.Add(DBNull.Value);
                    cmbSwapCrossBalmoProduct.Properties.Items.AddRange(swapCrossBalmoProducts);
                }

                List<Product> transferProducts =
                    GroupToEdit.Products.Where(p => p.Type == ProductType.Futures || p.Type == ProductType.Swap).ToList();

                if (transferProducts.Count > 0)
                {
                    cmbTransferProducts.Properties.Items.Add(DBNull.Value);
                    cmbTransferProducts.Properties.Items.AddRange(transferProducts);
                }
            }


            txtName.DataBindings.Add("EditValue", GroupToEdit, "Name", true);
            txtAbbreviation.DataBindings.Add("EditValue", GroupToEdit, "Abbreviation", true);
            cmbProducts.DataBindings.Add("EditValue", GroupToEdit, "SpreaderProduct", true);
            cmbSwapCrossProduct.DataBindings.Add("EditValue", GroupToEdit, "SwapCrossProduct", true, DataSourceUpdateMode.OnPropertyChanged, null);
            cmbSwapCrossBalmoProduct.DataBindings.Add("EditValue", GroupToEdit, "SwapCrossBalmoProduct", true, DataSourceUpdateMode.OnPropertyChanged, null);
            cmbTransferProducts.DataBindings.Add("EditValue", GroupToEdit, "TransferProduct", true, DataSourceUpdateMode.OnPropertyChanged, null);
            chkTasCheckRequired.DataBindings.Add("EditValue", GroupToEdit, "TasCheckRequired", true);
            teTasCheckTime.DataBindings.Add("EditValue", GroupToEdit, "TasCheckTime", true);

            CmbTasProductAliasSetEnabled(chkTasCheckRequired.Checked);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            txtDummy.Focus();

            var auditContext = ProductListForm.CreateAuditContext("Product Group Details");

            UpdatedGroupId = productManager.SaveGroup(GroupToEdit, auditContext);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkTasCheckRequired_CheckedChanged(object sender, EventArgs e)
        {
            CmbTasProductAliasSetEnabled(chkTasCheckRequired.Checked);
        }

        private void CmbTasProductAliasSetEnabled(bool enabledState)
        {
            teTasCheckTime.Enabled = enabledState;
        }
    }
}