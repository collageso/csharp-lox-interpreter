using LoxInterpreter.Diagnostic;

namespace LoxInterpreter.Engine;

public class ExecutionResult
{
    public DiagnosticCollector Diagnostics { get; }
    public object? Value { get; }

    public ExecutionResult(DiagnosticCollector diagnostics, object? value)
    {
        Diagnostics = diagnostics;
        Value = value;
    }
}

public class Engine
{

    public ExecutionResult Execute(string source)
    {
        var diagnostics = new DiagnosticCollector();
        object? result = source;
        return new ExecutionResult(diagnostics, result);
    }
}
