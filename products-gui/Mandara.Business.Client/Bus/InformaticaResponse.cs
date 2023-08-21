using System;
using System.Threading.Tasks;

namespace Mandara.Business.Client.Bus
{
    public static class InformaticaResponse
    {
        public static void HandleInformaticaResponse(byte[] received, Action<byte[]> responseHandler)
        {
            byte[] responseData = new byte[received.Length];

            received.CopyTo(responseData, 0);
            Task.Factory.StartNew(() => responseHandler(responseData));
        }
    }
}