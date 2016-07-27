using System;

namespace DotArchaius.Logging
{
    public class LogManager
    {
        private static ILogFactoryAdapter _logAdapter = new NullLogFactoryAdapter();

        internal static void RegisterLogAdapter(ILogFactoryAdapter adapter)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter));

            _logAdapter = adapter;
        }

        public static ILog GetLogger<T>()
        {
            return _logAdapter.GetLogger(typeof(T));
        }
    }
}