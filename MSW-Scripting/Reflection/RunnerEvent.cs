using System;

namespace MSW.Reflection
{
    public class RunnerEvent
    {
        private EventHandler<RunnerEventArgs> runtimeEvent;

        public virtual void FireEvent(RunnerEventArgs args)
        {
            runtimeEvent?.Invoke(this, args);
        }

        public virtual void RegisterEvent(EventHandler<RunnerEventArgs> e)
        {
            runtimeEvent += e;
        }

        public virtual void UnregisterEvent(EventHandler<RunnerEventArgs> e)
        {
            runtimeEvent -= e;
        }

        public virtual void ClearAllEvents()
        {
            var invokes = this.runtimeEvent.GetInvocationList();
            foreach (var invoke in invokes)
            {
                this.runtimeEvent -= (EventHandler<RunnerEventArgs>)invoke;
            }
        }
    }
}