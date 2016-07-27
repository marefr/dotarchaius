using System;
using DotArchaius.Validation;

namespace DotArchaius
{
    /// <summary>
    /// Base interface for properties.
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the time when the property was last updated.
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// Gets if the property is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the current value of the property as a string.
        /// </summary>
        /// <returns>the current value as a string</returns>
        string GetValueAsString();

        /// <summary>
        /// Gets the default value of the property as a string.
        /// </summary>
        /// <returns>the default value as a string</returns>
        string GetDefaultValueAsString();

        /// <summary>
        /// Add the callback to be triggered when the value of the property is changed.
        /// </summary>
        /// <param name="propertyChangedCallback">the property changed callback method</param>
        void AddCallback(Action propertyChangedCallback);

        /// <summary>
        /// Remove all callbacks registered through the instance of property.
        /// </summary>
        void RemoveAllCallbacks();

        /// <summary>
        /// Adds a validator to be used for validation when the property changes.
        /// </summary>
        /// <param name="validator">Validator to add</param>
        void AddValidator(IPropertyChangeValidator validator);
    }

    /// <summary>
    /// Base interface for properties which has a value and a default value.
    /// </summary>
    /// <typeparam name="T">The value type of the property</typeparam>
    public interface IProperty<out T> : IProperty
    {
        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the default value of the property.
        /// </summary>
        T DefaultValue { get; }
    }
}
