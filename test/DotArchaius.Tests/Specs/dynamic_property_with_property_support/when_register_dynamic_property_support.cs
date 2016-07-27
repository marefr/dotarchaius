using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_with_property_support
{
    [TestFixture]
    public class when_register_dynamic_property_support : DynamicPropertyWithPropertySupportTest
    {
        [Test]
        public void should_add_property_event_listener_to_dynamic_property_support()
        {
            DynamicPropertySupport.PropertyEventListener.ShouldNotBeNull();
        }
    }
}