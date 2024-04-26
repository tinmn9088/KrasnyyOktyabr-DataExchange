using System.Collections.Generic;
using System;

namespace KrasnyyOktyabr.ApplicationNet48.Linq;

/// <summary>
/// See <see cref="https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.Linq/src/System/Linq/Chunk.cs#L40C13-L50C48">Chunk.cs</see>.
/// </summary>
public static partial class Enumerable
{
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (size < 1)
        {
            throw new ArgumentOutOfRangeException();
        }

        return ChunkIterator(source, size);
    }

    private static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();

        if (e.MoveNext())
        {
            int arraySize = Math.Min(size, 4);

            int i;

            do
            {
                TSource[] array = new TSource[arraySize];

                array[0] = e.Current;

                i = 1;

                if (size != array.Length)
                {
                    for (; i < size && e.MoveNext(); i++)
                    {
                        if (i >= array.Length)
                        {
                            arraySize = (int)Math.Min((uint)size, 2 * (uint)array.Length);
                            Array.Resize(ref array, arraySize);
                        }

                        array[i] = e.Current;
                    }
                }
                else
                {
                    TSource[] local = array;

                    for (; (uint)i < (uint)local.Length && e.MoveNext(); i++)
                    {
                        local[i] = e.Current;
                    }
                }

                if (i != array.Length)
                {
                    Array.Resize(ref array, i);
                }

                yield return array;
            }
            while (i >= size && e.MoveNext());
        }
    }
}
