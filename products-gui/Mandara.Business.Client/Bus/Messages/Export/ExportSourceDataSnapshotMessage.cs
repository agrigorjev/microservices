using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Export
{
    public class ExportSourceDataSnapshotMessage : SnapshotMessageBase
    {
        public DateTime Date { get; set; }
        public int DataType { get; set; }
        public DateTime SnapshotDatetime { get; set; }

        public byte[] Data { get; set; }
    }
}