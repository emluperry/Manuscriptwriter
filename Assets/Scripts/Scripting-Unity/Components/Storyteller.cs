using UnityEngine;
using UnityEngine.Search;

using MSW.Compiler;
namespace MSW.Unity
{
    public class Storyteller : MonoBehaviour
    {
        [SerializeField] private MSWUnityLibrary[] libraries;
        private Compiler.Compiler compiler;
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

        // DEBUG !!!
        [SerializeField]
        [SearchContext("ext:txt dir:Resources")] // QOL: Limit the files to ONLY project text files within Resources. 
        private TextAsset testScript;

        private Runner runner;

        private void Start()
        {
            if(!testScript)
            {
                return;
            }

            compiler = new Compiler.Compiler()
            {
                ErrorLogger = Logger,
                FunctionLibrary = libraries,
            };
            var script = compiler.Compile(testScript.text);

            runner = new Runner(script)
            {
                Logger = Logger,
                OnFinish = CleanupOnFinish,
            };
            
            runner.Run();
        }

        private void Logger(string message)
        {
            Debug.LogError(message); 
        }

        private void CleanupOnFinish()
        {
            foreach (var library in this.libraries)
            {
                library.Cleanup();
            }
        }
    }
}
