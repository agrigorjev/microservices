using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.ProductGUI.Desks.OfficialProducts
{
    interface IDeskProductsRepository
    {
        void LoadProducts();
        List<Product> ProductsForDesk(List<Product> allValidProducts, int deskId);
        List<OfficialProductForDesk> OfficialProductsForDesk(int deskId);
        bool ProductExists(OfficialProductForDesk testProduct);
        DatabaseActionResult Add(OfficialProductForDesk newProduct);
        DatabaseActionResult Update(OfficialProductForDesk changedProduct);
        DatabaseActionResult Delete(OfficialProductForDesk productToDelete);
        DatabaseActionResult Delete(int officialProduct);
        int TotalProducts();
    }
}