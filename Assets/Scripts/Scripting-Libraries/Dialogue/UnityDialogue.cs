using MSW.Reflection;
using UnityEngine;
using TMPro;

namespace MSW.Unity.Dialogue
{
    public class UnityDialogue : MSWUnityLibrary
    {
        [SerializeField] private TextMeshProUGUI speaker;
        [SerializeField] private TextMeshProUGUI dialogue;
        
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(string speaker, string line)
        {
            this.speaker.text = speaker;
            this.dialogue.text = line;
            
            Debug.Log($"{speaker} says: {line}");
            return null;
        }
    }
}
