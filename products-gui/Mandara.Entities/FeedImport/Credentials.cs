using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Mandara.Entities.FeedImport
{
    [DataContract(IsReference = true)]
    public class Credentials
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
