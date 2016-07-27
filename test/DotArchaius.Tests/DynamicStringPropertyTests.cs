using DotArchaius.Extensions;
using DotArchaius.Validation;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests
{
    [TestFixture]
    public class DynamicStringPropertyTests : DynamicPropertyWithInMemoryPropertySupportTest
    {
        public const string PropertyName = "prop1";

        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("default", false)]
        public void PropertyTest(string defaultValue, bool isEmpty)
        {
            var prop = new DynamicStringProperty(PropertyName, defaultValue);
            prop.Name.ShouldBe(PropertyName);

            prop.IsEmpty.ShouldBe(isEmpty);
            prop.Value.ShouldBe(defaultValue);
            prop.GetValueAsString().ShouldBe(defaultValue);
            prop.GetDefaultValueAsString().ShouldBe(defaultValue);

            var expected = "value";
            Dictionary[PropertyName] = expected;

            prop.IsEmpty.ShouldBeFalse();
            prop.Value.ShouldBe(expected);
            prop.GetValueAsString().ShouldBe(expected);
            prop.GetDefaultValueAsString().ShouldBe(defaultValue);

            Dictionary[PropertyName] = defaultValue;

            prop.IsEmpty.ShouldBe(isEmpty);
            prop.Value.ShouldBe(defaultValue);
            prop.GetValueAsString().ShouldBe(defaultValue);
            prop.GetDefaultValueAsString().ShouldBe(defaultValue);

            expected = "value2";
            Dictionary[PropertyName] = expected;

            prop.IsEmpty.ShouldBeFalse();
            prop.Value.ShouldBe(expected);
            prop.GetValueAsString().ShouldBe(expected);
            prop.GetDefaultValueAsString().ShouldBe(defaultValue);

            string oldValue;
            Dictionary.TryRemove(PropertyName, out oldValue);

            prop.IsEmpty.ShouldBe(isEmpty);
            prop.Value.ShouldBe(defaultValue);
            prop.GetValueAsString().ShouldBe(defaultValue);
            prop.GetDefaultValueAsString().ShouldBe(defaultValue);
        }

        [Test]
        public void CallbackTest()
        {
            const string defaultValue = "default-default";
            var callbackCount = 0;
            var prop = new DynamicStringProperty(PropertyName, defaultValue);
            prop.AddCallback(() => callbackCount++);

            prop.Value.ShouldBe(defaultValue);
            callbackCount.ShouldBe(0);

            const string newValue = "new-value";
            Dictionary[PropertyName] = newValue;
            prop.Value.ShouldBe(newValue);

            callbackCount.ShouldBe(1);

            Dictionary.Clear();;

            prop.Value.ShouldBe(defaultValue);
            callbackCount.ShouldBe(2);
        }

        [Test]
        public void AddValidationTest()
        {
            var prop = new DynamicStringProperty(PropertyName, "default");
            prop.AddValidator(s =>
            {
                throw new PropertyValidationException("failed");
            });

            try
            {
                Dictionary[PropertyName] = "new";
                Assert.Fail("PropertyValidationException expected");
            }
            catch (PropertyValidationException ex)
            {
                ex.ShouldNotBe(null);
            }

            prop.Value.ShouldBe("default");
            Dictionary.ContainsKey(PropertyName).ShouldBe(false);

            try
            {
                Dictionary.TryAdd(PropertyName, "new");
                Assert.Fail("PropertyValidationException expected");
            }
            catch (PropertyValidationException ex)
            {
                ex.ShouldNotBe(null);
            }

            prop.Value.ShouldBe("default");
            Dictionary.ContainsKey(PropertyName).ShouldBe(false);

            try
            {
                Dictionary.GetOrAdd(PropertyName, "new");
                Assert.Fail("PropertyValidationException expected");
            }
            catch (PropertyValidationException ex)
            {
                ex.ShouldNotBe(null);
            }

            prop.Value.ShouldBe("default");
            Dictionary.ContainsKey(PropertyName).ShouldBe(false);
        }

        [Test]
        public void SetValidationTest()
        {
            var prop = new DynamicStringProperty(PropertyName, "default");
            Dictionary[PropertyName] = "xyz";

            prop.AddValidator(s =>
            {
                throw new PropertyValidationException("failed");
            });

            try
            {
                Dictionary[PropertyName] = "new";
                Assert.Fail("PropertyValidationException expected");
            }
            catch (PropertyValidationException ex)
            {
                ex.ShouldNotBe(null);
            }

            prop.Value.ShouldBe("xyz");
            Dictionary[PropertyName].ShouldBe("xyz");

            try
            {
                Dictionary.TryUpdate(PropertyName, "new", "xyz");
                Assert.Fail("PropertyValidationException expected");
            }
            catch (PropertyValidationException ex)
            {
                ex.ShouldNotBe(null);
            }

            prop.Value.ShouldBe("xyz");
            Dictionary[PropertyName].ShouldBe("xyz");

            try
            {
                Dictionary.AddOrUpdate(PropertyName, "old", (s, s1) => "new");
                Assert.Fail("PropertyValidationException expected");
            }
            catch (PropertyValidationException ex)
            {
                ex.ShouldNotBe(null);
            }

            prop.Value.ShouldBe("xyz");
            Dictionary[PropertyName].ShouldBe("xyz");
        }
    }
}
