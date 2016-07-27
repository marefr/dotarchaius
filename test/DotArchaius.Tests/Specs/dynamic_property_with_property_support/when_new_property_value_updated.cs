using System;
using DotArchaius.Util;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_with_property_support
{
    [TestFixture]
    public class when_new_property_value_updated : DynamicPropertyWithPropertySupportTest
    {
        public bool PropertyChangedRaised { get; set; }
        public DateTime UpdatedAt => new DateTime(2016, 7, 20, 12, 0, 0);

        protected override void Setup()
        {
            base.Setup();

            var dp = DynamicProperty.GetInstance(PropertyOneName);
            dp.GetString().ShouldBeNull();

            dp.AddCallback(() =>
            {
                PropertyChangedRaised = true;
            });

            SystemClock.Adjust(() => UpdatedAt);

            DynamicPropertySupport.PropertyEventListener.OnPropertyUpdated(PropertySource, PropertyOneName, PropertyOneValue);
        }

        [Test]
        public void should_update_property_value()
        {
            DynamicProperty.GetInstance(PropertyOneName).GetString().ShouldBe(PropertyOneValue);
        }

        [Test]
        public void should_update_property_updated_at()
        {
            DynamicProperty.GetInstance(PropertyOneName).UpdatedAt.ShouldBe(UpdatedAt);
        }

        [Test]
        public void should_raise_property_changed()
        {
            PropertyChangedRaised.ShouldBeTrue();
        }
    }
}