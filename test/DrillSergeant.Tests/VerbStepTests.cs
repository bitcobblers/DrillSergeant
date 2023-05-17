using FakeItEasy;
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

        public class StubStep : VerbStep<object,object>
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
        public record Context();
        public record Input();

        [Fact]
        public async Task NonAsyncMethodWithNoReturnReturnsNull()
        {
            // Arrange.
            var step = new StubStep_NonAsync_NoReturn();
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBeNull();
        }

        [Fact]
        public async Task AsyncMethodWithNoReturnReturnsNull()
        {
            // Arrange.
            var step = new StubStep_Async_NoReturn();
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBeNull();
        }

        [Fact]
        public async Task NonAsyncMethodWithReturnReturnsExpectedValue()
        {
            // Arrange.
            var step = new StubStep_NonAsync_ReturnsValue();
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBe(1);
        }

        [Fact]
        public async Task AsyncMethodWithReturnReturnsExpectedValue()
        {
            // Arrange.
            var step = new StubStep_Async_ReturnsValue();
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBe(1);
        }

        public class StubStep_NonAsync_NoReturn : VerbStep<Context, Input>
        {
            public StubStep_NonAsync_NoReturn() : base("Test")
            {
            }

            public void Test(Context context, Input input) { }
        }

        public class StubStep_Async_NoReturn : VerbStep<Context, Input>
        {
            public StubStep_Async_NoReturn() : base("Test")
            {
            }

            public Task Test(Context context, Input input) => Task.CompletedTask;
        }

        public class StubStep_NonAsync_ReturnsValue : VerbStep<Context, Input>
        {
            public StubStep_NonAsync_ReturnsValue() : base("Test")
            {
            }

            public int Test(Context context, Input input) => 1;
        }

        public class StubStep_Async_ReturnsValue : VerbStep<Context, Input>
        {
            public StubStep_Async_ReturnsValue() : base("Test")
            {
            }

            public Task<int> Test(Context context, Input input) => Task.FromResult(1);
        }

        public interface IStubInjectable
        {
            void DoSomething();
        }

        public class StubWithNoParameters : VerbStep<Context,Input>
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

        public class StubWithInjectableParameter : VerbStep<Context, Input>
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

        public class StubThatReturnsValue_Sync : VerbStep<Context, Input>
        {
            public StubThatReturnsValue_Sync()
                : base("Test")
            {
            }

            public string Test() => "expected";
        }

        public class StubThatReturnsValue_Async : VerbStep<Context, Input>
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
            var verbMethod = stub.PickHandler();
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
            var handler = stub.PickHandler();

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
            var handler = stub.PickHandler();

            // Assert.
            Assert.Equal(expected, handler.Method);
        }

        [Fact]
        public void ThrowsMissingVerbExceptionIfNoHandlerIsFound()
        {
            // Arrange.
            var stub = new StubWithNoVerb();

            // Assert.
            Assert.Throws<MissingVerbException>(() => stub.PickHandler());
        }

        [Theory]
        [InlineData(typeof(StubWithTwoHandlersSameNumberOfParameters_NoAsync))]
        [InlineData(typeof(StubWithTwoHandlers_SameNumberOfParameters_TwoAsync))]
        public void ThrowsAmbiguousVerbException_Scenarios(Type type)
        {
            // Arrange.
            var stub = (VerbStep<object, object>)Activator.CreateInstance(type)!;

            // Assert.
            Assert.Throws<AmbiguousVerbException>(() => stub.PickHandler());
        }

        public class StubWithMultipleSyncVerbs : VerbStep<object, object>
        {
            public StubWithMultipleSyncVerbs()
                : base("Test")
            {
            }

            public void Test() { }
            public void Test(int arg1) { }
        }

        public class StubWithSyncAndAsync_SameName : VerbStep<object, object>
        {
            public StubWithSyncAndAsync_SameName()
                : base("Test")
            {
            }

            public void Test(int arg) { }
            public Task Test(string arg) => Task.CompletedTask;
        }

        public class StubWithSyncAndAsync_DifferentName : VerbStep<object, object>
        {
            public StubWithSyncAndAsync_DifferentName()
                : base("Test")
            {
            }

            public void Test(int arg) { }
            public Task TestAsync(int arg) => Task.CompletedTask;
        }

        public class StubWithNoVerb : VerbStep<object, object>
        {
            public StubWithNoVerb()
                : base("ignored")
            {
            }
        }

        public class StubWithTwoHandlersSameNumberOfParameters_NoAsync : VerbStep<object, object>
        {
            public StubWithTwoHandlersSameNumberOfParameters_NoAsync()
                : base("Test")
            {
            }

            public void Test(int arg) { }

            public void Test(string arg) { }
        }

        public class StubWithTwoHandlers_SameNumberOfParameters_TwoAsync : VerbStep<object, object>
        {
            public StubWithTwoHandlers_SameNumberOfParameters_TwoAsync()
                : base("Test")
            {
            }

            public Task Test(string arg) => Task.CompletedTask;

            public Task Test(long arg) => Task.CompletedTask;
        }
    }
}