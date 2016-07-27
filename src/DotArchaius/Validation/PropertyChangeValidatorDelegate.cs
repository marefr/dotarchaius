using System;
using System.Collections.Generic;

namespace DotArchaius.Validation
{
    public class PropertyChangeValidatorDelegate : IPropertyChangeValidator
    {
        private readonly List<Action<string>> _validationMethods = new List<Action<string>>();

        public PropertyChangeValidatorDelegate(Action<string> validationMethod)
        {
            _validationMethods.Add(validationMethod);
        }

        public PropertyChangeValidatorDelegate(IEnumerable<Action<string>> validationMethods)
        {
            _validationMethods.AddRange(validationMethods);
        }

        public void Validate(string value)
        {
            foreach (var validationMethod in _validationMethods)
            {
                validationMethod(value);
            }
        }
    }
}