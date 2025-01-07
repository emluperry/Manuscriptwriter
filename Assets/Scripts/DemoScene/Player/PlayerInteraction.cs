using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Demo.Input;
using Demo.Entity;
using Demo.Interaction;
using MSW.Events;
using MSW.Unity;
using MSW.Unity.Events;

namespace Demo.Player
{
    public class PlayerInteraction : MonoBehaviour, IInput
    {
        private InputAction testInteraction;
        
        private CollisionHandler collisionHandler;
        private InteractableObject target;

        public Action<string> SwitchControlMap { get; set; }

        private void Awake()
        {
            this.collisionHandler = this.GetComponentInChildren<CollisionHandler>();
            
            this.collisionHandler.TriggerEnter += HandleTriggerEnter;
            this.collisionHandler.TriggerExit += HandleTriggerExit;
        }

        private void OnDestroy()
        {
            this.collisionHandler.TriggerEnter -= HandleTriggerEnter;
            this.collisionHandler.TriggerExit -= HandleTriggerExit;
        }

        private void HandleTriggerEnter(Collider obj)
        {
            var interactable = obj.GetComponent<InteractableObject>();
            if (interactable)
            {
                this.target?.StopOverlap();
                this.target = interactable;
                this.target.OnOverlap();
            }
        }
        
        private void HandleTriggerExit(Collider obj)
        {
            var interactable = obj.GetComponent<InteractableObject>();
            if (interactable == this.target)
            {
                this.target?.StopOverlap();
                this.target = null;
            }
        }

        public void SetupInput(InputSystem_Actions inputs)
        {
            testInteraction = inputs.Player.Interact;
        }

        public void EnableInput()
        {
            testInteraction.performed += HandleInteraction;
        }
        
        public void DisableInput()
        {
            testInteraction.performed -= HandleInteraction;
        }

        private void HandleInteraction(InputAction.CallbackContext obj)
        {
            //this.interactionEvent.FireEvent(this, new RunnerEventArgs(new List<object>() {"the player", "me"}));
            this.target.StartInteract("the player");
        }
    }
}
