using System.Text.RegularExpressions;

namespace AoC;

public partial class Day13 : Day
{
    public override int Number => 13;
    protected override InputType Input => InputType.Text;

    private static readonly Regex ConfigurationPattern = ConfigurationRegex();

    protected override void Run(in string input)
    {
        var configs = ParseInput(input);

        long total = MinimumTokens(configs, prizeBase: 0L);
        Log(total);

        total = MinimumTokens(configs, prizeBase: 10000000000000L);
        Log(total);
    }

    private static long MinimumTokens(List<Configuration> configs, long prizeBase) =>
        configs.Select(config => MinimumTokens(config, prizeBase)).Sum();

    private static long MinimumTokens(Configuration config, in long prizeBase)
    {
        var first = new Equation(config.A.X, config.B.X, prizeBase + config.Prize.X);
        var second = new Equation(config.A.Y, config.B.Y, prizeBase + config.Prize.Y);

        return TrySolveEquations(first, second, out long a, out long b)
            ? a * 3 + b
            : 0;
    }

    private static bool TrySolveEquations(in Equation first, in Equation second, out long a, out long b)
    {
        var commonAFirst = first * second.A;
        var commonASecond = second * first.A;

        if (commonAFirst.B != commonASecond.B)
        {
            var solveForB = commonAFirst - commonASecond;
            b = solveForB.Result / solveForB.B;
            a = (first.Result - b * first.B) / first.A;

            return first.Verify(a, b) && second.Verify(a, b);
        }

        a = 0;
        b = 0;
        return false;
    }

    private readonly record struct Equation(in long A, in long B, in long Result)
    {
        public static Equation operator *(in Equation equation, in long scalar) =>
            new(equation.A * scalar, equation.B * scalar, equation.Result * scalar);

        public static Equation operator -(in Equation first, in Equation second) =>
            new(first.A - second.A, first.B - second.B, first.Result - second.Result);

        public bool Verify(in long a, in long b) => a * A + b * B == Result;
    }

    private static List<Configuration> ParseInput(in string input) =>
        ConfigurationPattern.Matches(input)
            .Select(Configuration.Parse).ToList();

    private readonly record struct Configuration(in Vector2I A, in Vector2I B, in Vector2I Prize)
    {
        public static Configuration Parse(Match match)
        {
            var a = new Vector2I(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            var b = new Vector2I(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            var prize = new Vector2I(int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value));
            return new Configuration(a, b, prize);
        }
    }

    private readonly record struct Vector2I(in int X, in int Y);

    [GeneratedRegex(@"Button A: X\+(\d+), Y\+(\d+)$\nButton B: X\+(\d+), Y\+(\d+)$\nPrize: X=(\d+), Y=(\d+)",
        RegexOptions.Multiline)]
    private static partial Regex ConfigurationRegex();
}