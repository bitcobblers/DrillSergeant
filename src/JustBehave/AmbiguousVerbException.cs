using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace JustBehave
{
    [Serializable, ExcludeFromCodeCoverage]
    public class AmbiguousVerbException : Exception, ISerializable
    {
        public AmbiguousVerbException(string verb) : base($"Cannot pick an implementation for the verb ${verb}.  Two or more candidates have the same number of parameters.")
        {
            this.Verb = verb;
        }

        protected AmbiguousVerbException(SerializationInfo info, StreamingContext context)
        {
            this.Verb = info.GetString(nameof(this.Verb))!;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.Verb), this.Verb);
        }

        public string Verb { get; set; }
    }
}