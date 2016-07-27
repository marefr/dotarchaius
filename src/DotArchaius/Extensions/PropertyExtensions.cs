using System;
using DotArchaius.Validation;

namespace DotArchaius.Extensions
{
    public static class PropertyExtensions
    {
        public static void AddValidator<TValue>(this IProperty<TValue> property, params Action<string>[] validationMethods)
        {
            property.AddValidator(new PropertyChangeValidatorDelegate(validationMethods));
        }
    }
}
