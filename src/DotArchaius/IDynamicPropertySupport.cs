namespace DotArchaius
{
    public interface IDynamicPropertySupport
    {
        /// <summary>
        /// Get the string value of a given property. The string value will be further 
        /// cached and parsed into specific type for DynamicProperty.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The string value of the property</returns>
        string GetString(string propertyName);

        /// <summary>
        /// Add the property event listener. This is necessary for the <see cref="DynamicProperty"/> to 
        /// receive callback once a property is updated in the underlying <see cref="IDynamicPropertySupport"/>
        /// </summary>
        /// <param name="propertyEventListener">Listener to be added to <see cref="IDynamicPropertySupport"/></param>
        void AddPropertyEventListener(IPropertyEventListener propertyEventListener);
    }
}