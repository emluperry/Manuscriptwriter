using System;
using System.Collections.Generic;

using MSW.Scripting;
namespace MSW.Compiler
{
    public class Runner
    {
        public Action<string> Logger;
        public Action OnFinish;
        
        private Manuscript manuscript;
        private Interpreter interpreter;
        public Runner(Manuscript manuscript)
        {
            this.manuscript = manuscript;
            interpreter = new Interpreter(manuscript) { ReportRuntimeError = ReportRuntimeError, OnFinish = RunOnFinish};
        }

        public bool IsFinished()
        {
            return interpreter.IsFinished;
        }

        public void Run()
        {
            interpreter.RunUntilBreak();
        }

        private void RunOnFinish()
        {
            this.OnFinish?.Invoke();
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
