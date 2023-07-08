using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DrillSergeant.MSTest;
using DrillSergeant.MSTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            ?.CoerceCast<TestMethodOptions>();

        dynamic context = options?.TestContext!;
        var reporter = GetReporter(options?.TestContext);
        using var listener = new LogListener();

        var (elapsed, result) = TimedCall(() =>
        {
            object? classInstance;
            var method = testMethod.MethodInfo;
            var arguments = testMethod.Arguments;
            var timeout = options?.Timeout ?? 0;
            var cancelToken = options?.TestContext.CancellationTokenSource.Token ?? CancellationToken.None;

            try
            {
                classInstance = CreateTestClassInstance(testMethod.MethodInfo.DeclaringType);
            }
            catch (TestFailedException ex)
            {
                return new TestResult
                {
                    Outcome = UnitTestOutcome.Failed,
                    TestFailureException = ex
                };
            }

            return ExecuteInternal(classInstance, method, arguments, timeout, cancelToken);
        });

        result.Duration = elapsed;
        result.DebugTrace = listener.GetAndClearTrace();
        result.LogOutput = listener.GetAndClearStdOut();
        result.LogError = listener.GetAndClearStdErr();
        result.TestContextMessages = context.GetDiagnosticMessages();
        result.ResultFiles = context.GetResultFiles();

        context.ClearDiagnosticMessages();

        return new []
        {
            result
        };
    }

    private TestResult ExecuteInternal(object classInstance, MethodInfo method, object?[]? arguments, int timeout, CancellationToken cancelToken)
    {
        var waitTimeout = timeout == 0 ? Int32.MaxValue : timeout;
        var cancellationTokenSource = new CancellationTokenSource();

        var task = Task.Run(() =>
        {
            method.Invoke(classInstance, arguments);
        }, cancelToken);

        try
        {
            cancellationTokenSource.CancelAfter(waitTimeout);
            task.Wait(cancellationTokenSource.Token);
        }
        catch (OperationCanceledException ex)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return new TestResult
                {
                    Outcome = UnitTestOutcome.Aborted,
                    TestFailureException = ex,
                };
            }

            return new TestResult
            {
                Outcome = UnitTestOutcome.Timeout,
                TestFailureException = ex,
            };
        }
        catch (Exception ex)
        {
            return new TestResult
            {
                Outcome = UnitTestOutcome.Failed,
                TestFailureException = ex,
            };
        }

        return new TestResult
        {
            Outcome = UnitTestOutcome.Passed
        };
    }

    private static (TimeSpan, TestResult) TimedCall(Func<TestResult> action)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = action();
        
        return (stopwatch.Elapsed, result);
    }

    private static ITestReporter GetReporter(TestContext? context) =>
        context != null ? new RawTestReporter(context) : new NullTestReporter();

    internal static object CreateTestClassInstance(Type? type)
    {
        if (type == null)
        {
            throw new TestFailedException("The declaring type for the test method could not be determined.");
        }

        var ctor = type.GetConstructor(Array.Empty<Type>());

        if (ctor == null)
        {
            throw new TestFailedException("Unable to find a constructor with zero parameters for the test class.");
        }

        try
        {
            return ctor.Invoke(null);
        }
        catch (Exception ex)
        {
            throw new TestFailedException("Unable to instantiate new test class instance.", ex);
        }
    }
}

[Serializable]
public class TestFailedException : Exception
{
    public TestFailedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}