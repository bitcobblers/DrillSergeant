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
}