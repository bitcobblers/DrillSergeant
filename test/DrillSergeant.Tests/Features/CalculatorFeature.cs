﻿using DrillSergeant.Core;
using DrillSergeant.GWT;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DrillSergeant.Tests.Features;

public class CalculatorBehaviors
{
    public ITestOutputHelper output;

    public record Context
    {
        public int A { get; init; }
        public int B { get; init; }
        public int Result { get; init; }
    }

    public record Input(int A, int B, int Expected);

    public CalculatorBehaviors(ITestOutputHelper output)
    {
        this.output = output;
    }

    public static IEnumerable<object[]> AdditionInputs
    {
        get => new[]
        {
            new object[] { 1, 2, 3 },
            new object[] { 2, 3, 5 }
        };
    }

    //[BehaviorResolverSetup]
    //public IDependencyResolver ConfigureResolver()
    //{
    //    var resolver = A.Fake<IDependencyResolver>();
    //    A.CallTo(() => resolver.Resolve(typeof(Calculator))).Returns(new Calculator());
    //    return resolver;
    //}

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Task<Behavior<Context,Input>> AsyncAdditionBehavior(int a, int b, int expected, [Inject] Calculator calculator)
    {
        var behavior = new Behavior<Context, Input>(() => new Input(a, b, expected))
            .Given("Set first number", (c, i) => Task.FromResult(c with { A = i.A })) // Inline step declaration.
            .Given(SetSecondNumber)
            .When(AddNumbersAsync(calculator))
            .Then(new CheckResultStepAsync());

        return Task.FromResult(behavior);
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Behavior AdditionBehavior(int a, int b, int expected, [Inject] Calculator calculator)
    {
        return new Behavior<Context, Input>(() => new Input(a, b, expected))
            .Given("Set first number", (c, i) => c with { A = i.A }) // Inline step declaration.
            .Given(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then(new CheckResultStep());
    }

    // Step implemented as a normal method.
    public Context SetSecondNumber(Context context, Input input) => context with { B = input.B };

    public Task<Context> SetSecondNumberAsync(Context context, Input input) => Task.FromResult(context with { B = input.B });

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep<Context, Input> AddNumbers(Calculator calculator) =>
        new LambdaWhenStep<Context, Input>()
            .Named("Add numbers")
            .Handle((c, _) => c with { Result = calculator.Add(c.A, c.B) });

    public LambdaStep<Context, Input> AddNumbersAsync(Calculator calculator) =>
        new LambdaWhenStep<Context, Input>()
            .Named("Add numbers")
            .Handle((c, _) => Task.FromResult(c with { Result = calculator.Add(c.A, c.B) }));

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep<Context, Input>
    {
        public override void Then(Context context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
        }
    }

    public class CheckResultStepAsync : ThenStep<Context, Input>
    {
        public Task ThenAsync(Context context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
            return Task.CompletedTask;
        }
    }
}
