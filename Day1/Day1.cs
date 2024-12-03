namespace AoC;

public class Day1 : Day
{
    public override int Number => 1;

    protected override void Run(in string[] input)
    {
        int count = input.Length;
        List<int> lefts = new(count), rights = new(count);

        for (int i = 0; i < count; i++)
        {
            ParseLine(input[i], out int left, out int right);
            lefts.Add(left);
            rights.Add(right);
        }

        lefts.Sort();
        rights.Sort();

        int distance = 0, similarity = 0;
        for (int i = 0; i < count; i++)
        {
            int left = lefts[i], right = rights[i];
            distance += Distance(left, right);
            similarity += Similarity(left, rights);
        }

        Log(distance);
        Log(similarity);
    }

    private static void ParseLine(in string line, out int left, out int right)
    {
        string[] split = line.Split("   ");
        left = int.Parse(split[0]);
        right = int.Parse(split[1]);
    }

    private static int Distance(in int left, in int right)
    {
        return Math.Abs(left - right);
    }

    private static int Similarity(int left, in IList<int> rights)
    {
        return left * rights.Count(right => right == left);
    }
}