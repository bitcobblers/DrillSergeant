using System.Reflection;

namespace DrillSergeant.MSTest;

public static class ObjectExtensions
{
    public static object? GetPrivateProperty(this object instance, string property)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

        var instanceType = instance.GetType();
        var propertyInfo = instanceType.GetProperty(property, flags);

        return propertyInfo?.GetValue(instance);
    }

    public static T CoerceCast<T>(this object source) where T : class, new()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var target = new T();

        var sourceProperties = source.GetType().GetProperties(flags).ToDictionary(k => k.Name, v => v);
        var targetProperties = target.GetType().GetProperties(flags).ToDictionary(k => k.Name, v => v);

        var commonProperties = sourceProperties.Keys.Intersect(targetProperties.Keys);

        foreach (var propertyName in commonProperties)
        {
            var sourceProperty = sourceProperties[propertyName];
            var targetProperty = targetProperties[propertyName];
            var value = sourceProperty.GetValue(source);

            if (value == null)
            {
                continue;
            }

            if (value.GetType().IsAssignableTo(targetProperty.PropertyType))
            {
                targetProperty.SetValue(target, value);
            }
        }

        return target;
    }
}