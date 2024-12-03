using DrillSergeant.Xunit2;
using Xunit.Abstractions;
using Xunit.Sdk;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

namespace DrillSergeant.Tests;

public class BehaviorExecutorTests
{
    public class LoadBehaviorMethod : BehaviorExecutorTests
    {
        [Fact]
        public async Task LoadedBehaviorsAreAutomaticallyFrozen()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehavior();
            var method = typeof(StubWithBehavior).GetMethod("SampleBehavior");
            var parameters = Array.Empty<object?>();
            var executor = new BehaviorExecutor(reporter);

            // Act.
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Assert.
            behavior.IsFrozen.ShouldBeTrue();
        }

        class StubWithBehavior
        {
            public void SampleBehavior()
            {
            }
        }

        public class StubStep : VerbStep { }

        private async Task<Behavior> LoadSampleBehavior()
        {
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehavior();
            var method = typeof(StubWithBehavior).GetMethod("SampleBehavior");
            var parameters = Array.Empty<object?>();
            var executor = new BehaviorExecutor(reporter);

            return await executor.LoadBehavior(instance, method!, parameters);
        }
    }

    public class ExecuteMethod : BehaviorExecutorTests
    {
        private readonly ITestReporter _reporter;

        public ExecuteMethod(ITestOutputHelper outputHelper)
        {
            var output = (TestOutputHelper)outputHelper;
            var wrapper = new WrappedTestOutputHelper(output);
            _reporter = new RawTestReporter(wrapper);
        }

        [Fact]
        public async Task SuccessfulBehaviorSetsContext()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("SuccessfulBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            await executor.Execute(behavior, CancellationToken.None);

            // Assert.
            behavior.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task StepFailedEventFiredWhenStepFails()
        {
            // Arrange.
            bool errorCalled = false;

            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("FailingBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            executor.StepFailed += (_, _) => errorCalled = true;

            // Act.
            await executor.Execute(behavior, CancellationToken.None);

            // Assert.
            errorCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task FailingStepCausesSubsequentStepsToSkip()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("FailingBehaviorWithAdditionalSteps");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            await executor.Execute(behavior, CancellationToken.None);

            // Assert.
            behavior.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task OwnedObjectsSetByExtensionAreDisposedWithBehavior()
        {
            // Arrange.
            var executor = new BehaviorExecutor(_reporter);

            var obj = new StubDisposable();
            var behavior = BehaviorBuilder.Build(b =>
                b.AddStep(
                    new LambdaStep("Registers disposable")
                        .Handle(() => obj.OwnedByBehavior())));

            // Act.
            await executor.Execute(behavior, CancellationToken.None);
            behavior.Dispose();

            // Assert.
            obj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public async Task SkipsDisabledSteps()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("BehaviorWithSkippedStep");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            await executor.Execute(behavior, CancellationToken.None);

            // Assert.
            behavior.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task CancelCausesAllSubsequentStepsToSkip()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("BehaviorWithFive100MsSteps");
            var parameters = Array.Empty<object?>();
            var tokenSource = new CancellationTokenSource();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            tokenSource.CancelAfter(100);
            await executor.Execute(behavior, tokenSource.Token);

            // Assert.
            behavior.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task TimeoutThrowsTestFailedException()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("BehaviorWithFive100MsSteps");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act and assert.
            await Assert.ThrowsAsync<BehaviorTimeoutException>(
                () => executor.Execute(behavior, CancellationToken.None, 100));
        }

        [Fact]
        public async Task CleansUpStackAfterSuccessfulExecution()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("SuccessfulBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);
            var beforeStackSize = BehaviorBuilder.GetCurrentStack().Count;

            // Act.
            await executor.Execute(behavior, CancellationToken.None);
            var afterStackSize = BehaviorBuilder.GetCurrentStack().Count;

            // Assert.
            afterStackSize.ShouldBe(beforeStackSize);
        }

        [Fact]
        public async Task CleansUpStackIfExecutionThrowsException()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("BehaviorWithFive100MsSteps");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(_reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);
            var beforeStackSize = BehaviorBuilder.GetCurrentStack().Count;

            // Act.
            await Assert.ThrowsAsync<BehaviorTimeoutException>(
                () => executor.Execute(behavior, CancellationToken.None, 100));

            var afterStackSize = BehaviorBuilder.GetCurrentStack().Count;

            // Assert.
            afterStackSize.ShouldBe(beforeStackSize);
        }

        private class StubDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public void Dispose() => DisposeCount++;
        }

        private class StubWithBehaviors
        {
            public Behavior SuccessfulBehavior() =>
                BehaviorBuilder.Current.AddStep(
                    new LambdaStep("Successful step")
                        .Handle(() => CurrentBehavior.Context.IsSuccess = true));

            public Behavior FailingBehavior() =>
                BehaviorBuilder.Current
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")));

            public Behavior FailingBehaviorWithAdditionalSteps() =>
                BehaviorBuilder.Current
                    .AddStep(
                        new LambdaStep("Set context to true")
                            .Handle(() => CurrentBehavior.Context.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")))
                    .AddStep(
                        new LambdaStep("Set context to false")
                            .Handle(() => CurrentBehavior.Context.IsSuccess = true));

            public Behavior BehaviorWithSkippedStep() =>
                BehaviorBuilder.Current
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(() => CurrentBehavior.Context.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Skipped step")
                            .Handle(() => CurrentBehavior.Context.IsSuccess = false)
                            .Skip(() => true));

            public Behavior BehaviorWithFive100MsSteps() =>
                BehaviorBuilder.Current
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(() => CurrentBehavior.Context.IsSuccess = true))
                    .AddStep(
                        new AsyncLambdaStep("Delay 100ms (1)")
                            .Handle(() => Task.Delay(100)))
                    .AddStep(
                        new AsyncLambdaStep("Delay 100ms (2)")
                            .Handle(() => Task.Delay(100)))
                    .AddStep(
                        new AsyncLambdaStep("Delay 100ms (3)")
                            .Handle(() => Task.Delay(100)))
                    .AddStep(
                        new AsyncLambdaStep("Delay 100ms (4)")
                            .Handle(() => Task.Delay(100)))
                    .AddStep(
                        new AsyncLambdaStep("Delay 100ms (5)")
                            .Handle(() => Task.Delay(100)))
                    .AddStep(
                        new LambdaStep("Should not execute")
                            .Handle(() => CurrentBehavior.Context.IsSuccess = false));
        }
    }
}
