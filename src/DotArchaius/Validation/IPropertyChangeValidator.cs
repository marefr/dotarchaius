namespace DotArchaius.Validation
{
    /// <summary>
    /// Interface for property change validators.
    /// </summary>
    /// <exception cref="PropertyValidationException"></exception>
    public interface IPropertyChangeValidator
    {
        void Validate(string value);
    }
}
