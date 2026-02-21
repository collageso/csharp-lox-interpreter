namespace LoxInterpreter.Parsing;

using LoxInterpreter.Ast;
using LoxInterpreter.Abstractions;
using static LoxInterpreter.Abstractions.TokenType;
using LoxInterpreter.Reporting;

public class Parser
{
    private class ParseError : Exception { }
    private DiagnosticManager _diagnosticManager;

    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens, DiagnosticManager diagnosticManager)
    {
        _tokens = tokens;
        _diagnosticManager = diagnosticManager;
    }

    public Expr? Parse()
    {
        try
        {
            return ParseExpression();
        }
        catch (ParseError)
        {
            return null;
        }
    }

    private Expr ParseExpression()
    {
        return ParseEquality();
    }

    private Expr ParseEquality()
    {
        Expr expr = ParseComparison();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            Token op = Previous();
            Expr right = ParseComparison();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr ParseComparison()
    {
        Expr expr = ParseTerm();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = ParseTerm();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr ParseTerm()
    {
        Expr expr = ParseFactor();

        while (Match(MINUS, PLUS))
        {
            Token op = Previous();
            Expr right = ParseFactor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr ParseFactor()
    {
        Expr expr = ParseUnary();

        while (Match(SLASH, STAR))
        {
            Token op = Previous();
            Expr right = ParseUnary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr ParseUnary()
    {
        if (Match(BANG, MINUS))
        {
            Token op = Previous();
            Expr right = ParseUnary();
            return new Unary(op, right);
        }

        return ParsePrimary();
    }

    private Expr ParsePrimary()
    {
        if (Match(FALSE))
        {
            return new Literal(false);
        }

        if (Match(TRUE))
        {
            return new Literal(true);
        }

        if (Match(NIL))
        {
            return new Literal(null);
        }

        if (Match(NUMBER, STRING))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(LEFT_PAREN))
        {
            Expr expr = ParseExpression();
            Consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return Peek().Type == type;
    }

    private bool IsAtEnd() => Peek().Type == EOF;

    private Token Peek() => _tokens[_current];

    private Token Previous() => _tokens[_current - 1];

    private ParseError Error(Token token, string message)
    {
        _diagnosticManager.ReportParseError(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == SEMICOLON)
            {
                return;
            }

            switch (Peek().Type)
            {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                    return;
            }

            Advance();
        }
    }
}
