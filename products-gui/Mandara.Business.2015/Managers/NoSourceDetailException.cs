using System;

namespace Mandara.Business.Managers
{
    public class NoSourceDetailException : NullReferenceException
    {
        public NoSourceDetailException(string msg) : base(msg)
        { }

        public NoSourceDetailException(string msg, Exception innerExc) : base(msg, innerExc)
        { }
    }
}
