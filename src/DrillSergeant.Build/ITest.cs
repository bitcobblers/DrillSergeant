using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable VariableHidesOuterVariable

namespace DrillSergeant.Build;

[PublicAPI]
public interface ITest : ICompile, IHaveArtifacts
{
    AbsolutePath TestResultsDirectory => ArtifactDirectory / "TestResults";

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultsDirectory / "*.trx")
        .Produces(TestResultsDirectory / "*.xml")
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetNoBuild(SucceededTargets.Contains(Compile))
                .SetResultsDirectory(TestResultsDirectory)
                .When(IsServerBuild, _ => _
                    .SetDataCollector("XPlat Code Coverage")
                    .AddLoggers("GitHubActions;report-warnings=false"))
                .ResetVerbosity());
        });
}
