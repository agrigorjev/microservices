using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers.Base
{
    public static class LBMMessageExtensions
    {
        public static string GetIpFromLbmMessage(this LBMMessage lbmMessage)
        {
            if (lbmMessage == null || string.IsNullOrEmpty(lbmMessage.source()))
                return null;

            string[] strings = lbmMessage.source().Split(':');

            if (strings.Length == 3)
                return strings[1];

            return null;
        }
    }
}