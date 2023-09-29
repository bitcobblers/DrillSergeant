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
public interface IPack : ITest
{
    AbsolutePath PackagesDirectory => ArtifactDirectory / "packages";

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetVersion(GitVersion.SemVer)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackagesDirectory)
                .SetNoBuild(SucceededTargets.Contains(Compile))
                .EnableNoLogo()
                .EnableNoRestore()
                .EnableContinuousIntegrationBuild());
            
            ReportSummary(_ => _
                .AddPair("Packed version", GitVersion.SemVer)
                .AddPair("Packages", PackagesDirectory.GlobFiles("*.nupkg").Count.ToString()));
        });
}