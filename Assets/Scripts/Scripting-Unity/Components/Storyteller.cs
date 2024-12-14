using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Search;

using MSW.Compiler;
namespace MSW.Unity
{
    public class Storyteller : MonoBehaviour
    {
        //private void Awake()
        //{
        //    this.RegisterSceneObjects();
        //}

        //private void RegisterSceneObjects()
        //{
        //    // Get all Action components within the scene.
        //    var actionComponents = Object.FindObjectsByType<Actions>(FindObjectsSortMode.None);

        //    foreach (var actionComponent in actionComponents)
        //    {
        //        actionComponent.RunScript = this.RunScript;
        //    }
        //}

        //private void RunScript(string script)
        //{
            
        //}

        // DEBUG ONLY !!!
        [SerializeField]
        [SearchContext("ext:txt dir:Resources")] // QOL: Limit the files to ONLY project text files within Resources. 
        private TextAsset testScript;

        private Compiler.Compiler compiler;

        private void Start()
        {
            if(!testScript)
            {
                return;
            }

            compiler = new Compiler.Compiler()
            {
                ErrorLogger = LogError,
                FunctionLibrary = new List<object>() { new UnityDialogue() },
            };
            compiler.Compile(testScript.text);
        }

        private void LogError(string message)
        {
            Debug.LogError(message); 
        }
    }
}
