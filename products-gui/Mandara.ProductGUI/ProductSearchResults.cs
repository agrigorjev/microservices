using DevExpress.XtraEditors;
using Mandara.Business;
using Mandara.Entities;
using Mandara.ProductGUI.Desks;
using Mandara.ProductGUI.StringExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mandara.ProductGUI.Desks.OfficialProducts;

namespace Mandara.ProductGUI
{
    public partial class ProductSearchResults : XtraUserControl
    {
        private readonly ProductManager _productManager = new ProductManager();
        private readonly BindingList<Product> _results = new BindingList<Product>();
        private GmiCodesMapper _gmiCodesMapper;
        internal IDeskProductsRepository ProductsByDesk { get; set; }

        public ProductSearchResults()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            gcProducts.DataSource = _results;
        }


        /// <summary>
        /// Searches Products that contain searchString in Product Name, Alias or ExchangeContractCode
        /// </summary>
        /// <param name="searchString">Search string</param>
        public void Search(string searchString, int deskId)
        {
            _results.Clear();

            List<Product> foundProducts = GetMatchingProducts(searchString);

            if (deskId != Desk.Default.Id)
            {
                foundProducts = ProductsByDesk.ProductsForDesk(foundProducts, deskId);
            }

            foundProducts.OrderBy(product => product.Name).ToList().ForEach(product => _results.Add(product));
        }

        private List<Product> GetMatchingProducts(string searchString)
        {
            List<Product> foundProductsWithAliases = GetMatchingProductsWithAliases(searchString);
            IEnumerable<Product> foundProductsWithoutAliases = GetMatchingProductsWithoutAliases(
                searchString,
                foundProductsWithAliases.Select(product => product.ProductId).ToList());
            List<Product> foundProducts = foundProductsWithAliases.Concat(foundProductsWithoutAliases).ToList();
            List<int> foundProductsIds = foundProducts.Select(product => product.ProductId).ToList();

            return foundProducts.Concat(GetMatchingProductsByGmiCode(searchString, foundProductsIds)).ToList();
        }

        private List<Product> GetMatchingProductsWithAliases(string searchString)
        {
            return _productManager.GetProductAliasesWithProducts().Where(
                                      alias => IsMatch(alias.Product.ExchangeContractCode, searchString)
                                               || IsMatch(alias.Product.Name, searchString)
                                               || IsMatch(alias.Name, searchString)).Select(a => a.Product)
                                  .Distinct().ToList();
        }

        private bool IsMatch(string testTerm, string searchTerm)
        {
            return (testTerm ?? String.Empty).Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase);
        }

        private IEnumerable<Product> GetMatchingProductsWithoutAliases(string searchString, List<int> foundProductsIds)
        {
            List<Product> allProducts = _productManager.GetProducts();
            List<Product> productsWithoutAliases =
                allProducts.Where(p => !foundProductsIds.Contains(p.ProductId)).ToList();

            return productsWithoutAliases.Where(
                product => IsMatch(product.ExchangeContractCode, searchString)
                           || IsMatch(product.Name, searchString)).Distinct();
        }

        private IEnumerable<Product> GetMatchingProductsByGmiCode(string searchString, List<int> foundProductsIds)
        {
            if (_gmiCodesMapper == null)
            {
                _gmiCodesMapper = new GmiCodesMapper();
            }

            return _gmiCodesMapper.SearchProducts(searchString).Where(x => !foundProductsIds.Contains(x.ProductId));
        }

        public Product CurrentProduct => gvProducts.GetFocusedRow() as Product;
    }

    namespace StringExtension
    {
        public static class StringExtension
        {
            public static bool Contains(this string source, string toCheck, StringComparison comp)
            {
                return source.IndexOf(toCheck, comp) >= 0;
            }
        }
    }
}
