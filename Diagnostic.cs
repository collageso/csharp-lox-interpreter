using System.Collections;

namespace LoxInterpreter.Diagnostic;

public enum DiagnosticSeverity
{
    Error,
    Warning
}

public class Diagnostic
{
    public int Line { get; }
    public int Column { get; }
    public string Message { get; }
    public DiagnosticSeverity Severity { get; }

    public Diagnostic(int line, int column, string message, DiagnosticSeverity severity)
    {
        Line = line;
        Column = column;
        Message = message;
        Severity = severity;
    }

    public override string ToString() => $"[Line {Line}:{Column}] {Severity}: {Message}";
}

public class DiagnosticList : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Any => _diagnostics.Count > 0;
    public bool HasErrors => _diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);

    public void Report(int line, int column, string message, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        _diagnostics.Add(new Diagnostic(line, column, message, severity));
    }

    public void ReportBadCharacter(int line, int column, char character)
    {
        Report(line, column, $"Unexpected character: '{character}'");
    }

    public void ReportUnterminatedString(int line, int column)
    {
        Report(line, column, "Unterminated string literal.");
    }
}
