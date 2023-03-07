using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.CategoryInfo
{
    public class ProductCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Abbreviation { get; set; }

        private const int NoCategoryId = -1;
        private const string NoCategoryName = "";

        public static readonly ProductCategoryDto Default = new ProductCategoryDto()
        {
            CategoryId = NoCategoryId, CategoryName = NoCategoryName, Abbreviation = NoCategoryName
        };

        public bool IsDefault()
        {
            return Object.ReferenceEquals(this, Default)
                   || (NoCategoryId == CategoryId && NoCategoryName == CategoryName && NoCategoryName == Abbreviation);
        }
    }
}