using System;
using System.Configuration;

namespace Mandara.Business.Bus
{
    public class TopicDefinition
    {
        public const string DefaultSubsystemName = "IRM";
        private static string _defaultEnvironment;
        public static string DefaultEnvironment
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_defaultEnvironment))
                {
                    string defaultMsgEnviron = ConfigurationManager.AppSettings["MessagingEnvironment"] ?? "DEV";

                    DefaultEnvironment = defaultMsgEnviron;
                }

                return _defaultEnvironment;
            }

            set
            {
                if (null != _defaultEnvironment)
                {
                    throw new InvalidOperationException("The default environment for messaging can only be set once.");
                }

                if (null == value)
                {
                    throw new ArgumentNullException("Cannot set a null default messaging environment.");
                }

                _defaultEnvironment = value;
            }
        }

        public string DefaultTopicNameSuffix { get; set; }
        public string TopicAppSettingName { get; set; }
        public bool HasTopicAppSettingName
        {
            get
            {
                return null != TopicAppSettingName;
            }
        }
        public string Environment { get; set; }
        public string SubsystemName { get; set; }

        public TopicDefinition()
        {
            SubsystemName = DefaultSubsystemName;
            Environment = DefaultEnvironment;
        }
    }
}
