using System.Configuration;
using System;

namespace Mandara.Business.Config.Client
{
    public class PositionRemapSection : ConfigurationSection
    {
        [ConfigurationProperty("sourceCategoryId", IsRequired = true)]
        public int SourceCategoryId
        {
            get => (int)this["sourceCategoryId"];
            set => this["sourceCategoryId"] = value;
        }
        
        [ConfigurationProperty("targetCategoryId", IsRequired = true)]
        public int TargetCategoryId
        {
            get => (int)this["targetCategoryId"];
            set => this["targetCategoryId"] = value;
        }

        [ConfigurationProperty("cutOffDate", IsRequired = true)]
        public DateTime CutOffDate
        {
            get => (DateTime)this["cutOffDate"];
            set => this["cutOffDate"] = value;
        }

        private const int NoCategoryId = -1;
        private static readonly DateTime NoCategoryDate = DateTime.MinValue;

        public static PositionRemapSection Default = new PositionRemapSection()
        {
            SourceCategoryId = NoCategoryId, TargetCategoryId = NoCategoryId, CutOffDate = NoCategoryDate
        };

        public bool IsDefault()
        {
            return Object.ReferenceEquals(this, Default)
                   || (NoCategoryId == SourceCategoryId
                       && NoCategoryId == TargetCategoryId
                       && NoCategoryDate.Equals(CutOffDate));
        }
    }
}
