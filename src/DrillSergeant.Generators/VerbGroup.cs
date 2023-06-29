using System.Diagnostics;

namespace DrillSergeant.Generators;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VerbGroup : IEquatable<VerbGroup>
{
    public string Name { get; set; } = string.Empty;
    public string[] Verbs { get; set; } = Array.Empty<string>();

    private string DebuggerDisplay => $"{Name}: {string.Join(",", Verbs)}";

    public bool Equals(VerbGroup other)
    {
        return
            Name == other.Name &&
            Verbs.Length == other.Verbs.Length &&
            Verbs
                .Select((v, i) => (v, i))
                .All(x => x.v == other.Verbs[x.i]);
    }
}