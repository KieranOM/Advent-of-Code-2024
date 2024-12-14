namespace AoC;

public class Day11 : Day
{
    public override int Number => 11;
    protected override InputType Input => InputType.Text;

    private readonly record struct Context(in string Stone, in int Blinks);

    protected override void Run(in string input)
    {
        string[] stones = input.Split(' ', StringSplitOptions.TrimEntries);
        var cache = new Dictionary<Context, long>();

        Log(NumberOfStones(stones, blinks: 25, cache));
        Log(NumberOfStones(stones, blinks: 75, cache));
    }

    private static long NumberOfStones(in string[] stones, int blinks, Dictionary<Context, long> cache)
    {
        return stones.Select(stone => NumberOfStones(stone, blinks, cache)).Sum();
    }

    private static long NumberOfStones(in string stone, int blinks, Dictionary<Context, long> cache)
    {
        if (blinks-- == 0) return 1;

        var context = new Context(stone, blinks);
        if (cache.TryGetValue(context, out long count))
        {
            return count;
        }

        if (stone is "" or "0")
        {
            count = NumberOfStones("1", blinks, cache);
            cache[context] = count;
            return count;
        }

        if (stone.Length % 2 == 0)
        {
            int split = stone.Length / 2;
            string left = stone[..split];
            string right = stone[split..].TrimStart('0');

            count = NumberOfStones(left, blinks, cache) + NumberOfStones(right, blinks, cache);
            cache[context] = count;
            return count;
        }

        long multiplied = long.Parse(stone) * 2024L;

        count = NumberOfStones(multiplied.ToString(), blinks, cache);
        cache[context] = count;
        return count;
    }
}