using System;
using System.Diagnostics;

namespace DotArchaius.Util
{
    [DebuggerStepThrough]
    public static class SystemClock
    {
        private static Func<DateTime> _systemTimeProvider;
        private static readonly object Lock = new object();

        public static DateTime Now
        {
            get
            {
                if (_systemTimeProvider == null)
                {
                    Reset();
                }

                return _systemTimeProvider();
            }
        }

        public static void Adjust(Func<DateTime> func)
        {
            lock (Lock)
            {
                _systemTimeProvider = func;
            }
        }

        public static void Reset()
        {
            Adjust(() => DateTime.UtcNow);
        }
    }
}
