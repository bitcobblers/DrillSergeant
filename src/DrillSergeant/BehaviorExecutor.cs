using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DrillSergeant;

internal class BehaviorExecutor
{
    private record StepFilter(Func<IStep, bool> Condition, Action<IStep> Alternate);

    internal static readonly AsyncLocal<bool> IsExecuting = new();

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

    public Task Execute(Behavior behavior, CancellationToken cancellationToken, int timeout = 0)
    {
        try
        {
            CurrentBehavior.Set(behavior);
            IsExecuting.Value = true;

            return timeout == 0 ?
                ExecuteInternalNoTimeout(behavior, cancellationToken) :
                ExecuteInternalWithTimeout(behavior, timeout, cancellationToken);
        }
        finally
        {
            CurrentBehavior.Clear();
            IsExecuting.Value = false;
        }
    }

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

        var filters = new[]
        {
            new StepFilter( _ => cancellationToken.IsCancellationRequested, s => CancelStep(s, previousStepFailed)),
            new StepFilter( s => s.ShouldSkip, SkipStep),
            new StepFilter( _ => previousStepFailed, SkipFailedStep)
        };

        _reporter.WriteBlock("Input", behavior.Input);

        foreach (var step in behavior)
        {
            var filter = filters.FirstOrDefault(f => f.Condition(step));

            if (filter != null)
            {
                filter.Alternate(step);
                continue;
            }

            var elapsed = await TimedCall(async () =>
            {
                try
                {
                    CurrentBehavior.ResetContext();
                    await step.Execute();
                }
                catch (Exception ex)
                {
                    StepFailed(this, new StepFailedEventArgs(ex));
                    previousStepFailed = true;
                }
                finally
                {
                    CurrentBehavior.UpdateContext();
                }
            });

            _reporter.WriteStepResult(new StepExecutionResult
            {
                Verb = step.Verb,
                Name = step.Name,
                Skipped = false,
                Context = behavior.LogContext ? behavior.Context : null,
                Elapsed = elapsed,
                PreviousStepsFailed = previousStepFailed,
                Success = !previousStepFailed
            });
        }
    }

    private void CancelStep(IStep step, bool previousStepFailed)
    {
        _reporter.WriteStepResult(new StepExecutionResult
        {
            Verb = step.Verb,
            Name = step.Name,
            PreviousStepsFailed = previousStepFailed,
            CancelPending = true,
            Skipped = true,
            Success = true
        });
    }

    private void SkipStep(IStep step)
    {
        _reporter.WriteStepResult(new StepExecutionResult
        {
            Verb = step.Verb,
            Name = step.Name,
            Skipped = true,
            Success = true,
            PreviousStepsFailed = false
        });
    }

    private void SkipFailedStep(IStep step)
    {
        _reporter.WriteStepResult(new StepExecutionResult
        {
            Verb = step.Verb,
            Name = step.Name,
            PreviousStepsFailed = true,
            Skipped = true,
            Success = false
        });
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