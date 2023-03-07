using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Business.Bus.Messages.Authorization
{
    using Mandara.Business.Bus.Messages.Base;

    public class ChangePasswordResponse : MessageBase
    {
        public string Response { get; set; }
    }
}
