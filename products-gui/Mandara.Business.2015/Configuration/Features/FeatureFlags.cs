using System.Configuration;

namespace Mandara.Common.Configuration.Features
{
    public class FeatureFlag : ConfigurationElement
    {
        public const string ElemName = "feature";
        public const string FeatureName = "name";

        [ConfigurationProperty(FeatureName, IsRequired = true)]
        public string Name
        {
            get => base[FeatureName] as string;
            set => base[FeatureName] = value;
        }
    }

    [ConfigurationCollection(typeof(FeatureFlag), AddItemName = FeatureFlag.ElemName)]
    public class FeatureFlags : ConfigurationElementCollection
    {
        //public const string ElemName = "features";
        //public const string FlagName = "";

        protected override ConfigurationElement CreateNewElement()
        {
            return new FeatureFlag();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as FeatureFlag)?.Name ?? "";
        }

        //[ConfigurationProperty(FlagName, IsDefaultCollection = true)]
        //public List<FeatureFlag> Features
        //{
        //    get => base[FlagName] as List<FeatureFlag>;
        //    set => base[FlagName] = value;
        //}
    }

    public class FlagsConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "featureFlags";
        public const string FeaturesName = "";

        [ConfigurationProperty(FeaturesName, IsRequired = true, IsDefaultCollection = true)]
        public FeatureFlags Features
        {
            get => base[FeaturesName] as FeatureFlags;
            set => base[FeaturesName] = value;
        }

        public bool HasFeature(string featureName)
        {
            var allFeatures = Features.GetEnumerator();

            while (allFeatures.MoveNext())
            {
                if (((FeatureFlag)allFeatures.Current)?.Name == featureName)
                {
                    return true;
                }
            }

            return false;
        }

        public static FlagsConfigurationSection GetSection()
        {
            return (FlagsConfigurationSection)ConfigurationManager.GetSection(SectionName);
        }
    }
}
