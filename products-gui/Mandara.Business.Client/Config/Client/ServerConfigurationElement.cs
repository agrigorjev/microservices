using System.Configuration;

namespace Mandara.Business.Config.Client
{
    public class ServerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("prefix", IsRequired = true)]
        public string Prefix
        {
            get
            {
                return (string)this["prefix"];
            }
            set
            {
                this["prefix"] = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}