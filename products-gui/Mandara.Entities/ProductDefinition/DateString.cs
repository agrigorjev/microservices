using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Entities.ProductDefinition
{
    public class DateString
    {
        public string StringValue { get; set; }
        public DateTime DateValue { get; set; }

        public override string ToString()
        {
            return StringValue;
        }
    }
}
