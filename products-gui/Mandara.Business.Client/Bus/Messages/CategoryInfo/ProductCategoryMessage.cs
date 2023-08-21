using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages
{
    public class ProductCategoryMessage : MessageBase
    {
        public List<ProductCategory> Categories{get;set;}
    }
}