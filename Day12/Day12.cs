namespace AoC;

public class Day12 : Day
{
    public override int Number => 12;

    private delegate int Evaluator(in Region region);

    protected override void Run(in string[] input)
    {
        ParseInput(input, out var grid);
        var regions = grid.FindRegions();

        int priceByPerimeter = CalculateFencePricing(regions, Perimeter);
        Log(priceByPerimeter);

        int priceBySides = CalculateFencePricing(regions, Sides);
        Log(priceBySides);
    }

    private static int CalculateFencePricing(in List<Region> regions, Evaluator evaluator) =>
        regions.Select(region => CalculateFencePricing(region, evaluator)).Sum();

    private static int CalculateFencePricing(in Region region, in Evaluator evaluator) =>
        evaluator(region) * region.Plots.Count;

    private static int Perimeter(in Region region)
    {
        var plots = region.Plots;
        int perimeter = plots.Count * 4;
        foreach (var plot in plots)
        {
            if (plots.Contains(plot + Vector2I.Down)) perimeter -= 2;
            if (plots.Contains(plot + Vector2I.Right)) perimeter -= 2;
        }

        return perimeter;
    }

    private readonly record struct Visit(in Vector2I Position, in Vector2I Forward)
    {
        public Vector2I Left => Forward.TurnLeft();
        public Vector2I Right => Forward.TurnRight();
    }

    private static int Sides(in Region region)
    {
        var visits = new HashSet<Visit>();
        var plots = region.Plots;

        int sides = 0;
        foreach (var start in TopEdges(region))
        {
            var visit = new Visit(start + Vector2I.Up, Vector2I.Right);
            while (visits.Add(visit))
            {
                bool forward = plots.Contains(visit.Position + visit.Forward);
                bool diagonal = plots.Contains(visit.Position + visit.Forward + visit.Right);

                switch (forward, diagonal)
                {
                    case (forward: false, diagonal: false): // Outer corner
                        sides++;
                        visit = new Visit(visit.Position + visit.Forward + visit.Right, Forward: visit.Right);
                        break;
                    case (forward: true, _): // Inner corner
                        sides++;
                        visit = visit with { Forward = visit.Left };
                        break;
                    case (forward: false, _): // Side
                        visit = visit with { Position = visit.Position + visit.Forward };
                        break;
                }
            }
        }

        return sides;
    }

    private static IEnumerable<Vector2I> TopEdges(Region region)
    {
        return region.Plots.Where(plot => region.Plots.Contains(plot + Vector2I.Up) == false);
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
                var position = new Vector2I(x, y);
                grid.SetPlant(position, row[x]);
            }
        }
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static Vector2I Up => new(0, 1);
        public static Vector2I Down => new(0, -1);
        public static Vector2I Right => new(1, 0);
        public static Vector2I Left => new(-1, 0);

        public Vector2I TurnRight() => new(Y, -X);
        public Vector2I TurnLeft() => new(-Y, X);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);
    }

    private readonly record struct Region(in char Plant, in HashSet<Vector2I> Plots);

    private class Grid(int width, int height)
    {
        private readonly char[,] _grid = new char[width, height];

        private bool IsInBounds(in Vector2I position)
        {
            return position.X >= 0 && position.X < width &&
                   position.Y >= 0 && position.Y < height;
        }

        private char At(in Vector2I position) => _grid[position.X, position.Y];

        public void SetPlant(in Vector2I position, in char plant)
        {
            _grid[position.X, position.Y] = plant;
        }

        public List<Region> FindRegions()
        {
            var regions = new List<Region>();
            var recorded = new HashSet<Vector2I>();

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var pos = new Vector2I(x, y);
                if (recorded.Contains(pos)) continue;

                char plant = At(pos);
                var region = new Region(plant, []);
                ExploreRegion(region, pos);
                regions.Add(region);

                foreach (var plot in region.Plots) recorded.Add(plot);
            }

            return regions;
        }

        private void ExploreRegion(in Region region, in Vector2I pos)
        {
            if (IsInBounds(pos) == false) return;
            if (At(pos) != region.Plant) return;
            if (region.Plots.Add(pos) == false) return;

            ExploreRegion(region, pos + Vector2I.Up);
            ExploreRegion(region, pos + Vector2I.Down);
            ExploreRegion(region, pos + Vector2I.Left);
            ExploreRegion(region, pos + Vector2I.Right);
        }
    }
}