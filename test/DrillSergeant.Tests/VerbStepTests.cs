using System;
using System.Threading.Tasks;
using Xunit;
// ReSharper disable UnusedParameter.Global
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable IDE0060

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
            var stub = new StubWithSyncAndAsyncSameName();
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
            var stub = new StubWithSyncAndAsyncDifferentName();
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
        [InlineData(typeof(StubWithTwoHandlersSameNumberOfParametersNoAsync))]
        [InlineData(typeof(StubWithTwoHandlersSameNumberOfParametersTwoAsync))]
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

            public Delegate PickHandlerWrapper() => PickHandler();
        }

        public class StubWithMultipleSyncVerbs : StubWithExposedPickHandler
        {
            public void Test() { }
            public void Test(int arg1) { }
        }

        public class StubWithSyncAndAsyncSameName : StubWithExposedPickHandler
        {
            public void Test(int arg) { }
            public Task Test(string arg) => Task.CompletedTask;
        }

        public class StubWithSyncAndAsyncDifferentName : StubWithExposedPickHandler
        {
            public void Test(int arg) { }
            public Task TestAsync(int arg) => Task.CompletedTask;
        }

        public class StubWithNoVerb : StubWithExposedPickHandler
        {
        }

        public class StubWithTwoHandlersSameNumberOfParametersNoAsync : StubWithExposedPickHandler
        {
            public void Test(int arg) { }

            public void Test(string arg) { }
        }

        public class StubWithTwoHandlersSameNumberOfParametersTwoAsync : StubWithExposedPickHandler
        {
            public Task Test(string arg) => Task.CompletedTask;

            public Task Test(long arg) => Task.CompletedTask;
        }
    }
}