using System;
using MSW.Reflection;

namespace MSW.Console
{
    public class ConsoleDialogue
    {
        public RunnerEvent consoleEvent;
        
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(Context context, string name, string line)
        {
            System.Console.WriteLine($"{name} says: {line}");
            
            context.WaitForEvent(consoleEvent);
            return null;
        }
    }
}