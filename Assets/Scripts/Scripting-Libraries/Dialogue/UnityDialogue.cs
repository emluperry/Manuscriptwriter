using System;
using System.Collections.Generic;
using Demo.Input;
using MSW.Events;
using MSW.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using MSW.Unity.Events;

namespace MSW.Unity.Dialogue
{
    public class UnityDialogue : MSWUnityLibrary, IInput
    {
        [SerializeField] private TextMeshProUGUI speakerTextBox;
        [SerializeField] private TextMeshProUGUI dialogueTextBox;

        private Canvas canvas;

        #region MSW Events

        private InputAction continueInput;
        private RunnerEvent continueAction;
        
        [MSWEvent("{0} speaks with {1}")]
        public UnityMSWEvent interactionEvent;

        #endregion

        private void Awake()
        {
            this.canvas = this.GetComponentInChildren<Canvas>(true);
        }

        #region MSW Functions
        
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(Context context, string speaker, string line)
        {
            if (!canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(true);
                this.SwitchControlMap?.Invoke("UI");
            }
            
            this.speakerTextBox.text = speaker;
            this.dialogueTextBox.text = line;
            
            Debug.Log($"{speaker} says: {line}");
            
            context.WaitForEvent(continueAction);
            return null;
        }

        #endregion

        #region INPUTS

        public Action<string> SwitchControlMap { get; set; }

        public void SetupInput(InputSystem_Actions inputs)
        {
            this.continueInput = inputs.UI.Submit;
            
            this.continueAction = new RunnerEvent();
        }

        public void EnableInput()
        {
            this.continueInput.performed += HandleInput_Submit;
        }

        public void DisableInput()
        {
            this.continueInput.performed -= HandleInput_Submit;
        }
        
        private void HandleInput_Submit(InputAction.CallbackContext obj)
        {
            this.continueAction.FireEvent(this, new RunnerEventArgs(new List<object>() {obj.action.name}));
        }

        #endregion

        public override void Cleanup()
        {
            if (canvas != null)
            {
                canvas?.gameObject?.SetActive(false);
                this.SwitchControlMap?.Invoke("Player");
            }
        }
    }
}
