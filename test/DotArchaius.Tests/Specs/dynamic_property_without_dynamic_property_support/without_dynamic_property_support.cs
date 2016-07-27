using System;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_without_dynamic_property_support
{
    [TestFixture]
    public class when_requesting_a_new_dynamic_property_instance : DynamicPropertyWithoutPropertySupportTest
    {
        public DynamicProperty DynamicProperty { get; set; }

        protected override void Setup()
        {
            base.Setup();

            DynamicProperty = DynamicProperty.GetInstance(PropertyOneName);
        }

        [Test]
        public void should_return_same_property_name_as_initialized()
        {
            DynamicProperty.Name.ShouldBe(PropertyOneName);
        }

        [Test]
        public void should_return_default_datetime_value_for_updated_at()
        {
            DynamicProperty.UpdatedAt.ShouldBe(default(DateTime));
        }

        [Test]
        public void should_return_null_as_string_value()
        {
            DynamicProperty.GetString().ShouldBeNull();
        }

        [Test]
        public void should_return_default_value_as_string_value()
        {
            DynamicProperty.GetString("default").ShouldBe("default");
        }
    }
}