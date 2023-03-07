using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.CategoryInfo
{
    public class ProductCategoryDtoMessage : SnapshotMessageBase
    {
        public List<ProductCategoryDto> Categories { get; set; }
    }
}