using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Core;

public class BehaviorTestInvoker : XunitTestInvoker
{
    private readonly ITestOutputHelper _outputHelper;

    public BehaviorTestInvoker(ITestOutputHelper outputHelper, ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        => _outputHelper = outputHelper;

    protected override Task<decimal> InvokeTestMethodAsync(object testClassInstance)
    {
        if (TestCase.InitializationException != null)
        {
            var tcs = new TaskCompletionSource<decimal>();
            tcs.SetException(TestCase.InitializationException);
            return tcs.Task;
        }

        return TestCase.Timeout > 0
            ? InvokeTimeoutTestMethodAsync(testClassInstance)
            : InternalInvokeTestMethodAsync(testClassInstance);
    }

    private async Task<decimal> InvokeTimeoutTestMethodAsync(object testClassInstance)
    {
        var baseTask = InternalInvokeTestMethodAsync(testClassInstance);
        var resultTask = await Task.WhenAny(baseTask, Task.Delay(TestCase.Timeout));

        if (resultTask != baseTask)
        {
            throw new TestTimeoutException(TestCase.Timeout);
        }

        return baseTask.Result;
    }

    private async Task<decimal> InternalInvokeTestMethodAsync(object testClassInstance)
    {
        var oldSyncContext = SynchronizationContext.Current!;

        try
        {
            var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);

            SetSynchronizationContext(asyncSyncContext);

            await Aggregator.RunAsync(
                () => Timer.AggregateAsync(
                    () => InvokeBehavior(testClassInstance)
                )
            );
        }
        finally
        {
            SetSynchronizationContext(oldSyncContext);
        }

        return Timer.Total;
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;

    private async Task<IBehavior?> GetBehavior(object testClassInstance, object?[] parameters)
    {
        if(IsAsync(TestMethod))
        {
            dynamic asyncResult = TestMethod.Invoke(testClassInstance, parameters)!;
            object asyncBehavior = await asyncResult;

            if(asyncBehavior is IBehavior behavior)
            {
                return behavior;
            }
        }
        else
        {
            var syncResult = TestMethod.Invoke(testClassInstance, parameters);

            if (syncResult is IBehavior behavior)
            {
                return behavior;
            }
        }

        throw new InvalidOperationException("Test method did not return a behavior.");
    }

    private async Task InvokeBehavior(object testClassInstance)
    {
        var resolver = GetDependencyResolver(TestClass, testClassInstance) ?? new DefaultResolver();
        var parameters = ParseParameters(TestMethod, TestMethodArguments, resolver);
        var behavior = await GetBehavior(testClassInstance, parameters) ?? throw new InvalidOperationException("The test method did not return a valid behavior instance.");
        bool previousStepFailed = false;

        foreach (var step in behavior)
        {
            if (previousStepFailed)
            {
                FormatStepSkippedMessage(step.Name);
                continue;
            }

            var stepTimer = new ExecutionTimer();
            previousStepFailed = false;

            await stepTimer.AggregateAsync(async () =>
            {
                try
                {
                    await step.Execute(behavior.Context, behavior.Input, resolver);
                }
                catch (Exception ex)
                {
                    Aggregator.Add(ex);
                    previousStepFailed = true;
                }
            });

            FormatStepCompletedMessage(previousStepFailed, step.Name, stepTimer.Total);
        }

        await Task.CompletedTask;
    }

    private void FormatStepSkippedMessage(string name)
    {
        _outputHelper.WriteLine($"Step (skipped due to previous failure): {name}");
    }

    private void FormatStepCompletedMessage(bool failed, string name, decimal elapsed)
    {
        if (failed)
        {
            _outputHelper.WriteLine($"Step (failed): {name} took {elapsed:N2}s");
        }
        else
        {
            _outputHelper.WriteLine($"Step: {name} took {elapsed:N2}s");
        }
    }

    internal static IDependencyResolver? GetDependencyResolver(Type testClass, object instance)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        var setupMethod = (from method in testClass.GetMethods(flags)
                           let attr = method.GetCustomAttribute<BehaviorResolverSetupAttribute>()
                           where attr != null
                           where method.GetParameters().Length == 0
                           where method.ReturnType == typeof(IDependencyResolver)
                           select method).FirstOrDefault();

        if (setupMethod != null)
        {
            return (IDependencyResolver?)setupMethod.Invoke(instance, Array.Empty<object>());
        }

        return null;
    }

    internal static object?[] ParseParameters(MethodInfo method, object[] passedArguments, IDependencyResolver resolver)
    {
        var methodParams = method.GetParameters();
        var resultParams = new object[methodParams.Length];
        int passedArgumentsOffset = 0;
        int numInjectedParams = methodParams.Count(x => x.GetCustomAttribute<InjectAttribute>() != null);

        if (resultParams.Length - numInjectedParams != passedArguments.Length)
        {
            throw new InvalidOperationException("The number of non-injected parameters must match the number of input parameters.");
        }

        for (int i = 0; i < methodParams.Length; i++)
        {
            var param = methodParams[i];

            if (param.GetCustomAttribute<InjectAttribute>() != null)
            {
                resultParams[i] = resolver.Resolve(param.ParameterType);
            }
            else
            {
                resultParams[i] = passedArguments[passedArgumentsOffset++];
            }
        }

        return resultParams;
    }

    [SecuritySafeCritical]
    static void SetSynchronizationContext(SynchronizationContext context)
        => SynchronizationContext.SetSynchronizationContext(context);
}
