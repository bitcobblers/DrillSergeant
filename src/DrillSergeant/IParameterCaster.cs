using System;
using System.Collections.Generic;

namespace DrillSergeant;

public interface IParameterCaster
{
    object Cast(IDictionary<string, object?> source, Type type);
}
