# JustBehave
.net behavior driven testing written by developers, for developers.

# Summary

**JustBehave** is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package, write your behaviors in familiar C# syntax, and execute using your preferred test runner (e.g. xunit).

# Disclaimer

This is a prototypical/proposed framework for behavior driven development within the .net ecosystem.  Many of the ideas put forth have not been implemented (yet).  This project is still in its very earliest days of development and this content will likely change (probably drastically).

# Motivation (full of acerbic opinions and musings of a 15 year veteran)

Behavior driven testing is a bit of an oddity in software development.  Ask any developer about the tests they write on a daily basis and you'll commonly hear about unit and integration tests.  Bring up behavior testing however and you'll likely be met with a bemused stare followed by them walking away while muttering a soliloquy about how they're just integration tests or that it's someone else's job.  

*"But we use integration testing to verify our behavior."* is a common response, and to be fair it's not entirely wrong either.  There absolutely is a significant amount of overlap between the two and it doesn't help that the strata of testing methodologies is littered with proverbial landmines that can (and probably have) lead to gun fights between warring factions.  The terms are so loaded and ambiguous that some companies (e.g. Google) have dropped them entirely in favor of their own categorization [strategy](https://dilushakumarage.medium.com/how-google-tests-software-b5b7b999ccfa).  

As a software developer we develop a relationship with the tools we use daily and have a strong propensity to want to leverage them as much as possible.  We're familiar with them, understand how they work, their strengths, their quirks, etc...  So it's not unexpected to get push-back when frameworks like [gauge](https://gauge.org/) or [SpecFlow](https://specflow.org/) get brought up.  After all, seeking the comfort of the familiar is human psychology 101, and nothing is quite as scary as the unknown.  But in this particular instance, using these systems is likely a behavior (pardon the pun) borne out of necessity rather than desire.

## Just Let the Developers Write the Tests Already

The prevailing wisdom is that the behavior tests should be the responsibility of the stakeholders.  After all, they're the ones that cook up the requirements for the system, so that should make them eminently qualified to understand what the system's behavior should be as well, right?  Well, to be blunt, they aren't.  If they were, then all of the developers would just call it a day and go home since this would imply that the stakeholders already possess the technical aptitude necessary to write the system themselves.  Behavior testing, much like any other engineering exercise requires an enormous amount of forethought to implement correctly.  Even though companies don't ship their test code, it is their single greatest safeguard to verify that the product they *do* ship works as advertised.  As such, great care must go into the testing portion of an application.  Additionally, even though the tests themselves appear simple, their simplicity belies the required complexity necessary to implement the plumbing needed to execute them, and *that* is a highly technical task that warrants much skill and expertise.

Finally, Software developers are experts at translating requirements into actual software specifications.  It's literally our job to figure out how to turn a half-baked, high level idea into working code.  There aren't very many Tom Smykowskis in the real world.  We already know how to translate other types of requirements into code.  Just let us translate the tests as well already.  

## Stop Hiding Behind DSLs

There's a school of thought that if only we had a DSL expressive enough, yet simple to read/write that the stakeholders could finally be the masters of their own destiny when it comes to authoring behavior tests.  I hate to be the bearer of bad news, but no such DSL exists.  It never has, and it never will.  If it did, we would already be using it to write the system itself.  "Silver bullets" similar to this are heavily scrutinized (and ultimately dismissed) in [The Mythical Man-Month](https://www.amazon.com/Mythical-Man-Month-Software-Engineering-Anniversary/dp/0201835959/ref=sr_1_1?keywords=mythical+man+month&qid=1655084010&sprefix=mythical%2Caps%2C92&sr=8-1).  That said, products such as [Fitnesse](http://fitnesse.org/)(wiki), [gauge](https://gauge.org/)(markdown), and [SpecFlow](https://specflow.org/)(gherkin) have all made significant strides in promoting a simple, easy-to-read syntax for expressing system behavior.  Unfortunately they all suffer from an inherent impedence mismatch between their structure and actual programming code.  In order to "glue" these systems together, a number of strategies are used such as code generation (SpecFlow) or reflection (Fitnesse) to get types to "match up" with their specification counterpart.  SpecFlow and Gauge in particular do deserve extra credit for the great lengths they've gone to reduce this level of friction between test specifications and their underlying programming implementation, however.  

A common complaint I've often heard from developers when discussing the use of DSLs for business specifications is "why do I need this when I can just use C#?".  It's a reasonable question.  C# is a remarkably robust language and with a little skill it's entirely possible to write code that is nearly indistinguishable from a DSL.  In fact, some have already done this.  Consider [xbehave](https://github.com/adamralph/xbehave.net/wiki/Quick-start) which allows developers to delineate scenarios (behaviors) into a series of named steps with their actual implementation hidden behind a lambda.  These scenarios are seamlessly integrated into the xunit framework and executed using the exact same test runner that all of the other tests go through.  That means that the existing quality gates, reporting systems, and metrics gathering tools all continue to work exactly as before.

This is not to say that DSLs are useless.  Quite the opposite!  Gherkin is a fantastic syntax for describing system behavior and we all love to write documentation in Markdown syntax, but at the end of the day these are data structures, not programming languages, and tests are ultimately written in programming languages.  

## C# to the Rescue

It's hard to believe it, but C# is 20 years old at the time of writing.  I was a scrappy college student when it first hit the scene, and next year it'll be able to order a beer without a fake ID anymore.  The language today is very different today than it was in 2002 with many features common today that didn't even exist in its earlier days.  While it is not a metalanguage in the same vein as languages such as LISP or (to a lesser degree) C/C++, it does have a very malleable syntax and there are many tricks to make it look different from conventional programming code.  Instead of relying on a DSL, an alternative is to construct a programming API that "looks" like a DSL but is still compiled as regular C# code.

## Gherkin Without the Hassle

Gherkin is great.  It's simple, succinct, and can be used to express a wide variety of ideas.  The difficulty with it however is the aforementioned friction between specification files which need to go through a translation layer and the code itself.  To their credit, Tricentis (the authors of SpecFlow) has done a terrific job of minimizing this as much as possible by making clever use of build targets to seamlessly translate specifications (features) into their corresponding testing framework-specific code.  However, it can be opined that this complexity isn't really necessary.  In order to write a feature using SpecFlow the developer must first create a `.feature` file, annotate it with the markup for the scenarios to test, then create a corresponding C# class file(s) containing the actual code for each step.  In order for SpecFlow to make the connection between the two, specific `[Attribute]`s are used with magic strings containing regexes in order to capture parameters.  In addition, since a real-world application will likely have many features (and therefore many, many more individual steps defined) it is necessary to devise a strategy for [tagging](https://docs.specflow.org/projects/specflow/en/latest/Bindings/Scoped-Step-Definitions.html) steps early on in order to avoid global step pollution since by default any step can be used in any scenario test regardless of whether or not they have anything to do with that scenario.  

This isn't to besmirch SpecFlow.  It's a great product and works well.  I personally use it professionally.  But it isn't without its complexities.  As an engineer I often ask myself "Is there a better way to do this?".  

## Empower the Developers

With **JustBehave** we want to create a full featured behavior testing library without any dependency on foreign DSLs.  **JustBehave** provides a clean syntax for writing behavior tests using the gherkin syntax championed by [cucumber](https://cucumber.io/) entirely in C# code.  By eschewing the use of custom DSLs the developer can focus on writing the tests themselves rather than layers of proxy code to act as a go-between for high level business specifications which are usually written by the developers anyway during the requirements gathering phase.  Since the library ties into existing testing frameworks (e.g. xunit) it can be integrated directly into an existing build pipeline automatically with little-to-no additional effort.  Finally, testing frameworks are already fully capable of generating their own reports and can be leveraged to turn a behavior test into a human-readable report.

**JustBehave** provides the canvas to write behavior tests with many features automatically built-in.  How the tests are actually written is up to the purview of the developers.  '

# Tests as Data.  Data as Code

Features are composed of behaviors and behaviors are composed of individual steps.  

## Steps

Steps are the lowest level object in **JustBehave** type system that has full DI support.  There are three ways to create a step.

### Type Declaration

The first is to create a type that implements one of the three step types (e.g. Given, When, Then).  For example to implement a `Given` step:

```
public class MyGivenStep : GivenStep<MyContext, MyInput>
{
  public override MyContext Given(MyContext context, MyInput input)
  {
    // Do something...
    return context;
  }
}
```

Steps defined as class types are fully configurable and can be reused in any behavior of any feature.  

### Variable Declaration

A simpler way to create tests while still maintaining most of their flexibility is to use a lambda step.  Using the previous example as the basis it would look like this:

```
var myGivenStep = new LambdaGivenStep<MyContext, MyInput>()
  .Handle( (context,input) => 
  {
    // Do something...
    return context;
  });
```

Variable declarations can be reused within the feature that defined them, but are typically inaccessible outside of them (this is not a hard rule).  They maintain most of the flexibility of a type-based step, and are still able to access dependencies that would have otherwise by provided by DI via the use of closure.

### Inline Function Declaration

The simplest way to create a step is inline via the `BehaviorBuilder<TContext>` class and is demonstrated as such:

```
var builder = new BehaviorBuilder<MyContext>()
  .WithInput(MyData)
  .Given("MyGivenStep", (context, input) =>
  {
    // Do something...
    return context;
  });
```

This form requires no prior methods to be defined ahead of time and is intended for simple one-off steps since lambdas aren't reusable.  Dependencies are still accessible via closure, however many customization options are inaccessible.

## Behaviors

Behaviors can be defined by creating an implementation of the `Behavior<TContext>` class or using the `BehaviorBuilder<TContext>` helper class.  Behaviors define a context, inputs, and are followed by a series of steps in using GWT convention.  Unlike other BDD systems like SpecFlow, each step in a behavior with the exception of a `Then` assertion has a specified return that can be used to chain intermediate values together.  This is demonstrated in the calculator example below where the `When` step returns a value that is then chained into the following `Then` step without the need for any intermediate storage variables.

## Features

Features are simply classes that contain one or more properties of type `Behavior<TContext>`.  Features create a logical grouping of behaviors for a system.  Because they are regular classes to be instantiated by **JustBehave** all of the normal DI rules apply.

# First Class Support for DI

Unlike traditional unit testing which is heavy on mocks/stubs, behavior testing is intended to test the behavior of a system as if a user were using it.  Because of this, support for dependency injection is built into the library itself.  Depenencies for behaviors and steps are injected automatically by the system.

# Bring Your Own Testing Framework
**JustBehave** is not tied to any particular testing framework.  There are already several well-established testing frameworks out there that are, quite frankly, awesome.  We don't have the time, expertise, or motivation to attempt to re-invent that wheel again.  The core **JustBehave** library is designed to be testing-framework-agnostic.  Support for the three biggest frameworks ([xunit](https://xunit.net/), [nunit](https://nunit.org/), and [mstest](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)) will be distributed seperately from **JustBehave**.

# Example

Consider the simple (albeit contrived) example of a calculator found [here](https://github.com/bitcobblers/**JustBehave**/blob/main/src/**JustBehave**.Demo/CalculatorBehaviors.cs).

## System Under Test Injection

```
private readonly Calculator calculator;

public CalculatorBehaviors(Calculator calculator)
{
  this.calculator = calculator;
}
```

Since we're testing the behavior of a system, it is the responsibility of the DI system to construct it for us.  This also ensures that the same instance is accessible within our custom steps (see below).

## Context and Input

```
public record Context(int A, int B, int Result);
public record Input(int A, int B, int Expected);
```

All behavior tests have a `context` associated with them.  Each step in the behavior returns a new, updated copy of the `context`.  This allows us to track how the `context` changes throughout the behavior which is useful for troubleshooting failing behaviors.  Likewise, `input`s are immutable records containing all of the data needed by the behavior.  Inputs to a behavior can be a single one-off record, or any collection of records implementing `IEnumerable<T>`.

### Use of C# record types

While not strictly necessary, it is strongly suggested that the C# `record` type be used for `context` and `input`.  By default, the `record` type is immutable which makes it easy to track changes to `context`.  The syntax is also very terse and the language has built-in constructs for easily creating new records off of existing ones, which makes context changes easy to express.

```
public IEnumerable<Input> AdditionInputs
{
  get
  {
    yield return new Input(A: 1, B: 2, Expected: 3);
    yield return new Input(A: 2, B: 3, Expected: 5);
    yield return new Input(A: 3, B: 4, Expected: 7);
    yield return new Input(A: 4, B: 5, Expected: 9);
    yield return new Input(A: 5, B: 6, Expected: 11);
  }
}
```

In this example we're using a collection of inputs.  However, for tests interested in only a single data input, we can pass just that one element in instead.  Behind the scenes, **JustBehave** will treat a behavior test with a single input as a behavior test containing a collection of only one input.

## Define a Behavior

```
public Behavior<Context> Addition =>
  new BehaviorBuilder<Context>(nameof(Addition))
    .WithInput(AdditionInputs)
    .Given("Set first number", (c, i) => c with { A = i.A }) // Inline step declaration.
    .Given(SetSecondNumber)
    .When(AddNumbers)
    .Then<CheckResultStep>()
    .Build();
```

There's quite a bit going on here, so lets break this down line-by-line.  

The first thing to note is that behaviors are defined as class properties, not methods.  Since behaviors are actual data constructs, this allows the test runner to analyze them for additional information before executing them.  Behaviors are defined fluently using the [fluent pattern](https://en.wikipedia.org/wiki/Fluent_interface) convention.

```
.WithInput(AdditionInputs)
```

This establishes the input to be used by the behavior.  Every subsequent step will have access to this data.  

Note: the input can be reset at any point during a test.  When this occurs, any subsequent steps will use the updated input.

```
.Given("Set first number", (c, i) => c with { A = i.A }) // Inline step declaration.
```

This is the most basic form of a step.  Here we define it as a named lambda.  A `Given` lambda takes two parameters: the `context` and `input` and will return an updated `context` to be used in the next step.  If the method does not need to update the `context` then it may simply return `void`.   While not strictly required, it is necessary enough to include a string name of the step when using this convention, otherwise the builder won't know what to call it and will default to the name of the lambda which is generated by the compiler.  Using C#'s native closure capabilities, lambda steps can access all dependencies for the behavior class directly without needing to pass them into the step.  

```
.Given(SetSecondNumber)

...

public Context SetSecondNumber(Context context, Input input) => context with { B = input.B };
```

In this case we're defining the step as an instance method of the class.  This allows the step to be reused across multiple behaviors.  Additionally, since this is a named method, there is no need to supply a string name to the step in the `Given()` call since this can be generated automatically.   

```
.When(AddNumbers)
                
...

public WhenStep<Context, Input, int> AddNumbers => new LambdaWhenStep<Context, Input, int>()
  .Named("Add numbers")
  .Handle((c, _) => this.calculator.Add(c.A, c.B))
  .Teardown(() => Console.WriteLine("I do cleanup"));
```

Here we define the step as an instance of `LambdaWhenStep<>`.  This gives us the flexibility to control much more about the step, such as its exact name, any setup/cleanup code, policities, etc...

```
.Then<CheckResultStep>()

...

public class CheckResultStep : ThenStep<Context, Input, int>
{
  public override void Then(Context context, Input input, int result)
  {
    Console.WriteLine($"{input.Expected} == {result}: {input.Expected == result}");
  }
}
```

Finally, we define a step as an implementation of the `ThisStep<>` type.  While lambda steps are very powerful, their accessibility is generally limited to the type they were defined in.  Further, their flexibility is limited to what their API wraps.  By definining a step and implementing one of the base step types, it is possible to fully customize how steps behave.  Since **JustBehave** relies on a DI back-end for type construction, any dependencies required by the behavior are automatically injected in the step as well.

### Given/When/Then (GWT) Structure

To be consistent with common BDD testing methodologies, **JustBehave** uses the GWT convention for naming steps as opposed to the common Arrange/Act/Assert (AAA) convention that most developers are used to.  While this convention is purely semantic, it serves two purposes:

1. While subjective, GWT is a little more human-friendly in the sense that is lays out in common language the basic behaviors and expectations of a scenario.
2. Even though behavior tests may eventually run through the same test runners as the developers' unit/integration tests, behavior tests are *not* in fact unit tests or integration tests.  Their purpose is to test the behavior of the system as if a user were actually using it.  Using a GWT convention over AAA serves to add contrast between the two flavors of testing.

