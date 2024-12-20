namespace AoC;

public class Day16 : Day
{
    public override int Number => 16;

    protected override void Run(in string[] input)
    {
        var maze = ParseMaze(input);
        var solution = maze.Solve();
        Log(solution.Score);

        int unique = solution.Paths.SelectMany(path => path).Distinct().Count();
        Log(unique);
    }

    private class Maze
    {
        public Transform2I Start;
        public Vector2I End;

        public readonly HashSet<Vector2I> Walls = [];

        private bool IsWall(in Vector2I position) => Walls.Contains(position);

        public Solution Solve()
        {
            var traversals = new Queue<Traversal>();
            traversals.Enqueue(new Traversal(0, Start, [Start.Position]));

            var bestTransforms = new Dictionary<Transform2I, int>();
            var best = new Solution(int.MaxValue);

            while (traversals.Count > 0)
            {
                var traversal = traversals.Dequeue();
                if (traversal.Score > best.Score) continue;

                if (traversal.Transform.Position == End)
                {
                    if (traversal.Score < best.Score)
                    {
                        best = new Solution(traversal.Score);
                        best.Paths.Add(traversal.Path);
                    }
                    else if (traversal.Score == best.Score)
                    {
                        best.Paths.Add(traversal.Path);
                    }

                    continue;
                }

                if (IsWall(traversal.Transform.Forward) == false) TryEnqueue(traversal.Step());
                if (IsWall(traversal.Transform.Left) == false) TryEnqueue(traversal.Left());
                if (IsWall(traversal.Transform.Right) == false) TryEnqueue(traversal.Right());
            }

            return best;

            void TryEnqueue(in Traversal candidate)
            {
                if (bestTransforms.GetValueOrDefault(candidate.Transform, int.MaxValue) < candidate.Score) return;

                bestTransforms[candidate.Transform] = candidate.Score;
                traversals.Enqueue(candidate);
            }
        }
    }

    private readonly record struct Solution(in int Score)
    {
        public readonly List<List<Vector2I>> Paths = [];
    }

    private readonly record struct Traversal(in int Score, in Transform2I Transform, in List<Vector2I> Path)
    {
        public Traversal Step() => new(Score + 1, Transform with { Position = Transform.Forward },
            [..Path, Transform.Forward]);

        public Traversal Left() => Turn(Transform.Direction.Anticlockwise());
        public Traversal Right() => Turn(Transform.Direction.Clockwise());

        private Traversal Turn(in Vector2I direction) =>
            new(Score + 1000, Transform with { Direction = direction }, Path);
    }

    private readonly record struct Transform2I(Vector2I Position, Vector2I Direction)
    {
        public Vector2I Left => Position + Direction.Anticlockwise();
        public Vector2I Right => Position + Direction.Clockwise();
        public Vector2I Forward => Position + Direction;
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static readonly Vector2I Up = new(0, 1);
        public static readonly Vector2I Down = new(0, -1);
        public static readonly Vector2I Right = new(1, 0);
        public static readonly Vector2I Left = new(-1, 0);

        public Vector2I Clockwise() => new(-Y, X);
        public Vector2I Anticlockwise() => new(Y, -X);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);
    }

    private static Maze ParseMaze(in string[] input)
    {
        const char wall = '#', start = 'S', end = 'E';

        int width = input[0].Length, height = input.Length, invert = input.Length - 1;
        var maze = new Maze();

        for (int y = 0; y < height; y++)
        {
            string row = input[invert - y];
            for (int x = 0; x < width; x++)
            {
                var position = new Vector2I(x, y);
                switch (row[x])
                {
                    case wall:
                        maze.Walls.Add(position);
                        break;
                    case start:
                        maze.Start = new Transform2I(position, Vector2I.Right);
                        break;
                    case end:
                        maze.End = position;
                        break;
                }
            }
        }

        return maze;
    }
}