using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
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
            DotNetTasks.DotNetTest(_ => _
                    .Apply(TestSettings)
                    .CombineWith(TestProjects, (_, p) => _
                        .Apply(TestProjectSettings, p)),
                completeOnFailure: true);
        });
    
    Configure<DotNetTestSettings> TestSettings => BaseTestSettings;

    Configure<DotNetTestSettings, Project> TestProjectSettings => BaseTestProjectSettings;
    
    IEnumerable<Project> TestProjects =>
        from p in Solution.GetAllProjects("*")
        where p.Name.EndsWith(".Tests")
        select p;
    
    sealed Configure<DotNetTestSettings> BaseTestSettings => _ => _
        .SetConfiguration(Configuration)
        .SetNoBuild(SucceededTargets.Contains(Compile))
        .SetResultsDirectory(TestResultsDirectory)
        .When(IsServerBuild, _ => _.SetDataCollector("XPlat Code Coverage"))
        .ResetVerbosity();

    sealed Configure<DotNetTestSettings, Project> BaseTestProjectSettings => (_, p) => _
        .SetProjectFile(p)
        .When(GitHubActions.Instance is not null && p.HasPackageReference("GitHubActionsTestLogger"), _ => _
            .AddLoggers("GitHubActions;report-warnings=false"))
        .AddLoggers($"trx;LogFileName={p.Name}.trx");
}
