using DotArchaius.Util;
using NUnit.Framework;
using Shouldly;

namespace DotArchaius.Tests.Util
{
    [TestFixture]
    public class TypeConvertHelperTests
    {
        [Test]
        public void should_convert_string_to_string()
        {
            var value = TypeConvertHelper.ConvertValue<string>("value");
            value.ShouldBe("value");
        }

        [Test]
        public void should_convert_string_to_int()
        {
            var value = TypeConvertHelper.ConvertValue<int>("10");
            value.ShouldBe(10);
        }

        [Test]
        public void should_convert_true_string_to_nullable_boolean()
        {
            var value = TypeConvertHelper.ConvertValue<bool?>("true");
            value.HasValue.ShouldBeTrue();
            value.ShouldBe(true);
        }

        [Test]
        public void should_convert_false_string_to_nullable_boolean()
        {
            var value = TypeConvertHelper.ConvertValue<bool?>("false");
            value.HasValue.ShouldBeTrue();
            value.ShouldBe(false);
        }
    }
}
