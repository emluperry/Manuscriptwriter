using System;
using MSW.Events;
using UnityEngine;

namespace MSW.Unity.Events
{
    [CreateAssetMenu(fileName = "SE_New", menuName = "Scriptable Objects/UnityMSWEvent")]
    public class UnityMSWEvent : ScriptableObject, IRunnerEvent
    {
        private EventHandler<IRunnerEventArgs> eventInstance;
        
        public void FireEvent(object sender, IRunnerEventArgs args)
        {
            eventInstance?.Invoke(sender, args);
        }

        public void RegisterEvent(EventHandler<IRunnerEventArgs> e)
        {
            this.eventInstance += e;
        }

        public void UnregisterEvent(EventHandler<IRunnerEventArgs> e)
        {
            this.eventInstance -= e;
        }

        public void ClearAllEvents()
        {
            foreach (var del in this.eventInstance.GetInvocationList())
            {
                this.eventInstance -= (EventHandler<IRunnerEventArgs>)del;
            }

            this.eventInstance = null;
        }
    }
}
