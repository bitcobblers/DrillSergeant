using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DrillSergeant;

public static class DelegateExtensions
{
    /// <summary>
    /// Converts a method to a delegate.
    /// </summary>
    /// <param name="methodInfo">The method to convert.</param>
    /// <param name="target">The target instance holding the method.</param>
    /// <returns>A delegate to the method.</returns>
    /// <remarks>
    /// Original source: // Source: https://stackoverflow.com/questions/940675/getting-a-delegate-from-methodinfo
    /// </remarks>
    public static Delegate ToDelegate(this MethodInfo methodInfo, object target)
    {
        Func<Type[], Type> getType;
        var isAction = methodInfo.ReturnType.Equals(typeof(void));
        var types = methodInfo.GetParameters().Select(p => p.ParameterType);

        if (isAction)
        {
            getType = Expression.GetActionType;
        }
        else
        {
            getType = Expression.GetFuncType;
            types = types.Concat(new[] { methodInfo.ReturnType });
        }

        if (methodInfo.IsStatic)
        {
            return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
        }

        return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
    }
}
