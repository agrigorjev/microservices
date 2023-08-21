using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.ProductGUI
{
    public class ProductsEditor
    {
        public void ShowProductsGui()
        {
            ProductListForm frmProductsList = new ProductListForm();
            frmProductsList.Show();
        }
    }
}
