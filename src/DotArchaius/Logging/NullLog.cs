using System;
using System.Diagnostics;

namespace DotArchaius.Logging
{
    [DebuggerStepThrough]
    internal class NullLog : ILog
    {
        public void Debug(object message)
        {
            
        }

        public void Debug(object message, Exception exception)
        {
            
        }

        public void Error(object message)
        {
            
        }

        public void Error(object message, Exception exception)
        {
            
        }

        public void Info(object message)
        {
            
        }

        public void Info(object message, Exception eception)
        {
            
        }

        public void Trace(object message)
        {
            
        }

        public void Trace(object message, Exception exception)
        {
            
        }

        public void Warn(object message)
        {
            
        }

        public void Warn(object message, Exception exception)
        {
            
        }
    }
}