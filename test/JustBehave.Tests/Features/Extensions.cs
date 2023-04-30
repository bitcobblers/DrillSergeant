﻿using System.Collections.Generic;
using System.Linq;

namespace JustBehave.Tests.Features;

public static class Extensions
{
    public static IEnumerable<object[]> ToObjectArray<T>(this IEnumerable<T> items) 
        => from i in items select new object[] { i };
}
