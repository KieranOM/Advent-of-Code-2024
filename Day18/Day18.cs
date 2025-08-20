namespace AoC;

public class Day18 : Day
{
    public override int Number => 18;

    protected override void Run(in string[] input)
    {
        var grid = new Grid(71, 71);
        AddFallenBytes(grid, input, 1024);

        var start = new Vector2I(0, 0);
        var end = new Vector2I(70, 70);

        var traversal = grid.Traverse(start, end);
        var steps = traversal.Count - 1;
        Log(steps);

        for (int i = 1025; i < input.Length; ++i)
        {
            var position = AddFallenByte(grid, input, i);
            if (traversal.Contains(position) == false) continue;

            traversal = grid.Traverse(start, end);
            if (traversal.Count == 0)
            {
                Log(position);
                break;
            }
        }
    }

    private static void AddFallenBytes(in Grid grid, in string[] input, in int count)
    {
        for (int i = 0; i < count; ++i)
        {
            AddFallenByte(grid, input, i);
        }
    }

    private static Vector2I AddFallenByte(in Grid grid, in string[] input, in int index)
    {
        var fallen = Vector2I.Parse(input[index]);
        grid.AddObstacle(fallen);
        return fallen;
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        private static Vector2I Up => new(0, 1);
        private static Vector2I Down => new(0, -1);
        private static Vector2I Left => new(-1, 0);
        private static Vector2I Right => new(1, 0);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);

        public static Vector2I operator -(in Vector2I left, in Vector2I right) =>
            new(left.X - right.X, left.Y - right.Y);

        public static Vector2I Parse(in ReadOnlySpan<char> input)
        {
            const char separator = ',';
            int index = input.IndexOf(separator);
            int x = int.Parse(input[..index]);
            int y = int.Parse(input[(index + 1)..]);
            return new Vector2I(x, y);
        }

        public IEnumerable<Vector2I> Adjacent()
        {
            yield return this + Up;
            yield return this + Down;
            yield return this + Left;
            yield return this + Right;
        }
    }

    private class Grid(int width, int height)
    {
        private readonly HashSet<Vector2I> _obstacles = [];

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
            return _obstacles.Contains(position);
        }

        public List<Vector2I> Traverse(in Vector2I start, in Vector2I end)
        {
            var costs = new Dictionary<Vector2I, int>
            {
                [start] = 0
            };
            var history = new Dictionary<Vector2I, Vector2I>();

            var queue = new PriorityQueue<Vector2I, int>();
            queue.Enqueue(start, 0);

            while (queue.TryDequeue(out var current, out _))
            {
                int cost = costs[current];
                if (current == end)
                {
                    return BuildTraversal(current, history);
                }

                int nextCost = cost + 1;

                foreach (var next in current.Adjacent())
                {
                    if (IsInBounds(next) == false || IsObstacle(next)) continue;
                    if (IsCheaper(next, nextCost) == false) continue;

                    costs[next] = nextCost;
                    history[next] = current;

                    int priority = nextCost + Heuristic(next, end);
                    queue.Enqueue(next, priority);
                }
            }

            return [];

            bool IsCheaper(in Vector2I position, in int cost)
            {
                return costs.TryGetValue(position, out int existingCost) == false || existingCost > cost;
            }
        }

        private static int Heuristic(in Vector2I from, in Vector2I to)
        {
            var delta = from - to;
            return Math.Abs(delta.X) + Math.Abs(delta.Y);
        }

        private static List<Vector2I> BuildTraversal(Vector2I current, Dictionary<Vector2I, Vector2I> history)
        {
            var traversal = new List<Vector2I> { current };
            while (history.TryGetValue(current, out current))
            {
                traversal.Insert(0, current);
            }

            return traversal;
        }
    }
}