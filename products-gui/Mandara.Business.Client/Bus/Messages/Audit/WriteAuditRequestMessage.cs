using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Audit
{
    public class WriteAuditRequestMessage : MessageBase
    {
        public string MessageType { get; set; }
        public string ContextName { get; set; }
        public string Source { get; set; }
        public string ObjectDescription { get; set; }
        public string PropertyName { get; set; }
        public string PreviousValue { get; set; }
        public string CurrentValue { get; set; }

        public override string ToString()
        {
            return $"{MessageType} [{ContextName} - {Source}]{ReportingObject()}{ChangeDescription()}";
        }

        private string ReportingObject()
        {
            return String.IsNullOrWhiteSpace(ObjectDescription) ? "" : $" - {ObjectDescription}";
        }

        private string ChangeDescription()
        {
            if (String.IsNullOrWhiteSpace(PropertyName))
            {
                return "";
            }

            string from = String.IsNullOrWhiteSpace(PreviousValue) ? "" : $" from {PreviousValue}";
            string to = String.IsNullOrWhiteSpace(CurrentValue) ? "" : $" to {CurrentValue}";

            return $" - {PropertyName}{from}{to}";
        }
    }
}