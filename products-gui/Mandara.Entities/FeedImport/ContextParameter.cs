using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Mandara.Entities.FeedImport
{
    [DataContract(IsReference = true)]
    public class ContextParameter
    {
        [DataMember]
        public IList<FeedCredentials> CredentialsList{ get; set; }
    }
}
