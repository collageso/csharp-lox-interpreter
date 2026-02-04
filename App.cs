using LoxInterpreter.Diagnostic;
using LoxInterpreter.Engine;

namespace App;

public class App
{
    private static readonly Engine _engine = new();

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Error: File not found at {path}");
            Environment.Exit(66);
        }

        string source = File.ReadAllText(path);
        var executionResult = _engine.Execute(source);

        if (executionResult.DiagnosticList.HasErrors)
        {
            PrintDiagnostics(executionResult.DiagnosticList);
            Environment.Exit(65);
        }

        PrintValue(executionResult.Value);
    }

    private static void RunPrompt()
    {
        Console.WriteLine("Lox Interpreter (C# Version)");
        Console.WriteLine("Type 'exit' to quit.");

        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();

            if (line == null || line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var executionResult = _engine.Execute(line);

            if (executionResult.DiagnosticList.Any)
            {
                PrintDiagnostics(executionResult.DiagnosticList);
            }

            if (!executionResult.DiagnosticList.HasErrors)
            {
                PrintValue(executionResult.Value);
            }
        }
    }

    private static void PrintValue(object? value)
    {
        if (value == null) return;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(value.ToString());
        Console.ResetColor();
    }

    private static void PrintDiagnostics(DiagnosticList diagnosticList)
    {
        foreach (var diagnostic in diagnosticList)
        {
            Console.ForegroundColor = diagnostic.Severity == DiagnosticSeverity.Error
                ? ConsoleColor.Red
                : ConsoleColor.Yellow;
            Console.WriteLine(diagnostic);
        }
        Console.ResetColor();
    }
}
