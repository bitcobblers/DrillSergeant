namespace JustBehave.Demo
{
    // Aliases.
    using SetterStep = ArrangeStep<CalculatorContext, CalculatorInput>;
    using OperatorStep = ActStep<CalculatorContext, CalculatorInput, int>;
    using CheckResultStep = AssertStep<CalculatorContext, CalculatorInput, int>;
    using CalculatorBehaviorBuilder = BehaviorBuilder<CalculatorContext, CalculatorInput>;
    using CalculatorBehavior = Behavior<CalculatorContext, CalculatorInput>;

    public class CalculatorBehaviors
    {
        public CalculatorBehavior Addition =>
            new CalculatorBehaviorBuilder(nameof(Addition))
                .WithInput(AdditionInputs)
                .Arrange("Set first number", (c, i) => c with { A = i.A })
                .Arrange(SetSecondNumber)
                .Act(AddNumbers)
                .Assert(CheckResult)
                .Build();

        public SetterStep SetSecondNumber =>
            new LambdaArrangeStep<CalculatorContext, CalculatorInput>()
                .Handle((c, i) => c with { B = i.B });

        public OperatorStep AddNumbers => OperatorStep.Lamda().Handle((c, _) => c.A + c.B);

        public CheckResultStep CheckResult =>
            new LambdaAssertStep<CalculatorContext, CalculatorInput, int>()
                .Handle((_, input, result) =>
                {
                    Console.WriteLine($"{input.Expected} == {result}: {input.Expected == result}");
                });

        public IEnumerable<CalculatorInput> AdditionInputs
        {
            get
            {
                yield return new CalculatorInput(A: 1, B: 2, Expected: 3);
                yield return new CalculatorInput(A: 2, B: 3, Expected: 5);
                yield return new CalculatorInput(A: 3, B: 4, Expected: 7);
                yield return new CalculatorInput(A: 4, B: 5, Expected: 9);
                yield return new CalculatorInput(A: 5, B: 6, Expected: 11);
            }
        }
    }

    public record CalculatorContext(int A, int B, int Result);
    public record CalculatorInput(int A, int B, int Expected);
}