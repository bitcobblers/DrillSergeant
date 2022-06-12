namespace JustBehave.Demo
{
    public class CalculatorBehaviors
    {
        public record Context(int A, int B, int Result);
        public record Input(int A, int B, int Expected);

        public Behavior<Context> Addition =>
            new BehaviorBuilder<Context>(nameof(Addition))
                .WithInput(AdditionInputs)
                .Arrange("Set first number", (c, i) => c with { A = i.A }) // Inline step declaration.
                .Arrange(SetSecondNumber)
                .Act(AddNumbers)
                .Assert<CheckResultStep>()
                .Build();

        public IEnumerable<Input> AdditionInputs
        {
            get
            {
                yield return new Input(A: 1, B: 2, Expected: 3);
                yield return new Input(A: 2, B: 3, Expected: 5);
                yield return new Input(A: 3, B: 4, Expected: 7);
                yield return new Input(A: 4, B: 5, Expected: 9);
                yield return new Input(A: 5, B: 6, Expected: 11);
            }
        }

        // Step implemented as a normal method.
        public Context SetSecondNumber(Context context, Input input) => context with { B = input.B };

        // Step implemented as a lambda for greater flexibility.
        public ActStep<Context, Input, int> AddNumbers => ActStep<Context, Input, int>.Lamda()
            .Named("Add numbers")
            .Handle((c, _) => c.A + c.B)
            .Teardown(() => Console.WriteLine("I do cleanup"));

        // Step implemented as type full customization and reusability.
        public class CheckResultStep : AssertStep<Context, Input, int>
        {
            public override void Assert(Context context, Input input, int result)
            {
                Console.WriteLine($"{input.Expected} == {result}: {input.Expected == result}");
            }
        }
    }
}