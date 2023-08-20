using System.Threading.Tasks;

namespace DrillSergeant.Tests;

public class AsyncStepResultTests
{
    public class Converters : AsyncStepResultTests
    {
        [Fact]
        public async Task ConversionToTaskCanBeAwaited()
        {
            // Arrange.
            var stepResult = new AsyncStepResult<string>(
                "ignored",
                func: () => Task.FromResult("expected"),
                isExecuting: () => true);

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
            var stepResult = new AsyncStepResult<string>(
                "ignored",
                func: () => Task.FromResult("expected"),
                isExecuting: () => true);

            // Act.
            string result = await stepResult;

            // Assert.
            result.ShouldBe("expected");
        }

        [Fact]
        public async Task AwaitingWithoutSettingValueThrowsStepResultNotSetException()
        {
            // Arrange.
            var stepResult = new AsyncStepResult<string>(
                "ignored",
                func: null,
                isExecuting: () => true);

            // Assert.
            await Should.ThrowAsync<StepResultNotSetException>(async () => await stepResult);
        }
    }

    public class ResolveMethod : AsyncStepResultTests
    {
        [Fact]
        public async Task ValueIsOnlyEvaluatedOnce()
        {
            // Arrange.
            var step = new AsyncStepResult<object>(
                "ignored",
                func: () => Task.FromResult(new object()),
                isExecuting: () => true);

            // Act.
            var value1 = await step.Resolve();
            var value2 = await step.Resolve();

            // Assert.
            value1.ShouldBeSameAs(value2);
        }

        [Fact]
        public async Task ValueIsAutomaticallyConvertedToSameType()
        {
            // Arrange.
            var step = new AsyncStepResult<bool>(
                "ignored",
                func: () => Task.FromResult(true),
                isExecuting: () => true);

            // Act.
            bool value = await step;

            // Assert.
            value.ShouldBeTrue();
            Assert.True(await step);
        }

        [Fact]
        public async Task AttemptingToResolveValueOutsideExecutionThrowsEagerStepResultEvaluationException()
        {
            // Arrange.
            var step = new AsyncStepResult<bool>(
                "ignored",
                func: () => Task.FromResult(true),
                isExecuting: () => false);

            // Assert.
            await Should.ThrowAsync<EagerStepResultEvaluationException>(async () => await step.Resolve());
        }

        [Fact]
        public async Task AttemptingToResolveWithoutSettingResultThrowsStepResultNotSetException()
        {
            // Arrange.
            var step = new AsyncStepResult<bool>(
                "ignored",
                func: null,
                isExecuting: () => true);

            // Assert.
            await Should.ThrowAsync<StepResultNotSetException>(async () => await step.Resolve());
        }
    }
}