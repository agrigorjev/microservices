using System;
using System.Configuration;
using Optional;

namespace Mandara.Business.Config
{
    public static class ConfigurationExtensions
    {
        public static Option<string, Exception> GetSetting(this string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(setting))
            {
                return Option.None<string, Exception>(
                    new ConfigurationErrorsException(String.Format("{0} not set in configuration.", key)));
            }

            return Option.Some<string, Exception>(setting);   
        }
    }
}