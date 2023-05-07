using System;
using System.Diagnostics.CodeAnalysis;

namespace JustBehave.Core;

[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class InjectAttribute : Attribute
{
}
