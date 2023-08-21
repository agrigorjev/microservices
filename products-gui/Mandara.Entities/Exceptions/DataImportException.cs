using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Entities.Exceptions
{
    [Serializable]
    public class DataImportException : Exception
    {
        public int RowNumber { get; set; }
        public ErrorLevel ErrorLevel { get; set; }

        public DataImportException(string message, int rowNumber, ErrorLevel errorLevel = ErrorLevel.Critical)
            : base(message)
        {
            ErrorLevel = errorLevel;
            RowNumber = rowNumber;
        }

        protected DataImportException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            
        }        
    }

}
