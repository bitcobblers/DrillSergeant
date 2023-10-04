using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

[ExcludeFromCodeCoverage, Serializable]
public class EagerStepResultEvaluationException : Exception
{
    public EagerStepResultEvaluationException(string name)
        : base($"Cannot evaluate step '{name}' result.  Evaluation can only occur while executing a behavior.")
    {
        Name = name;
    }

    protected EagerStepResultEvaluationException(SerializationInfo info, StreamingContext context)
    {
        Name = info.GetString(nameof(Name))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Name), Name);
    }

    public string Name { get; private set; }
}
