namespace AoC;

public class Day10 : Day
{
    public override int Number => 10;
    protected override InputType Input => InputType.Lines;

    protected override void Run(in string[] input)
    {
        ParseInput(input, out var grid);
        var paths = grid.FindPaths();

        int score = paths
            .GroupBy(path => path[0])
            .Select(path => path
                .DistinctBy(path => path[^1])
                .Count())
            .Sum();
        Log(score);

        int rating = paths.Count;
        Log(rating);
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
                int altitude = row[x] - '0';
                var position = new Vector2I(x, y);
                grid.SetAltitude(position, altitude);
            }
        }
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static Vector2I Up => new(0, 1);
        public static Vector2I Down => new(0, -1);
        public static Vector2I Right => new(1, 0);
        public static Vector2I Left => new(-1, 0);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);
    }

    private class Grid(int width, int height)
    {
        private const int End = 9;
        private const int Start = 0;

        private readonly int[,] _grid = new int[width, height];

        private readonly List<Vector2I> _starts = [];

        private bool IsInBounds(in Vector2I position)
        {
            return position.X >= 0 && position.X < width &&
                   position.Y >= 0 && position.Y < height;
        }

        public void SetAltitude(Vector2I position, int altitude)
        {
            _grid[position.X, position.Y] = altitude;
            if (altitude == Start) _starts.Add(position);
        }

        public List<List<Vector2I>> FindPaths()
        {
            var paths = new List<List<Vector2I>>();
            foreach (var start in _starts)
            {
                var builder = new List<Vector2I>(End + 1);
                AddPaths(start, -1, builder, paths);
            }

            return paths;
        }

        private void AddPaths(Vector2I position, int lastAltitude, List<Vector2I> builder, List<List<Vector2I>> paths)
        {
            if (IsInBounds(position) == false) return;

            int altitude = _grid[position.X, position.Y];
            if (altitude != lastAltitude + 1) return;

            builder.Add(position);

            if (altitude == End)
            {
                paths.Add([..builder]);
            }
            else
            {
                AddPaths(position + Vector2I.Up, altitude, builder, paths);
                AddPaths(position + Vector2I.Down, altitude, builder, paths);
                AddPaths(position + Vector2I.Left, altitude, builder, paths);
                AddPaths(position + Vector2I.Right, altitude, builder, paths);
            }

            builder.RemoveAt(builder.Count - 1);
        }
    }
}