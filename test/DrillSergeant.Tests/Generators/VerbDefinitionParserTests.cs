using DrillSergeant.Generators;
using Shouldly;
using System.Linq;
using Xunit;

namespace DrillSergeant.Tests.Generators;

public class VerbDefinitionParserTests
{
    public class FilterLinesMethod : VerbDefinitionParserTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NullOrEmptyInputContentReturnsEmptyArray(string content)
        {
            // Act.
            var result = VerbDefinitionParser.FilterLines(content).ToArray();

            // Assert.
            result.ShouldBeEmpty();
        }

        [Fact]
        public void CommentsAreStripped()
        {
            // Arrange.
            var content = "# This is a comment";

            // Act.
            var result = VerbDefinitionParser.FilterLines(content).ToArray();

            // Assert.
            result.ShouldBeEmpty();
        }

        [Fact]
        public void EmptyLinesAreStripped()
        {
            // Arrange.
            var content = @"
A

B";

            // Act.
            var result = VerbDefinitionParser.FilterLines(content).ToArray();

            // Assert.
            result.ShouldBe(new[] { "A", "B" });
        }

        [Theory]
        [InlineData("A\rB")]
        [InlineData("A\r\nB")]
        [InlineData("A\nB")]
        [InlineData("A\r\n\nB")]
        public void VariousNewlineEncodingsSupported(string content)
        {
            // Act.
            var result = VerbDefinitionParser.FilterLines(content);

            // Assert.
            result.ShouldBe(new[] { "A", "B" });
        }
    }

    public class ParseLineMethod : VerbDefinitionParserTests
    {
        [Fact]
        public void ParsesValidLine()
        {
            // Arrange.
            var line = "GWT:Given,When,Then";
            var expected = new VerbGroup
            {
                Name = "GWT",
                Verbs = new[] { "Given", "When", "Then" }
            };

            // Act.
            var result = VerbDefinitionParser.ParseLine(line);

            // Assert.
            result.ShouldBe(expected);
        }

        [Fact]
        public void WhitespaceIsTrimmedFromVerbs()
        {
            // Arrange.
            var line = "GWT:Given , When , Then";
            var expected = new VerbGroup
            {
                Name = "GWT",
                Verbs = new[] { "Given", "When", "Then" }
            };

            // Act.
            var result = VerbDefinitionParser.ParseLine(line);

            // Assert.
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData("no_divider")]
        [InlineData("no_verbs:")]
        [InlineData("b@d_name:verb")]
        [InlineData("bad_verbs:v!rb")]
        public void MalformedLineReturnsNull(string line)
        {
            // Act.
            var result = VerbDefinitionParser.ParseLine(line);

            // Assert.
            result.ShouldBeNull();
        }
    }

    [Fact]
    public void SuccessfullyParsesVerbDefinition()
    {
        // Arrange.
        var content = @"
# This is a comment

GWT:Given,When,Then
AAA:Arrange,Act,Assert";

        var expected = new[]
        {
            new VerbGroup
            {
                Name = "GWT",
                Verbs = new[] { "Given", "When", "Then" }
            },
            new VerbGroup
            {
                Name = "AAA",
                Verbs = new[] { "Arrange", "Act", "Assert" }
            }
        };

        // Act.
        var result = VerbDefinitionParser.Parse(content).ToArray();

        // Assert.
        result.ShouldBe(expected, ignoreOrder: true);
    }
}
