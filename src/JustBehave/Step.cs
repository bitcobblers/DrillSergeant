using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JustBehave
{
    public class Step : IDisposable
    {
        public record VerbMethod(MethodInfo Method, bool IsAsync);

        private bool isDisposed;

        protected Step(string verb)
            : this(verb, null)
        {
        }

        protected Step(string verb, string? name)
        {
            this.Verb = verb;
            this.Name = string.IsNullOrWhiteSpace(name) ? this.GetType().Name! : name.Trim();
        }

        ~Step()
        {
            this.Dispose(disposing: false);
        }

        public string Verb { get; }
        public virtual string Name { get; set; }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual object? Execute(IDependencyResolver resolver)
        {
            var handler = this.PickHandler();
            var parameters = this.ResolveParameters(resolver, handler.Method.GetParameters());

            if (handler.IsAsync)
            {
                var taskType = handler.Method.ReturnType;
                var task = handler.Method.Invoke(this, parameters)!;

                taskType.GetMethod("Wait", Array.Empty<Type>())?.Invoke(task, null);
                
                if(taskType.GenericTypeArguments.Length>0)
                {
                    return taskType.GetProperty("Result")?.GetValue(task, null);
                }

                return null;
            }

            return handler.Method.Invoke(this, parameters);
        }

        internal VerbMethod PickHandler()
        {
            static int isAsync(Type t) => t.Name == typeof(Task).Name || t.Name == typeof(Task<>).Name ? 1 : 0;

            var candidates = from m in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                             where m.Name == this.Verb || m.Name == this.Verb + "Async"
                             let numParameters = m.GetParameters().Length
                             let returnsTask = isAsync(m.ReturnType)
                             orderby numParameters descending, returnsTask descending
                             select new VerbMethod(m, returnsTask == 1);

            if(candidates.Any() == false)
            {
                throw new MissingVerbException(this.Verb);
            }

            return candidates.First();
        }

        private object?[] ResolveParameters(IDependencyResolver resolver, ParameterInfo[] parameters)
        {
            return (from p in parameters
                    select resolver.Resolve(p.ParameterType)).ToArray();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.isDisposed)
            {
                this.Teardown();
            }

            this.isDisposed = true;
        }

        protected virtual void Teardown()
        {
        }
    }
}