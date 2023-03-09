using System;
using Ninject.Extensions.Logging;

namespace Mandara.ProductGUI.Log
{
    public class NullLogger : LoggerBase
    {
        public NullLogger(Type type) : base(type) {}
        public NullLogger(string name) : base(name) {}
        public override void Debug(string message)
        {
        }

        public override void Debug(string format, params object[] args)
        {
        }

        public override void Debug(Exception exception, string format, params object[] args)
        {
        }

        public override void DebugException(string message, Exception exception)
        {
        }

        public override void Info(string message)
        {
        }

        public override void Info(string format, params object[] args)
        {
        }

        public override void Info(Exception exception, string format, params object[] args)
        {
        }

        public override void InfoException(string message, Exception exception)
        {
        }

        public override void Trace(string message)
        {
        }

        public override void Trace(string format, params object[] args)
        {
        }

        public override void Trace(Exception exception, string format, params object[] args)
        {
        }

        public override void TraceException(string message, Exception exception)
        {
        }

        public override void Warn(string message)
        {
        }

        public override void Warn(string format, params object[] args)
        {
        }

        public override void Warn(Exception exception, string format, params object[] args)
        {
        }

        public override void WarnException(string message, Exception exception)
        {
        }

        public override void Error(string message)
        {
        }

        public override void Error(string format, params object[] args)
        {
        }

        public override void Error(Exception exception, string format, params object[] args)
        {
        }

        public override void ErrorException(string message, Exception exception)
        {
        }

        public override void Fatal(string message)
        {
        }

        public override void Fatal(string format, params object[] args)
        {
        }

        public override void Fatal(Exception exception, string format, params object[] args)
        {
        }

        public override void FatalException(string message, Exception exception)
        {
        }

        public override bool IsDebugEnabled
        {
            get { return false; }
        }

        public override bool IsInfoEnabled
        {
            get { return false; }
        }

        public override bool IsTraceEnabled
        {
            get { return false; }
        }

        public override bool IsWarnEnabled
        {
            get { return false; }
        }

        public override bool IsErrorEnabled
        {
            get { return false; }
        }

        public override bool IsFatalEnabled
        {
            get { return false; }
        }
    }
}
