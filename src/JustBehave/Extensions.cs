using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JustBehave;

public static class Extensions
{
    public static IEnumerable<object[]> ToObjectArray<T>(this IEnumerable<T> items) =>
        from i in items select new object[] { i };

    internal static bool IsAsync(this MethodInfo method) => 
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;
}
