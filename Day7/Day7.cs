namespace AoC;

public class Day7 : Day
{
    public override int Number => 7;

    [Flags]
    private enum Operator
    {
        Add = 1,
        Multiply = 2,
        Concatenate = 4
    }

    protected override void Run(in string[] input)
    {
        var equations = input.Select(ParseEquation).ToArray();

        long firstCalibrations =
            SumSolvableResults(equations, Operator.Add | Operator.Multiply);
        Log(firstCalibrations);

        long secondCalibrations =
            SumSolvableResults(equations, Operator.Add | Operator.Multiply | Operator.Concatenate);
        Log(secondCalibrations);
    }

    private static long SumSolvableResults(in Equation[] equations, Operator ops)
    {
        return equations.Where(equation => equation.IsSolvable(ops))
            .Sum(equation => equation.Result);
    }

    private static Equation ParseEquation(string line)
    {
        string[] split = line.Split(": ");
        long result = long.Parse(split[0]);
        long[] numbers = split[1].Split(' ').Select(long.Parse).ToArray();
        return new Equation(result, numbers);
    }

    private readonly record struct Equation(long Result, ArraySegment<long> Numbers)
    {
        public bool IsSolvable(in Operator ops) => IsSolvable(ops, Numbers[0], Numbers[1..]);

        private bool IsSolvable(in Operator ops, in long head, in ArraySegment<long> tail)
        {
            if (tail.Count == 0) return head == Result;
            return IsSolvable(ops, Operator.Add, head, tail) ||
                   IsSolvable(ops, Operator.Multiply, head, tail) ||
                   IsSolvable(ops, Operator.Concatenate, head, tail);
        }

        private bool IsSolvable(in Operator ops, in Operator op, in long head, in ArraySegment<long> tail) =>
            ops.HasFlag(op) && IsSolvable(ops, Evaluate(op, head, tail[0]), tail[1..]);

        private static long Evaluate(in Operator op, in long lhs, in long rhs) =>
            op switch
            {
                Operator.Add => lhs + rhs,
                Operator.Multiply => lhs * rhs,
                Operator.Concatenate => long.Parse($"{lhs}{rhs}")
            };
    }
}