using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace DrillSergeant.Tests;

public class AsyncStepResultTests
{
    [Fact]
    public Task ValueIsOnlyEvaluatedOnce() => RunInState(ExecutionState.Executing, async () =>
    {
        // Arrange.
        var step = new AsyncStepResult<object>("ignored", () => Task.FromResult(new object()));

        // Act.
        var value1 = await step.Resolve();
        var value2 = await step.Resolve();

        // Assert.
        value1.ShouldBeSameAs(value2);
    });


    [Fact]
    public Task AttemptingToResolveValueOutsideExecutionThrowsEagerStepResultEvaluationException() =>
        RunInState(ExecutionState.NotExecuting, async () =>
        {
            // Arrange.
            var step = new AsyncStepResult<bool>("ignored", () => Task.FromResult(true));
            BehaviorExecutor.State.Value = ExecutionState.NotExecuting;

            // Assert.
            await Should.ThrowAsync<EagerStepResultEvaluationException>(async () => await step.Resolve());
        });

    [Fact]
    public Task AttemptingToResolveWithoutSettingResultThrowsStepResultNotSetException() =>
        RunInState(ExecutionState.Executing, async () =>
        {
            // Arrange.
            var step = new AsyncStepResult<bool>("ignored");

            // Assert.
            await Should.ThrowAsync<StepResultNotSetException>(async () => await step.Resolve());
        });

    private static Task RunInState(ExecutionState state, Func<Task> action)
    {
        try
        {
            BehaviorExecutor.State.Value = state;
            return action();
        }
        finally
        {
            BehaviorExecutor.State.Value = ExecutionState.NotExecuting;
        }
    }
}