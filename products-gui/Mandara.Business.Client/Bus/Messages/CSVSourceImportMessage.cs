using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Exceptions;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Business.Bus.Messages
{
    public class CSVSourceImportMessage:SnapshotMessageBase
    {
        public string FileName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int DataType { get; set; }
        public List<string[]> Data{ get; set; }
        public int ContolCount { get; set; }

    }
    public class CSVResultMessage : MessageBase
    {
        public List<Error> Warnings { get; set; }

    }
}
