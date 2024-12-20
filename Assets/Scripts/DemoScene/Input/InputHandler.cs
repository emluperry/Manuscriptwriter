using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.Input
{
    public class InputHandler : MonoBehaviour
    {
        private InputSystem_Actions inputActions;
        private PlayerInput playerInput;
        [SerializeField] private GameObject player;
        
        void Awake()
        {
            inputActions = new InputSystem_Actions();
            this.playerInput = this.GetComponent<PlayerInput>();
            
            this.playerInput.currentActionMap = inputActions.UI;
            this.playerInput.defaultActionMap = inputActions.UI.Get().name;
            this.playerInput.actions = inputActions.asset;

            foreach (var inputComponent in FindObjectsByType<InputSetup>(FindObjectsSortMode.None))
            {
                inputComponent.SetupInput(inputActions);
                inputComponent.EnableInput();
            }
        }

        private void OnDestroy()
        {
            this.inputActions.Disable();
        }
    }
}
