using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

[ExcludeFromCodeCoverage, Serializable]
public class StepResultNotSetException : Exception
{
    public StepResultNotSetException(string name)
        : base($"The result for the step '{name}' was not set.")
    {
        Name = name;
    }

    protected StepResultNotSetException(SerializationInfo info, StreamingContext context)
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
