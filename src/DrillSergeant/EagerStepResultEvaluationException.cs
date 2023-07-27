using System;
using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

[ExcludeFromCodeCoverage, Serializable]
#pragma warning disable CA2229
public class EagerStepResultEvaluationException : Exception
#pragma warning restore CA2229
{
    public EagerStepResultEvaluationException()
        : base("Cannot evaluate step result.  Evaluation can only occur while executing a behavior.")
    {
    }
}
