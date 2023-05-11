using System;

namespace DrillSergeant.Core;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class BehaviorResolverSetupAttribute : Attribute
{
}