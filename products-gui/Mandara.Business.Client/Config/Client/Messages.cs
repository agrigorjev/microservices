using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Mandara.Business.Config;

namespace Mandara.Business.Client.Config.Client
{
    public class Messages : ConfigurationElement
    {
        public const string ElemName = "Messages";

        [ConfigurationProperty(SerialiseDates.ElemName, IsRequired = false)]
        public SerialiseDates HandleDates
        {
            get => (SerialiseDates)this[SerialiseDates.ElemName];
            set => this[SerialiseDates.ElemName] = value;
        }
    }

    public class SerialiseDates : ConfigurationElement
    {
        public const string ElemName = "SerialiseDates";
        private const string ToProperty = "to";

        [ConfigurationProperty(ToProperty, IsRequired = true)]
        public string To
        {
            get => (string)this[ToProperty];
            set => this[ToProperty] = value;
        }

        public DateTimeSerialisation SerialiseTo() => DateSerialisation.To(To);
    }
}