using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages
{
    [Serializable]
    public class SecurityDefinitionsSnapshotMessageDto : SnapshotMessageBase
    {
        public List<SecurityDefinitionDto> SecurityDefinitions { get; set; }
    }
}
