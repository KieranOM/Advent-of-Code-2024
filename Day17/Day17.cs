namespace AoC;

public class Day17 : Day
{
    public override int Number => 17;

    protected override void Run(in string[] input)
    {
        var program = ParseProgram(input);

        var output = program.Execute();
        Log(string.Join(',', output));

        long reversed = Reverse(program);
        Log(reversed);
    }

    private static long Reverse(Program program)
    {
        var expected = program.Instructions;
        int digits = expected.Count;

        // The program counts in an undetermined non-distinct sequence through a 3-bit binary number system
        // Given this, the interval of inputs that produce n digits is [(2^3)^(n-1), ((2^3)^n)-1]
        long start = EightRaisedBy(digits - 1);

        // The most significant digit is at the end, so we work backwards
        int index = expected.Count - 1;

        // Recursively solves the reversal by manipulating each digit in place in a depth-first manner
        return Reverse(expected, program, start, index);
    }

    private static long Reverse(List<int> expected, Program program, long start, int index)
    {
        const int valuesPerDigit = 8;
        const long invalid = -1;

        if (index == -1) return start;

        long step = EightRaisedBy(index);
        for (int i = 0; i < valuesPerDigit - 1; ++i)
        {
            long candidate = start + step * i;
            program.A = candidate;

            var outputs = program.Execute();
            if (expected[index] != outputs[index]) continue;

            long answer = Reverse(expected, program, candidate, index - 1);
            if (answer != invalid) return answer;
        }

        return invalid;
    }

    private static long EightRaisedBy(in int power)
    {
        // 8^n = 2^3n, 2^n = 1 << n
        return 1L << (3 * power);
    }

    private class Program
    {
        public long A, B, C;
        private int _programCounter;
        private readonly List<int> _outputs = [];
        public List<int> Instructions = [];

        public List<int> Execute()
        {
            _programCounter = 0;
            _outputs.Clear();

            while (_programCounter < Instructions.Count)
            {
                var opcode = (Opcode)Instructions[_programCounter++];
                int operand = Instructions[_programCounter++];
                Execute(opcode, operand);
            }

            return _outputs;
        }

        private void Execute(in Opcode opcode, in int operand)
        {
            switch (opcode)
            {
                case Opcode.Adv:
                    A /= 1L << (int)Combo(operand);
                    break;
                case Opcode.Bxl:
                    B ^= operand;
                    break;
                case Opcode.Bst:
                    B = Combo(operand) % 8L;
                    break;
                case Opcode.Jnz:
                    if (A != 0) _programCounter = operand;
                    break;
                case Opcode.Bxc:
                    B ^= C;
                    break;
                case Opcode.Out:
                    _outputs.Add((int)(Combo(operand) % 8L));
                    break;
                case Opcode.Bdv:
                    B = A / (1L << (int)Combo(operand));
                    break;
                case Opcode.Cdv:
                    C = A / (1L << (int)Combo(operand));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        private long Combo(in int operand)
        {
            return operand switch
            {
                >= 0 and <= 3 => operand,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new ArgumentOutOfRangeException(nameof(operand))
            };
        }

        private enum Opcode : byte
        {
            Adv = 0,
            Bxl = 1,
            Bst = 2,
            Jnz = 3,
            Bxc = 4,
            Out = 5,
            Bdv = 6,
            Cdv = 7
        }
    }

    private static Program ParseProgram(in string[] lines)
    {
        return new Program
        {
            A = ParseRegister(lines[0]),
            B = ParseRegister(lines[1]),
            C = ParseRegister(lines[2]),
            Instructions = ParseInstructions(lines[4])
        };

        static int ParseRegister(in ReadOnlySpan<char> line) => int.Parse(AfterColon(line));

        static List<int> ParseInstructions(in ReadOnlySpan<char> line)
        {
            var instructions = new List<int>();
            var instructionsLine = AfterColon(line);
            foreach (var range in instructionsLine.Split(','))
            {
                instructions.Add(int.Parse(instructionsLine[range]));
            }

            return instructions;
        }

        static ReadOnlySpan<char> AfterColon(in ReadOnlySpan<char> line) => line[(line.LastIndexOf(':') + 1)..];
    }
}