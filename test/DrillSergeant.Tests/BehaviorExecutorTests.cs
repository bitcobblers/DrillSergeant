using System;
using System.Threading.Tasks;
using DrillSergeant.Reporting;
using Xunit;
using FakeItEasy;
using Shouldly;

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

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act and assert.
            await Assert.ThrowsAsync<InvalidOperationException>(() => executor.LoadBehavior());
        }

        [Fact]
        public async Task NoReturnedBehaviorThrowsInvalidOperationException_Async()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("NotABehavior_Async");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act and assert.
            await Assert.ThrowsAsync<InvalidOperationException>(() => executor.LoadBehavior());
        }

        [Fact]
        public async Task AsyncMethodSetsBehaviorProperty()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("AsyncBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act.
            await executor.LoadBehavior();

            // Assert.
            executor.Behavior.ShouldNotBeNull();
        }

        [Fact]
        public async Task SyncMethodSetsBehaviorProperty()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("SyncBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act.
            await executor.LoadBehavior();

            // Assert.
            executor.Behavior.ShouldNotBeNull();
        }

        private class StubWithBehaviors
        {
            public void NotABehavior_Sync()
            {
            }

            public Task NotABehavior_Async() => Task.CompletedTask;

            public Behavior SyncBehavior() => new();

            public Task<Behavior> AsyncBehavior() => Task.FromResult(new Behavior());
        }
    }

    public class ExecuteMethod : BehaviorExecutorTests
    {
        [Fact]
        public async Task NullBehaviorThrowsInvalidOperationException()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("UnknownMethod");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act and assert.
            await Assert.ThrowsAsync<InvalidOperationException>(() => executor.Execute());
        }

        [Fact]
        public async Task SuccessfulBehaviorSetsContext()
        {
            // Arrange.
            var reporter = A.Fake<ITestReporter>();
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("SuccessfulBehavior");
            var parameters = Array.Empty<object?>();

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act.
            await executor.LoadBehavior();
            await executor.Execute();

            // Assert.
            executor.Behavior!.Context["IsSuccess"].ShouldBe(true);
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

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            executor.StepFailed += (_, _) => errorCalled = true;

            // Act.
            await executor.LoadBehavior();
            await executor.Execute();

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

            var executor = new BehaviorExecutor(reporter, instance, method!, parameters);

            // Act.
            await executor.LoadBehavior();
            await executor.Execute();

            // Assert.
            executor.Behavior!.Context["IsSuccess"].ShouldBe(true);
        }

        private class StubWithBehaviors
        {
            public Behavior SuccessfulBehavior() =>
                new Behavior()
                    .AddStep(
                        new LambdaStep("Successful step")
                            .Handle(c => c.IsSuccess = true));

            public Behavior FailingBehavior() =>
                new Behavior()
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")));

            public Behavior FailingBehaviorWithAdditionalSteps() =>
                new Behavior()
                    .AddStep(
                        new LambdaStep("Set context to true")
                            .Handle(c => c.IsSuccess = true))
                    .AddStep(
                        new LambdaStep("Failing step")
                            .Handle(() => throw new Exception("Failed")))
                    .AddStep(
                        new LambdaStep("Set context to false")
                            .Handle(c => c.IsSuccess = false));
        }
    }
}