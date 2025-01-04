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
            
            this.playerInput.actions = inputActions.asset;
            this.playerInput.defaultActionMap = inputActions.UI.Get().name;
            
            this.playerInput.currentActionMap = inputActions.UI;

            foreach (var inputComponent in FindObjectsByType<InputSetup>(FindObjectsSortMode.None))
            {
                inputComponent.SetupInput(inputActions, SwitchActionMap);
                inputComponent.EnableInput();
            }
        }

        private void SwitchActionMap(string name)
        {
            if (inputActions == null)
            {
                return;
            }
            
            switch (name)
            {
                case "Player":
                    this.ChangeCurrentActionMap(inputActions.Player);
                    break;
                case "UI":
                    this.ChangeCurrentActionMap(inputActions.UI);
                    break;
            }
        }

        public void ChangeCurrentActionMap(InputActionMap newMap)
        {
            if (this.player == null)
            {
                return;
            }
            
            this.playerInput.currentActionMap?.Disable();
            this.playerInput.currentActionMap = newMap;
            this.playerInput.currentActionMap?.Enable();
        }

        private void OnDestroy()
        {
            this.inputActions.Disable();
        }
    }
}
