using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DrillSergeant;

/// <summary>
/// Defines a parameter caster that uses reflection to instantiate the target object being casted to.
/// </summary>
internal static class ParameterCaster
{
    /// <summary>
    /// Casts a context dictionary to the given type.
    /// </summary>
    /// <param name="source">The source data to convert.</param>
    /// <param name="type">The type to convert to.</param>
    /// <returns>The casted object.</returns>
    /// <exception cref="ParameterCastFailedException">Thrown when the target type is a primitive or array.</exception>
    public static object Cast(IDictionary<string, object?> source, Type type)
    {
        if (type == typeof(object))
        {
            return source;
        }

        if (type.IsPrimitive ||
            type.IsArray ||
            type == typeof(string))
        {
            throw new ParameterCastFailedException(type, "Cannot cast to a primitive type.");
        }

        return InstantiateInstance(source, type);
    }

    internal static object InstantiateInstance(IDictionary<string, object?> source, Type type)
    {
        var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        if (constructors.Length is 0 or > 1)
        {
            throw new ParameterCastFailedException(type, $"The target type {type.FullName} must have exactly one constructor.");
        }

        var ctor = constructors[0];
        var ctorArguments = GetConstructorParameters(source, ctor);
        var properties = GetProperties(type, ctor);
        var target = ctor.Invoke(ctorArguments);

        foreach (var property in from p in properties
                                 where source.ContainsKey(p.Name)
                                 select p)
        {
            var value = source[property.Name]!;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    private static IEnumerable<PropertyInfo> GetProperties(Type type, ConstructorInfo ctor)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
        var ctorParameters = ctor.GetParameters();
        var properties = type.GetProperties(flags);

        return properties.Where(p => ctorParameters.Any(x => x.Name == p.Name) == false).ToArray();
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    private static object?[] GetConstructorParameters(IDictionary<string, object?> source, ConstructorInfo ctor)
    {
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

        return arguments.ToArray();
    }
}