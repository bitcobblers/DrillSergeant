using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

    private async Task InvokeBehavior(object testClassInstance)
    {
        var behavior = await GetBehavior(testClassInstance, TestMethodArguments) ?? throw new InvalidOperationException("The test method did not return a valid behavior instance.");
        bool previousStepFailed = false;

        WriteLogContext(behavior.LogContext, "Input", behavior.Input);

        foreach (var step in behavior)
        {
            if (previousStepFailed)
            {
                FormatStepSkippedMessage(step.Verb, step.Name);
                continue;
            }

            var stepTimer = new ExecutionTimer();
            previousStepFailed = false;

            await stepTimer.AggregateAsync(async () =>
            {
                try
                {
                    await step.Execute(behavior.Context, behavior.Input);
                }
                catch (Exception ex)
                {
                    Aggregator.Add(ex);
                    previousStepFailed = true;
                }
            });

            FormatStepCompletedMessage(previousStepFailed, step.Verb, step.Name, stepTimer.Total);
            WriteLogContext(behavior.LogContext, "Context", behavior.Context);
        }

        await Task.CompletedTask;
    }

    private async Task<IBehavior?> GetBehavior(object testClassInstance, object?[] parameters)
    {
        if (IsAsync(TestMethod))
        {
            dynamic asyncResult = TestMethod.Invoke(testClassInstance, parameters)!;
            object asyncBehavior = await asyncResult;

            if (asyncBehavior is IBehavior behavior)
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

    private void WriteLogContext(bool shouldLog, string label, object context)
    {
        if (shouldLog == false)
        {
            return;
        }

        var serializationSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Error = (s, e) =>
            {
                e.ErrorContext.Handled = true;
            }
        };

        var serializedContext = JsonConvert.SerializeObject(context, serializationSettings);
        _outputHelper.WriteLine($"{label}: {serializedContext}");
        _outputHelper.WriteLine(string.Empty);
    }

    private void FormatStepSkippedMessage(string verb, string name)
    {
        _outputHelper.WriteLine($"☐ {verb} (skipped due to previous failure): {name}");
    }

    private void FormatStepCompletedMessage(bool failed, string verb, string name, decimal elapsed)
    {
        var icon = failed ? "❎" : "✅";
        _outputHelper.WriteLine($"{icon} {verb}: {name} took {elapsed:N2}s");
    }

    [SecuritySafeCritical]
    static void SetSynchronizationContext(SynchronizationContext context)
        => SynchronizationContext.SetSynchronizationContext(context);
}
