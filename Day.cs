using System.Diagnostics;

namespace AoC;

public abstract class Day
{
    public abstract int Number { get; }
    private string LogPrefix => $"[Day {Number}] ";

    protected abstract void Run(in string[] input);

    public void Run()
    {
        string inputPath = $"Day{Number}/input.txt";
        if (!File.Exists(inputPath))
        {
            Log("Input file does not exist");
            return;
        }

        if (!TryReadAllLines(inputPath, out string[] input))
        {
            Log("Unable to read input file");
            return;
        }

        Log("Running...");

        var stopwatch = Stopwatch.StartNew();
        Run(input);
        stopwatch.Stop();

        Log($"Time elapsed: {stopwatch.Elapsed}");
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

    protected void Log(in string message)
    {
        Console.WriteLine(LogPrefix + message);
    }
}