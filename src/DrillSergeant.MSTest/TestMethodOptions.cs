using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrillSergeant.MSTest;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
internal class TestMethodOptions
{
    public int Timeout { get; set; }
    public TestContext? TestContext { get; set; }
    public bool CaptureDebugTraces { get; set; }
}