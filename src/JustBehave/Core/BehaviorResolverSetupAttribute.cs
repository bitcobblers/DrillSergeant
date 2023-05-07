using System;

namespace JustBehave.Core;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class BehaviorResolverSetupAttribute : Attribute
{
}