﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<Statement> statements = Enumerable.Empty<Statement>();

            try
            {
                scanner = new Scanner(source);

                List<Token> tokens = scanner.ScanLines();

                parser = new Parser(tokens, FunctionLibrary) { ReportTokenError = ReportTokenError };
                statements = parser.Parse();
            }
            catch(Exception e)
            {
                this.ErrorLogger($"[Internal Error] {e.Message}");
                hasError = true;
            }

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