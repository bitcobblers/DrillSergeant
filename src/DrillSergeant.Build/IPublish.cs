using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;

// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable VariableHidesOuterVariable

namespace DrillSergeant.Build;

[PublicAPI]
public interface IPublish : IPack, ITest
{
    [Parameter] string NuGetSource => TryGetValue(() => NuGetSource) ?? "https://api.nuget.org/v3/index.json";
    [Secret, Parameter] string NuGetApiKey => TryGetValue(() => NuGetApiKey)!;

    Target Publish => _ => _
        .DependsOn(Test, Pack)
        .Requires(() => NuGetApiKey)
        .OnlyWhenDynamic(() => IsServerBuild)
        .Executes(() =>
        {
            Log.Information("Executing Publish");
            // DotNetTasks.DotNetNuGetPush(_ => _
            //         .Apply(PushSettings)
            //         .CombineWith(PushPackageFiles, (_, p) => _
            //             .SetTargetPath(p)),
            //     degreeOfParallelism: 1,
            //     completeOnFailure: true);
        });

    Configure<DotNetNuGetPushSettings> PushSettings => _ => _
        .SetSource(NuGetSource)
        .SetApiKey(NuGetApiKey);

    IEnumerable<AbsolutePath> PushPackageFiles => PackagesDirectory.GlobFiles("*.nupkg");
}
