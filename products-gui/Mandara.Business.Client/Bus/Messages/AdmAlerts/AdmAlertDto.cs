using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AdmAlertDto
    {
        public int AlertId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public string ThresholdValue { get; set; }
        public string ActualValue { get; set; }
        public string AlertType { get; set; }
        public string Book { get; set; }

        public string TriggerKey { get; set; }

        public static AdmAlertDto Compose(string key, string subject, string message, string actualValue, string serializedValue, AdministrativeAlert admAlert)
        {
            return new AdmAlertDto
                       {
                           TriggerKey = key,
                           AlertId = admAlert.AlertId,
                           ThresholdValue = admAlert.ThresholdValueString,
                           ActualValue = actualValue,
                           SerializedValue = serializedValue,
                           AlertType = admAlert.TypeAlertString,
                           Book = admAlert.Portfolio == null ? string.Empty : admAlert.Portfolio.Name,
                           Subject = subject,
                           Message = message
                       };
        }

        public string SerializedValue { get; set; }

        public static AdmAlertDto FromHistory(AlertHistory entry)
        {
            return new AdmAlertDto
            {
                TriggerKey = entry.TriggerKey,
                AlertId = entry.AlertId,
                ThresholdValue = entry.ThresholdValue,
                ActualValue = entry.ActualValue,
                AlertType = entry.AlertType,
                Book = entry.BookName,
                Subject = entry.Subject,
                Message = entry.Message
            };
        }
    }
}