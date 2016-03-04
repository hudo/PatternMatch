using NUnit.Framework;

namespace PatternMatch.Tests
{
    public class PatternTests
    {
        [TestCase(150, "> 100")]
        [TestCase(70, "> 50")]
        [TestCase(20, "> 10")]
        [TestCase(2, "")]
        public void Condition_with_action(int n, string expected)
        {
            var result = Pattern.Match<int, string>(n)
                .When(x => x > 100, () => "> 100")
                .When(x => x > 50, () => "> 50")
                .When(x => x > 10, () => "> 10")
                .Otherwise.Default(() => "");

            Assert.AreEqual(result, expected);
        }

        [Test]
        public void Match_type()
        {
            var result = Pattern.Match<IBase, int>(new Foo() {A = 5})
                .When<IFoo>(foo => foo.A)
                .When<IBoo>(boo => boo.B)
                .Otherwise.Default(() => 0);

            Assert.AreEqual(5, result);
        }

        [Test]
        public void Instance_value()
        {
            var value = 5;
            var result = Pattern.Match<object, string>(value)
                .When(5, () => value.ToString())
                .When("10", () => "10")
                .When(new Foo(), () => "foo")
                .Result;

            Assert.AreEqual("5", result);
        }

        interface IFoo : IBase { int A { get; } }
        class Foo : IFoo { public int A { get; set; } }

        interface IBoo : IBase { int B { get; } }
        class Boo : IBoo { public int B { get; set; } }

        interface IBase { }
    }
}
