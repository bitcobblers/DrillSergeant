using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DrillSergeant;

/// <summary>
/// Defines a parameter caster that uses reflection to instantiate the target object being casted to.
/// </summary>
public class ReflectionParameterCaster : IParameterCaster
{
    /// <inheritdoc cref="IParameterCaster.Cast(IDictionary{string, object?}, Type)" />
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

        var target = InstantiateTarget(type);
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;

        foreach (var property in from p in type.GetProperties(flags)
                                 where source.ContainsKey(p.Name)
                                 select p)
        {
            var value = source[property.Name]!;

            if (value == null)
            {
                continue;
            }

            if (value.GetType().IsAssignableTo(property.PropertyType))
            {
                property.SetValue(target, value, null);
            }
        }

        return target;
    }

    private static object InstantiateTarget(Type type)
    {
        var ctor = type.GetConstructor(Array.Empty<Type>());

        return ctor == null
            ? throw new InvalidOperationException(
                $"The target type {type.FullName} does not have a parameterless constructor and cannot be used for parameter resolution.")
            : ctor.Invoke(Array.Empty<object?>());
    }
}