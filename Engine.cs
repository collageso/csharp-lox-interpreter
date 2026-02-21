using LoxInterpreter.Reporting;
using LoxInterpreter.Scanning;
using LoxInterpreter.Parsing;

namespace LoxInterpreter.Runtime;

public class ExecutionResult
{
    public DiagnosticManager DiagnosticManager { get; }
    public object? Value { get; }

    public ExecutionResult(DiagnosticManager diagnosticManager, object? value)
    {
        DiagnosticManager = diagnosticManager;
        Value = value;
    }
}

public class Engine
{
    public ExecutionResult Execute(string source)
    {
        var diagnosticManager = new DiagnosticManager();
        var lexer = new Lexer(source, diagnosticManager);
        var tokens = lexer.ScanTokens();
        var parser = new Parser(tokens, diagnosticManager);
        var expression = parser.Parse();

        return new ExecutionResult(diagnosticManager, expression);
    }
}
