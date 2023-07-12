using Shouldly;
using Xunit;

namespace DrillSergeant.Tests;

public class BehaviorTests
{
    public class AddStepMethod : BehaviorTests
    {
        [Fact]
        public void AddingNullStepDoesNotChangeStepCollection()
        {
            // Arrange.
            var behavior = new Behavior();

            // Act.
            behavior.AddStep(null);

            // Assert.
            behavior.ShouldBeEmpty();
        }
    }

    public class BackgroundMethod : BehaviorTests
    {
        [Fact]
        public void AddingNullBackgroundDoesNotChangeStepCollection()
        {
            // Arrange.
            var behavior = new Behavior();

            // Act.
            behavior.Background(null);

            // Assert.
            behavior.ShouldBeEmpty();
        }
    }

    public class OwnsMethod : BehaviorTests
    {
        [Fact]
        public void AddingNullOwnershipDoesNotUpdateDisposablesCollection()
        {
            // Arrange.
            var behavior = new Behavior();

            // Act.
            behavior.Owns(null);

            // Assert.
            behavior.OwnedDisposables.ShouldBeEmpty();
        }
    }


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
