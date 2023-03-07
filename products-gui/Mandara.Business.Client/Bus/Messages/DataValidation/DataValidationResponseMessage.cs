using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Business.Bus.Messages.DataValidation
{
    public class DataValidationResponseMessage : MessageBase
    {
        public List<Error> Errors { get; set; }
    }
}
