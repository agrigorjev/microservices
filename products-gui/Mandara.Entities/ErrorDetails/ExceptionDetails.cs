using System;
using System.ComponentModel;
using System.Text;
using Mandara.Entities.Extensions;

namespace Mandara.Entities.ErrorDetails
{
    public class ExceptionDetails : ErrorDetails
    {
        [Category("Exception Details")]
        [Description("")]
        [DisplayName("Stack Trace")]
        public string StackTrace { get; set; }

        [Category("Exception Details")]
        [Description("")]
        [DisplayName("Source")]
        public string Source { get; set; }

        [Category("Exception Details")]
        [Description("")]
        [DisplayName("Target Site")]
        public string TargetSite { get; set; }

        [Category("Exception Details")]
        [Description("")]
        [DisplayName("Message")]
        public string Message { get; set; }

        [Category("Exception Details")]
        [Description("")]
        [DisplayName("Full Text")]
        public string FullText { get; set; }

        [DisplayName("Trade Type")]
        public string TrdType { get; set; }

        public ExceptionDetails()
        {
        }

        public ExceptionDetails(Exception ex)
        {
            if (ex == null)
                return;

            Message = ex.Message;
            StackTrace = ex.StackTrace;
            TargetSite = ex.TargetSite.ToDataString();
            Source = ex.Source;

            Exception e = ex;
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.AppendFormat("{0}: {1}{2}", e.GetType().FullName, e.Message, Environment.NewLine);
                sb.AppendFormat("Source: {0}{1}{1}", e.Source, Environment.NewLine);
                sb.AppendFormat("Target site: {0}{1}{1}", e.TargetSite.ToDataString(), Environment.NewLine);
                sb.AppendFormat("Stack trace: {1}{0}{1}{1}{1}", e.StackTrace ?? "", Environment.NewLine);

                e = e.InnerException;

            } while (e != null);

            FullText = sb.ToString();
        }
    }
}
