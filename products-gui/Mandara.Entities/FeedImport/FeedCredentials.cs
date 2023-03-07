using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Mandara.Entities.FeedImport
{
     [DataContract(IsReference = true)]
    public class FeedCredentials
    {
        public FeedCredentials(){}

        public FeedCredentials(string userName, string password, FeedSourceType feedSourceType)
        {
            Credentials = new Credentials() {UserName = userName, Password = password};
            SourceDataType = feedSourceType;
        }

         
        [DataMember]
        public Credentials Credentials { get; set; }

        [DataMember]
        public FeedSourceType SourceDataType { get; set; }    
    }
}
