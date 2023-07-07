using DrillSergeant.MSTest;
using Shouldly;

namespace DrillSergeant.Tests.MSTest;

[TestClass]
public class ObjectExtensionsTests
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

    public class StubWithPrivateProperties
    {
        public object PrivatePropertyValue() => PrivateProperty;

        private object PrivateProperty { get; set; } = new object();
    }
}
