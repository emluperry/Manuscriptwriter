using System;
using System.Collections.Generic;

namespace MSW.Reflection
{
    public class RunnerEventArgs : EventArgs
    {
        public readonly List<object> args;
        public RunnerEventArgs(List<object> args)
        {
            this.args = args;
        }
    }
}