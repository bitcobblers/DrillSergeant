using Moq;
using System.Threading.Tasks;
using Xunit;

namespace JustBehave.Tests
{
    public class StepTests
    {
        public class ConstructorMethod : StepTests
        {
            [Fact]
            public void VerbIsSetCorrectly()
            {
                // Act.
                var step = new StubStep(null);

                // Assert.
                Assert.Equal("Stub", step.Verb);
            }

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

            public class StubStep : Step
            {
                public StubStep(string? name) : base("Stub", name) { }
            }
        }

        public class ExecuteMethod : StepTests
        {
            [Fact]
            public void CanExecuteMethodWithNoParameters()
            {
                // Arrange.
                var resolver = new Mock<IDependencyResolver>();
                var stub = new StubWithNoParameters();

                // Act.
                _ = stub.Execute(resolver.Object);

                // Assert.
                Assert.True(stub.HasExecuted);
            }

            [Fact]
            public void InjectsParametersIntoMethod()
            {
                // Arrange.
                var injectable = new Mock<IStubInjectable>();
                var resolver = new Mock<IDependencyResolver>();
                var stub = new StubWithInjectableParameter();

                resolver.Setup(x => x.Resolve(typeof(IStubInjectable))).Returns(injectable.Object);

                // Act.
                _ = stub.Execute(resolver.Object);

                // Assert.
                injectable.Verify(x => x.DoSomething(), Times.Once());
            }

            [Fact]
            public void ReturnsExpectedResultFromSyncMethod()
            {
                // Arrange.
                var resolver = new Mock<IDependencyResolver>();
                var stub = new StubThatReturnsValue_Sync();

                // Act.
                var result = stub.Execute(resolver.Object);

                // Assert.
                Assert.Equal("expected", result);
            }

            [Fact]
            public void ReturnsExpectedResultFromAsyncMethod()
            {
                // Arrange.
                var resolver = new Mock<IDependencyResolver>();
                var stub = new StubThatReturnsValue_Async();

                // Act.
                var result = stub.Execute(resolver.Object);

                // Assert.
                Assert.Equal("expected", result);
            }

            public interface IStubInjectable
            {
                void DoSomething();
            }

            public class StubWithNoParameters : Step
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

            public class StubWithInjectableParameter : Step
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

            public class StubThatReturnsValue_Sync : Step
            {
                public StubThatReturnsValue_Sync()
                    : base("Test")
                {
                }

                public string Test() => "expected";
            }

            public class StubThatReturnsValue_Async : Step
            {
                public StubThatReturnsValue_Async()
                    : base("Test")
                {
                }

                public Task<string> Test() => Task.FromResult("expected");
            }
        }

        public class PickHandlerMethod : StepTests
        {
            [Fact]
            public void PicksVerbWithMostParameters()
            {
                // Arrange.
                var stub = new StubWithMultipleSyncVerbs();
                var expected = stub.GetType().GetMethod("Test", new[] { typeof(int) });

                // Act.
                Step.VerbMethod verbMethod = stub.PickHandler();
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


            public class StubWithMultipleSyncVerbs : Step
            {
                public StubWithMultipleSyncVerbs()
                    : base("Test")
                {
                }

                public void Test() { }
                public void Test(int arg1) { }
            }

            public class StubWithSyncAndAsync_SameName : Step
            {
                public StubWithSyncAndAsync_SameName()
                    : base("Test")
                {
                }

                public void Test(int arg) { }
                public Task Test(string arg) => Task.CompletedTask;
            }

            public class StubWithSyncAndAsync_DifferentName : Step
            {
                public StubWithSyncAndAsync_DifferentName()
                    : base("Test")
                {
                }

                public void Test(int arg) { }
                public Task TestAsync(int arg) => Task.CompletedTask;
            }

            public class StubWithNoVerb : Step
            {
                public StubWithNoVerb()
                    : base("ignored")
                { 
                }
            }
        }
    }
}