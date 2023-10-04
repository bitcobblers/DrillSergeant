using DrillSergeant.Build;
using Nuke.Common;

// ReSharper disable once CheckNamespace
// ReSharper disable once RedundantExtendsListEntry
class Build : NukeBuild, ITest, IPublish
{
    public static int Main() => Execute<Build>();
}
