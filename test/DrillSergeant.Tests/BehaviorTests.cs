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
