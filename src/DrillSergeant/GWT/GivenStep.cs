namespace DrillSergeant.GWT;

public class GivenStep<TContext, TInput> : VerbStep<TContext, TInput>
{
    public GivenStep()
        : base("Given")
    {
    }

    public virtual void Given(TContext context, TInput input)
    {
    }
}