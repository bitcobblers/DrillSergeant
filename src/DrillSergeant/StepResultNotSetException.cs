﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

[ExcludeFromCodeCoverage]
public class StepResultNotSetException : Exception
{
    public StepResultNotSetException(string name)
        : base($"The result for the step '{name}' was not set.")
    {
        Name = name;
    }

    public string Name { get; private set; }
}
