using DotArchaius.Validation;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_with_property_support
{
    [TestFixture]
    public class when_updating_property : DynamicPropertyWithPropertySupportTest
    {
        public bool Validated { get; set; }

        protected override void Setup()
        {
            base.Setup();

            var dp = DynamicProperty.GetInstance(PropertyOneName);
            dp.GetString().ShouldBeNull();

            dp.AddValidator(new PropertyChangeValidatorDelegate(s =>
            {
                Validated = true;
            }));

            DynamicPropertySupport.PropertyEventListener.OnUpdatingProperty(PropertySource, PropertyOneName, null);
        }

        [Test]
        public void should_validate_property()
        {
            Validated.ShouldBeTrue();
        }
    }
}