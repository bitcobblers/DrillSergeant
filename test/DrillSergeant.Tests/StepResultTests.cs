namespace DrillSergeant.Tests;

public class StepResultTests
{
    public class Converters :StepResultTests
    {
        [Fact]
        public void WithoutSettingValueThrowsStepResultNotSetException()
        {
            // Arrange.
            var stepResult = new StepResult<string>(
                "ignored",
                func: null,
                isExecuting: () => true);

            // Assert.
            Should.Throw<StepResultNotSetException>(() =>
            {
                string _ = stepResult;
            });
        }
    }

    public class ResolveMethod : StepResultTests
    {
        [Fact]
        public void ValueIsOnlyEvaluatedOnce()
        {
            // Arrange.
            var step = new StepResult<object>(
                "ignored",
                func: () => new object(),
                isExecuting: () => true);

            // Act.
            var value1 = step.Resolve();
            var value2 = step.Resolve();

            // Assert.
            value1.ShouldBeSameAs(value2);
        }

        [Fact]
        public void ValueIsAutomaticallyConvertedToSameType()
        {
            // Arrange.
            var step = new StepResult<bool>(
                "ignored",
                func: () => true,
                isExecuting: () => true);

            // Act.
            bool value = step;

            // Assert.
            value.ShouldBeTrue();
            Assert.True(step);
        }

        [Fact]
        public void AttemptingToResolveValueOutsideExecutionThrowsEagerStepResultEvaluationException()
        {
            // Arrange.
            var step = new StepResult<bool>(
                "ignored",
                func: () => true,
                isExecuting: () => false);

            // Assert.
            Should.Throw<EagerStepResultEvaluationException>(() => step.Resolve());
        }

        [Fact]
        public void AttemptingToResolveWithoutSettingResultThrowsStepResultNotSetException()
        {
            // Arrange.
            var step = new AsyncStepResult<bool>(
                "ignored",
                func: null,
                isExecuting: () => true);

            // Assert.
            Should.Throw<StepResultNotSetException>(() => step.Resolve());
        }
    }
}