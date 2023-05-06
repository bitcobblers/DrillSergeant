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
                    async () =>
                    {
                        var parameters = TestMethod.GetParameters().Select(x => Activator.CreateInstance(x.ParameterType)).ToArray();

                        if (TestMethod.Invoke(testClassInstance, parameters) is not Behavior behavior)
                        {
                            Aggregator.Add(new InvalidOperationException($"Behavior tests must return an instance of type {typeof(Behavior).FullName}"));
                            return;
                        }

                        var resolver = new DefaultResolver();
                        var context = behavior.InitContext();
                        bool stepFailed;

                        resolver.Register(context.GetType(), context);
                        resolver.Register(behavior.InputType, TestMethodArguments.First());

                        foreach (var step in behavior.Steps)
                        {
                            var stepTimer = new ExecutionTimer();
                            stepFailed = false;
                            
                            await stepTimer.AggregateAsync(() =>
                            {
                                try
                                {
                                    var result = step.Execute(resolver);

                                    if (result != null)
                                    {
                                        resolver.Register(result.GetType(), result);
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Aggregator.Add(ex);
                                    stepFailed = true;
                                }

                                return Task.CompletedTask;
                            });

                            if(stepFailed)
                            {
                                _outputHelper.WriteLine($"Step (failed): {step.Name} took {stepTimer.Total:N2}s");
                            }
                            else
                            {
                                _outputHelper.WriteLine($"Step: {step.Name} took {stepTimer.Total:N2}s");
                            }
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

    [SecuritySafeCritical]
    static void SetSynchronizationContext(SynchronizationContext context)
        => SynchronizationContext.SetSynchronizationContext(context);
}
