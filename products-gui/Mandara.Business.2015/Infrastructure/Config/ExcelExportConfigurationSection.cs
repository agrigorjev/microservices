using System.Configuration;

namespace Mandara.Business.Infrastructure.Config
{
    public class ExcelExportConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("Export")]
        public PathsCollection Items
        {
            get { return ((PathsCollection)(base["Export"])); }
        }
    }
    [ConfigurationCollection(typeof(PathElement))]
    public class PathsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PathElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PathElement)(element)).BookId;
        }

        public PathElement this[int idx]
        {
            get { return (PathElement)BaseGet(idx); }
        }
    }
    public class PathElement : ConfigurationElement
    {

        [ConfigurationProperty("bookId", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string BookId
        {
            get { return ((string)(base["bookId"])); }
            set { base["bookId"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Path
        {
            get { return ((string)(base["path"])); }
            set { base["path"] = value; }
        }
    }
}
