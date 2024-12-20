using System;

namespace MSW.Reflection
{
    public class Context
    {
        internal RunnerEvent pauseEvent = null;

        public void WaitForEvent(RunnerEvent eventHandler)
        {
            this.pauseEvent = eventHandler;
        }
    }
}