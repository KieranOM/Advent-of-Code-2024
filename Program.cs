namespace AoC;

internal static class Program
{
    private static readonly Day[] Days =
    [
        new Day1(),
        new Day2(),
        new Day3(),
        new Day4(),
        new Day5(),
        new Day6(),
        new Day7(),
        new Day8(),
        new Day9(),
        new Day10(),
        new Day11(),
        new Day12(),
        new Day13(),
        new Day14(),
        new Day15(),
        new Day16(),
    ];

    public static async Task Main(string[] args)
    {
        string input = GetDayNumberInput(args);
        if (!ValidateDayNumberInput(input, out int number))
        {
            Console.WriteLine("Invalid day entered (1-25)");
            return;
        }

        var day = Days.SingleOrDefault(day => day.Number == number);
        if (day == null)
        {
            Console.WriteLine($"Day {number} does not exist");
            return;
        }

        await day.Run();
    }

    private static string GetDayNumberInput(in string[] args)
    {
        if (args is { Length: > 0 }) return args[0];
        Console.Write("Day: ");
        return Console.ReadLine() ?? "";
    }

    private static bool ValidateDayNumberInput(string input, out int number)
    {
        return int.TryParse(input, out number) && number is >= 1 and <= 25;
    }
}