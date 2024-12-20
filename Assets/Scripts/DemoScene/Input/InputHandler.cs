using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.Input
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerInput playerInput;
        [SerializeField] private GameObject player;
        
        void Awake()
        {
            InputSystem_Actions inputActions = new InputSystem_Actions();
            this.playerInput = this.GetComponent<PlayerInput>();

            this.playerInput.defaultActionMap = inputActions.UI.Get().name;
            this.playerInput.actions = inputActions.asset;

            foreach (var inputComponent in FindObjectsByType<InputSetup>(FindObjectsSortMode.None))
            {
                inputComponent.SetupInput(inputActions);
                inputComponent.EnableInput();
            }
        }
    }
}
