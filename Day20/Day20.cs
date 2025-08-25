namespace AoC;

public class Day20 : Day
{
    public override int Number => 20;

    protected override void Run(in string[] input)
    {
        ParseInput(input, out var tracks, out var start, out var end);
        var path = Traverse(tracks, start, end);

        var deprecated = Cheats(path, count: 2);
        Log(deprecated);

        var latest = Cheats(path, count: 20);
        Log(latest);
    }

    private static List<Vector2I> Traverse(HashSet<Vector2I> track, in Vector2I start, in Vector2I end)
    {
        var path = new List<Vector2I>(track.Count);

        var current = start;
        var previous = current;

        while (current != end)
        {
            path.Add(current);

            foreach (var adjacent in current.Adjacent())
            {
                if (track.Contains(adjacent) == false) continue;
                if (adjacent == previous) continue;

                previous = current;
                current = adjacent;
                break;
            }
        }

        path.Add(end);
        return path;
    }

    private static int Cheats(List<Vector2I> path, int count)
    {
        const int target = 100;

        var cheats = 0;
        for (var start = 0; start < path.Count; ++start)
        {
            for (var end = start + 1; end < path.Count; ++end)
            {
                var distance = Distance(path[start], path[end]);
                if (distance <= count)
                {
                    var cheat = start + distance;
                    var saving = end - cheat;

                    if (saving >= target)
                    {
                        ++cheats;
                    }
                }
            }
        }

        return cheats;
    }

    private static int Distance(in Vector2I start, in Vector2I end)
    {
        return Math.Abs(end.X - start.X) + Math.Abs(end.Y - start.Y);
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static Vector2I Up => new(0, 1);
        public static Vector2I Down => new(0, -1);
        public static Vector2I Right => new(1, 0);
        public static Vector2I Left => new(-1, 0);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);

        public IEnumerable<Vector2I> Adjacent()
        {
            yield return this + Up;
            yield return this + Down;
            yield return this + Right;
            yield return this + Left;
        }
    }

    private static void ParseInput(in string[] input, out HashSet<Vector2I> tracks, out Vector2I start,
        out Vector2I end)
    {
        const char wall = '#', s = 'S', e = 'E';

        int width = input[0].Length, height = input.Length;
        tracks = [];

        start = default;
        end = default;

        for (var y = 0; y < height; y++)
        {
            var row = input[y];
            for (var x = 0; x < width; x++)
            {
                var type = row[x];
                if (row[x] == wall) continue;

                var position = new Vector2I(x, y);

                if (type == s)
                    start = position;
                else if (type == e)
                    end = position;

                tracks.Add(position);
            }
        }
    }
}