using System.Diagnostics;

namespace AoC;

public abstract class Day
{
    protected enum InputType
    {
        Lines,
        Text
    }

    private static class Message
    {
        public const string InputNotExist = "Input file does not exist";
        public const string InputReadError = "Unable to read input file";
        public const string Running = "Running...";
        public const string Elapsed = "Time elapsed: ";
    }

    public abstract int Number { get; }
    protected virtual bool IsAsync => false;
    protected virtual InputType Input => InputType.Lines;

    private string LogPrefix => $"[Day {Number}] ";

    protected virtual void Run(in string[] input)
    {
    }

    protected virtual void Run(in string input)
    {
    }

    protected virtual Task RunAsync(string[] input) => Task.CompletedTask;

    protected virtual Task RunAsync(string input) => Task.CompletedTask;

    public async Task Run()
    {
        string inputPath = $"Day{Number}/input.txt";
        if (!File.Exists(inputPath))
        {
            Log(Message.InputNotExist);
            return;
        }

        if (Input == InputType.Lines)
        {
            await RunLines(inputPath);
        }
        else
        {
            await RunText(inputPath);
        }
    }

    private async Task RunLines(string inputPath)
    {
        if (!TryReadAllLines(inputPath, out string[] input))
        {
            Log(Message.InputReadError);
            return;
        }

        Log(Message.Running);
        var stopwatch = Stopwatch.StartNew();

        if (IsAsync) await RunAsync(input);
        else Run(input);

        stopwatch.Stop();
        Log(Message.Elapsed + stopwatch.Elapsed);
    }

    private async Task RunText(string inputPath)
    {
        if (!TryReadAllText(inputPath, out string input))
        {
            Log(Message.InputReadError);
            return;
        }

        Log(Message.Running);
        var stopwatch = Stopwatch.StartNew();

        if (IsAsync) await RunAsync(input);
        else Run(input);

        stopwatch.Stop();
        Log(Message.Elapsed + stopwatch.Elapsed);
    }

    private static bool TryReadAllLines(in string path, out string[] lines)
    {
        try
        {
            lines = File.ReadAllLines(path);
            return true;
        }
        catch
        {
            lines = [];
            return false;
        }
    }

    private static bool TryReadAllText(in string path, out string text)
    {
        try
        {
            text = File.ReadAllText(path);
            return true;
        }
        catch
        {
            text = string.Empty;
            return false;
        }
    }

    protected void Log<T>(in T value)
    {
        Console.WriteLine(LogPrefix + value);
    }
}