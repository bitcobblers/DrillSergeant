using FakeItEasy;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace JustBehave.Tests;

public class BaseStepTests
{
    public class IsAsyncMethod : BaseStepTests
    {
        [Theory]
        [InlineData(nameof(NonAsyncMethod), false)]
        [InlineData(nameof(AsyncWithNoReturn), true)]
        [InlineData(nameof(AsyncWithReturn), true)]
        public void Scenarios(string methodName, bool expected)
        {
            // Arrange.
            var method = GetMethod(typeof(IsAsyncMethod), methodName);

            // Act.
            bool result = BaseStep<object, object>.IsAsync(method);

            // Assert.
            result.ShouldBe(expected);
        }

        private void NonAsyncMethod() { }
        private Task AsyncWithNoReturn() => Task.CompletedTask;
        private Task<int> AsyncWithReturn() => Task.FromResult(0);
    }

    public class ResolveParametersMethod : BaseStepTests
    {
        public record Context();
        public record Input();

        private readonly MethodInfo stubExecuteMethodWithParameters = GetMethod(typeof(ResolveParametersMethod), nameof(StubExecuteMethodWithParameters));

        [Fact]
        public void ContextParameterResolvesToPassedContext()
        {
            // Arrange.
            var step = A.Fake<BaseStep<Context, Input>>(options => options.CallsBaseMethods());
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var parameters = stubExecuteMethodWithParameters.GetParameters();

            // Act.
            var resolvedParameters = step.ResolveParameters(resolver, context, input, parameters);
            var resolvedContext = resolvedParameters.First(x => x!.GetType() == typeof(Context));

            // Assert.
            resolvedContext.ShouldBeSameAs(context);
        }

        [Fact]
        public void InputParameterResolvesToPassedInput()
        {
            // Arrange.
            var step = A.Fake<BaseStep<Context, Input>>(options => options.CallsBaseMethods());
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var parameters = stubExecuteMethodWithParameters.GetParameters();

            // Act.
            var resolvedParameters = step.ResolveParameters(resolver, context, input, parameters);
            var resolvedInput = resolvedParameters.First(x => x!.GetType() == typeof(Input));

            // Assert.
            resolvedInput.ShouldBeSameAs(input);
        }

        [Fact]
        public void OtherParameterResolvedUsingResolver()
        {
            // Arrange.
            var step = A.Fake<BaseStep<Context, Input>>(options => options.CallsBaseMethods());
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var parameters = stubExecuteMethodWithParameters.GetParameters();

            // Act.
            var resolvedParameters = step.ResolveParameters(resolver, context, input, parameters);

            // Assert.
            A.CallTo(() => resolver.Resolve(typeof(DateTime))).MustHaveHappened();
        }

        private void StubExecuteMethodWithParameters(Context context, Input input, DateTime dateDependency)
        {
        }
    }

    private static MethodInfo GetMethod(Type type, string name)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        return type.GetMethod(name, flags)!;
    }
}
