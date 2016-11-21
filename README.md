## Pattern matching helper class for c#  

Poor mans pattern matching for usage in c# until it gets native support. Requires .NET 4.6 or Core. 

#### Examples  

```csharp

string result = Pattern.Match<int, string>(70)  
    .When(x => x > 100, () => "> 100")  
    .When(x => x > 50, () => "> 50")  
    .When(x => x > 10, () => "> 10")  
    .Default(() => "");

Assert.Equal(result, "> 50");


var result = Pattern.Match<IBase, int>(new Foo() {A = 5})
    .When<IFoo>(foo => foo.A)
    .When<IBoo>(boo => boo.B)
    .Default(() => 0);

Assert.Equal(5, result);


var value = 5;
var result = Pattern.Match<object, string>(value)
    .When(5, () => value.ToString())
    .When("10", () => "10")
    .When(new Foo(), () => "foo")
    .Result;

Assert.Equal("5", result);

```