namespace AoC;

public class Day19 : Day
{
    public override int Number => 19;

    protected override void Run(in string[] input)
    {
        ParseTowelsAndPatterns(input, out var towels, out var designs);

        var cache = new Dictionary<string, long>(StringComparer.Ordinal)
        {
            [string.Empty] = 1L
        };

        var arrangements = Array.ConvertAll(designs, design => CalculateArrangements(design, towels, cache));

        var possible = arrangements.Count(variations => variations > 0);
        Log(possible);

        var total = arrangements.Sum();
        Log(total);
    }

    private static long CalculateArrangements(in string pattern, in string[] towels,
        Dictionary<string, long> cache)
    {
        if (cache.TryGetValue(pattern, out var arrangements))
            return arrangements;

        if (pattern.Length == 0) return 1;

        arrangements = 0;
        foreach (var towel in towels)
        {
            if (pattern.StartsWith(towel, StringComparison.Ordinal))
            {
                var next = pattern[towel.Length..];
                arrangements += CalculateArrangements(next, towels, cache);
            }
        }

        cache[pattern] = arrangements;
        return arrangements;
    }

    private static void ParseTowelsAndPatterns(in string[] input, out string[] towels, out string[] patterns)
    {
        towels = input[0].Split(", ");
        patterns = input[2..];
    }
}