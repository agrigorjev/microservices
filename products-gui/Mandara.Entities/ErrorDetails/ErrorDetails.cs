using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using Mandara.Entities.Calculation;
using Mandara.Entities.Trades;

namespace Mandara.Entities.ErrorDetails
{
    [Serializable]
    [XmlInclude(typeof(ExceptionDetails))]
    [XmlInclude(typeof(TradeDetails))]
    public class ErrorDetails
    {
        public static ErrorDetails Create(object o)
        {
            if (o is string)
            {
                return new StringDetails(o as string);
            }

            if (o is Exception)
            {
                return new ExceptionDetails(o as Exception);
            }

            if (o is TradeCapture)
            {
                return new TradeDetails(o as TradeCapture);
            }

            if (o is DataRow)
            {
                return new TradeDetails(o as DataRow);
            }

            if (o is SourceDetail)
            {
                if (((SourceDetail)o).SourceData != null && ((SourceDetail)o).SourceData.Type == SourceDataType.Seals)
                {
                    return new SealDetails(o as SourceDetail);
                }
                else
                {
                    return new SourceDetails(o as SourceDetail);
                }
            }

            if (o is TradeTransferErrorDetails)
            {
                return new TransferErrorErrorDetails(o as TradeTransferErrorDetails);
            }

            if (o is TasCheckDetail)
                return new TasCheckErrorDetail(o as TasCheckDetail);

            if (o is RolloffDetail)
                return new RolloffErrorDetail(o as RolloffDetail);

            if (o is ErrorDetails)
                return o as ErrorDetails;

            if (o is FailureCallbackInfo)
                return new FailureCallbackDetails((FailureCallbackInfo) o);

            return null;
        }

        public static string Serialize(ErrorDetails error)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ErrorDetails));

            using (var ms = new MemoryStream())
            {
                using (var gzStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    xmlSerializer.Serialize(gzStream, error);
                }

                ms.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static ErrorDetails Deserialize(string str)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ErrorDetails));

            using (var ms = new MemoryStream(Convert.FromBase64String(str)))
            {
                ms.Seek(0, SeekOrigin.Begin);

                using (var gzStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    return (ErrorDetails)xmlSerializer.Deserialize(gzStream);
                }
            }
        }
    }

    [Serializable]
    public class StringDetails : ErrorDetails
    {
        public string Details { get; set; }

        public StringDetails(string str)
        {
            Details = str;
        }
    }
}
