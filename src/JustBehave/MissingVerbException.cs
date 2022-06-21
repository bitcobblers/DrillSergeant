using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace JustBehave
{
    [Serializable, ExcludeFromCodeCoverage]
    public class MissingVerbException : Exception, ISerializable
    {
        public MissingVerbException(string verb) : base($"Could not find any implementation for the verb ${verb}.")
        {
            this.Verb = verb;
        }

        protected MissingVerbException(SerializationInfo info, StreamingContext context)
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