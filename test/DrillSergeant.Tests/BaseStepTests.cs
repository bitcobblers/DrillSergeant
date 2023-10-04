using System.Reflection;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable IDE0060
#pragma warning disable IDE0051

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
            bool result = BaseStep.IsAsync(method);

            // Assert.
            result.ShouldBe(expected);
        }

        private void NonAsyncMethod() { }
        private Task AsyncWithNoReturn() => Task.CompletedTask;
        private Task<int> AsyncWithReturn() => Task.FromResult(0);
    }

    private static MethodInfo GetMethod(IReflect type, string name)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        return type.GetMethod(name, flags)!;
    }
}
