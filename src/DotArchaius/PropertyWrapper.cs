using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DotArchaius.Validation;

namespace DotArchaius
{
    /// <summary>
    /// A wrapper around DynamicProperty and associates it with a type.
    /// </summary>
    /// <typeparam name="TValue">The type of the DynamicProperty</typeparam>
    public abstract class PropertyWrapper<TValue> : IProperty<TValue>, IEquatable<IProperty<TValue>>
    {
        private static readonly HashSet<Type> SubclassesWithNoCallback = new HashSet<Type>();
        private ImmutableList<Action> _callbacks = new List<Action>().ToImmutableList();

        static PropertyWrapper()
        {
            RegisterSubClassWithNoCallback<DynamicStringProperty>();
        }

        protected PropertyWrapper(string propertyName, TValue defaultValue)
        {
            WrappedProperty = DynamicProperty.GetInstance(propertyName);
            DefaultValue = defaultValue;

            if (!SubclassesWithNoCallback.Contains(GetType()))
            {
                WrappedProperty.AddCallback(PropertyChanged);
            }
        }

        /// <summary>
        /// Register a subclass to not receive callbacks.
        /// </summary>
        /// <typeparam name="T">The type of the subclass</typeparam>
        public static void RegisterSubClassWithNoCallback<T>()
        {
            SubclassesWithNoCallback.Add(typeof(T));
        }

        protected DynamicProperty WrappedProperty { get; }

        /// <summary>
        /// Gets the current value of the property.
        /// </summary>
        public abstract TValue Value { get; }

        /// <summary>
        /// Gets the default value of the property.
        /// </summary>
        public TValue DefaultValue { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name => WrappedProperty.Name;

        /// <summary>
        /// Gets the time when the property was last updated.
        /// </summary>
        public DateTime UpdatedAt => WrappedProperty.UpdatedAt;

        /// <summary>
        /// Gets if the property is empty.
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Gets the current value of the property as a string.
        /// </summary>
        /// <returns>the current value as a string</returns>
        public string GetValueAsString()
        {
            return Value?.ToString();
        }

        /// <summary>
        /// Gets the default value of the property as a string.
        /// </summary>
        /// <returns>the default value as a string</returns>
        public string GetDefaultValueAsString()
        {
            return DefaultValue?.ToString();
        }

        /// <summary>
        /// Add the callback to be triggered when the value of the property is changed.
        /// </summary>
        /// <param name="propertyChangedCallback">the property changed callback method</param>
        public void AddCallback(Action propertyChangedCallback)
        {
            if (propertyChangedCallback != null)
            {
                WrappedProperty.AddCallback(propertyChangedCallback);
                _callbacks = _callbacks.Add(propertyChangedCallback);
            }
        }

        /// <summary>
        /// Remove all callbacks registered through the instance of property.
        /// </summary>
        public void RemoveAllCallbacks()
        {
            foreach (var callback in _callbacks)
            {
                WrappedProperty.RemoveCallback(callback);
            }
        }

        /// <summary>
        /// Adds a validator to be used for validation when the property changes.
        /// </summary>
        /// <param name="validator">Validator to add</param>
        public void AddValidator(IPropertyChangeValidator validator)
        {
            if (validator != null)
            {
                WrappedProperty.AddValidator(validator);
            }
        }

        protected virtual void PropertyChanged()
        {
            PropertyChanged(Value);
        }

        protected virtual void PropertyChanged(TValue newValue) { }

        public bool Equals(IProperty<TValue> other)
        {
            return Name == other.Name &&
                   Value.Equals(other.Value) &&
                   DefaultValue.Equals(other.DefaultValue) &&
                   UpdatedAt == other.UpdatedAt;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var property = obj as PropertyWrapper<TValue>;

            if (property != null)
            {
                return Equals(property);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^
                   Value.GetHashCode() ^
                   DefaultValue.GetHashCode() ^
                   UpdatedAt.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name}[name={Name}, value={WrappedProperty.GetString()}, defaultValue={DefaultValue}]";
        }
    }
}