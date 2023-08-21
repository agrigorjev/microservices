namespace Mandara.Business.Bus.Messages.Spreader
{
    using System.Collections.Generic;
    using Mandara.Business.Bus.Messages.Base;
    using Mandara.Entities;

    public class SpreaderProductsResponseMessage : MessageBase
    {
        public List<ProductCategory> Categories { get; set; }
    }
}
