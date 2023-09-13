using System.Collections.Generic;
using System.Linq;
using DrillSergeant.Build;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;

// ReSharper disable once CheckNamespace
[GitHubActions(
    "ci",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    ImportSecrets = new[] { nameof(IReportCoverage.CodecovToken) },
    On = new[] { GitHubActionsTrigger.WorkflowDispatch, GitHubActionsTrigger.Push, GitHubActionsTrigger.PullRequest },
    InvokedTargets = new[] { nameof(ITest.Test), nameof(IReportCoverage.ReportCoverage) })]
[GitHubActions(
    "publish",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    On = new[] { GitHubActionsTrigger.WorkflowDispatch },
    InvokedTargets = new[] { nameof(IPack.Pack) })]
class Build : NukeBuild, IPublish, IReportCoverage
{
    public static int Main() => Execute<Build>();

    // ---

    [CI] readonly GitHubActions GitHubActions;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    Nuke.Common.ProjectModel.Solution IHaveSolution.Solution => Solution;

    public IEnumerable<Project> TestProjects =>
        from p in Solution.GetAllProjects("*")
        where p.Name.Contains(".Tests") && !p.Name.EndsWith(".Shared")
        select p;
}
