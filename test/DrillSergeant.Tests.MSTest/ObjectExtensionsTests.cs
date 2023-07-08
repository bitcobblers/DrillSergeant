using System.Diagnostics;
using DrillSergeant.MSTest;
using Shouldly;

namespace DrillSergeant.Tests.MSTest;

[TestClass]
public class ObjectExtensionsTests
{
    public static TestContext? MyContext;

    [ClassInitialize]
    public static void Init(TestContext context) => MyContext = context;

    [Behavior]
    public void MyBehavior()
    {
        Console.Out.WriteLine("OUT LINE");
        MyContext?.WriteLine("FOO");
        Console.Error.WriteLine("ERR LINE");
        Trace.WriteLine("TRACE LINE");
        Trace.WriteLine("TRACE LINE2");
        Debug.WriteLine("DEBUG LINE");
        Debug.WriteLine("DEBUG LINE2");
    }


    [TestClass]
    public class GetPrivatePropertyMethod : ObjectExtensionsTests
    {
        [TestMethod]
        public void UnknownPropertyReturnsNull()
        {
            // Arrange.
            var stub = new StubWithPrivateProperties();

            // Act.
            var result = stub.GetPrivateProperty("unknown");

            // Assert.
            result.ShouldBeNull();
        }

        [TestMethod]
        public void KnownPropertyReturnsPropertyValue()
        {
            // Arrange.
            var stub = new StubWithPrivateProperties();
            var expected = stub.PrivatePropertyValue();

            // Act.
            var result = stub.GetPrivateProperty("PrivateProperty");

            // Assert.
            result.ShouldBeSameAs(expected);
        }

        private class StubWithPrivateProperties
        {
            public object PrivatePropertyValue() => PrivateProperty;

            private object PrivateProperty { get; set; } = new object();
        }
    }

    [TestClass]
    public class CoerceCastMethod : ObjectExtensionsTests
    {
        [TestMethod]
        public void BindsMatchingType()
        {
            // Arrange.
            var source = new StubTypeSource
            {
                IntProperty = 1
            };

            // Act.
            var result = source.CoerceCast<StubTypeTarget>();

            // Assert.
            result.IntProperty.ShouldBe(1);
        }

        [TestMethod]
        public void BindsPropertyWithCommonRoot()
        {
            // Arrange.
            var source = new StubTypeSource
            {
                Stub = new StubImplementation()
            };

            // Act.
            var result = source.CoerceCast<StubTypeTarget>();

            // Assert.
            result.Stub.ShouldNotBeNull();
        }

        private interface IStub
        {
        }

        private class StubImplementation : IStub
        {
        }

        private class StubTypeSource
        {
            public int IntProperty { get; set; }

            public IStub? Stub { get; set; }
        }

        private class StubTypeTarget
        {
            public int IntProperty { get; set; }

            public StubImplementation? Stub { get; set; }
        }
    }
}
