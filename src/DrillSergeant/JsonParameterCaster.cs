using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

[ExcludeFromCodeCoverage]
[Obsolete("This was the original dynamic caster for parameter resolution.  It should not be used anymore because it is unable to preserve reference types.")]
public class JsonParameterCaster : IParameterCaster
{
    public object Cast(IDictionary<string, object?> source, Type type)
    {
        if (type == typeof(object))
        {
            return source;
        }

        if (type.IsPrimitive ||
            type.IsArray ||
            type == typeof(string))
        {
            throw new InvalidOperationException("Cannot cast to a primitive type.");
        }

        var serialized = JsonConvert.SerializeObject(source);
        var converted = JsonConvert.DeserializeObject(serialized, type)!;

        return converted;
    }
}
