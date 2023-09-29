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
public interface IPublish : IPack
{
    [Parameter] string NuGetSource => TryGetValue(() => NuGetSource) ?? "https://api.nuget.org/v3/index.json";
    [Secret, Parameter] string NuGetApiKey => TryGetValue(() => NuGetApiKey)!;

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Requires(() => Configuration == Configuration.Release)
        .OnlyWhenDynamic(() => IsServerBuild)
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                    .SetSource(NuGetSource)
                    .EnableSkipDuplicate()
                    .SetApiKey(NuGetApiKey)
                    .CombineWith(PackageFiles, (_, p) => _
                        .SetTargetPath(p)),
                degreeOfParallelism: 1,
                completeOnFailure: true);
        });
    
    IEnumerable<AbsolutePath> PackageFiles => PackagesDirectory.GlobFiles("*.nupkg");
}
