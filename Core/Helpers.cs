using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace maidoc.Core;

public static class Helpers {
    public static void ForEachCellIndex(
        int width,
        int height,
        Action<int, int> action
    ) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                action(x, y);
            }
        }
    }

    public static void ForEach<T>(this IEnumerable<T> stuff, Action<T> action) {
        foreach (var it in stuff) {
            action(it);
        }
    }

    public static int Abs(this int i) => Math.Abs(i);

    /// <summary>
    /// The same as <see cref="ImmutableArray{T}.RemoveAt"/>, but uses a fancy <see cref="Index"/>, and also returns the <see cref="removed"/> <typeparamref name="T"/>.
    /// </summary>
    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt" />
    public static ImmutableArray<T> RemoveAt<T>(this ImmutableArray<T> array, Index index, out T removed) {
        var offset = index.GetOffset(array.Length);
        return array.RemoveAt(offset, out removed);
    }

    /// <summary>
    /// The same as <see cref="ImmutableArray{T}.RemoveAt"/>, but also returns the <see cref="removed"/> <typeparamref name="T"/>.
    /// </summary>
    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt" />
    public static ImmutableArray<T> RemoveAt<T>(this ImmutableArray<T> array, int index, out T removed) {
        removed = array[index];
        return array.RemoveAt(index);
    }

    public static bool IsDistinct<T>(this ReadOnlySpan<T> span) where T : IEquatable<T> {
        for (int i = 0; i < span.Length; i++) {
            var current   = span[i];
            var remaining = span[i..];
            if (remaining.Contains(current)) {
                return false;
            }
        }

        return true;
    }

    public static ImmutableArray<T> RequireDistinct<T>(in this                         ImmutableArray<T> array,
        [CallerArgumentExpression(nameof(array))] string            _array = "") {
        for (var i = 0; i < array.Length-1; i++) {
            var current        = array[i];
            var duplicateIndex = array.IndexOf(current, i + 1);
            if (duplicateIndex != -1) {
                throw new ArgumentException(
                    $"The element {current} is duplicated at indices {i} and {duplicateIndex}!");
            }
        }

        return array;
    }

    public static E Transition<E>(this E currentState, E fromState, E toState) where E : struct, Enum {
        Require.State(EqualityComparer<E>.Default.Equals(currentState, fromState));
        return toState;
    }
}