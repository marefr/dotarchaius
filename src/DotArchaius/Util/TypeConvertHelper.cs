using System;
using System.ComponentModel;
using System.Reflection;

namespace DotArchaius.Util
{
    public static class TypeConvertHelper
    {
        public static T ConvertValue<T>(string value)
        {
            return (T)ConvertValue(typeof(T), value);
        }

        public static object ConvertValue(Type type, string value)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return ConvertValue(Nullable.GetUnderlyingType(type), value);
            }

            try
            {
                return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert '{value}' to type '{type.Name}'", ex);
            }
        }
    }
}
