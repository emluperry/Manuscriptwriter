using System;
using System.Collections.Generic;
using MSW.Reflection;

namespace MSW.Scripting
{
    internal class When : Statement
    {
        public readonly RunnerEvent runnerEvent;
        public readonly Statement body;
        public readonly List<Expression> arguments;
        
        public When(RunnerEvent runnerEvent, Statement body, List<Expression> arguments)
        {
            this.runnerEvent = runnerEvent;
            this.body = body;
            this.arguments = arguments;
        }
        
        public override object Accept(IMSWStatementVisitor visitor)
        {
            return visitor.VisitWhenBlock(this);
        }
    }
}