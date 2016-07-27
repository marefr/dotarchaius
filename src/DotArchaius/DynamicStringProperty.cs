namespace DotArchaius
{
    /// <summary>
    /// A dynamic property whose value is a string.
    /// </summary>
    public class DynamicStringProperty : PropertyWrapper<string>
    {
        public DynamicStringProperty(string propertyName, string defaultValue)
            : base(propertyName, defaultValue)
        {
        }

        public override string Value => WrappedProperty.GetString(DefaultValue);

        public override bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    }
}