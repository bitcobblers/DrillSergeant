using System;
using System.Dynamic;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class VerbStepTests
{
    public class ConstructorMethod : VerbStepTests
    {
        [Fact]
        public void NameIsSetCorrectly()
        {
            // Act.
            var step = new StubStep("expected");

            // Assert.
            Assert.Equal("expected", step.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NullOrEmptyNameDefaultsToTypeName(string name)
        {
            // Act.
            var step = new StubStep(name);

            // Assert.
            Assert.Equal(typeof(StubStep).Name, step.Name);
        }

        public class StubStep : VerbStep
        {
            public StubStep(string name)
                : base(name)
            {
            }
        }
    }

    public class ExecuteMethod : VerbStepTests
    {
        public class Context
        {
            public int Value { get; set; }
        }

        public record Input();

        [Fact]
        public async Task NonAsyncMethodWithReturnReturnsExpectedValue()
        {
            // Arrange.
            var step = new StubStep_NonAsync_ReturnsValue();
            dynamic context = new ExpandoObject();
            dynamic input = new ExpandoObject();

            // Act.
            await step.Execute(context, input);

            // Assert.
            Assert.Equal(1, context.Value);
        }

        [Fact]
        public async Task AsyncMethodWithReturnReturnsExpectedValue()
        {
            // Arrange.
            var step = new StubStep_Async_ReturnsValue();
            dynamic context = new ExpandoObject();
            dynamic input = new ExpandoObject();

            // Act.
            await step.Execute(context, input);

            // Assert.
            Assert.Equal(1, context.Value);
        }

        public class StubStep_NonAsync_ReturnsValue : VerbStep
        {
            public void Test(Context context, Input input) => context.Value = 1;

            public override string Verb => "Test";
        }

        public class StubStep_Async_ReturnsValue : VerbStep
        {
            public Task Test(Context context, Input input)
            {
                context.Value = 1;
                return Task.CompletedTask;
            }

            public override string Verb => "Test";
        }

        public interface IStubInjectable
        {
            void DoSomething();
        }

        public class StubWithNoParameters : VerbStep
        {
            public bool HasExecuted { get; private set; }

            public void Test()
            {
                this.HasExecuted = true;
            }

            public override string Verb => "Test";
        }

        public class StubWithInjectableParameter : VerbStep
        {
            public void Test(IStubInjectable injectable)
            {
                injectable.DoSomething();
            }

            public override string Verb => "Test";
        }

        public class StubThatReturnsValue_Sync : VerbStep
        {
            public string Test() => "expected";

            public override string Verb => "Test";
        }

        public class StubThatReturnsValue_Async : VerbStep
        {
            public Task<string> Test() => Task.FromResult("expected");

            public override string Verb => "Test";
        }
    }

    public class PickHandlerMethod : VerbStepTests
    {
        [Fact]
        public void PicksVerbWithMostParameters()
        {
            // Arrange.
            var stub = new StubWithMultipleSyncVerbs();
            var expected = stub.GetType().GetMethod("Test", new[] { typeof(int) });

            // Act.
            var verbMethod = stub.PickHandlerWrapper();
            var handler = verbMethod;

            // Assert.
            Assert.Equal(expected, handler.Method);
        }

        [Fact]
        public void PrefersAsyncOverSyncWhenEqualParameters_SameName()
        {
            // Arrange.
            var stub = new StubWithSyncAndAsync_SameName();
            var expected = stub.GetType().GetMethod("Test", new[] { typeof(string) });

            // Act.
            var handler = stub.PickHandlerWrapper();

            // Assert.
            Assert.Equal(expected, handler.Method);
        }

        [Fact]
        public void PrefersAsyncOverSyncWhenEqualParameters_DifferentName()
        {
            // Arrange.
            var stub = new StubWithSyncAndAsync_DifferentName();
            var expected = stub.GetType().GetMethod("TestAsync", new[] { typeof(int) });

            // Act.
            var handler = stub.PickHandlerWrapper();

            // Assert.
            Assert.Equal(expected, handler.Method);
        }

        [Fact]
        public void ThrowsMissingVerbExceptionIfNoHandlerIsFound()
        {
            // Arrange.
            var stub = new StubWithNoVerb();

            // Assert.
            Assert.Throws<MissingVerbHandlerException>(() => stub.PickHandlerWrapper());
        }

        [Theory]
        [InlineData(typeof(StubWithTwoHandlersSameNumberOfParameters_NoAsync))]
        [InlineData(typeof(StubWithTwoHandlers_SameNumberOfParameters_TwoAsync))]
        public void ThrowsAmbiguousVerbException_Scenarios(Type type)
        {
            // Arrange.
            var stub = (StubWithExposedPickHandler)Activator.CreateInstance(type)!;

            // Assert.
            Assert.Throws<AmbiguousVerbHandlerException>(() => stub.PickHandlerWrapper());
        }

        public class StubWithExposedPickHandler : VerbStep
        {
            public override string Verb => "Test";

            public Delegate PickHandlerWrapper() => this.PickHandler();
        }

        public class StubWithMultipleSyncVerbs : StubWithExposedPickHandler
        {
            public void Test() { }
            public void Test(int arg1) { }
        }

        public class StubWithSyncAndAsync_SameName : StubWithExposedPickHandler
        {
            public void Test(int arg) { }
            public Task Test(string arg) => Task.CompletedTask;
        }

        public class StubWithSyncAndAsync_DifferentName : StubWithExposedPickHandler
        {
            public void Test(int arg) { }
            public Task TestAsync(int arg) => Task.CompletedTask;
        }

        public class StubWithNoVerb : StubWithExposedPickHandler
        {
        }

        public class StubWithTwoHandlersSameNumberOfParameters_NoAsync : StubWithExposedPickHandler
        {
            public void Test(int arg) { }

            public void Test(string arg) { }
        }

        public class StubWithTwoHandlers_SameNumberOfParameters_TwoAsync : StubWithExposedPickHandler
        {
            public Task Test(string arg) => Task.CompletedTask;

            public Task Test(long arg) => Task.CompletedTask;
        }
    }
}