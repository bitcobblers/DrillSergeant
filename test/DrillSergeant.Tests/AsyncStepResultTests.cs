using System;
using System.Threading.Tasks;

namespace DrillSergeant.Tests;

public class AsyncStepResultTests
{
    public class Converters : AsyncStepResultTests
    {
        [Fact]
        public async Task ConversionToTaskCanBeResolved()
        {
            // Arrange.
            var stepResult = new AsyncStepResult<string>("ignored", () => Task.FromResult("expected"));

            // Act.
            Task<string> castedResult = stepResult;
            var result = await castedResult;

            // Assert.
            result.ShouldBe("expected");
        }

        [Fact]
        public async Task CanAwaitConvertInOneOperation()
        {
            // Arrange.
            var stepResult = new AsyncStepResult<string>("ignored", () => Task.FromResult("expected"));

            // Act.
            string result = await stepResult;

            // Assert.
            result.ShouldBe("expected");
        }

        [Fact]
        public async Task AwaitingWithoutSettingValueReturnsDefault()
        {
            // Arrange.
            var stepResult = new AsyncStepResult<string>("ignored");

            // Act.
            string? result = await stepResult;

            // Assert.
            result.ShouldBeNull();
        }
    }

    public class ResolveMethod : AsyncStepResultTests
    {
        [Fact]
        public Task ValueIsOnlyEvaluatedOnce() => RunInState(isExecuting: true, async () =>
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
            RunInState(isExecuting: false, async () =>
            {
                // Arrange.
                var step = new AsyncStepResult<bool>("ignored", () => Task.FromResult(true));

                // Assert.
                await Should.ThrowAsync<EagerStepResultEvaluationException>(async () => await step.Resolve());
            });

        [Fact]
        public Task AttemptingToResolveWithoutSettingResultThrowsStepResultNotSetException() =>
            RunInState(isExecuting: true, async () =>
            {
                // Arrange.
                var step = new AsyncStepResult<bool>("ignored");

                // Assert.
                await Should.ThrowAsync<StepResultNotSetException>(async () => await step.Resolve());
            });

        private static Task RunInState(bool isExecuting, Func<Task> action)
        {
            try
            {
                BehaviorExecutor.IsExecuting.Value = isExecuting;
                return action();
            }
            finally
            {
                BehaviorExecutor.IsExecuting.Value = false;
            }
        }
    }
}