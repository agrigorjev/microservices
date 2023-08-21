using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages
{
    [Serializable]
    public class SecurityDefinitionsSnapshotMessage : SnapshotMessageBase
    {
        public List<SecurityDefinition> SecurityDefinitions { get; set; }
    }
}
