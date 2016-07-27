using System;

namespace DotArchaius.Logging
{
    public interface ILog
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void Error(object message);
        void Error(object message, Exception exception);
        void Info(object message);
        void Info(object message, Exception eception);
        void Trace(object message);
        void Trace(object message, Exception exception);
        void Warn(object message);
        void Warn(object message, Exception exception);
    }
}
