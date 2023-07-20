using DrillSergeant.Xunit2;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Tests;

public class BehaviorExecutorTests
{
    public class LoadBehaviorMethod : BehaviorExecutorTests
    {
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
            await executor.Execute(behavior!, CancellationToken.None);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
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
            await executor.Execute(behavior!, CancellationToken.None);

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
            await executor.Execute(behavior!, CancellationToken.None);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task OwnedObjectsSetByExtensionAreDisposedWithBehavior()
        {
            // Arrange.
            var executor = new BehaviorExecutor(_reporter);

            var obj = new StubDisposable();
            var behavior = BehaviorBuilder.Reset()
                .AddStep(
                    new LambdaStep("Registers disposable")
                        .Handle(c => c.Obj = obj.OwnedByBehavior()!));

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
            await executor.Execute(behavior!, CancellationToken.None);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
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
            await executor.Execute(behavior!, tokenSource.Token);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
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
                () => executor.Execute(behavior!, CancellationToken.None, 100));
        }

        private class StubDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public void Dispose() => DisposeCount++;
        }

        private class StubWithBehaviors
        {
            public Behavior SuccessfulBehavior() =>
                BehaviorBuilder.Reset()
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(c => c.IsSuccess = true));

            public Behavior FailingBehavior() =>
                BehaviorBuilder.Reset()
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")));

            public Behavior FailingBehaviorWithAdditionalSteps() =>
                BehaviorBuilder.Reset()
                    .AddStep(
                        new LambdaStep("Set context to true")
                            .Handle(c => c.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")))
                    .AddStep(
                        new LambdaStep("Set context to false")
                            .Handle(c => c.IsSuccess = false));

            public Behavior BehaviorWithSkippedStep() =>
                BehaviorBuilder.Reset()
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(c => c.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Skipped step")
                            .Handle(c => c.IsSuccess = false)
                            .Skip(() => true));

            public Behavior BehaviorWithFive100MsSteps() =>
                BehaviorBuilder.Reset()
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(c => c.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Delay 100ms (1)")
                            .HandleAsync(() => Task.Delay(100)))
                    .AddStep(
                        new LambdaStep("Delay 100ms (2)")
                            .HandleAsync(() => Task.Delay(100)))
                    .AddStep(
                        new LambdaStep("Delay 100ms (3)")
                            .HandleAsync(() => Task.Delay(100)))
                    .AddStep(
                        new LambdaStep("Delay 100ms (4)")
                            .HandleAsync(() => Task.Delay(100)))
                    .AddStep(
                        new LambdaStep("Delay 100ms (5)")
                            .HandleAsync(() => Task.Delay(100)))
                    .AddStep(
                        new LambdaStep("Should not execute")
                            .Handle(c => c.IsSuccess = false));
        }
    }
}
