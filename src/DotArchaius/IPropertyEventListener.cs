namespace DotArchaius
{
    /// <summary>
    /// Listener that handles property event notifications. It handles events to add a property, set property, 
    /// remove property, load and clear of the configuration source. 
    /// </summary>
    /// <remarks><see cref="IDynamicPropertySupport"/> registers this type listener with a 
    /// <see cref="IDynamicPropertySupport"/> to receivecallbacks on changes so that it can dynamically 
    /// change a value of a DynamicProperty.</remarks>
    public interface IPropertyEventListener
    {
        /// <summary>
        /// Notifies this listener about a new source of properties being invalidated and/or added.
        /// </summary>
        /// <param name="source">the event source</param>
        void OnPropertySourceLoaded(object source);

        /// <summary>
        /// Notifies this listener about a new value being added for the given property.
        /// </summary>
        /// <param name="source">the event source</param>
        /// <param name="propertyName">the property name</param>
        /// <param name="newValue">the new property value</param>
        void OnAddingProperty(object source, string propertyName, object newValue);

        /// <summary>
        /// Notifies this listener about a new value have been added for the given property.
        /// </summary>
        /// <param name="source">the event source</param>
        /// <param name="propertyName">the property name</param>
        /// <param name="newValue">the new property value</param>
        void OnPropertyAdded(object source, string propertyName, object newValue);

        /// <summary>
        /// Notifies this listener about a value being updated for the given property.
        /// </summary>
        /// <param name="source">the event source</param>
        /// <param name="propertyName">the property name</param>
        /// <param name="updatedValue">the updated property value</param>
        void OnUpdatingProperty(object source, string propertyName, object updatedValue);

        /// <summary>
        /// Notifies this listener about a value that have been updated for the given property.
        /// </summary>
        /// <param name="source">the event source</param>
        /// <param name="propertyName">the property name</param>
        /// <param name="updatedValue">the updated property value</param>
        void OnPropertyUpdated(object source, string propertyName, object updatedValue);

        /// <summary>
        /// Notifies this listener about a property being removed.
        /// </summary>
        /// <param name="source">the event source</param>
        /// <param name="propertyName">the property name</param>
        /// <param name="currentValue">the current property value</param>
        void OnRemovingProperty(object source, string propertyName, object currentValue);

        /// <summary>
        /// Notifies this listener about a property that have been removed.
        /// </summary>
        /// <param name="source">the event source</param>
        /// <param name="propertyName">the property name</param>
        void OnPropertyRemoved(object source, string propertyName);

        /// <summary>
        /// Notifies this listener that all properties are being cleared.
        /// </summary>
        /// <param name="source">the event source</param>
        void OnClearingProperties(object source);

        /// <summary>
        /// Notifies this listener that all properties have been cleared.
        /// </summary>
        /// <param name="source">the event source</param>
        void OnPropertiesCleared(object source);
    }
}