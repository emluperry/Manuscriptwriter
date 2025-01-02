using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.Input
{
    public class InputSetup : MonoBehaviour
    {
        private IEnumerable<IInput> inputComponents;
        
        public virtual void SetupInput(InputSystem_Actions inputs, Action<string> switchActionMap)
        {
            inputComponents = this.GetComponents<IInput>();
            foreach (var inputComp in inputComponents)
            {
                inputComp.SwitchControlMap += switchActionMap;
                inputComp.SetupInput(inputs);
            }
        }

        public virtual void DestroyInput(Action<string> switchActionMap)
        {
            this.DisableInput();
            foreach (var inputComp in inputComponents)
            {
                inputComp.SwitchControlMap -= switchActionMap;
            }
        }

        public virtual void EnableInput()
        {
            foreach (var inputComp in inputComponents)
            {
                inputComp.EnableInput();
            }
        }

        public virtual void DisableInput()
        {
            foreach (var inputComp in inputComponents)
            {
                inputComp.DisableInput();
            }
        }
    }
}
