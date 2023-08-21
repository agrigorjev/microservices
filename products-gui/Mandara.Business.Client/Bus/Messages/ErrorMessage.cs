using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.ErrorReporting;

namespace Mandara.Business.Bus.Messages
{
    public class ErrorMessage : MessageBase
    {
        public List<Error> Errors { get; set; }
    }
}