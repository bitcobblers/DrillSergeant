namespace JustBehave.Demo
{
    public class CalculatorBehaviors
    {
        public Behavior<CalculatorContext> Addition =>
            new BehaviorBuilder<CalculatorContext>(nameof(Addition))
                .WithInput(AdditionInputs)
                .Arrange("Set first number", (c, i) => c with { A = i.A })
                .Arrange(SetSecondNumber)
                .Act(AddNumbers)
                .Assert(CheckResult)
                .Build();

        public CalculatorContext SetSecondNumber(CalculatorContext context, CalculatorInput input) => context with { B = input.B };

        public int AddNumbers(CalculatorContext context, CalculatorInput input) => context.A + context.B;

        public void CheckResult(CalculatorContext context, CalculatorInput input, int result)
        {
            Console.WriteLine($"{input.Expected} == {result}: {input.Expected == result}");
        }

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