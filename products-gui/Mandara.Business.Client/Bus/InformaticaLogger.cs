using System.Text;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Client.Managers;
using Mandara.Business.Helpers;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus
{
    static internal class InformaticaLogger
    {
        public static void LogRequestResponse<T>(ILogger log, string method, LBMRequest lbmreq, LBMMessage lbmmsg) where T : SnapshotMessageBase
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("{0} , topic {1} , type = {2} request_data={3} message_date={4} result={5}",
                    method,
                    lbmmsg.topicName(),
                    LbmTypeToName(lbmmsg.type()),
                    GetMessageHeader(lbmreq.data()),
                    GetMessageHeader(lbmmsg.data()),
                    UnpackMessage<T>(lbmmsg.data()));
            }
        }

        public static void LogRequestResponseHeader(ILogger log, string method, LBMRequest lbmreq, LBMMessage lbmmsg)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("{0} , topic {1} , type = {2} request_data={3} message_date={4}",
                    method,
                    lbmmsg.topicName(),
                    LbmTypeToName(lbmmsg.type()),
                    GetMessageHeader(lbmreq.data()),
                    GetMessageHeader(lbmmsg.data()));
            }
        }

        public static void LogResponse<T>(ILogger log, string method, LBMMessage lbmmsg) where T : SnapshotMessageBase
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("{0} , topic {1} , type = {2} message_date={3} result={4}", 
                    method, 
                    lbmmsg.topicName(), 
                    LbmTypeToName(lbmmsg.type()), 
                    GetMessageHeader(lbmmsg.data()), 
                    UnpackMessage<T>(lbmmsg.data()));
            }
        }

        public static string GetMessageHeader(byte[] data)
        {
            string messageData = GetDataAsString(data);
            if (messageData.Length > 150)
            {
                return messageData.Substring(0, 100) + "...";
            }

            return messageData;
        }

        private static string GetDataAsString(byte[] data)
        {
            if (data == null || data.Length == 0) return "";
            byte[] unzippedBytes = data;
            if (data.IsPossiblyGZippedBytes())
            {
                unzippedBytes = ZipManager.UnzipBytes(data);
            }

            return Encoding.UTF8.GetString(unzippedBytes);
        }

        public static string LbmTypeToName(int lbmType)
        {
            switch (lbmType)
            {
                case LBM.MSG_DATA: return "MSG_DATA";
                case LBM.MSG_NO_SOURCE_NOTIFICATION: return "MSG_NO_SOURCE_NOTIFICATION";
                case LBM.MSG_BOS: return "MSG_BOS";
                case LBM.MSG_EOS: return "MSG_EOS";
                case LBM.MSG_REQUEST: return "MSG_REQUEST";
                case LBM.MSG_RESPONSE: return "MSG_RESPONSE";
                case LBM.MSG_UNRECOVERABLE_LOSS: return "MSG_UNRECOVERABLE_LOSS";
                case LBM.MSG_UNRECOVERABLE_LOSS_BURST: return "MSG_UNRECOVERABLE_LOSS_BURST";
                default:
                    return $"Unknown Type {lbmType}";
            }
        }

        public static T UnpackMessage<T>(byte[] bytes) where T : SnapshotMessageBase
        {
            return JsonHelper.Deserialize<T>(bytes);
        }

        public static void LogMessage(LBMMessage lbmmsg, ILogger log)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("type {0} , topic = {1} message_data={2}",
                    LbmTypeToName(lbmmsg.type()),
                    lbmmsg.topicName(),
                    GetMessageHeader(lbmmsg.data()));
            }
        }
    }
}