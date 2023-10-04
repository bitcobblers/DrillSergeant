using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Nuke.Common.Tooling;

// ReSharper disable InconsistentNaming

namespace DrillSergeant.Build;

[PublicAPI]
[TypeConverter(typeof(TypeConverter<Configuration>))]
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public class Configuration : Enumeration
{
    public static Configuration Debug = new() { Value = nameof(Debug) };
    public static Configuration Release = new() { Value = nameof(Release) };

    public static implicit operator string(Configuration configuration) => configuration.Value;
}