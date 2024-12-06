using System.Text.RegularExpressions;

namespace AoC;

public partial class Day5 : Day
{
    public override int Number => 5;

    private readonly Regex _rulesPattern = RulesRegex();
    private readonly Regex _updatesPattern = UpdatesRegex();

    protected override void Run(in string[] input)
    {
        ParseInput(input, out var rules, out var updates);

        int validMiddlePageSum = GetValidUpdates(updates, rules, validity: true)
            .Sum(MiddleValue);

        Log(validMiddlePageSum);

        int correctedMiddlePageSum = GetValidUpdates(updates, rules, validity: false)
            .Select(update => CorrectOrder(update, rules))
            .Sum(MiddleValue);

        Log(correctedMiddlePageSum);
    }

    private static IEnumerable<List<int>> GetValidUpdates(IEnumerable<List<int>> updates,
        Dictionary<int, List<int>> rules,
        bool validity)
    {
        return updates.Where(update => IsValidUpdate(update, rules) == validity);
    }

    private static bool IsValidUpdate(List<int> update, Dictionary<int, List<int>> rules)
    {
        for (int i = 0; i < update.Count; i++)
        {
            if (rules.TryGetValue(update[i], out var rule) == false) continue;
            if (update.Take(i).Any(rule.Contains)) return false;
        }

        return true;
    }

    private static List<int> CorrectOrder(List<int> update, Dictionary<int, List<int>> rules)
    {
        update.Sort(ByRules);
        return update;

        int ByRules(int left, int right)
        {
            if (rules.TryGetValue(left, out var rule) && rule.Contains(right)) return -1;
            return 1;
        }
    }

    private static int MiddleValue(List<int> update) => update[update.Count / 2];

    private void ParseInput(in string[] input, out Dictionary<int, List<int>> rules, out List<List<int>> updates)
    {
        string text = string.Join("\n", input);

        rules = _rulesPattern.Matches(text)
            .Select(ParseRule)
            .GroupBy(rule => rule.Page, rule => rule.ComesBefore)
            .ToDictionary(group => group.Key, group => group.ToList());

        updates = _updatesPattern.Matches(text)
            .Select(ParseUpdate)
            .ToList();
    }

    private static (int Page, int ComesBefore) ParseRule(Match match)
    {
        return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
    }

    private static List<int> ParseUpdate(Match match)
    {
        return match.Value.Split(',').Select(int.Parse).ToList();
    }

    [GeneratedRegex(@"^(\d+)\|(\d+)$", RegexOptions.Multiline)]
    private static partial Regex RulesRegex();

    [GeneratedRegex(@"^\d+(?:,\d+)+$", RegexOptions.Multiline)]
    private static partial Regex UpdatesRegex();
}