using System;
using System.Diagnostics;

namespace DotArchaius.Logging
{
    [DebuggerStepThrough]
    internal class NullLogFactoryAdapter : ILogFactoryAdapter
    {
        public ILog GetLogger(Type type)
        {
            return new NullLog();
        }

        public ILog GetLogger(string key)
        {
            return new NullLog();
        }
    }
}