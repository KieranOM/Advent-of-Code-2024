using System.Text.RegularExpressions;

namespace AoC;

public partial class Day15 : Day
{
    public override int Number => 15;
    protected override InputType Input => InputType.Text;

    private static readonly Regex WarehousePattern = WarehouseRegex();
    private static readonly Regex DirectionsPattern = DirectionsRegex();

    protected override void Run(in string input)
    {
        var directions = ParseDirections(input);

        var warehouse = ParseWarehouse(input, wide: false);
        warehouse.Execute(directions);
        Log(warehouse.CalculateGPSCoordinates());

        var widehouse = ParseWarehouse(input, wide: true);
        widehouse.Execute(directions);
        Log(widehouse.CalculateGPSCoordinates());
    }

    private static Warehouse ParseWarehouse(in string input, in bool wide)
    {
        return ParseWarehouse(WarehousePattern.Match(input).Value.Split('\n', StringSplitOptions.TrimEntries), wide);
    }

    private static Warehouse ParseWarehouse(in string[] input, in bool wide)
    {
        const char wall = '#', box = 'O', robot = '@';

        int widthScalar = wide ? 2 : 1;

        int width = input[0].Length, height = input.Length, invert = input.Length - 1;
        var warehouse = new Warehouse(width * widthScalar, height);

        for (int y = 0; y < height; y++)
        {
            string row = input[invert - y];
            for (int x = 0; x < width; x++)
            {
                var position = new Vector2I(x * widthScalar, y);
                switch (row[x])
                {
                    case wall:
                        warehouse.Walls.Add(position);
                        if (wide) warehouse.Walls.Add(position + Vector2I.Right);
                        break;
                    case box:
                        var size = new Vector2I(widthScalar, 1);
                        warehouse.Boxes.Add(new Box(position, size));
                        break;
                    case robot:
                        warehouse.SetRobot(position); break;
                }
            }
        }

        return warehouse;
    }

    private static List<Vector2I> ParseDirections(in string input)
    {
        const char up = '^', down = 'v', left = '<', right = '>';
        string match = DirectionsPattern.Match(input).Value;

        var list = new List<Vector2I>(match.Length);
        foreach (char character in match)
        {
            if (char.IsWhiteSpace(character)) continue;
            list.Add(character switch
            {
                up => Vector2I.Up,
                down => Vector2I.Down,
                left => Vector2I.Left,
                right => Vector2I.Right,
            });
        }

        return list;
    }

    private class Warehouse(int width, int height)
    {
        public readonly HashSet<Vector2I> Walls = [];
        public readonly List<Box> Boxes = [];

        private readonly List<Box> _touchingBoxesBuffer = [];

        private Vector2I _robot;

        private bool IsWall(in Vector2I position) => Walls.Contains(position);

        private bool IsBox(in Vector2I position, out Box result) => IsBox(position, Array.Empty<Box>(), out result);

        private bool IsBox(in Vector2I position, in IReadOnlyList<Box> excluding, out Box result)
        {
            foreach (var box in Boxes)
            {
                if (box.Contains(position) == false) continue;
                if (excluding.Contains(box)) continue;
                result = box;
                return true;
            }

            result = default;
            return false;
        }

        public void SetRobot(in Vector2I position)
        {
            _robot = position;
        }

        public void Execute(in List<Vector2I> directions)
        {
            foreach (var direction in directions) Execute(direction);
        }

        private void Execute(in Vector2I direction)
        {
            var position = _robot + direction;
            if (IsWall(position)) return;
            if (IsBox(position, out var box)) TryPushBoxes(box, direction);
            else SetRobot(position);
        }

        private void TryPushBoxes(in Box box, Vector2I direction)
        {
            var touching = FindTouchingBoxes(box, direction);
            if (CanPushBoxes(touching, direction) == false) return;

            foreach (var touched in touching) Boxes.Remove(touched);
            foreach (var touched in touching) Boxes.Add(touched.Move(direction));

            SetRobot(_robot + direction);
        }

        private bool CanPushBoxes(in IReadOnlyList<Box> boxes, in Vector2I direction)
        {
            foreach (var box in boxes)
            {
                foreach (var position in box.Positions())
                {
                    var projected = position + direction;
                    if (IsWall(projected)) return false;
                }
            }

            return true;
        }

        private List<Box> FindTouchingBoxes(in Box box, in Vector2I direction)
        {
            _touchingBoxesBuffer.Clear();
            _touchingBoxesBuffer.Add(box);
            AddTouchingBoxes(box, _touchingBoxesBuffer, direction);
            return _touchingBoxesBuffer;
        }

        private void AddTouchingBoxes(in Box box, in List<Box> touching, in Vector2I direction)
        {
            (var start, var explore, int explorations) = Explore(box, direction);
            for (int i = 0; i < explorations; i++)
            {
                var position = start + explore * i;
                if (IsBox(position, excluding: touching, out var touched) == false) continue;

                touching.Add(touched);
                AddTouchingBoxes(touched, touching, direction);
            }
        }

        private static (Vector2I start, Vector2I explore, int explorations) Explore(in Box box, in Vector2I direction)
        {
            return direction == Vector2I.Up ? (box.TopLeft + direction, Vector2I.Right, box.Size.X) :
                direction == Vector2I.Down ? (box.BottomLeft + direction, Vector2I.Right, box.Size.X) :
                direction == Vector2I.Left ? (box.BottomLeft + direction, Vector2I.Up, box.Size.Y) :
                direction == Vector2I.Right ? (box.BottomRight + direction, Vector2I.Up, box.Size.Y) : default;
        }

        public int CalculateGPSCoordinates()
        {
            return Boxes.Sum(box => 100 * (height - 1 - box.Position.Y) + box.Position.X);
        }
    }

    private readonly record struct Box(in Vector2I Position, in Vector2I Size)
    {
        public Vector2I TopLeft => Position with { Y = Position.Y + Size.Y - 1 };
        public Vector2I BottomLeft => Position;
        public Vector2I BottomRight => Position with { X = Position.X + Size.X - 1 };

        public bool Contains(in Vector2I position)
        {
            return position.X >= Position.X && position.X < Position.X + Size.X &&
                   position.Y >= Position.Y && position.Y < Position.Y + Size.Y;
        }

        public IEnumerable<Vector2I> Positions()
        {
            for (int x = 0; x < Size.X; x++)
            for (int y = 0; y < Size.Y; y++)
                yield return Position + new Vector2I(x, y);
        }

        public Box Move(in Vector2I direction) =>
            this with { Position = Position + direction };
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static readonly Vector2I Up = new(0, 1);
        public static readonly Vector2I Down = new(0, -1);
        public static readonly Vector2I Right = new(1, 0);
        public static readonly Vector2I Left = new(-1, 0);

        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);

        public static Vector2I operator *(in Vector2I vector, in int scalar) =>
            new(vector.X * scalar, vector.Y * scalar);
    }

    [GeneratedRegex(@"^#+$\n(.+$\n)+^#+$", RegexOptions.Multiline)]
    private static partial Regex WarehouseRegex();

    [GeneratedRegex(@"^[\^v<>\n]+", RegexOptions.Multiline)]
    private static partial Regex DirectionsRegex();
}