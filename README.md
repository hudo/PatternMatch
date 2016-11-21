## Just another Pattern matching helper class for c#  

Until we get proper native pattern matching with C# 7, I present you *Poor Mans Pattern Matching* for usage in .NET 4.6.1 or NETStandard (Core) projects! 

#### Examples  

```csharp

// match condition:
string result = Pattern.Match<int, string>(70)  
    .When(x => x > 100, () => "> 100")  
    .When(x => x > 50, () => "> 50")  
    .When(x => x > 10, () => "> 10")  
    .Otherwise.Default(() => "");

Assert.Equal(result, "> 50");

// match generic type:
var result = Pattern.Match<IBase, int>(new Foo() {A = 5})
    .When<IFoo>(foo => foo.A)
    .When<IBoo>(boo => boo.B)
    .Otherwise.Throw("Foo is not of type IFoo nor IBoo, buhuhu:(");

Assert.Equal(5, result);


// match any type:
var value = 5;
var result = Pattern.Match<object, string>(value)
    .When(5, () => value.ToString())
    .When("10", () => "10")
    .When(new Foo(), () => "foo")
    .Result;

Assert.Equal("5", result);


```

#### Nuget

https://www.nuget.org/packages/JA.PatternMatch 
