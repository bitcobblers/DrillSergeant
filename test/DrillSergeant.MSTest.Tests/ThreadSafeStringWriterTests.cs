using System.Globalization;

namespace DrillSergeant.MSTest.Tests;

[TestClass]
public class ThreadSafeStringWriterTests
{
    [TestMethod]
    public void ToStringReturnsCurrentContents()
    {
        // Arrange.
        var writer = new ThreadSafeStringWriter(CultureInfo.InvariantCulture, "ignored");

        // Act.
        writer.WriteLine("test");
        var result = writer.ToString();

        // Assert.
        result.ShouldStartWith("test"); // WriteLine() adds a newline.
    }

    [TestMethod]
    public void ToStringAndClearClearsWriter()
    {
        // Arrange.
        var writer = new ThreadSafeStringWriter(CultureInfo.InvariantCulture, "ignored");

        // Act.
        writer.WriteLine("test");
        var expectedText = writer.ToStringAndClear();
        var emptyText = writer.ToString();

        // Assert.
        expectedText.ShouldNotBeEmpty();
        emptyText.ShouldBeEmpty();
    }
}