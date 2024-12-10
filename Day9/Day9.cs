namespace AoC;

public class Day9 : Day
{
    public override int Number => 9;
    protected override InputType Input => InputType.Text;

    protected override void Run(in string input)
    {
        var disk = ParseDisk(input);

        var fragmented = new List<int?>(disk);
        CompressFragment(fragmented);
        long checksum = Checksum(fragmented);
        Log(checksum);

        var whole = new List<int?>(disk);
        CompressWhole(whole);
        checksum = Checksum(whole);
        Log(checksum);
    }

    private static void CompressFragment(in List<int?> disk)
    {
        int freeIndex = 0;
        for (int i = disk.Count - 1; i >= 0; i--)
        {
            int? block = disk[i];
            if (block.HasValue == false) continue;

            freeIndex = disk.IndexOf(null, freeIndex);
            if (freeIndex == -1 || freeIndex >= i) return;

            Write(disk, block, freeIndex, 1);
            Write(disk, null, i, 1);
        }
    }

    private record struct Blocks(int Index, int Count);
    private record struct File(int ID, Blocks Blocks);

    private static void CompressWhole(List<int?> disk)
    {
        GroupDisk(disk, out var files, out var frees);

        for (int i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];
            (int fileIndex, int fileSize) = file.Blocks;

            int index = frees.FindIndex(block => block.Count >= file.Blocks.Count);
            if (index == -1) continue;

            (int freeIndex, int freeSize) = frees[index];
            if (freeIndex >= fileIndex) continue;

            if (freeSize > fileSize)
                frees[index] = new Blocks(freeIndex + fileSize, freeSize - fileSize);
            else
                frees.RemoveAt(index);

            Write(disk, file.ID, freeIndex, fileSize);
            Write(disk, null, fileIndex, fileSize);
        }
    }

    private static void GroupDisk(in List<int?> disk, out List<File> files, out List<Blocks> frees)
    {
        files = [];
        frees = [];

        for (int i = 0, last = disk.Count - 1; i <= last; i++)
        {
            int count = 1;
            while (i < last && disk[i] == disk[i + 1])
            {
                ++count;
                ++i;
            }

            int? block = disk[i];
            var blocks = new Blocks(i - count + 1, count);
            if (block.HasValue) files.Add(new File(block.Value, blocks));
            else frees.Add(blocks);
        }
    }

    private static void Write<T>(in List<T> list, in T value, in int index, in int count)
    {
        for (int i = 0; i < count; i++) list[i + index] = value;
    }

    private static long Checksum(in List<int?> disk)
    {
        long checksum = 0;
        for (int i = 0; i < disk.Count; i++) checksum += i * (disk[i] ?? 0L);
        return checksum;
    }

    private static List<int?> ParseDisk(in string input)
    {
        List<int?> disk = [];
        for (int i = 0; i < input.Length; i++)
        {
            bool isFile = i % 2 == 0;
            int blocks = input[i] - '0';
            int? id = isFile
                ? i / 2
                : null;

            for (int j = 0; j < blocks; j++) disk.Add(id);
        }

        return disk;
    }
}