using FakeItEasy;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

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
            var context = new Context();
            var input = new Input();
            var parameters = stubExecuteMethodWithParameters.GetParameters();

            // Act.
            var resolvedParameters = step.ResolveParameters(context, input, parameters);
            var resolvedContext = resolvedParameters.First(x => x!.GetType() == typeof(Context));

            // Assert.
            resolvedContext.ShouldBeSameAs(context);
        }

        [Fact]
        public void InputParameterResolvesToPassedInput()
        {
            // Arrange.
            var step = A.Fake<BaseStep<Context, Input>>(options => options.CallsBaseMethods());
            var context = new Context();
            var input = new Input();
            var parameters = stubExecuteMethodWithParameters.GetParameters();

            // Act.
            var resolvedParameters = step.ResolveParameters(context, input, parameters);
            var resolvedInput = resolvedParameters.First(x => x!.GetType() == typeof(Input));

            // Assert.
            resolvedInput.ShouldBeSameAs(input);
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
