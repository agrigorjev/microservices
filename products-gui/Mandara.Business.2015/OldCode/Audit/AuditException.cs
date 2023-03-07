using System;

namespace Mandara.Business
{
    public class AuditException : ApplicationException
    {
        public AuditException(string message) : base(message)
        {
            
        }
    }
}