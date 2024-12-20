using System;

namespace MSW.Reflection
{
    public class RunnerEvent
    {
        private Action runtimeEvent;

        public void FireEvent()
        {
            runtimeEvent?.Invoke();
        }

        public void RegisterEvent(Action e)
        {
            runtimeEvent += e;
        }

        public void UnregisterEvent(Action e)
        {
            runtimeEvent -= e;
        }
    }
}