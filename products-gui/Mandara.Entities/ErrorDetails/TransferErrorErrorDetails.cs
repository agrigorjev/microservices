using System.ComponentModel;

namespace Mandara.Entities.ErrorDetails
{
    public class TransferErrorErrorDetails : ErrorDetails
    {
        [Category("Transfer Error")]
        public string ProductDescription { get; set; }

        [Category("Transfer Error")]
        public string Exchange { get; set; }

        [Category("Transfer Error")]
        public string UnderlyingSecurityID { get; set; }

        [Category("Transfer Error")]
        public string UnderlyingSecurityIDSource { get; set; }

        [Category("Transfer Error")]
        public int? ProductId { get; set; }

        [Category("Transfer Error")]
        public string Message { get; set; }

        public TransferErrorErrorDetails(TradeTransferErrorDetails error)
        {
            ProductDescription = error.Product;
            Exchange = error.Exchange;
            UnderlyingSecurityID = error.UnderlyingSecurityID;
            UnderlyingSecurityIDSource = error.UnderlyingSecurityIDSource;
            ProductId = error.ProductId;
            Message = error.Message;
        }
    }
}