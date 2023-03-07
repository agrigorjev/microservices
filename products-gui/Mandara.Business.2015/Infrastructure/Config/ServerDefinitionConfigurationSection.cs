using System.Configuration;
using Mandara.Business.Config;

namespace Mandara.Business.Infrastructure.Config
{
    public class ServerDefinitionConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "ServerDefinition";
        private const string SerialiseDatesProperty = "serialiseDatesTo";

        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "1")]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("prefix", IsRequired = true, DefaultValue = "1")]
        public string Prefix
        {
            get => (string)this["prefix"];
            set => this["prefix"] = value;
        }
        
        [ConfigurationProperty(SerialiseDatesProperty, IsRequired = false, DefaultValue = "")]
        public string SerialiseDatesTo
        {
            get => (string)this[SerialiseDatesProperty];
            set => this[SerialiseDatesProperty] = value;
        }

        public DateTimeSerialisation DatesSerialiseTo() => DateSerialisation.To(SerialiseDatesTo);
    }
}