using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Demo.Input;
using Demo.Entity;

namespace Demo.Player
{
    public class PlayerMovement : MonoBehaviour, IInput, IControllerComponent
    {
        public Action<string> SwitchControlMap { get; set; }
        
        private Movement3D movementComponent;

        private InputAction moveInputAction;

        public void SetupController(ControllerBase controller)
        {

        }

        public void SetupPlayer(EntityInitialiser playerObject)
        {
            movementComponent = playerObject.GetComponent<Movement3D>();
        }

        private void OnDestroy()
        {
            DisableInput();
        }

        #region INPUT SETUP

        public void SetupInput(InputSystem_Actions inputs)
        {
            moveInputAction = inputs.Player.Move;
        }

        public void EnableInput()
        {
            if (moveInputAction != null)
            {
                moveInputAction.performed += Input_MovePerformed;
                moveInputAction.canceled += Input_MoveCancelled;
            }
        }

        public void DisableInput()
        {
            if (moveInputAction != null)
            {
                moveInputAction.performed -= Input_MovePerformed;
                moveInputAction.canceled -= Input_MoveCancelled;
            }
        }
        #endregion

        #region INPUTS
        private void Input_MovePerformed(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            var moveInput = new Vector2(input.x, input.y);

            movementComponent?.TriggerMovementCoroutine(moveInput);
        }

        private void Input_MoveCancelled(InputAction.CallbackContext ctx)
        {
            movementComponent?.TriggerCancelMovement();
        }
        #endregion
    }
}
