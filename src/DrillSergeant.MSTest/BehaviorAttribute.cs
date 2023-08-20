using DrillSergeant.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
public sealed class BehaviorAttribute : TestMethodAttribute
{
    // ReSharper disable once UnusedMember.Global
    [Obsolete("Feature properties in the Behavior attribute do not work for MSTest.  Use [TestCategory] instead.")]
    [ExcludeFromCodeCoverage]
    public string? Feature { get; set; }

    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var options = testMethod
            .GetPrivateProperty("TestMethodOptions")
            ?.CoerceCast<TestMethodOptions>()!;

        var classType = testMethod.MethodInfo.DeclaringType;
        var method = testMethod.MethodInfo;
        var arguments = testMethod.Arguments;
        var timeout = options?.Timeout ?? 0;
        var captureTrace = options?.CaptureDebugTraces ?? false;
        var cancelToken = options?.TestContext?.CancellationTokenSource.Token ?? CancellationToken.None;
        dynamic context = options?.TestContext!;

        using var listener = new LogListener(captureTrace);
        var reporter = new RawTestReporter(listener.StdOut);

        (TimeSpan elapsed, TestResult result) = TimedCall(() =>
        {
            object? classInstance;

            try
            {
                classInstance = CreateTestClassInstance(classType);
            }
            catch (TestFailedException ex)
            {
                return TestResultFailed(ex);
            }

            return ExecuteInternal(
                new BehaviorExecutor(reporter),
                classInstance,
                method,
                arguments ?? Array.Empty<object?>(),
                timeout,
                cancelToken);
        });

        result.Duration = elapsed;
        result.DebugTrace = LogListener.GetAndClearTrace();
        result.LogOutput = listener.GetAndClearStdOut();
        result.LogError = listener.GetAndClearStdErr();
        result.TestContextMessages = context.GetDiagnosticMessages();
        result.ResultFiles = context.GetResultFiles();

        context.ClearDiagnosticMessages();

        return new[]
        {
            result
        };
    }

    internal static TestResult ExecuteInternal(
        BehaviorExecutor executor,
        object classInstance,
        MethodInfo method,
        object?[] arguments,
        int timeout,
        CancellationToken cancelToken)
    {
        var waitTimeout = timeout == 0 ? int.MaxValue : timeout * 1000;
        using var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            cancellationTokenSource.CancelAfter(waitTimeout);

            var task = Task.Run(async () =>
            {
                var exceptions = new List<Exception>();
                using var behavior = await executor.LoadBehavior(classInstance, method, arguments);

                executor.StepFailed += (_, e) => exceptions.Add(e.Exception);

                await executor.Execute(behavior, cancelToken, timeout);

                return exceptions.Any()
                    ? TestResultFailed(new AggregateException(exceptions))
                    : TestResultPassed();

            }, cancelToken);

            task.Wait(cancellationTokenSource.Token);

            return task.Result;
        }
        catch (AggregateException aggregate)
        {
            var inner = aggregate.InnerExceptions.First();

            return inner switch
            {
                BehaviorTimeoutException => TestResultTimeout(inner),
                OperationCanceledException => TestResultAborted(inner),
                _ => TestResultFailed(inner)
            };
        }
        catch (Exception ex)
        {
            return TestResultFailed(ex);
        }
    }

    internal static object CreateTestClassInstance(Type? type)
    {
        if (type == null)
        {
            throw new TestFailedException("The declaring type for the test method could not be determined.");
        }

        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch (Exception ex)
        {
            throw new TestFailedException("Unable to instantiate new test class instance.", ex);
        }
    }

    private static (TimeSpan, TestResult) TimedCall(Func<TestResult> action)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = action();

        return (stopwatch.Elapsed, result);
    }

    [ExcludeFromCodeCoverage]
    private static TestResult TestResultPassed() =>
        NewResult(UnitTestOutcome.Passed);

    [ExcludeFromCodeCoverage]
    private static TestResult TestResultFailed(Exception? exception = null) =>
        NewResult(UnitTestOutcome.Failed, exception);

    [ExcludeFromCodeCoverage]
    private static TestResult TestResultTimeout(Exception? exception = null) =>
        NewResult(UnitTestOutcome.Timeout, exception);

    [ExcludeFromCodeCoverage]
    private static TestResult TestResultAborted(Exception? exception = null) =>
        NewResult(UnitTestOutcome.Aborted, exception);

    private static TestResult NewResult(UnitTestOutcome outcome, Exception? exception = null) => new()
    {
        Outcome = outcome,
        TestFailureException = exception
    };
}
