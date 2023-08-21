using System.Configuration;
using Mandara.Business.Client.Config.Client;

namespace Mandara.Business.Config.Client
{
    public class ServersConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("defaultPrefix", IsRequired = true, DefaultValue = "1")]
        public string DefaultPrefix
        {
            get => (string)this["defaultPrefix"];
            set => this["defaultPrefix"] = value;
        }

        [ConfigurationProperty("Servers", IsRequired = true)]
        [ConfigurationCollection(
            typeof(ServerConfigurationElement),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ConfigurationElements<ServerConfigurationElement> Servers =>
            (ConfigurationElements<ServerConfigurationElement>)this["Servers"];

        [ConfigurationProperty(Messages.ElemName, IsRequired = false)]
        public Messages MessageConfig
        {
            get => (Messages)this[Messages.ElemName];
            set => this[Messages.ElemName] = value;
        }
    }
}