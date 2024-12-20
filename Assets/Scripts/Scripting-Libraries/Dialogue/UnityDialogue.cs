using Demo.Input;
using MSW.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace MSW.Unity.Dialogue
{
    public class UnityDialogue : MSWUnityLibrary, IInput
    {
        [SerializeField] private TextMeshProUGUI speaker;
        [SerializeField] private TextMeshProUGUI dialogue;

        private Canvas canvas;

        private InputAction continueInput;
        private RunnerEvent continueAction;

        private void Start()
        {
            this.canvas = this.GetComponentInChildren<Canvas>(true);
        }

        [MSWFunction("{0}: {1}")]
        public object RunDialogue(Context context, string speaker, string line)
        {
            canvas.gameObject.SetActive(true);
            
            this.speaker.text = speaker;
            this.dialogue.text = line;
            
            Debug.Log($"{speaker} says: {line}");
            
            context.WaitForEvent(continueAction);
            return null;
        }

        #region INPUTS

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
            this.continueAction.FireEvent();
        }

        #endregion

        public override void Cleanup()
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
