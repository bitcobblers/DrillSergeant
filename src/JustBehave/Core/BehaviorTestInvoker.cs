using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace JustBehave.Core;

public class BehaviorTestInvoker : XunitTestInvoker
{
    public BehaviorTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
    {
    }

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
                    async () =>
                    {
                        var parameters = TestMethod.GetParameters().Select(x => (object?)null).ToArray();
                        var behavior = TestMethod.Invoke(testClassInstance, parameters) as Behavior;

                        if(behavior==null)
                        {
                            var behaviorType = typeof(Behavior);
                            Aggregator.Add(new InvalidOperationException($"Behavior tests must return an instance of type {behaviorType.FullName}"));
                        }

                        await Task.CompletedTask;
                    }
                )
            );
        }
        finally
        {
            SetSynchronizationContext(oldSyncContext);
        }

        return Timer.Total;
    }

    protected virtual async Task<decimal> InternalInvokeTestMethodAsync_Old(object testClassInstance)
    {
        var oldSyncContext = SynchronizationContext.Current!;

        try
        {
            var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
            SetSynchronizationContext(asyncSyncContext);

            await Aggregator.RunAsync(
                () => Timer.AggregateAsync(
                    async () =>
                    {
                        var parameterCount = TestMethod.GetParameters().Length;
                        var valueCount = TestMethodArguments == null ? 0 : TestMethodArguments.Length;
                        
                        if (parameterCount != valueCount)
                        {
                            Aggregator.Add(
                                new InvalidOperationException(
                                    $"The test method expected {parameterCount} parameter value{(parameterCount == 1 ? "" : "s")}, but {valueCount} parameter value{(valueCount == 1 ? "" : "s")} {(valueCount == 1 ? "was" : "were")} provided."
                                )
                            );
                        }
                        else
                        {
                            var result = CallTestMethod(testClassInstance);
                            var task = GetTaskFromResult(result);
                            
                            if (task != null)
                            {
                                if (task.Status == TaskStatus.Created)
                                {
                                    throw new InvalidOperationException("Test method returned a non-started Task (tasks must be started before being returned)");
                                }

                                await task;
                            }
                            else
                            {
                                var ex = await asyncSyncContext.WaitForCompletionAsync();

                                if (ex != null)
                                {
                                    Aggregator.Add(ex);
                                }
                            }
                        }
                    }
                )
            );
        }
        finally
        {
            SetSynchronizationContext(oldSyncContext);
        }

        return Timer.Total;
    }

    [SecuritySafeCritical]
    static void SetSynchronizationContext(SynchronizationContext context)
        => SynchronizationContext.SetSynchronizationContext(context);
}
