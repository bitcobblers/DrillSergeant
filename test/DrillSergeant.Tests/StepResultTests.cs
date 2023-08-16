using System;
using System.Threading.Tasks;

namespace DrillSergeant.Tests;

public class StepResultTests
{
    [Fact]
    public Task ValueIsOnlyEvaluatedOnce() => RunInState(isExecuting: true, () =>
    {
        // Arrange.
        var step = new StepResult<object>("ignored", () => new object());

        // Act.
        var value1 = step.Resolve();
        var value2 = step.Resolve();

        // Assert.
        value1.ShouldBeSameAs(value2);
    });

    [Fact]
    public Task ValueIsAutomaticallyConvertedToSameType() => RunInState(isExecuting: true, () =>
    {
        // Arrange.
        var step = new StepResult<bool>("ignored", () => true);

        // Act.
        bool value = step;

        // Assert.
        value.ShouldBeTrue(); // <-- implicit conversion doesn't work here.
        Assert.True(step);
    });

    [Fact]
    public Task AttemptingToResolveValueOutsideExecutionThrowsEagerStepResultEvaluationException() =>
        RunInState(isExecuting: false, () =>
        {
            // Arrange.
            var step = new StepResult<bool>("ignored", () => true);
            BehaviorExecutor.IsExecuting.Value = false;

            // Assert.
            Should.Throw<EagerStepResultEvaluationException>(() => step.Resolve());
        });

    [Fact]
    public Task AttemptingToResolveWithoutSettingResultThrowsStepResultNotSetException() =>
        RunInState(isExecuting: true, () =>
        {
            // Arrange.
            var step = new AsyncStepResult<bool>("ignored");

            // Assert.
            Should.Throw<StepResultNotSetException>(() => step.Resolve());
        });

    private static Task RunInState(bool isExecuting, Action action)
    {
        try
        {
            BehaviorExecutor.IsExecuting.Value = isExecuting;
            action();

            return Task.CompletedTask;
        }
        finally
        {
            BehaviorExecutor.IsExecuting.Value = false;
        }
    }
}