using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrillSergeant.MSTest;

internal class TestMethodOptions
{
    public int Timeout { get; set; }
    public TestContext? TestContext { get; set; }
    public bool CaptureDebugTraces { get; set; }
}