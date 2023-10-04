using DrillSergeant.Generators;

namespace DrillSergeant.Tests.Generators;

public class VerbGroupTests
{
    public class EqualsMethod : VerbGroupTests
    {
        [Fact]
        public void DifferentNameFails()
        {
            // Arrange.
            var group1 = new VerbGroup
            {
                Name = "Group1"
            };

            var group2 = new VerbGroup
            {
                Name = "Group2"
            };

            // Act.
            var result = group1.Equals(group2);

            // Assert.
            result.ShouldBeFalse();
        }

        [Fact]
        public void DifferentVerbsFails()
        {
            // Arrange.
            var group1 = new VerbGroup
            {
                Name = "Group",
                Verbs = new[] { "Verb1" }
            };

            var group2 = new VerbGroup
            {
                Name = "Group",
                Verbs = new[] { "Verb2" }
            };

            // Act.
            var result = group1.Equals(group2);

            // Assert.
            result.ShouldBeFalse();
        }

        [Fact]
        public void DifferentNumberOfVerbsFails()
        {
            // Arrange.
            var group1 = new VerbGroup
            {
                Name = "Group",
                Verbs = new[] { "Verb1" }
            };

            var group2 = new VerbGroup
            {
                Name = "Group",
                Verbs = new[] { "Verb1", "Verb2" }
            };

            // Act.
            var result = group1.Equals(group2);

            // Assert.
            result.ShouldBeFalse();
        }

        [Fact]
        public void MatchingGroupPasses()
        {
            // Arrange.
            var group1 = new VerbGroup
            {
                Name = "Group",
                Verbs = new[] { "Verb1" }
            };

            var group2 = new VerbGroup
            {
                Name = "Group",
                Verbs = new[] { "Verb1" }
            };

            // Act.
            var result = group1.Equals(group2);

            // Assert.
            result.ShouldBeTrue();
        }
    }
}