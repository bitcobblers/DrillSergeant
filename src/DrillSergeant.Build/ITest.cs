using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;

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
            DotNetTasks.DotNetTest(_ => _
                    .Apply(TestSettings)
                    .CombineWith(TestProjects, (_, p) => _
                        .Apply(TestProjectSettings, p)),
                completeOnFailure: true);
        });

    Configure<DotNetTestSettings> TestSettings => _ => _
        .SetConfiguration(Configuration)
        .SetNoBuild(SucceededTargets.Contains(Compile))
        .ResetVerbosity()
        .SetResultsDirectory(TestResultsDirectory);

    Configure<DotNetTestSettings, Project> TestProjectSettings => (_, p) => _
        .SetProjectFile(p)
        .When(GitHubActions.Instance is not null && p.HasPackageReference("GitHubActionsTestLogger"), _ => _
            .AddLoggers("GitHubActions;report-warnings=false"))
        .AddLoggers($"trx;LogFileName={p.Name}.trx")
        .When(InvokedTargets.Contains((this as IReportCoverage)?.ReportCoverage) || IsServerBuild, _ => _
            .SetCoverletOutput(TestResultsDirectory / $"{p.Name}.xml"));

    IEnumerable<Project> TestProjects { get; }
}