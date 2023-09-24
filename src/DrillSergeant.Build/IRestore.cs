using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable VariableHidesOuterVariable

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

    Configure<DotNetRestoreSettings> RestoreSettings => BaseRestoreSettings;
    
    sealed Configure<DotNetRestoreSettings> BaseRestoreSettings => _ => _
        .SetProjectFile(Solution);
}
