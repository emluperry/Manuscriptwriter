using MSW.Reflection;
using UnityEngine;

namespace MSW.Unity
{
    public class UnityDialogue
    {
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(string name, string line)
        {
            Debug.Log($"{name} says: {line}");
            return null;
        }
    }
}
