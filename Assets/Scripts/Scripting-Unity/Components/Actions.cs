using System;
using UnityEngine;
using UnityEngine.Search;

namespace MSW.Unity
{
    public class Actions : MonoBehaviour
    {
        [SerializeField] [SearchContext("ext:txt dir:Resources")] // QOL: Limit the files to ONLY project text files within Resources. 
        private TextAsset actionScript;
        
        public TextAsset ActionScript => actionScript;

        public Action RunScript;
    }
}
