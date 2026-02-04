using LoxInterpreter.Diagnostic;

namespace LoxInterpreter.Engine;

public class ExecutionResult
{
    public DiagnosticList DiagnosticList { get; }
    public object? Value { get; }

    public ExecutionResult(DiagnosticList diagnosticList, object? value)
    {
        DiagnosticList = diagnosticList;
        Value = value;
    }
}

public class Engine
{

    public ExecutionResult Execute(string source)
    {
        var diagnosticList = new DiagnosticList();
        object? result = source;
        return new ExecutionResult(diagnosticList, result);
    }
}
