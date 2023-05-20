namespace DrillSergeant.GWT;

public class WhenStep<TContext, TInput> : VerbStep<TContext, TInput>
{
    public WhenStep()
        : base("When")
    {
    }

    public virtual void When(TContext context, TInput input)
    {
    }
}