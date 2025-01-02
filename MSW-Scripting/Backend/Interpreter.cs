using System;
using System.Collections.Generic;
using System.Linq;
using MSW.Reflection;
using MSW.Events;

namespace MSW.Scripting
{
    internal class Interpreter : IMSWExpressionVisitor, IMSWStatementVisitor
    {
        // Context Settings
        private IRunnerEvent PauseEvent = null;
        
        internal bool IsFinished { get; private set; }
        internal Action OnFinish;
        
        // Error reporting
        public Action<MSWRuntimeException> ReportRuntimeError;
        
        private Environment globals;
        private Environment environment;

        private IEnumerator<Statement> statementEnumerator;
        private List<IRunnerEvent> activeEvents = new List<IRunnerEvent>();

        public Interpreter(Manuscript manuscript)
        {
            this.globals = new Environment();
            this.environment = globals;
            
            this.statementEnumerator = manuscript.statements.GetEnumerator();
            
            this.RunScriptSetup();
        }

        ~Interpreter()
        {
            this.RunScriptCleanup();
        }

        private void RunScriptSetup()
        {
            while (this.statementEnumerator.MoveNext())
            {
                if (this.statementEnumerator.Current is When whenStatement)
                {
                    whenStatement.runnerEvent.RegisterEvent((s, e) => HandleEvent(s, e, whenStatement));

                    if (!this.activeEvents.Contains(whenStatement.runnerEvent))
                    {
                        this.activeEvents.Add(whenStatement.runnerEvent);
                    }
                }
            }
            
            this.statementEnumerator.Reset();
        }

        public void RunScriptCleanup()
        {
            // iterate through events and clear invocation list
            foreach (IRunnerEvent runnerEvent in this.activeEvents)
            {
                runnerEvent.ClearAllEvents();
            }
            
            this.statementEnumerator?.Dispose();
            this.statementEnumerator = null;
        }
        
        private void HandleEvent(object sender, IRunnerEventArgs eventArgs, When visitor)
        {
            // When receiving this event,
            // if the arguments are the same as the passed arguments,
            // then execute the body
            
            // Convert the expression arguments.
            var arguments = new object[visitor.arguments.Count];
            for (int i = 0; i < visitor.arguments.Count(); i++)
            {
                arguments[i] = this.Evaluate(visitor.arguments[i]);
            }
            
            if (!eventArgs.HasValidArguments(arguments))
            {
                return;
            }
            
            // get the event handler arguments
            for (int i = 0; i < visitor.arguments.Count(); i++)
            {
                if (!arguments[i].Equals(eventArgs.Args[i]))
                {
                    return;
                }
            }
            
            // if the event arguments match up, run the body.
            this.Execute(visitor.body);
        }

        public void RunUntilBreak()
        {
            bool runNext = true;
            while (runNext)
            {
                runNext = this.InterpretNextLine();
            }
        }

        private bool InterpretNextLine()
        {
            if (this.PauseEvent != null)
            {
                return false;
            }
            
            try
            {
                if (this.statementEnumerator.MoveNext())
                {
                    this.Execute(this.statementEnumerator.Current);
                    return this.PauseEvent == null;
                }
                else
                {
                    this.IsFinished = true;
                    this.OnFinish?.Invoke();
                    return false;
                }
            }
            catch (MSWRuntimeException e)
            {
                ReportRuntimeError?.Invoke(e);
            }

            return false;
        }

        private void HandleContext(Context context)
        {
            if (context.pauseEvent != null)
            {
                this.PauseEvent = context.pauseEvent;
                this.PauseEvent.RegisterEvent(HandlePauseEvent);
            }
        }

        private void HandlePauseEvent(object sender, IRunnerEventArgs eventArgs)
        {
            this.PauseEvent?.UnregisterEvent(HandlePauseEvent);
            this.PauseEvent = null;
            
            this.RunUntilBreak();
        }

        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private object Execute(Statement statement)
        {
            return statement.Accept(this);
        }

        private void ExecuteBlock(IEnumerable<Statement> statements, Environment blockEnvironment)
        {
            Environment previous = this.environment;

            try
            {
                this.environment = blockEnvironment;

                foreach(Statement statement in statements)
                {
                    this.Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        private bool IsTrue(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(obj is bool b)
            {
                return b;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if(a == null && b == null)
            {
                return true;
            }

            if(a == null)
            {
                return false;
            }

            return a.Equals(b);
        }
        
        #region ERRORS + VALIDATION

        private void ValidateNumberOperand(Token op, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new MSWRuntimeException(op, $"Operand {(operand != null ? operand.ToString() : string.Empty)} must be a number.");
        }
        #endregion

        #region Expression Visitors
        public object VisitAssignment(Assign visitor)
        {
            object value = this.Evaluate(visitor.value);
            environment.Assign(visitor.token, value);
            return value;
        }

        public object VisitVariable(Variable visitor)
        {
            return environment.Get(visitor.token);
        }

        public object VisitLiteral(Literal expr)
        {
            return expr.literal;
        }

        public object VisitGrouping(Grouping expr)
        {
            return this.Evaluate(expr.expression);
        }

        public object VisitBinary(Binary visitor)
        {
            object left = this.Evaluate(visitor.left);
            object right = this.Evaluate(visitor.right);

            switch(visitor.op.type)
            {
                // ARITHMETIC
                case TokenType.MINUS:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left - (double)right;
                case TokenType.DIVIDE:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left / (double)right;
                case TokenType.MULTIPLY:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if(left is double ld && right is double rd)
                    {
                        return ld + rd;
                    }

                    if(left is string ls && right is string rs)
                    {
                        return ls + rs;
                    }

                    throw new MSWRuntimeException(visitor.op, "Operands must be two numbers or two strings.");

                // COMPARISON
                case TokenType.GREATER:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    this.ValidateNumberOperand(visitor.op, left);
                    this.ValidateNumberOperand(visitor.op, right);
                    return (double)left <= (double)right;

                // EQUALITY
                case TokenType.EQUAL_EQUAL:
                    return this.IsEqual(left, right);
                case TokenType.NOT_EQUAL:
                    return !this.IsEqual(left, right);
            }

            return null;
        }

        public object VisitUnary(Unary visitor)
        {
            object right = this.Evaluate(visitor.right);

            switch(visitor.op.type)
            {
                case TokenType.NOT:
                    return !IsTrue(right);
                case TokenType.MINUS:
                    this.ValidateNumberOperand(visitor.op, right);
                    return -(double)right;
            }

            return null;
        }

        public object VisitLogical(Logical visitor)
        {
            object left = this.Evaluate(visitor.left);

            if(visitor.op.type == TokenType.OR)
            {
                if(this.IsTrue(left))
                {
                    return left;
                }
            }
            else
            {
                if(!this.IsTrue(left))
                {
                    return left;
                }
            }

            return this.Evaluate(visitor.right);
        }

        public object VisitCall(Call visitor)
        {
            object[] arguments = new object[visitor.arguments.Count + 1];

            Context ctx = new Context();
            arguments[0] = ctx;
            for (int i = 0; i < visitor.arguments.Count(); i++)
            {
                arguments[i + 1] = this.Evaluate(visitor.arguments[i]);
            }

            object output = visitor.function?.DynamicInvoke(visitor.target, arguments);
            
            // Handle Context
            this.HandleContext(ctx);

            return output;
        }

        #endregion

        #region STATEMENT VISITORS
        public object VisitExpression(StatementExpression visitor)
        {
            this.Evaluate(visitor.expression);
            return null;
        }

        public object VisitPrint(Print visitor)
        {
            object value = this.Evaluate(visitor.expression);
            Console.WriteLine(value != null ? value.ToString() : "Null");
            return null;
        }

        public object VisitVar(VarDeclaration visitor)
        {
            object value = null;
            if(visitor.initialiser != null)
            {
                value = Evaluate(visitor.initialiser);
            }

            environment.Define(visitor.token.lexeme, value);
            return null;
        }

        public object VisitBlock(Block visitor)
        {
            this.ExecuteBlock(visitor.statements, new Environment(environment));
            return null;
        }

        public object VisitIfBlock(If visitor)
        {
            if(this.IsTrue(this.Evaluate(visitor.condition)))
            {
                this.Execute(visitor.thenBranch);
            }
            else if(visitor.elseBranch != null)
            {
                this.Execute(visitor.elseBranch);
            }

            return null;
        }

        public object VisitWhileBlock(While visitor)
        {
            while(this.IsTrue(this.Evaluate(visitor.condition)))
            {
                this.Execute(visitor.statement);
            }

            return null;
        }

        public object VisitWhenBlock(When visitor)
        {
            // These shouldn't be visited in standard execution.
            return null;
        }

        #endregion
    }
}
