using System.Text.RegularExpressions;

namespace DrillSergeant.Generators;

public static class VerbDefinitionParser
{
    private static readonly Regex ValidIdentifierPattern = new Regex(@"^[\w_]+[\w\d_]*$");

    public static IEnumerable<VerbGroup> Parse(string? content)
    {
        return from line in FilterLines(content)
               let verbGroup = ParseLine(line)
               where verbGroup != null
               select verbGroup;
    }

    internal static IEnumerable<string> FilterLines(string? content)
    {
        content = (content ?? string.Empty).Trim();

        return from line in Regex.Split(content, @"[\r\n]+")
               let trimmedLine = line.Trim()
               where trimmedLine.Length > 0
               where trimmedLine.StartsWith("#") == false
               select trimmedLine;
    }

    internal static VerbGroup? ParseLine(string line)
    {
        var parts = line.Split(':');

        if (parts.Length != 2)
        {
            return null;
        }

        var name = parts[0];
        var verbs = parts[1]
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToArray();

        if (ValidIdentifierPattern.IsMatch(name) == false ||
            verbs.Length == 0 ||
            verbs.Any(v => ValidIdentifierPattern.IsMatch(v) == false))
        {
            return null;
        }

        return new VerbGroup
        {
            Name = name,
            Verbs = verbs
        };
    }
}