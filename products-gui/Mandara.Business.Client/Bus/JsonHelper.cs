using Mandara.Business.Bus.Messages;
using Mandara.Business.Client.Managers;
using Mandara.Business.Helpers;
using Mandara.Entities.ErrorReporting;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Mandara.Business.Bus
{
    public static class JsonHelper
    {
        private static TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;
        private static DateTimeZoneHandling DefaultTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
        private static readonly JsonSerializerSettings serialisationSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
        };

        /// <summary>
        /// This is so that not all apps are automatically forced to serialised DateTime the same way.
        /// This does not work in cases where multiple threads are using serialisation and required different time zone
        /// handling.
        /// </summary>
        /// <param name="timeZoneHandling"></param>
        public static void WithTimeZoneHandling(DateTimeZoneHandling timeZoneHandling)
        {
            serialisationSettings.DateTimeZoneHandling = timeZoneHandling;
        }

        public static void ResetTimeZoneHandling()
        {
            serialisationSettings.DateTimeZoneHandling = DefaultTimeZoneHandling;
        }

        public static byte[] Serialize<T>(T t) where T : class
        {
            if (t == null)
                return null;

            string serializedObject = SerializeToString(t);

            if (string.IsNullOrEmpty(serializedObject))
                return null;

            return Encoding.UTF8.GetBytes(serializedObject);
        }

        public static string SerializeToString<T>(T toSerialise) where T : class
        {
            if (toSerialise == null)
            {
                return null;
            }

            SetTypeNameHandling(toSerialise);

            return JsonConvert.SerializeObject(toSerialise, serialisationSettings);
        }

        private static void SetTypeNameHandling<T>(T toSerialise) where T : class
        {
            if (toSerialise is LiveFeedReplaySnapshotMessageDto || toSerialise is LiveFeedReplaySnapshotMessage)
            {
                serialisationSettings.TypeNameHandling = TypeNameHandling.Auto;
                return;
            }

            serialisationSettings.TypeNameHandling = DefaultTypeNameHandling;
        }

        public static T DeserializeQuiet<T>(string serialized) where T : class
        {
            if (serialized == null)
                return null;

            try
            {
                return DoDeserialize<T>(serialized);
            }
            catch
            {
                return null;
            }
        }

        public static T Deserialize<T>(string serialized) where T : class
        {
            if (serialized == null)
                return null;
            try
            {
                return DoDeserialize<T>(serialized);
            }
            catch (Exception ex)
            {
                string txt = string.Format(
                    "Cannot parse following message [{0}]. Exception message [{1}]",
                    serialized,
                    ex.Message);
                ReportError(ex, txt);
                return null;
            }
        }

        private static void ReportError(Exception ex, string msg)
        {

            ErrorReportingHelper.ReportError("Unknown", "IRM", ErrorType.Exception, msg, null, ex, ErrorLevel.Critical);

        }

        private static T DoDeserialize<T>(string serialized) where T : class
        {
            T result = JsonConvert.DeserializeObject<T>(serialized, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return result;
        }

        public static T Deserialize<T>(byte[] bytes) where T : class
        {
            if (bytes == null)
            {
                return null;
            }

            try
            {
                if (bytes.IsPossiblyGZippedBytes())
                {
                    bytes = ZipManager.UnzipBytes(bytes);
                }

                string text = Encoding.UTF8.GetString(bytes);
                T result = Deserialize<T>(text);

                return result;
            }
            catch (Exception ex)
            {
                ReportError(ex, ex.Message);
                return null;
            }
        }
    }



}
