using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace DrillSergeant.Build;

[PublicAPI]
public interface ICompile : IRestore, IHaveConfiguration
{
    Target Compile => _ => _
        .DependsOn(Restore)
        .WhenSkipped(DependencyBehavior.Skip)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .Apply(CompileSettings));
        });

    Configure<DotNetBuildSettings> CompileSettings => _ => _
        .SetProjectFile(Solution)
        .SetConfiguration(Configuration)
        .When(IsServerBuild, _ => _
            .EnableContinuousIntegrationBuild())
        .SetNoRestore(SucceededTargets.Contains(Restore));

    IEnumerable<(Project Project, string Framework)> PublishConfigurations =>
        Array.Empty<(Project Project, string Framework)>();
}