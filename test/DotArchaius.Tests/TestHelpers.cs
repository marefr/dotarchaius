using System.Collections.Generic;
using DotArchaius.Util;
using NUnit.Framework;

namespace DotArchaius.Tests
{
    [TestFixture]
    public abstract class BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            SystemClock.Reset();
            Setup();
        }

        [TearDown]
        public void TearDown()
        {
            Teardown();
        }

        protected virtual void Setup() { }
        protected virtual void Teardown() { }
    }

    [TestFixture]
    public class DynamicPropertyWithoutPropertySupportTest : BaseTest
    {
        public const string PropertyOneName = "prop1";
        public const string PropertyOneValue = "value1";

        protected override void Teardown()
        {
            DynamicProperty.Reset();

            base.Teardown();
        }
    }

    [TestFixture]
    public class DynamicPropertyWithPropertySupportTest : BaseTest
    {
        public const string PropertySource = "test";
        public const string PropertyOneName = "prop1";
        public const string PropertyTwoName = "prop2";
        public const string PropertyThreeName = "prop3";
        public const string PropertyOneValue = "value1";
        public const string PropertyTwoValue = "value2";
        public const string PropertyThreeValue = "value3";

        public DynamicPropertySupport DynamicPropertySupport { get; private set; }

        protected override void Setup()
        {
            DynamicPropertySupport = new DynamicPropertySupport();
            DynamicProperty.RegisterWithDynamicPropertySupport(DynamicPropertySupport);

            base.Setup();
        }

        protected override void Teardown()
        {
            DynamicProperty.Reset();

            base.Teardown();
        }
    }

    public class DynamicPropertySupport : IDynamicPropertySupport
    {
        public Dictionary<string, string> Dictionary { get; set; }
        public IPropertyEventListener PropertyEventListener { get; private set; }

        public string GetString(string propertyName)
        {
            return Dictionary?[propertyName];
        }

        public void AddPropertyEventListener(IPropertyEventListener propertyEventListener)
        {
            PropertyEventListener = propertyEventListener;
        }
    }
}