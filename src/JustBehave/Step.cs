using System;

namespace JustBehave
{
    public class Step : IDisposable
    {
        private bool isDisposed;

        ~Step()
        {
            this.Dispose(disposing: false);
        }

        public virtual string Name => this.GetType().FullName!;

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
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