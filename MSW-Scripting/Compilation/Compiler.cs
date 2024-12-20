using System;
using System.Collections.Generic;
using MSW.Scripting;

namespace MSW.Compiler
{
    public class Compiler
    {
        public IEnumerable<object> FunctionLibrary;
        public Action<string> ErrorLogger;
        
        private bool hasError = false;
        private Scanner scanner;
        private Parser parser;

        public Manuscript Compile(string source)
        {
            hasError = false;
            scanner = new Scanner(source);

            List<Token> tokens = scanner.ScanLines();

            parser = new Parser(tokens, FunctionLibrary) { ReportTokenError = ReportTokenError };
            IEnumerable<Statement> statements = parser.Parse();

            if(hasError)
            {
                return null;
            }

            Manuscript script = new Manuscript(statements);
            return script;
        }
        
        private void ReportTokenError(Token token, string message)
        {
            hasError = true;
            if(token?.type == TokenType.EOF)
            {
                Report(token.line, "at end", message);
            }
            else
            {
                Report(token?.line ?? 0, $"at '{token?.lexeme}'", message);
            }
        }
        
        private void ReportRuntimeError(MSWRuntimeException ex)
        {
            hasError = true;
            this.Report(ex.operatorToken.line, "", ex.Message);
        }

        private void Report(int line, string where, string message)
        {
            ErrorLogger?.Invoke($"[Line {line} - {where}]: {message}");
        }
    }
}