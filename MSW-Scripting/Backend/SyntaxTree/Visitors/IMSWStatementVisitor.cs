namespace MSW.Scripting
{
    internal interface IMSWStatementVisitor
    {
        object VisitExpression(StatementExpression visitor);
        object VisitPrint(Print visitor);

        object VisitVar(VarDeclaration visitor);

        object VisitBlock(Block visitor);

        object VisitIfBlock(If visitor);

        object VisitWhileBlock(While visitor);

        object VisitWhenBlock(When visitor);
    }
}
