namespace JustBehave;

public class GivenStep<TContext, TInput> : VerbStep<TContext, TInput>
{
    public GivenStep()
        : base("Given")
    {
    }

    public virtual TContext Given(TContext context, TInput input) => context;
}