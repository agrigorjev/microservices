using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Mandara.Entities.FeedImport
{
     [DataContract(IsReference = true)]
    public class FeedImportException:Exception
    {
        public FeedImportException(){}
        
        public FeedImportException(string message):base(message){}

        public FeedImportException(string message, Exception innerException) : base(message, innerException) { }
    }
}
