using DrillSergeant.MSTest;
using DrillSergeant.MSTest.Reporting;
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

        var reporter = new RawTestReporter();
        var classType = testMethod.MethodInfo.DeclaringType;
        var method = testMethod.MethodInfo;
        var arguments = testMethod.Arguments;
        var timeout = options?.Timeout ?? 0;
        var cancelToken = options?.TestContext.CancellationTokenSource.Token ?? CancellationToken.None;
        dynamic context = options?.TestContext!;

        using var listener = new LogListener();

        var (elapsed, result) = TimedCall(() =>
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
                reporter, 
                classInstance, 
                method, 
                arguments ?? Array.Empty<object?>(), 
                timeout, 
                cancelToken);
        });

        result.Duration = elapsed;
        result.DebugTrace = listener.GetAndClearTrace();
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

    internal static TestResult ExecuteInternal(ITestReporter reporter, object classInstance, MethodInfo method, object?[] arguments, int timeout,
        CancellationToken cancelToken)
    {
        var waitTimeout = timeout == 0 ? int.MaxValue : timeout;
        using var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            cancellationTokenSource.CancelAfter(waitTimeout);
            
            var task = Task.Run(async () =>
            {
                Exception? failureException = null;

                var executor = new BehaviorExecutor(reporter);
                var behavior = await executor.LoadBehavior(classInstance, method, arguments);

                executor.StepFailed += (_, e) => failureException = e.Exception;

                await executor.Execute(behavior);

                return failureException == null ? 
                    TestResultPassed() : 
                    TestResultFailed(failureException);

            }, cancelToken);

            task.Wait(cancellationTokenSource.Token);

            return task.Result;
        }
        catch (OperationCanceledException ex)
        {
            return cancelToken.IsCancellationRequested ? TestResultAborted(ex) : TestResultTimeout(ex);
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

    private static TestResult TestResultPassed() =>
        NewResult(UnitTestOutcome.Passed);

    private static TestResult TestResultFailed(Exception? exception = null) =>
        NewResult(UnitTestOutcome.Failed, exception);

    private static TestResult TestResultTimeout(Exception? exception = null) =>
        NewResult(UnitTestOutcome.Timeout, exception);

    private static TestResult TestResultAborted(Exception? exception = null) =>
        NewResult(UnitTestOutcome.Aborted, exception);

    private static TestResult NewResult(UnitTestOutcome outcome, Exception? exception = null) => new()
    {
        Outcome = outcome,
        TestFailureException = exception
    };
}
