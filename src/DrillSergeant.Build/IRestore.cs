using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IRestore : IHaveSolution
{
    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .Apply(RestoreSettings));
        });

    Configure<DotNetRestoreSettings> RestoreSettings => _ => _
        .SetProjectFile(Solution);
}