namespace JustBehave.Demo
{
    // Aliases.
    using SetterStep = ArrangeStep<CalculatorContext, CalculatorInput>;
    using OperatorStep = ActStep<CalculatorContext, CalculatorInput, int>;
    using CheckResultStep = AssertStep<CalculatorContext, CalculatorInput, int>;

    public class CalculatorBehaviors
    {
        public SetterStep SetFirstNumber =>
            new LambdaArrangeStep<CalculatorContext, CalculatorInput>()
                .Handle((c, i) => c with { A = i.A });

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
    }

    public record CalculatorContext(int A, int B, int Result);
    public record CalculatorInput(int A, int B, int Expected);
}