using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaGivenStep<TContext, TInput> : LambdaStep<TContext, TInput>
{
    public LambdaGivenStep()
        : base("Given")
    {
    }
}