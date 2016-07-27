using System;
using System.Collections.Generic;
using DotArchaius.Util;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_with_property_support
{
    [TestFixture]
    public class when_existing_property_value_added : DynamicPropertyWithPropertySupportTest
    {
        public bool PropertyChangedRaised { get; set; }
        public DateTime UpdatedAt => new DateTime(2016, 7, 20, 12, 0, 0);

        protected override void Setup()
        {
            base.Setup();

            var dp = DynamicProperty.GetInstance(PropertyOneName);
            dp.GetString().ShouldBeNull();

            DynamicPropertySupport.Dictionary = new Dictionary<string, string>
            {
                {PropertyOneName, PropertyOneValue }
            };

            SystemClock.Adjust(() => UpdatedAt);

            DynamicPropertySupport.PropertyEventListener.OnPropertySourceLoaded(PropertySource);

            dp.AddCallback(() =>
            {
                PropertyChangedRaised = true;
            });

            SystemClock.Adjust(() => new DateTime(2016, 7, 26, 12, 0, 0));

            DynamicPropertySupport.PropertyEventListener.OnPropertyAdded(PropertySource, PropertyOneName, PropertyOneValue);
        }

        [Test]
        public void should_not_update_property_value()
        {
            DynamicProperty.GetInstance(PropertyOneName).GetString().ShouldBe(PropertyOneValue);
        }

        [Test]
        public void should_not_update_property_updated_at()
        {
            DynamicProperty.GetInstance(PropertyOneName).UpdatedAt.ShouldBe(UpdatedAt);
        }

        [Test]
        public void should_not_raise_property_changed()
        {
            PropertyChangedRaised.ShouldBeFalse();
        }
    }
}