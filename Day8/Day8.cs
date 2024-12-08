namespace AoC;

public class Day8 : Day
{
    public override int Number => 8;
    protected override InputType Input => InputType.Lines;

    delegate void Evaluator(in Grid grid, in Vector2I position, in Vector2I delta, in HashSet<Vector2I> antinodes);

    protected override void Run(in string[] input)
    {
        ParseInput(input, out var grid);

        var projected = EvaluateAntinodes(grid, ProjectedAntinodes);
        Log(projected.Count);

        var repeated = EvaluateAntinodes(grid, RepeatedAntinodes);
        Log(repeated.Count);
    }

    private static void ProjectedAntinodes(in Grid grid, in Vector2I position, in Vector2I delta,
        in HashSet<Vector2I> antinodes)
    {
        var projection = position + delta;
        if (grid.IsInBounds(projection)) antinodes.Add(projection);
    }

    private static void RepeatedAntinodes(in Grid grid, in Vector2I position, in Vector2I delta,
        in HashSet<Vector2I> antinodes)
    {
        var projection = position;
        while (grid.IsInBounds(projection))
        {
            antinodes.Add(projection);
            projection += delta;
        }
    }

    private static HashSet<Vector2I> EvaluateAntinodes(in Grid grid, in Evaluator evaluator)
    {
        var result = new HashSet<Vector2I>();
        foreach (var (_, positions) in grid.Antennas)
        {
            foreach (var first in positions)
            {
                foreach (var second in positions)
                {
                    if (first == second) continue;
                    var delta = second - first;

                    evaluator(grid, second, delta, result);
                }
            }
        }

        return result;
    }

    private static void ParseInput(in string[] input, out Grid grid)
    {
        int width = input[0].Length, height = input.Length, invert = input.Length - 1;
        grid = new Grid(width, height);

        for (int y = 0; y < height; y++)
        {
            string row = input[invert - y];
            for (int x = 0; x < width; x++)
            {
                char cell = row[x];
                var position = new Vector2I(x, y);

                if (IsAntenna(cell)) grid.AddAntenna(cell, position);
            }
        }
    }

    private static bool IsAntenna(in char cell)
    {
        return cell is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9';
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);

        public static Vector2I operator -(in Vector2I left, in Vector2I right) =>
            new(left.X - right.X, left.Y - right.Y);
    }

    private class Grid(int width, int height)
    {
        public IReadOnlyDictionary<char, List<Vector2I>> Antennas => _antennas;
        private readonly Dictionary<char, List<Vector2I>> _antennas = new();

        public void AddAntenna(in char antenna, in Vector2I position)
        {
            if (_antennas.TryGetValue(antenna, out var positions) == false)
            {
                positions = [];
            }

            positions.Add(position);
            _antennas[antenna] = positions;
        }

        public bool IsInBounds(in Vector2I position)
        {
            return position.X >= 0 && position.X < width &&
                   position.Y >= 0 && position.Y < height;
        }
    }
}