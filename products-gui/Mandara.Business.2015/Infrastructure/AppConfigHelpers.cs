using System.Configuration;
using Mandara.Business.Infrastructure.Config;

namespace Mandara.Business.Infrastructure
{
    public static class AppConfigHelpers
    {
        public static string GetServerPrefixFromConfig()
        {
            var serverDefinitionSection =
                ConfigurationManager.GetSection("ServerDefinition") as ServerDefinitionConfigurationSection;

            string prefix = "1";

            if (serverDefinitionSection == null)
            {
            }
            else
            {
                prefix = serverDefinitionSection.Prefix;
            }

            return prefix;
        }
    }
}