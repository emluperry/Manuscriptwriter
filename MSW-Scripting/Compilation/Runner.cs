using System;
using System.Collections.Generic;

using MSW.Scripting;
namespace MSW.Compiler
{
    public class Runner
    {
        public Action<string> Logger;
        
        private Manuscript manuscript;
        private Interpreter interpreter;
        public Runner(Manuscript manuscript)
        {
            this.manuscript = manuscript;
            interpreter = new Interpreter(manuscript) { ReportRuntimeError = ReportRuntimeError };
        }

        public bool IsFinished()
        {
            return interpreter.IsFinished;
        }

        public void RunUntilBreak()
        {
            bool runNext = true;
            while (runNext)
            {
                runNext = interpreter.InterpretNextLine();
            }
        }

        private void ReportRuntimeError(MSWRuntimeException ex)
        {
            this.Report(ex.operatorToken.line, "", ex.Message);
        }

        private void Report(int line, string where, string message)
        {
            Logger?.Invoke($"ERROR: [Line {line} - {where}]: {message}");
        }
    }
}
