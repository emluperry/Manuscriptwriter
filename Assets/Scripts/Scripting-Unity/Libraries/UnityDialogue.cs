using MSW.Reflection;
using UnityEngine;

namespace MSW.Unity
{
    [CreateAssetMenu(fileName = "UnityDialogue-Instance", menuName = "Manuscriptwriter/Libraries/UnityDialogue Instance")]
    public class UnityDialogue : UnityLibraryScriptableObject
    {
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(string speaker, string line)
        {
            Debug.Log($"{speaker} says: {line}");
            return null;
        }
    }
}
