using System;
using System.IO;
using System.Text;

namespace Lox;

public class Lox
{
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
        string source = File.ReadAllText(path);
        Run(source);
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();

            if (line == null)
            {
                break;
            }

            Run(line);
        }
    }

    private static void Run(string source)
    {
        Console.WriteLine($"Input: {source}");
    }
}


