namespace AoC;

internal static class Program
{
    private static readonly Day[] Days =
    [
        new Day1(),
        new Day2(),
        new Day3(),
        new Day4(),
    ];

    public static void Main(string[] args)
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

        day.Run();
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