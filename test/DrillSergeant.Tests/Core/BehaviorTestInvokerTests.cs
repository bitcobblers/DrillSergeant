using DrillSergeant.Core;
using Xunit;

namespace DrillSergeant.Tests.Core;

public class BehaviorTestInvokerTests
{
    //public class GetDependencyResolverTests : BehaviorTestInvokerTests
    //{
    //    [Fact]
    //    public void SetupMethodReturnsResolver()
    //    {
    //        // Arrange.
    //        var instance = new StubWithSetupMethod();

    //        // Act.
    //        var result = BehaviorTestInvoker.GetDependencyResolver(typeof(StubWithSetupMethod), instance);

    //        // Assert.
    //        Assert.NotNull(result);
    //    }

    //    [Fact]
    //    public void NoSetupMethodReturnsNull()
    //    {
    //        // Arrange.
    //        var instance = new StubWithNoSetupMethod();

    //        // Act.
    //        var result = BehaviorTestInvoker.GetDependencyResolver(typeof(StubWithNoSetupMethod), instance);

    //        // Assert.
    //        Assert.Null(result);
    //    }

    //    [Fact]
    //    public void SetupWithWrongReturnTypeReturnsNull()
    //    {
    //        // Arrange.
    //        var instance = new StubWithSetupMethod_WrongReturn();

    //        // Act.
    //        var result = BehaviorTestInvoker.GetDependencyResolver(typeof(StubWithSetupMethod_WrongReturn), instance);

    //        // Assert.
    //        Assert.Null(result);
    //    }

    //    [Fact]
    //    public void SetupWithParametersReturnsNull()
    //    {
    //        // Arrange.
    //        var instance = new StubWithSetupMethod_InvalidArgs();

    //        // Act.
    //        var result = BehaviorTestInvoker.GetDependencyResolver(typeof(StubWithSetupMethod_InvalidArgs), instance);

    //        // Assert.
    //        Assert.Null(result);
    //    }

    //    public class StubWithNoSetupMethod { }

    //    public class StubWithSetupMethod
    //    {
    //        [BehaviorResolverSetup]
    //        public IDependencyResolver Setup() => new DefaultResolver();
    //    }

    //    public class StubWithSetupMethod_WrongReturn
    //    {
    //        [BehaviorResolverSetup]
    //        public bool Setup() => false;
    //    }

    //    public class StubWithSetupMethod_InvalidArgs
    //    {
    //        [BehaviorResolverSetup]
    //        public IDependencyResolver Setup(int invalid) => new DefaultResolver();
    //    }
    //}

    //public class ParseParametersMethod : BehaviorTestInvokerTests
    //{
    //    [Fact]
    //    public void EmptyParametersReturnsEmptyArray()
    //    {
    //        // Arrange.
    //        var method = GetMethod(nameof(StubWithNoParameters));
    //        var args = Array.Empty<object>();
    //        var resolver = A.Fake<IDependencyResolver>();

    //        // Act.
    //        var result = BehaviorTestInvoker.ParseParameters(method, args, resolver);

    //        // Assert.
    //        Assert.Empty(result);
    //    }

    //    [Fact]
    //    public void EmptyParametersWithSingleInjectReturnsInstantiatedDependency()
    //    {
    //        // Arrange.
    //        var injected = new object();
    //        var method = GetMethod(nameof(StubWithSingleInjectable));
    //        var args = Array.Empty<object>();
    //        var resolver = A.Fake<IDependencyResolver>();

    //        A.CallTo(() => resolver.Resolve(typeof(object))).Returns(injected);

    //        // Act.
    //        var result = BehaviorTestInvoker.ParseParameters(method, args, resolver);

    //        // Assert.
    //        Assert.NotEmpty(result);
    //        Assert.NotNull(result[0]);
    //        Assert.Same(injected, result[0]);
    //    }

    //    [Fact]
    //    public void TooFewInputsThrowsInvalidOperationException()
    //    {
    //        // Arrange.
    //        var method = GetMethod(nameof(StubWithOneInputAndOneInjectable));
    //        var args = Array.Empty<object>();
    //        var resolver = A.Fake<IDependencyResolver>();

    //        // Assert.
    //        Assert.Throws<InvalidOperationException>(() => BehaviorTestInvoker.ParseParameters(method, args, resolver));
    //    }

    //    [Fact]
    //    public void TooManyInputsThrowsInvalidOperationException()
    //    {
    //        // Arrange.
    //        var method = GetMethod(nameof(StubWithOneInputAndOneInjectable));
    //        var args = new object[] { 1, 2 };
    //        var resolver = A.Fake<IDependencyResolver>();

    //        // Assert.
    //        Assert.Throws<InvalidOperationException>(() => BehaviorTestInvoker.ParseParameters(method, args, resolver));
    //    }

    //    private MethodInfo GetMethod(string name) => 
    //        typeof(ParseParametersMethod).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!;

    //    private void StubWithNoParameters() { }
    //    private void StubWithSingleInjectable([Inject] object _) { }
    //    private void StubWithOneInputAndOneInjectable(int _, [Inject] DateTime ignored) { }
    //}
}
