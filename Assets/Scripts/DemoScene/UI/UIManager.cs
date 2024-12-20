using System;
using MSW.Unity.Dialogue;
using UnityEngine;

namespace Demo.UI
{
    public class UIManager : MonoBehaviour
    {
        private UnityDialogue dialogueFunctionality;

        private void Awake()
        {
            this.dialogueFunctionality = this.GetComponent<UnityDialogue>();
        }
    }

}