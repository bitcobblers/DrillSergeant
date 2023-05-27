using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class VerbStepTests
{
    public class ConstructorMethod : VerbStepTests
    {
        [Fact]
        public void VerbIsSetCorrectly()
        {
            // Act.
            var step = new StubStep("test");

            // Assert.
            Assert.Equal("test", step.Verb);
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            // Act.
            var step = new StubStep("ignored", "expected");

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
            var step = new StubStep("ignored", name);

            // Assert.
            Assert.Equal(typeof(StubStep).Name, step.Name);
        }

        public class StubStep : VerbStep<object>
        {
            public StubStep(string verb) : base(verb)
            {
            }

            public StubStep(string verb, string name)
                : base(verb, name)
            {
            }
        }
    }

    public class ExecuteMethod : VerbStepTests
    {
        public record Context
        {
            public int Value { get; set; }
        }

        public record Input();

        [Fact]
        public async Task NonAsyncMethodWithReturnReturnsExpectedValue()
        {
            // Arrange.
            var step = new StubStep_NonAsync_ReturnsValue();
            var context = new Context();
            var input = new Input();

            // Act.
            await step.Execute(context, input);

            // Assert.
            context.Value.ShouldBe(1);
        }

        [Fact]
        public async Task AsyncMethodWithReturnReturnsExpectedValue()
        {
            // Arrange.
            var step = new StubStep_Async_ReturnsValue();
            var context = new Context();
            var input = new Input();

            // Act.
            await step.Execute(context, input);

            // Assert.
            context.Value.ShouldBe(1);
        }

        public class StubStep_NonAsync_ReturnsValue : VerbStep<Input>
        {
            public StubStep_NonAsync_ReturnsValue() : base("Test")
            {
            }

            public void Test(Context context, Input input) => context.Value = 1;
        }

        public class StubStep_Async_ReturnsValue : VerbStep<Input>
        {
            public StubStep_Async_ReturnsValue() : base("Test")
            {
            }

            public Task Test(Context context, Input input)
            {
                context.Value = 1;
                return Task.CompletedTask;
            }
        }

        public interface IStubInjectable
        {
            void DoSomething();
        }

        public class StubWithNoParameters : VerbStep<Input>
        {
            public StubWithNoParameters()
                : base("Test")
            {
            }

            public bool HasExecuted { get; private set; }

            public void Test()
            {
                this.HasExecuted = true;
            }
        }

        public class StubWithInjectableParameter : VerbStep<Input>
        {
            public StubWithInjectableParameter()
                : base("Test")
            {
            }

            public void Test(IStubInjectable injectable)
            {
                injectable.DoSomething();
            }
        }

        public class StubThatReturnsValue_Sync : VerbStep<Input>
        {
            public StubThatReturnsValue_Sync()
                : base("Test")
            {
            }

            public string Test() => "expected";
        }

        public class StubThatReturnsValue_Async : VerbStep<Input>
        {
            public StubThatReturnsValue_Async()
                : base("Test")
            {
            }

            public Task<string> Test() => Task.FromResult("expected");
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
            Assert.Throws<MissingVerbException>(() => stub.PickHandlerWrapper());
        }

        [Theory]
        [InlineData(typeof(StubWithTwoHandlersSameNumberOfParameters_NoAsync))]
        [InlineData(typeof(StubWithTwoHandlers_SameNumberOfParameters_TwoAsync))]
        public void ThrowsAmbiguousVerbException_Scenarios(Type type)
        {
            // Arrange.
            var stub = (StubWithExposedPickHandler)Activator.CreateInstance(type)!;

            // Assert.
            Assert.Throws<AmbiguousVerbException>(() => stub.PickHandlerWrapper());
        }

        public class StubWithExposedPickHandler : VerbStep<object>
        {
            public StubWithExposedPickHandler()
                : base("Test")
            {
            }

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