using System;

namespace DotArchaius.Validation
{
    public class PropertyValidationException : Exception
    {
        public PropertyValidationException(string message)
            : base(message)
        {

        }

        public PropertyValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public PropertyValidationException(string propertyName, string message)
            : base($"Validation of property {propertyName} failed: {message}")
        {

        }

        public PropertyValidationException(string propertyName, string message, Exception innerException)
            : base($"Validation of property {propertyName} failed: {message}", innerException)
        {

        }
    }
}