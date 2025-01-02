using System;
using System.Collections.Generic;
using Demo.Input;
using MSW.Events;
using MSW.Unity;
using MSW.Unity.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.Player
{
    public class PlayerInteraction : MonoBehaviour, IInput
    {
        private InputAction testInteraction;
        [SerializeField] private UnityMSWEvent interactionEvent;

        public Action<string> SwitchControlMap { get; set; }

        public void SetupInput(InputSystem_Actions inputs)
        {
            testInteraction = inputs.Player.Interact;
        }

        public void EnableInput()
        {
            testInteraction.performed += TestInteractionFunc;
        }

        private void TestInteractionFunc(InputAction.CallbackContext obj)
        {
            this.interactionEvent.FireEvent(this, new RunnerEventArgs(new List<object>() {"the player", "me"}));
        }

        public void DisableInput()
        {
            testInteraction.performed -= TestInteractionFunc;
        }
    }
}
