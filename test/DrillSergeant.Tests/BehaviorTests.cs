using Shouldly;
using Xunit;

namespace DrillSergeant.Tests;

public class BehaviorTests
{
    public class DisposeMethod : BehaviorTests
    {
        [Fact]
        public void DisposesStepsInBehavior()
        {
            // Arrange.
            var step = new StepWithDispose();
            var behavior = new Behavior()
                .AddStep(step);

            // Act.
            behavior.Dispose();

            // Assert.
            step.DisposeCalled.ShouldBeTrue();
        }

        [Fact]
        public void OwnedObjectsAreDisposedWithBehavior()
        {
            // Arrange.
            var obj = new StepWithDispose();
            var behavior =
                new Behavior()
                    .Owns(obj);

            // Act.
            behavior.Dispose();

            // Assert.
            obj.DisposeCalled.ShouldBeTrue();
        }

        [Fact]
        public void BehaviorTakesOwnershipOfBackgroundObjects()
        {
            // Arrange.
            var obj = new StepWithDispose();
            var background =
                new Behavior()
                    .Owns(obj);

            var behavior =
                new Behavior()
                    .Background(background);

            // Act.
            behavior.Dispose();

            // Assert.
            obj.DisposeCalled.ShouldBeTrue();
        }

        private class StepWithDispose : VerbStep
        {
            public bool DisposeCalled { get; private set; }

            public void Test()
            {
            }

            public override string Verb => "Test";

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                DisposeCalled = true;
            }
        }
    }
}
