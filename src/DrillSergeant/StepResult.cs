using JetBrains.Annotations;
using System;

namespace DrillSergeant;

public class StepResult<T>
{
    private readonly Lazy<T> _value;

    public StepResult(Func<T> func) => _value = new Lazy<T>(func);

    [PublicAPI]
    public T Value
    {
        get
        {
            //if (CurrentBehavior.IsExecuting == false)
            //{
            //    throw new InvalidOperationException("Cannot evaluate step result outside of a test");
            //}

            return _value.Value;
        }
    }

    public static implicit operator T(StepResult<T> stepResult) => stepResult.Value;
}