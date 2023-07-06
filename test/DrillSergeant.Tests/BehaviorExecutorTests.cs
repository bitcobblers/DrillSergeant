using DrillSergeant.Reporting;
using FakeItEasy;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class BehaviorExecutorTests
{
    public class LoadBehaviorMethod : BehaviorExecutorTests
    {
        [Fact]
        public async Task NoReturnedBehaviorThrowsInvalidOperationException_Sync()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("NotABehavior_Sync");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);

            // Act and assert.
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => executor.LoadBehavior(instance, method!, parameters));
        }

        [Fact]
        public async Task NoReturnedBehaviorThrowsInvalidOperationException_Async()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("NotABehavior_Async");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);

            // Act and assert.
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => executor.LoadBehavior(instance, method!, parameters));
        }

        [Fact]
        public async Task AsyncMethodSetsBehaviorProperty()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("AsyncBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);

            // Act.
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Assert.
            behavior.ShouldNotBeNull();
        }

        [Fact]
        public async Task SyncMethodSetsBehaviorProperty()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("SyncBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);

            // Act.
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Assert.
            behavior.ShouldNotBeNull();
        }

        private class StubWithBehaviors
        {
            public void NotABehavior_Sync()
            {
            }

            public Task NotABehavior_Async() => Task.CompletedTask;

            public Behavior SyncBehavior() => BehaviorBuilder.New();

            public Task<Behavior> AsyncBehavior() => Task.FromResult(BehaviorBuilder.New());
        }
    }

    public class ExecuteMethod : BehaviorExecutorTests
    {
        [Fact]
        public async Task SuccessfulBehaviorSetsContext()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("SuccessfulBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            await executor.Execute(behavior);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task StepFailedEventFiredWhenStepFails()
        {
            // Arrange.
            bool errorCalled = false;

            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("FailingBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            executor.StepFailed += (_, _) => errorCalled = true;

            // Act.
            await executor.Execute(behavior);

            // Assert.
            errorCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task FailingStepCausesSubsequentStepsToSkip()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("FailingBehaviorWithAdditionalSteps");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            await executor.Execute(behavior);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
        }

        [Fact]
        public async Task OwnedObjectsSetByExtensionAreDisposedWithBehavior()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var executor = new BehaviorExecutor(reporter);

            var obj = new StubDisposable();
            var behavior = BehaviorBuilder.New()
                .AddStep(
                    new LambdaStep("Registers disposable")
                        .Handle(c => c.Obj = obj.OwnedByBehavior()));

            // Act.
            await executor.Execute(behavior);
            behavior.Dispose();

            // Assert.
            obj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public async Task SkipsDisabledSteps()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("BehaviorWithSkippedStep");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter);
            using var behavior = await executor.LoadBehavior(instance, method!, parameters);

            // Act.
            await executor.Execute(behavior);

            // Assert.
            behavior!.Context["IsSuccess"].ShouldBe(true);
        }

        private class StubDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public void Dispose() => DisposeCount++;
        }

        private class StubWithBehaviors
        {
            public Behavior SuccessfulBehavior() =>
                BehaviorBuilder.New()
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(c => c.IsSuccess = true));

            public Behavior FailingBehavior() =>
                BehaviorBuilder.New()
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")));

            public Behavior FailingBehaviorWithAdditionalSteps() =>
                BehaviorBuilder.New()
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
                BehaviorBuilder.New()
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(c => c.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Skipped step")
                            .Handle(c => c.IsSuccess = false)
                            .Skip(() => true));
        }
    }
}
