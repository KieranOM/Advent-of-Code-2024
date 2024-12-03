namespace AoC;

public class Day2 : Day
{
    public override int Number => 2;

    private const int MinimumDelta = 1, MaximumDelta = 3;

    private readonly List<int> _dampener = [];

    protected override void Run(in string[] input)
    {
        int[][] reports = ParseReports(input);

        int safe = 0, safeWithDampener = 0;
        foreach (int[] report in reports)
        {
            if (IsReportSafe(report))
            {
                ++safe;
                ++safeWithDampener;
            }
            else if (IsReportSafeWithDampener(report))
            {
                ++safeWithDampener;
            }
        }

        Log(safe);
        Log(safeWithDampener);
    }

    private static int[][] ParseReports(string[] input) => input.Select(ParseReport).ToArray();
    private static int[] ParseReport(string line) => line.Split(" ").Select(int.Parse).ToArray();

    private static bool IsReportSafe(IList<int> report)
    {
        int direction = report[0] < report[1]
            ? 1
            : -1;

        for (int i = 0, end = report.Count - 1; i < end; i++)
        {
            int delta = (report[i + 1] - report[i]) * direction;
            if (IsSafeDelta(delta) == false) return false;
        }

        return true;
    }

    private static bool IsSafeDelta(in int delta) => delta is >= MinimumDelta and <= MaximumDelta;

    private bool IsReportSafeWithDampener(int[] report)
    {
        for (int i = 0; i < report.Length; i++)
        {
            _dampener.Clear();
            _dampener.AddRange(report);
            _dampener.RemoveAt(i);
            if (IsReportSafe(_dampener)) return true;
        }

        return false;
    }
}