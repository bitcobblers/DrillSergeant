using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

[ExcludeFromCodeCoverage]
public class EagerStepResultEvaluationException : Exception
{
    public EagerStepResultEvaluationException(string name)
        : base($"Cannot evaluate step '{name}' result.  Evaluation can only occur while executing a behavior.")
    {
        Name = name;
    }

    public string Name { get; private set; }
}
