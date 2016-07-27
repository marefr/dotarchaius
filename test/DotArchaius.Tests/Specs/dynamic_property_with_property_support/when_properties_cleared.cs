using System;
using System.Collections.Generic;
using DotArchaius.Util;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_with_property_support
{
    [TestFixture]
    public class when_properties_cleared : DynamicPropertyWithPropertySupportTest
    {
        public DateTime UpdatedAt => new DateTime(2016, 7, 20, 12, 0, 0);

        protected override void Setup()
        {
            base.Setup();

            var dp1 = DynamicProperty.GetInstance(PropertyOneName);
            dp1.GetString().ShouldBeNull();

            var dp2 = DynamicProperty.GetInstance(PropertyTwoName);
            dp2.GetString().ShouldBeNull();

            var dp3 = DynamicProperty.GetInstance(PropertyThreeName);
            dp3.GetString().ShouldBeNull();

            DynamicPropertySupport.Dictionary = new Dictionary<string, string>
            {
                {PropertyOneName, PropertyOneValue },
                {PropertyTwoName, PropertyTwoValue },
                {PropertyThreeName, PropertyThreeValue }
            };

            DynamicPropertySupport.PropertyEventListener.OnPropertySourceLoaded(PropertySource);

            SystemClock.Adjust(() => UpdatedAt);

            DynamicPropertySupport.Dictionary = null;
            DynamicPropertySupport.PropertyEventListener.OnPropertiesCleared(PropertySource);
        }

        [TestCase(PropertyOneName)]
        [TestCase(PropertyTwoName)]
        [TestCase(PropertyThreeName)]
        public void should_update_property_value(string propertyName)
        {
            DynamicProperty.GetInstance(propertyName).GetString().ShouldBeNull();
        }

        [TestCase(PropertyOneName)]
        [TestCase(PropertyTwoName)]
        [TestCase(PropertyThreeName)]
        public void should_update_updated_at(string propertyName)
        {
            DynamicProperty.GetInstance(propertyName).UpdatedAt.ShouldBe(UpdatedAt);
        }
    }
}