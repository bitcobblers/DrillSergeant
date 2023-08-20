# DrillSergeant
.net behavior driven testing written by developers, for developers.

[![Build](https://github.com/BitCobblers/DrillSergeant/actions/workflows/test.yml/badge.svg)](https://github.com/bitcobblers/DrillSergeant/actions/workflows/test.yml)
[![Nuget](https://img.shields.io/nuget/v/DrillSergeant.svg)](https://www.nuget.org/packages/DrillSergeant/)
[![Nuget](https://img.shields.io/nuget/dt/DrillSergeant)](https://www.nuget.org/packages/DrillSergeant/)
[![codecov](https://codecov.io/gh/bitcobblers/DrillSergeant/branch/main/graph/badge.svg?token=R9MKC6IJXE)](https://codecov.io/gh/bitcobblers/DrillSergeant)
[![CodeQL](https://github.com/bitcobblers/DrillSergeant/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/bitcobblers/DrillSergeant/actions/workflows/github-code-scanning/codeql)
[![Qodana](https://github.com/bitcobblers/DrillSergeant/actions/workflows/code_quality.yml/badge.svg?branch=main)](https://github.com/bitcobblers/DrillSergeant/actions/workflows/code_quality.yml)

# Introduction

DrillSergeant is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.  Unlike other behavior testing frameworks which rely on external feature files to write scenarios, DrillSergeant lets you write behavior tests 100% in C# code.

## Getting Started

For a complete example of a feature, see the following [example](https://github.com/bitcobblers/DrillSergeant/blob/main/test/DrillSergeant.Tests.Shared/Features/CalculatorFeature.cs).
For something more complex, see the [DemoStore](https://github.com/bitcobblers/StoreDemo) repo.

## The Obligatory Hello World Example

Lets say we have a `Calculator` service.  For this example we'll define it as a simple class.
```CSharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
}
```
We can write a behavior test like so:
```CSharp
public class CalculatorTests
{
    [Behavior]
    [InlineData(1,2,3)] // [TestCase] for NUnit or [DataRow] for MSTest.
    [InlineData(2,3,5)]
    public void TestAdditionBehavior(int a, int b, int expected)
    {
        var calculator = Given("Create a calculator", () => new Calculator());
        var result = When($"Add {a} and {b}", () => calculator.Add(a, b));
        Then(CheckResult("Check result", () => Assert.Equal(expected, result));
    }
}
```

And the test runner output should look like this:

![image](https://github.com/bitcobblers/DrillSergeant/assets/5205466/3d5b364c-3549-42e6-aaea-67373faa8aa8)

Behaviors are written in same fashion as a normal unit test.  The only difference is that it is marked using the `[Behavior]` attribute.

## A More Advanced Example

From the [StoreDemo](https://github.com/bitcobblers/StoreDemo) project we define a behavior test to verify that we can create a new shopping cart, add items to it, and then purchase its contents:

```CSharp
[Behavior]
public void PurchasingItemsInCartCreatesNewOrder()
{
    var client = _api.CreateClient();

    Given(CartSteps.NewCart(client));
    Given(CartSteps.LoadProducts(client));
    Given(CartSteps.AddRandomProductToCart(client));
    When(OrderingSteps.PlaceOrder(client));
    Then(OrderingSteps.CheckOrderId());
}
```

Where `client` is an instance of `HttpClient`.  Within `CartSteps` we define the following steps:

```CSharp
public static class CartSteps
{
    private static readonly Random random = new();

    public static LambdaStep NewCart(HttpClient client) =>
        new LambdaStep("Create new cart")
            .HandleAsync(async () =>
            {
                var url = $"api/cart/new";
                var response = await client.GetStringAsync(url);

                CurrentBehavior.Context.CartId = int.Parse(response);
            });

    public static LambdaStep LoadProducts(HttpClient client) =>
        new LambdaStep("Get product list")
            .HandleAsync(async () =>
            {
                var url = "api/products";
                var response = await client.GetFromJsonAsync<Product[]>(url);

                CurrentBehavior.Context.Products = response;
            });

    public static LambdaStep AddRandomProductToCart(HttpClient client) =>
        new LambdaStep("Add random product to cart")
            .HandleAsync(async () =>
            {
                var cartId = (int)CurrentBehavior.Context.CartId;
                var products = (Product[])CurrentBehavior.Context.Products;
                var product = products[random.Next(0, products.Length)];

                var url = "api/cart/add";
                await client.PostAsJsonAsync(url, new AddProductRequest(cartId, product.Id, 1));
            });
}
```

And within `OrderingSteps` we define the steps:

```CSharp
    public static LambdaStep PlaceOrder(HttpClient client) =>
        new LambdaStep("Place order")
            .HandleAsync(async () =>
            {
                var cartId = (int)CurrentBehavior.Context.CartId;
                var order = new PlaceOrderRequest(cartId);
                var url = "api/order/place";

                var response = await client.PostAsJsonAsync(url, order);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var body = await response
                        .Content
                        .ReadFromJsonAsync<PlaceOrderResponse>();

                    CurrentBehavior.Context.OrderId = body?.OrderNumber;
                }
                else
                {
                    CurrentBehavior.Context.OrderId = null;
                }
            });

    public static LambdaStep CheckOrderId() =>
        new LambdaStep("Check order id is set")
            .Handle(() => Assert.NotNull(CurrentBehavior.Context.OrderId));
}

```

This time when we run the test we get the following output in our test runner:

![image](https://github.com/bitcobblers/DrillSergeant/assets/5205466/f70a692c-7a30-4ca3-93d3-9687068e2be4)

## Why Write Tests This Way?

Unlike in normal unit tests, which are intended to test the correctness of individual methods, behaviors tests validate whether one or more components actually behave in the way expected when given "normal" inputs.  Because of this, behaviors are composed of a series of pluggable steps that can be re-used in different scenarios.  See the [Cucumber](https://cucumber.io/docs/guides/overview/) documentation for an introduction into behavior testing.

## Comparison with 3rd Party Acceptance Testing Tools (e.g., SpecFlow, Fitnesse, Gauge)

DrillSergeant was borne out of frustration of using 3rd party testing tools.  While tools such as SpecFlow and Gauge have gotten easier to use over time, they require installing 3rd party plugins/runners in the developer environment.  Additionally they require separate files for authoring the tests themselves (`.feature` for Specflow, `.wiki` for FitNesse, and `.md` for Gauge).  This relies on a mixture of code generation and reflection magic in order to bind the test specifications with the code that actually runs them, which adds a layer of complexity.

DrillSergeant takes a different approach to this problem.  Rather than rely on DSLs and complex translation layers, it engrafts additional capabilities to the xunit framework to make it easy to write behavior-driven with familiar C# syntax.  No new DSLs to learn, no build task fussiness, no reflection shenanigans.  Just a simple API written entirely in C# code that can be tested/debugged the exact same way as all of your other unit tests.

For a longer-winded explanation, see the following [blog post](https://www.bitcobblers.com/b/behavior-driven-testing/).

## Test Runner Compatibility

Originally DrillSergeant was built around xunit and has been well tested with it.  As of version 0.2.0 support has been added for NUnit and MSTest.  

The NUnit integration is likely to be fairly stable since the framework was designed with extensibility support in mind.  This made adding hooks for DrillSergeant fairly trivial.

The MSTest integration on the other hand should be considered experimental.  This is because that framework has very limited support for extensibility and needed several somewhat invasive hacks to get working.  If anyone has experience with MSTest and would like to help with this please let us know!

## Installation

DrillSergeant is a regular library and can be installed via package manager with either the `Install-Package` or `dotnet add package` commands.  Note that because DrillSergeant is still in beta that you will need check the 'Include Prelease' checkbox to find it in nuget manager.

|Framework|Package             |Example                                  |
|---------|--------------------|-----------------------------------------|
|Xunit    |DrillSergeant.Xunit2|`dotnet add package DrillSergeant.Xunit2`|
|NUnit    |DrillSergeant.NUnit3|`dotnet add package DrillSergeant.NUnit3`|
|MSTest   |DrillSergeant.MSTest|`dotnet add package DrillSergeant.MSTest`|

## Support

If you encounter any issues running tests or would like a feature added please do so [here](https://github.com/bitcobblers/DrillSergeant/issues/new/choose).  DrillSergeant is still fairly new and under active development.

And if you like the project, be sure to give it a star!

## More Information

For more information, please see the [wikis](https://github.com/bitcobblers/DrillSergeant/wiki).  
For an introduction, please see this [Medium](https://medium.com/@michael.vastarelli/behavior-testing-with-drill-sergeant-cd9e747688da) article.
