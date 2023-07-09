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

        return IsRecord(type)
            ? InstantiateRecord(source, type)
            : InstantiateClass(source, type);
    }

    internal static object InstantiateClass(IDictionary<string, object?> source, Type type)
    {
        var ctor = type.GetConstructor(Array.Empty<Type>());
        var target = ctor == null
            ? throw new InvalidOperationException(
                $"The target type {type.FullName} does not have an empty constructor and cannot be used for parameter resolution.")
            : ctor.Invoke(Array.Empty<object?>());

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;

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

    internal static object InstantiateRecord(IDictionary<string, object?> source, Type type)
    {
        var ctor = type.GetConstructors()[0];
        var ctorParameters = ctor.GetParameters();
        var arguments = new List<object?>();

        foreach (var parameter in ctorParameters)
        {
            if (source.TryGetValue(parameter.Name!, out var value) == false)
            {
                arguments.Add(null);
            }
            else if (value != null && value.GetType().IsAssignableTo(parameter.ParameterType))
            {
                arguments.Add(value);
            }
            else
            {
                arguments.Add(null);
            }
        }

        return ctor.Invoke(arguments.ToArray());
    }

    internal static bool IsRecord(Type type)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        var constructors = type.GetConstructors(flags);

        if (type.IsValueType || constructors.Length != 1)
        {
            return false;
        }

        var ctor = constructors[0];
        var ctorParameters = ctor.GetParameters().ToDictionary(k => k.Name!, v => v.ParameterType);
        var properties = type.GetProperties(flags).ToDictionary(k => k.Name, v => v.PropertyType);

        if (ctorParameters.Count == 0)
        {
            return false;
        }

        return ctorParameters.All(x =>
        {
            if (properties.TryGetValue(x.Key, out var propType))
            {
                return propType == x.Value;
            }

            return false;
        });
    }
}