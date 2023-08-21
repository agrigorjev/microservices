using Google.Protobuf;
using Mandara.Entities;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AuditMessageDetails = Mandara.Proto.Encoder.Generated.AuditMessageDetails;

namespace Mandara.Proto.Encoder
{
    public static class ProtoEncoder
    {
        public static void EncodeDetailsAsGzipProto(AuditMessage message)
        {
            AuditMessageDetails mapObject = CloneFromEntites(message.Details);
            using (var ms = new MemoryStream())
            {
                using (var gzStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    using (CodedOutputStream output = new CodedOutputStream(gzStream))
                    {
                        mapObject.WriteTo(output);
                    }
                }
                ms.Seek(0, SeekOrigin.Begin);
                message.MessageDetails = Convert.ToBase64String(ms.ToArray());
            }
        }

        public static void DecodeDetailsAsGzipProto(AuditMessage message)
        {
            var messageDetails = message.MessageDetails;
            AuditMessageDetails result = new AuditMessageDetails();
            if (!string.IsNullOrEmpty(messageDetails))
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(messageDetails)))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var gzStream = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        result.MergeFrom(gzStream);
                        message.Details = CloneFromProto(result);
                    }
                }
            }
        }

        private static AuditMessageDetails CloneFromEntites(Entities.AuditMessageDetails entitiesDetails)
        {
            AuditMessageDetails result = new AuditMessageDetails()
            {
                Property = Sanitize(entitiesDetails.Property),
                NewValue = Sanitize(entitiesDetails.NewValue),
                OldValue = Sanitize(entitiesDetails.OldValue),
                Children = {entitiesDetails.Children.Select(CloneFromEntites)}
            };
            return result;
        }

        private static string Sanitize(string entitiesDetailsProperty)
        {
            return entitiesDetailsProperty ?? "";
        }

        private static Entities.AuditMessageDetails CloneFromProto(AuditMessageDetails protoDetails)
        {
            Entities.AuditMessageDetails result = new Entities.AuditMessageDetails()
            {
                Property = protoDetails.Property,
                NewValue = protoDetails.NewValue,
                OldValue = protoDetails.OldValue,
                Children = protoDetails.Children.Select(CloneFromProto).ToList()
            };
            return result;
        }
    }
}
