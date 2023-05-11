namespace DrillSergeant;

public class LambdaThenStep<TContext, TInput> : LambdaStep<TContext, TInput>
{
    public LambdaThenStep()
        : base("Then")
    {
    }
}