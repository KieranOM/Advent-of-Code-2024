namespace AoC;

public class Day6 : Day
{
    public override int Number => 6;

    protected override void Run(in string[] input)
    {
        ParseInput(input, out var grid, out var start);

        var visitedPositions = CalculateVisitedPositions(grid, start);
        Log(visitedPositions.Count);

        var loopObstaclePositions = CalculateLoopObstaclePositions(grid, start, visitedPositions);
        Log(loopObstaclePositions.Count);
    }

    private static HashSet<Vector2I> CalculateVisitedPositions(in Grid grid, in Vector2I start)
    {
        var visits = new HashSet<Visit>();
        _ = TraverseFullPath(grid, start, Vector2I.Up, visits);
        return visits.Select(visit => visit.Position).ToHashSet();
    }

    private static TraverseResult TraverseFullPath(in Grid grid, Vector2I position, Vector2I direction,
        in HashSet<Visit> visited)
    {
        while (true)
        {
            var result = grid.Traverse(position, direction, visited, out var end);
            if (result != TraverseResult.Obstacle) return result;

            direction = direction.Rotate();
            position = end;
        }
    }

    private static HashSet<Vector2I> CalculateLoopObstaclePositions(Grid grid, Vector2I start,
        HashSet<Vector2I> candidates)
    {
        candidates.Remove(start);
        return candidates.Where(IsLoopObstaclePosition).ToHashSet();

        bool IsLoopObstaclePosition(Vector2I position)
        {
            grid.TemporaryObstacle = position;
            return TraverseFullPath(grid, start, Vector2I.Up, []) == TraverseResult.Loop;
        }
    }

    private static void ParseInput(in string[] input, out Grid grid, out Vector2I start)
    {
        const char guard = '^', obstacle = '#';

        int width = input[0].Length, height = input.Length, invert = input.Length - 1;
        grid = new Grid(width, height);
        start = default;

        for (int y = 0; y < height; y++)
        {
            string row = input[invert - y];
            for (int x = 0; x < width; x++)
            {
                char cell = row[x];
                var position = new Vector2I(x, y);
                if (cell == guard) start = position;
                else if (cell == obstacle) grid.AddObstacle(position);
            }
        }
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static Vector2I Up => new(0, 1);
        public Vector2I Rotate() => new(Y, -X);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);
    }

    private readonly record struct Visit(in Vector2I Position, in Vector2I Direction);

    private class Grid(int width, int height)
    {
        private readonly HashSet<Vector2I> _obstacles = [];
        public Vector2I? TemporaryObstacle { get; set; }

        public void AddObstacle(in Vector2I position)
        {
            _obstacles.Add(position);
        }

        private bool IsInBounds(in Vector2I position)
        {
            return position.X >= 0 && position.X < width &&
                   position.Y >= 0 && position.Y < height;
        }

        private bool IsObstacle(in Vector2I position)
        {
            return TemporaryObstacle == position || _obstacles.Contains(position);
        }

        public TraverseResult Traverse(in Vector2I from, in Vector2I direction, in HashSet<Visit> visited,
            out Vector2I current)
        {
            current = from;
            while (true)
            {
                var visit = new Visit(current, direction);
                if (visited.Add(visit) == false) return TraverseResult.Loop;

                var next = current + direction;
                if (IsInBounds(next) == false) return TraverseResult.Success;
                if (IsObstacle(next)) return TraverseResult.Obstacle;
                current = next;
            }
        }
    }

    private enum TraverseResult
    {
        Success,
        Obstacle,
        Loop
    }
}