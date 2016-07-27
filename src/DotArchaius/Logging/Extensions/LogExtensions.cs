using System;

namespace DotArchaius.Logging.Extensions
{
    public static class LogExtensions
    {
        public static void DebugFormat(this ILog log, string format, params object[] args)
        {
            log.Debug(string.Format(format, args));
        }

        public static void DebugFormat(this ILog log, string format, Exception exception, params object[] args)
        {
            log.Debug(string.Format(format, args), exception);
        }

        public static void ErrorFormat(this ILog log, string format, params object[] args)
        {
            log.Error(string.Format(format, args));
        }

        public static void ErrorFormat(this ILog log, string format, Exception exception, params object[] args)
        {
            log.Error(string.Format(format, args), exception);
        }

        public static void InfoFormat(this ILog log, string format, params object[] args)
        {
            log.Info(string.Format(format, args));
        }

        public static void InfoFormat(this ILog log, string format, Exception exception, params object[] args)
        {
            log.Info(string.Format(format, args), exception);
        }

        public static void TraceFormat(this ILog log, string format, params object[] args)
        {
            log.Trace(string.Format(format, args));
        }

        public static void TraceFormat(this ILog log, string format, Exception exception, params object[] args)
        {
            log.Trace(string.Format(format, args), exception);
        }

        public static void WarnFormat(this ILog log, string format, params object[] args)
        {
            log.Warn(string.Format(format, args));
        }

        public static void WarnFormat(this ILog log, string format, Exception exception, params object[] args)
        {
            log.Warn(string.Format(format, args), exception);
        }
    }
}
