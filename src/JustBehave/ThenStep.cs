namespace JustBehave;

public class ThenStep<TContext, TInput> : VerbStep<TContext, TInput>
{
    public ThenStep()
        : base("Then")
    {
    }

    public virtual void Then(TContext context, TInput input) { }
}
