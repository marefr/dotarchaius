using System;

namespace DotArchaius.Logging
{
    public interface ILogFactoryAdapter
    {
        ILog GetLogger(Type type);
        ILog GetLogger(string key);
    }
}