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

    public class SetInputMethod : BehaviorTests
    {
        [Fact]
        public void BehaviorIsInitiallySetToEmptyDictionary()
        {
            // Arrange.
            var behavior = new Behavior();

            // Act.
            var input = behavior.Input;

            // Assert.
            input.ShouldBeEmpty();
        }

        [Fact]
        public void SettingInputOverwritesExistingProperties()
        {
            // Arrange.
            var input1 = new { A = "error" };
            var input2 = new { A = "expected" };
            var behavior = new Behavior(input1);

            // Act.
            behavior.SetInput(input2);

            // Assert.
            behavior.Input["A"].ShouldBe("expected");
        }

        [Fact]
        public void SettingInputRemovesOldProperties()
        {
            // Arrange.
            var input1 = new { A = "error" };
            var input2 = new { B = "ignored" };
            var behavior = new Behavior(input1);

            // Act.
            behavior.SetInput(input2);

            // Assert.
            behavior.Input.ShouldNotContainKey("A");
        }
    }

    public class FreezeMethod : BehaviorTests
    {
        [Fact]
        public void AttemptingToAddStepToFrozenBehaviorThrowsBehaviorFrozenException()
        {
            // Arrange.
            using var behavior = new Behavior();

            // Act.
            behavior.Freeze();

            // Assert.
            Assert.Throws<BehaviorFrozenException>(
                () => behavior.AddStep(new StubStep()));
        }

        [Fact]
        public void AttemptingToSetInputToFrozenBehaviorThrowsBehaviorFrozenException_Object()
        {
            // Arrange.
            using var behavior = new Behavior();

            // Act.
            behavior.Freeze();

            // Assert.
            Assert.Throws<BehaviorFrozenException>(
                () => behavior.SetInput((object?)null));
        }

        [Fact]
        public void AttemptingToSetInputToFrozenBehaviorThrowsBehaviorFrozenException_Dictionary()
        {
            // Arrange.
            using var behavior = new Behavior();

            // Act.
            behavior.Freeze();

            // Assert.
            Assert.Throws<BehaviorFrozenException>(
                () => behavior.SetInput(null));
        }

        [Fact]
        public void AttemptingToAddBackgroundToFrozenBehaviorThrowsBehaviorFrozenException()
        {
            // Arrange.
            using var behavior = new Behavior();

            // Act.
            behavior.Freeze();

            // Assert.
            Assert.Throws<BehaviorFrozenException>(
                () => behavior.Background(new Behavior()));
        }

        [Fact]
        public void AttemptingToEnableContextLoggingToFrozenBehaviorThrowsBehaviorFrozenException()
        {
            // Arrange.
            using var behavior = new Behavior();

            // Act.
            behavior.Freeze();

            // Assert.
            Assert.Throws<BehaviorFrozenException>(
                () => behavior.EnableContextLogging());
        }

        private class StubStep : VerbStep { }
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
