using System.Collections.Generic;
using Xunit;

namespace JA.PatternMatch.Tests
{
    public class PatternTests
    {
        [Theory]
        [InlineData(150, "> 100")]
        [InlineData(70, "> 50")]
        [InlineData(20, "> 10")]
        [InlineData(2, "")]
        public void Condition_with_action(int n, string expected)
        {
            var result = Pattern.Match<int, string>(n)
                .When(x => x > 100, () => "> 100")
                .When(x => x > 50, () => "> 50")
                .When(x => x > 10, () => "> 10")
                .Otherwise.Default(() => "");

            Assert.Equal(result, expected);
        }

        [Fact]
        public void Match_type()
        {
            var result = Pattern.Match<IBase, int>(new Foo() {A = 5})
                .When<IFoo>(foo => foo.A)
                .When<IBoo>(boo => boo.B)
                .Otherwise.Default(() => 0);

            Assert.Equal(5, result);
        }

        [Fact]
        public void Instance_value()
        {
            var value = 5;
            var result = Pattern.Match<object, string>(value)
                .When(5, () => value.ToString())
                .When("10", () => "10")
                .When(new Foo(), () => "foo")
                .Result;

            Assert.Equal("5", result);
        }

        interface IFoo : IBase { int A { get; } }
        class Foo : IFoo { public int A { get; set; } }

        interface IBoo : IBase { int B { get; } }
        class Boo : IBoo { public int B { get; set; } }

        interface IBase { }
    }
}
