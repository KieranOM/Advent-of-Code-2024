namespace AoC;

public class Day4 : Day
{
    public override int Number => 4;

    private const string Xmas = "XMAS", XmasReversed = "SAMX";
    private const string Mas = "MAS", MasReversed = "SAM";

    private static readonly Grid[] XmasWindows =
    [
        Grid.Horizontal(Xmas), Grid.Horizontal(XmasReversed),
        Grid.Vertical(Xmas), Grid.Vertical(XmasReversed),
        Grid.DiagonalDown(Xmas), Grid.DiagonalDown(XmasReversed),
        Grid.DiagonalUp(Xmas), Grid.DiagonalUp(XmasReversed)
    ];

    private static readonly Grid[] CrossMasWindows =
    [
        Grid.Cross(down: Mas, up: Mas),
        Grid.Cross(down: Mas, up: MasReversed),
        Grid.Cross(down: MasReversed, up: Mas),
        Grid.Cross(down: MasReversed, up: MasReversed)
    ];

    protected override void Run(in string[] input)
    {
        var grid = new Grid(input);
        int xmasInstances = EvaluateWindows(grid, XmasWindows);
        Log(xmasInstances);

        int crossMasInstances = EvaluateWindows(grid, CrossMasWindows);
        Log(crossMasInstances);
    }

    private static int EvaluateWindows(Grid grid, in Grid[] windows)
    {
        int count = 0;
        for (int y = 0; y < grid.Height; y++)
        for (int x = 0; x < grid.Width; x++)
            count += windows.Count(window => grid.Evaluate(window, x, y));
        return count;
    }

    private class Grid
    {
        private const char Wildcard = '.';
        private readonly char[,] _array;

        public int Width { get; }
        public int Height { get; }

        public Grid(in string[] input)
        {
            Height = input.Length;
            Width = input[0].Length;
            _array = new char[Height, Width];

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _array[y, x] = input[y][x];
        }

        private Grid(in char[,] array)
        {
            _array = array;
            Height = _array.GetLength(0);
            Width = _array.GetLength(1);
        }

        public bool Evaluate(in Grid window, in int atX, in int atY)
        {
            if (Fits(window, atX, atY) == false) return false;

            for (int y = 0; y < window.Height; y++)
            for (int x = 0; x < window.Width; x++)
            {
                if (window.At(x, y) == Wildcard) continue;
                if (window.At(x, y) != At(atX + x, atY + y)) return false;
            }

            return true;
        }

        private bool Fits(in Grid window, in int x, in int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height &&
                   x + window.Width <= Width &&
                   y + window.Height <= Height;
        }

        private char At(in int x, in int y) => _array[y, x];

        public static Grid Horizontal(in string input)
        {
            char[,] array = new char[1, input.Length];
            for (int x = 0; x < input.Length; x++) array[0, x] = input[x];
            return new Grid(array);
        }

        public static Grid Vertical(in string input)
        {
            char[,] grid = new char[input.Length, 1];
            for (int y = 0; y < input.Length; y++) grid[y, 0] = input[y];
            return new Grid(grid);
        }

        public static Grid DiagonalDown(in string input)
        {
            int size = input.Length;
            char[,] array = new char[size, size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                array[y, x] = y == x ? input[x] : Wildcard;
            return new Grid(array);
        }

        public static Grid DiagonalUp(in string input)
        {
            int size = input.Length, invert = size - 1;
            char[,] array = new char[size, size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                array[invert - y, x] = y == x ? input[x] : Wildcard;
            return new Grid(array);
        }

        public static Grid Cross(in string down, in string up) => Splat(DiagonalDown(down), DiagonalUp(up));

        private static Grid Splat(in Grid source, in Grid destination)
        {
            for (int y = 0; y < destination.Height; y++)
            for (int x = 0; x < destination.Width; x++)
                destination._array[y, x] = SplatCell(source.At(x, y), destination.At(x, y));
            return destination;

            static char SplatCell(in char source, in char destination)
            {
                if (destination == Wildcard) return source;
                if (source == Wildcard) return destination;
                return source;
            }
        }
    }
}