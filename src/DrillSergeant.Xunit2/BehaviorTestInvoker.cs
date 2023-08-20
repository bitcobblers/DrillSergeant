using System.Reflection;
using System.Security;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

internal class BehaviorTestInvoker : XunitTestInvoker
{
    private readonly ITestReporter _reporter;

    public BehaviorTestInvoker(ITestReporter reporter, ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        => _reporter = reporter;

    protected override Task<decimal> InvokeTestMethodAsync(object testClassInstance)
    {
        if (TestCase.InitializationException == null)
        {
            return InternalInvokeTestMethodAsync(testClassInstance);
        }

        var tcs = new TaskCompletionSource<decimal>();
        tcs.SetException(TestCase.InitializationException);
        return tcs.Task;
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
                        var executor = new BehaviorExecutor(_reporter);
                        using var behavior = await executor.LoadBehavior(testClassInstance, TestMethod, TestMethodArguments);

                        executor.StepFailed += (_, e) => Aggregator.Add(e.Exception);

                        await executor.Execute(behavior, CancellationTokenSource.Token, TestCase.Timeout);
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
