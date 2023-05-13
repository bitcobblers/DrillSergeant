namespace DrillSergeant.GWT;

public class WhenStep<TContext, TInput> : VerbStep<TContext, TInput>
{
    public WhenStep()
        : base("When")
    {
    }

    public virtual TContext When(TContext context, TInput input) => context;
}