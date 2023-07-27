using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class StepResultTests
{
    [Fact]
    public Task ValueIsOnlyEvaluatedOnce() => RunInState(ExecutionState.Executing, () =>
    {
        // Arrange.
        var step = new StepResult<object>(() => new object());

        // Act.
        var value1 = step.Value;
        var value2 = step.Value;

        // Assert.
        value1.ShouldBeSameAs(value2);
    });

    [Fact]
    public Task ValueIsAutomaticallyConvertedToSameType() => RunInState(ExecutionState.Executing, () =>
    {
        // Arrange.
        var step = new StepResult<bool>(() => true);

        // Act.
        bool value = step;

        // Assert.
        value.ShouldBeTrue(); // <-- implicit conversion doesn't work here.
        Assert.True(step);
    });

    [Fact]
    public Task AttemptingToResolveValueOutsideExecutionThrowsEagerStepResultEvaluationException() =>
        RunInState(ExecutionState.NotExecuting, () =>
        {
            // Arrange.
            var step = new StepResult<bool>(() => true);
            BehaviorExecutor.State.Value = ExecutionState.NotExecuting;

            // Assert.
            Should.Throw<EagerStepResultEvaluationException>(() => step.Value);
        });

    private static Task RunInState(ExecutionState state, Action action)
    {
        try
        {
            BehaviorExecutor.State.Value = state;
            action();

            return Task.CompletedTask;
        }
        finally
        {
            BehaviorExecutor.State.Value = ExecutionState.NotExecuting;
        }
    }
}