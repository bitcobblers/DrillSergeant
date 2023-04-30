using System;

namespace JustBehave.Core;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class InjectAttribute : Attribute
{
}
