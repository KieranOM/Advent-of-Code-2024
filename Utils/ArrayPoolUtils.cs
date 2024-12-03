using System.Buffers;

namespace AoC.Utils;

public static class ArrayPool
{
    public static ArrayLease<T> Rent<T>(int minimumLength, out T[] array)
    {
        array = ArrayPool<T>.Shared.Rent(minimumLength);
        return new ArrayLease<T>(array);
    }

    public readonly struct ArrayLease<T>(T[] array) : IDisposable
    {
        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(array);
        }
    }
}