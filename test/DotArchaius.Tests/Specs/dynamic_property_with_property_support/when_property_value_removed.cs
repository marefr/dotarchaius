using System;
using System.Collections.Generic;
using DotArchaius.Util;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Specs.dynamic_property_with_property_support
{
    [TestFixture]
    public class when_property_value_removed : DynamicPropertyWithPropertySupportTest
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

            DynamicPropertySupport.PropertyEventListener.OnPropertySourceLoaded(PropertySource);

            dp.AddCallback(() =>
            {
                PropertyChangedRaised = true;
            });

            SystemClock.Adjust(() => UpdatedAt);

            DynamicPropertySupport.PropertyEventListener.OnPropertyRemoved(PropertySource, PropertyOneName);
        }

        [Test]
        public void should_update_property_value()
        {
            DynamicProperty.GetInstance(PropertyOneName).GetString().ShouldBeNull();
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