using LoxInterpreter.Reporting;
using LoxInterpreter.Runtime;
using LoxInterpreter.Abstractions;
using LoxInterpreter.Ast;
using LoxInterpreter.Tools;

namespace LoxInterpreter.Cli;

public class Program
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
        var result = _engine.Execute(source);

        if (result.DiagnosticManager.HasErrors)
        {
            PrintDiagnostics(result.DiagnosticManager);
            Environment.Exit(65);
        }

        PrintValue(result.Value);
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

            var result = _engine.Execute(line);

            if (result.DiagnosticManager.Any)
            {
                PrintDiagnostics(result.DiagnosticManager);
            }

            if (!result.DiagnosticManager.HasErrors)
            {
                PrintValue(result.Value);
            }
        }
    }

    private static void PrintValue(object? value)
    {
        if (value == null) return;

        Console.ForegroundColor = ConsoleColor.Green;

        if (value is IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Console.WriteLine(token.ToString());
            }
        }
        else if (value is Expr expr)
        {
            Console.WriteLine(new AstPrinter().Print(expr));
        }
        else
        {
            Console.WriteLine(value.ToString());
        }

        Console.ResetColor();
    }

    private static void PrintDiagnostics(DiagnosticManager diagnosticManager)
    {
        foreach (var diagnostic in diagnosticManager)
        {
            Console.ForegroundColor = diagnostic.Severity == DiagnosticSeverity.Error
                ? ConsoleColor.Red
                : ConsoleColor.Yellow;
            Console.WriteLine(diagnostic);
        }
        Console.ResetColor();
    }
}
