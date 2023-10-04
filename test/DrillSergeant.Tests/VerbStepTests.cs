
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
        public void ThrowsMissingVerbExceptionIfNoHandlerIsFound()
        {
            // Arrange.
            var stub = new StubWithNoVerb();

            // Assert.
            Assert.Throws<MissingVerbHandlerException>(() => stub.PickHandlerWrapper());
        }

        [Fact]
        public void VerbWithParametersThrowsAmbiguousVerbHandlerException()
        {
            // Arrange.
            var stub = new StubWithMultipleSyncVerbs();

            // Assert.
            Assert.Throws<AmbiguousVerbHandlerException>(() => stub.PickHandlerWrapper());
        }

        [Fact]
        public void PrefersAsyncOverSync()
        {
            // Arrange.
            var stub = new StubWithSyncAndAsyncDifferentName();
            var expected = stub.GetType().GetMethod("TestAsync", Array.Empty<Type>());

            // Act.
            var handler = stub.PickHandlerWrapper();

            // Assert.
            Assert.Equal(expected, handler.Method);
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

        public class StubWithSyncAndAsyncDifferentName : StubWithExposedPickHandler
        {
            public void Test() { }
            public Task TestAsync() => Task.CompletedTask;
        }

        public class StubWithNoVerb : StubWithExposedPickHandler
        {
        }
    }
}