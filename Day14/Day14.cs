using System.Text.RegularExpressions;

namespace AoC;

public partial class Day14 : Day
{
    public override int Number => 14;
    protected override InputType Input => InputType.Text;

    private static readonly Regex RobotPattern = RobotRegex();

    private const int Width = 101, Height = 103;

    protected override void Run(in string input)
    {
        var robots = ParseInput(input);

        int safety = SafetyFactor([..robots]);
        Log(safety);

        int ticks = FindMinimumCentroidDeviation([..robots], ticks: 10000);
        Log(ticks);
    }

    private static int SafetyFactor(in List<Robot> robots)
    {
        const int ticks = 100;
        for (int i = 0; i < robots.Count; i++) robots[i] = robots[i].Tick(ticks);

        return robots.Count(robot => robot.Position is { X: < Width / 2, Y: < Height / 2 }) *
               robots.Count(robot => robot.Position is { X: < Width / 2, Y: > Height / 2 }) *
               robots.Count(robot => robot.Position is { X: > Width / 2, Y: < Height / 2 }) *
               robots.Count(robot => robot.Position is { X: > Width / 2, Y: > Height / 2 });
    }

    private static int Wrap(in int value, in int max) => (value % max + max) % max;

    private static int FindMinimumCentroidDeviation(List<Robot> robots, int ticks)
    {
        int lowest = int.MaxValue;
        int tick = -1;

        for (int i = 0; i < ticks; i++)
        {
            for (int j = 0; j < robots.Count; j++) robots[j] = robots[j].Tick(1);

            var centroid = Centroid(robots);
            int deviation = Deviation(robots, centroid);
            if (deviation >= lowest) continue;

            lowest = deviation;
            tick = i;
        }

        return tick + 1;
    }

    private static Vector2I Centroid(in List<Robot> robots)
    {
        int x = 0, y = 0;
        foreach (var robot in robots)
        {
            x += robot.Position.X;
            y += robot.Position.Y;
        }

        return new Vector2I(x, y) / robots.Count;
    }

    private static int Deviation(in List<Robot> robots, Vector2I centroid) =>
        robots.Sum(robot => Vector2I.ManhattanDistance(robot.Position, centroid));

    private static List<Robot> ParseInput(in string input) =>
        RobotPattern.Matches(input)
            .Select(Robot.Parse).ToList();

    private readonly record struct Robot(in Vector2I Position, in Vector2I Velocity)
    {
        public static Robot Parse(Match match)
        {
            var position = new Vector2I(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            var velocity = new Vector2I(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            return new Robot(position, velocity);
        }

        public Robot Tick(in int ticks)
        {
            var projection = Position + Velocity * ticks;
            var wrapped = new Vector2I(Wrap(projection.X, Width), Wrap(projection.Y, Height));
            return this with { Position = wrapped };
        }
    }

    private readonly record struct Vector2I(in int X, in int Y)
    {
        public static Vector2I operator +(in Vector2I left, in Vector2I right) =>
            new(left.X + right.X, left.Y + right.Y);

        public static Vector2I operator *(in Vector2I vector, in int scalar) =>
            new(vector.X * scalar, vector.Y * scalar);

        public static Vector2I operator /(in Vector2I vector, in int scalar) =>
            new(vector.X / scalar, vector.Y / scalar);

        public static int ManhattanDistance(in Vector2I from, in Vector2I to) =>
            Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.X);
    }

    [GeneratedRegex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)", RegexOptions.Multiline)]
    private static partial Regex RobotRegex();
}