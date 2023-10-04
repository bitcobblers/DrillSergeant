using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable VariableHidesOuterVariable

namespace DrillSergeant.Build;

[PublicAPI]
public interface IPack : ITest, IHaveGitHubActions
{
    AbsolutePath PackagesDirectory => ArtifactDirectory / "packages";

    Target Pack => _ => _
        .DependsOn(Test)
        .OnlyWhenDynamic(() => IsTag)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            var version = IsPullRequest
                ? string.Join('.', GitVersion.SemVer.Split('.').Take(3).Union(BuildNumber))
                : GitVersion.SemVer;
            
            DotNetPack(_ => _
                .EnableNoLogo()
                .EnableNoRestore()
                .EnableNoBuild()
                .EnableIncludeSymbols()
                .SetProject(Solution)
                .SetVersion(version)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackagesDirectory));
            
            ReportSummary(_ => _
                .AddPair("Packed version", version)
                .AddPair("Packages", PackagesDirectory.GlobFiles("*.nupkg").Count.ToString()));
        });

    bool IsPullRequest => GitHubActions?.IsPullRequest ?? false;
    bool IsTag => (GitHubActions?.Ref ?? string.Empty).Contains("refs/tags", StringComparison.OrdinalIgnoreCase);
    string[] BuildNumber => GitHubActions is not null ? new[] { GitHubActions.RunNumber.ToString() } : Array.Empty<string>();
}