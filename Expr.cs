namespace LoxInterpreter.Ast;

using LoxInterpreter.Abstractions;

public interface IVisitor<T>
{
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);
}

public abstract class Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public class Binary : Expr
{
    public Expr Left { get; }
    public Token Operator { get; }
    public Expr Right { get; }

    public Binary(Expr left, Token @operator, Expr right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
}

public class Grouping : Expr
{
    public Expr Expression { get; }

    public Grouping(Expr expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
}

public class Literal : Expr
{
    public object? Value { get; }

    public Literal(object? value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
}

public class Unary : Expr
{
    public Token Operator { get; }
    public Expr Right { get; }

    public Unary(Token @operator, Expr right)
    {
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
}

