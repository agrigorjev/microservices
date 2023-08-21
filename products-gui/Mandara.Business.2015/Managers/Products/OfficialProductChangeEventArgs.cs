using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Entities;

namespace Mandara.Business.Managers.Products
{
    public enum OfficialProductChangeType
    {
        Added,
        Removed,
        NameChange,
    }

    public class OfficialProductChangeEventArgs
    {
        public OfficialProduct Changed { get; private set; }
        public string PreviousName { get; private set; }
        public OfficialProductChangeType Change { get; private set; }

        public OfficialProductChangeEventArgs(OfficialProduct changed, OfficialProductChangeType changeType)
        {
            if (changeType == OfficialProductChangeType.NameChange)
            {
                throw new ArgumentException(
                    "Official product name change - Missing the original name of the official product");
            }

            Changed = changed;
            Change = changeType;
        }

        public OfficialProductChangeEventArgs(
            OfficialProduct changed,
            string previousName,
            OfficialProductChangeType changeType)
        {
            Changed = changed;
            PreviousName = previousName;
            Change = changeType;
        }
    }
}
