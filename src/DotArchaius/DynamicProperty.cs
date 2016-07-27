using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using DotArchaius.Logging;
using DotArchaius.Logging.Extensions;
using DotArchaius.Util;
using DotArchaius.Validation;

namespace DotArchaius
{
    public class DynamicProperty
    {
        private static readonly ILog Log = LogManager.GetLogger<DynamicProperty>();
        private static IDynamicPropertySupport _dynamicPropertySupport;
        private static readonly ConcurrentDictionary<string, DynamicProperty> AllProperties = new ConcurrentDictionary<string, DynamicProperty>();
        private readonly object _lockObject = new object();
        private string _stringValue;
        private ImmutableList<Action> _callbacks = new List<Action>().ToImmutableList<Action>();
        private ImmutableList<IPropertyChangeValidator> _validators = new List<IPropertyChangeValidator>().ToImmutableList<IPropertyChangeValidator>();

        private readonly CachedValue<string> _cachedStringValue;
        private readonly CachedValue<int> _cachedIntegerValue;

        /// <summary>
        /// Gets the DynamicProperty for a given property name.
        /// </summary>
        /// <param name="propertyName">the name of the property</param>
        /// <returns>a DynamicProperty object</returns>
        /// <remarks>This may be a previously constructed object, 
        /// or an object constructed on-demand to satisfy the request.</remarks>
        public static DynamicProperty GetInstance(string propertyName)
        {
            return AllProperties.GetOrAdd(propertyName, s => new DynamicProperty(propertyName));
        }

        protected DynamicProperty()
        {
            _cachedStringValue = new CachedValue<string>(this);
            _cachedIntegerValue = new CachedValue<int>(this);
        }

        /// <summary>
        /// Create a new DynamicProperty with a given property name.
        /// </summary>
        /// <param name="name">the name of the property</param>
        private DynamicProperty(string name)
            : this()
        {
            Name = name;
            UpdateValue();
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///  Gets the time when the property value was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Gets the current value of the property as a string.
        /// </summary>
        /// <param name="defaultValue">the value to return if the property is not defined</param>
        /// <returns>the current property value, or the default value there is none</returns>
        public string GetString(string defaultValue = null)
        {
            return _cachedStringValue.GetValue(defaultValue);
        }

        /// <summary>
        /// Adds a callback which will be triggered each time the value of the property is updated.
        /// </summary>
        /// <param name="action">the callback</param>
        public void AddCallback(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _callbacks = _callbacks.Add(action);
        }

        /// <summary>
        /// Removes a callback so that it will no longer be triggered when the value of 
        /// the propety is updated.
        /// </summary>
        /// <param name="callback">the callback</param>
        /// <returns>true if the callback was previously registered</returns>
        public void RemoveCallback(Action callback)
        {
            _callbacks = _callbacks.Remove(callback);
        }

        /// <summary>
        /// Adds a validator which will be executed each time the value of the property is updated.
        /// </summary>
        /// <param name="validator">the property change validator</param>
        public void AddValidator(IPropertyChangeValidator validator)
        {
            if (validator == null) throw new ArgumentNullException(nameof(validator));

            _validators = _validators.Add(validator);
        }

        private void RaisePropertyChangedEvent()
        {
            foreach (var callback in _callbacks)
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in DynamicProperty callback", ex);
                }
            }
        }

        private void Validate(string newValue)
        {
            foreach (var validator in _validators)
            {
                try
                {
                    validator.Validate(newValue);
                }
                catch (PropertyValidationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new PropertyValidationException(Name, "Unexpected exception during validation", ex);
                }
            }
        }

        /// <summary>
        /// Updates the value of the property, if there are any changes.
        /// </summary>
        /// <returns>return true if the value actually changed</returns>
        private bool UpdateValue()
        {
            string newValue;

            try
            {
                if (_dynamicPropertySupport != null)
                {
                    newValue = _dynamicPropertySupport.GetString(Name);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unable to update property '{0}'", ex, Name);
                return false;
            }
            return UpdateValue(newValue);
        }

        internal bool UpdateValue(object newValue)
        {
            var nv = newValue?.ToString();

            lock (_lockObject)
            {
                if ((nv == null && _stringValue == null) ||
                    (nv != null && nv.Equals(_stringValue)))
                {
                    return false;
                }

                _stringValue = nv;
                _cachedStringValue.Flush();

                UpdatedAt = SystemClock.Now;
                return true;
            }
        }

        internal static bool UpdateProperty(string propertyName, object value)
        {
            DynamicProperty prop;
            if (AllProperties.TryGetValue(propertyName, out prop))
            {
                if (prop != null && prop.UpdateValue(value))
                {
                    prop.RaisePropertyChangedEvent();
                    return true;
                }
            }

            return false;
        }

        internal static bool UpdateAllProperties()
        {
            bool changed = false;

            foreach (var propertyName in AllProperties.Keys)
            {
                DynamicProperty prop = null;
                if (AllProperties.TryGetValue(propertyName, out prop))
                {
                    if (prop != null && prop.UpdateValue())
                    {
                        prop.RaisePropertyChangedEvent();
                        changed = true;
                    }
                }
            }

            return changed;
        }

        internal static void Validate(string propertyName, object value)
        {
            DynamicProperty prop;
            if (AllProperties.TryGetValue(propertyName, out prop))
            {
                if (prop != null)
                {
                    var newValue = value?.ToString();
                    prop.Validate(newValue);
                }
            }
        }

        private static void Initialize(IDynamicPropertySupport dynamicPropertySupport)
        {
            _dynamicPropertySupport = dynamicPropertySupport;
            _dynamicPropertySupport.AddPropertyEventListener(new DynamicPropertyEventListener());
            UpdateAllProperties();
        }

        public static void RegisterWithDynamicPropertySupport(IDynamicPropertySupport config)
        {
            Initialize(config);
        }

        /// <summary>
        /// Resets the DynamicProperty by clearing all cached properties and the 
        /// registered <see cref="IDynamicPropertySupport"/>, if any.
        /// </summary>
        public static void Reset()
        {
            _dynamicPropertySupport = null;
            AllProperties.Clear();
        }

        public override string ToString()
        {
            return $"DynamicProperty [name={Name}, value={GetString()}";
        }

        /// <summary>
        /// A cached value of a particular type.
        /// </summary>
        /// <typeparam name="T">the type of the cached value</typeparam>
        private class CachedValue<T>
        {
            private readonly DynamicProperty _dynamicProperty;
            private bool _isCached;
            private Exception _exception;
            private T _value;

            public CachedValue(DynamicProperty dynamicProperty)
            {
                _dynamicProperty = dynamicProperty;
                Flush();
            }

            /// <summary>
            /// Flushes the cached value.
            /// </summary>
            /// <remarks>Must be called with the lock variable held by this thread.</remarks>
            protected internal void Flush()
            {
                _isCached = false;
                _exception = null;
                _value = default(T);
            }

            /// <summary>
            /// Gets the cached value.
            /// </summary>
            /// <param name="defaultValue">the value to return if there was a problem</param>
            /// <returns>the parsed value, or the default if there was no string value or a problem 
            /// during parse</returns>
            /// <remarks>If the value has not yet been parsed from the string value, parse it now. 
            /// If there is no string value, or there was a parse error, returns the given default 
            /// value.</remarks>
            public T GetValue(T defaultValue = default(T))
            {
                if (!_isCached)
                {
                    lock (_dynamicProperty._lockObject)
                    {
                        if (!_isCached)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(_dynamicProperty._stringValue))
                                {
                                    Log.Debug($"Value of {_dynamicProperty.Name}' is empty, using the default value");
                                    _value = defaultValue;
                                }
                                else
                                {
                                    _value = Parse(_dynamicProperty._stringValue);
                                }

                                _exception = null;
                            }
                            catch (Exception ex)
                            {
                                _value = defaultValue;
                                Log.Error($"Unable to set value for property '{_dynamicProperty.Name}'", ex);
                                _exception = new ArgumentException($"Unable to get value for property '{_dynamicProperty.Name}'", ex);
                            }
                            finally
                            {
                                _isCached = true;
                            }
                        }
                    }
                }

                if (_exception != null)
                {
                    throw _exception;
                }

                return _value;
            }

            /// <summary>
            /// Parse a string, converting it to an object of the value type.
            /// </summary>
            /// <param name="rep">the string representation to parse</param>
            /// <returns></returns>
            protected virtual T Parse(string rep)
            {
                try
                {
                    return TypeConvertHelper.ConvertValue<T>(rep);
                }
                catch (InvalidOperationException ex)
                {
                    Log.Error($"Unable to parse value for property {_dynamicProperty.Name}", ex);
                    throw;
                }
            }

            public override string ToString()
            {
                if (!_isCached)
                {
                    return "{not cached}";
                }

                if (_exception != null)
                {
                    return "{Exception: " + _exception + "}";
                }

                return "{Value: " + _value + "}";
            }
        }

        private class DynamicPropertyEventListener : IPropertyEventListener
        {
            public void OnPropertySourceLoaded(object source)
            {
                UpdateAllProperties();
            }

            public void OnAddingProperty(object source, string propertyName, object newValue)
            {
                Validate(propertyName, newValue);
            }

            public void OnPropertyAdded(object source, string propertyName, object newValue)
            {
                UpdateProperty(propertyName, newValue);
            }

            public void OnUpdatingProperty(object source, string propertyName, object updatedValue)
            {
                Validate(propertyName, updatedValue);
            }

            public void OnPropertyUpdated(object source, string propertyName, object updatedValue)
            {
                UpdateProperty(propertyName, updatedValue);
            }

            public void OnRemovingProperty(object source, string propertyName, object currentValue)
            {

            }

            public void OnPropertyRemoved(object source, string propertyName)
            {
                UpdateProperty(propertyName, null);
            }

            public void OnClearingProperties(object source)
            {

            }

            public void OnPropertiesCleared(object source)
            {
                UpdateAllProperties();
            }
        }
    }
}
