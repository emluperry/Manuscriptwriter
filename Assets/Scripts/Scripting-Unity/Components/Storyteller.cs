using System;
using UnityEngine;
using UnityEngine.Search;

using MSW.Compiler;
using Object = UnityEngine.Object;

namespace MSW.Unity
{
    public class Storyteller : MonoBehaviour
    {
        [SerializeField] private MSWUnityLibrary[] libraries;
        private Compiler.Compiler compiler;
        private Runner runner;
        
        private void Awake()
        {
            compiler = new Compiler.Compiler()
            {
                ErrorLogger = Logger,
                FunctionLibrary = libraries,
            };
            
            this.RegisterSceneObjects();
        }

        private void OnDestroy()
        {
            this.CleanupOnFinish();
        }

        private void RegisterSceneObjects()
        {
            // Get all Action components within the scene.
            var actionComponents = Object.FindObjectsByType<Actions>(FindObjectsSortMode.None);

            foreach (var actionComponent in actionComponents)
            {
                actionComponent.RunScript = this.RunScript;
            }
        }

        private void RunScript(string script)
        {
            var manuscript = compiler.Compile(script);

            runner = new Runner(manuscript)
            {
                Logger = Logger,
                OnFinish = CleanupOnFinish,
            };
            
            runner.Run();
        }
        
        private void CleanupOnFinish()
        {
            foreach (var library in this.libraries)
            {
                library.Cleanup();
            }
        }

        #region DEBUGGING

        [SerializeField]
        [SearchContext("ext:txt dir:Resources")] // QOL: Limit the files to ONLY project text files within Resources. 
        private TextAsset testScript;

        private void Start()
        {
            if(!testScript)
            {
                return;
            }
            
            this.RunScript(testScript.text);
        }

        private void Logger(string message)
        {
            Debug.LogError(message); 
        }

        #endregion
    }
}
