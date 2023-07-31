using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DrillSergeant;

internal class BehaviorExecutor
{
    public event EventHandler<StepFailedEventArgs> StepFailed = delegate { };

    private readonly ITestReporter _reporter;

    public BehaviorExecutor(ITestReporter reporter) => _reporter = reporter;

    public async Task<Behavior> LoadBehavior(object instance, MethodInfo method, object?[] parameters)
    {
        var input = new Dictionary<string, object?>();
        var methodParameters = method.GetParameters();

        for (int i = 0; i < methodParameters.Length; i++)
        {
            input[methodParameters[i].Name!] = parameters[i];
        }

        return await BehaviorBuilder.BuildAsync(async b =>
        {
            b.SetInput(input);

            if (IsAsync(method))
            {
                dynamic asyncResult = method.Invoke(instance, parameters)!;
                await asyncResult;
            }
            else
            {
                method.Invoke(instance, parameters);
            }

            return b.Freeze();
        });
    }

    public Task Execute(Behavior behavior, CancellationToken cancellationToken, int timeout = 0) =>
        BehaviorBuilder.PushAsync(behavior, () =>
            timeout == 0 ?
                ExecuteInternalNoTimeout(behavior, cancellationToken) :
                ExecuteInternalWithTimeout(behavior, timeout, cancellationToken));

    private async Task ExecuteInternalWithTimeout(IBehavior behavior, int timeout, CancellationToken cancellationToken)
    {
        var executeTask = ExecuteInternalNoTimeout(behavior, cancellationToken);
        var resultTask = await Task.WhenAny(executeTask, Task.Delay(timeout, cancellationToken));

        if (resultTask != executeTask)
        {
            throw new BehaviorTimeoutException(timeout);
        }
    }

    private async Task ExecuteInternalNoTimeout(IBehavior behavior, CancellationToken cancellationToken)
    {
        bool previousStepFailed = false;

        _reporter.WriteBlock("Input", behavior.Input);

        foreach (var step in behavior)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _reporter.WriteStepResult(new StepResult
                {
                    Verb = step.Verb,
                    Name = step.Name,
                    PreviousStepsFailed = previousStepFailed,
                    CancelPending = true,
                    Skipped = true,
                    Success = true
                });

                continue;
            }

            if (step.ShouldSkip)
            {
                _reporter.WriteStepResult(new StepResult
                {
                    Verb = step.Verb,
                    Name = step.Name,
                    Skipped = true,
                    Success = true,
                    PreviousStepsFailed = false
                });

                continue;
            }

            if (previousStepFailed)
            {
                _reporter.WriteStepResult(new StepResult
                {
                    Verb = step.Verb,
                    Name = step.Name,
                    PreviousStepsFailed = true,
                    Skipped = true,
                    Success = false
                });

                continue;
            }

            var elapsed = await TimedCall(async () =>
            {
                try
                {
                    await step.Execute(behavior.Context, behavior.Input);
                }
                catch (Exception ex)
                {
                    StepFailed(this, new StepFailedEventArgs(ex));
                    previousStepFailed = true;
                }
            });

            _reporter.WriteStepResult(new StepResult
            {
                Verb = step.Verb,
                Name = step.Name,
                Skipped = false,
                Context = behavior.LogContext ? behavior.Context : null,
                Elapsed = elapsed,
                PreviousStepsFailed = previousStepFailed,
                Success = !previousStepFailed,
            });
        }
    }

    private static bool IsAsync(MethodInfo method) =>
        method.ReturnType.IsAssignableTo(typeof(Task));

    private static async Task<decimal> TimedCall(Func<Task> task)
    {
        var stopwatch = Stopwatch.StartNew();
        await task();
        return (decimal)stopwatch.Elapsed.TotalSeconds;
    }
}