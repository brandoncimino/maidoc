using System;
using System.Collections.Immutable;
using System.Numerics;
using maidoc.Scenes;

namespace maidoc;

public static class Lineup {
    private static T Sum<T>(this ReadOnlySpan<T> span) where T : struct, IAdditionOperators<T, T, T> {
        var sum = default(T);

        foreach (var item in span) {
            sum += item;
        }

        return sum;
    }

    public static ImmutableArray<LineDistance> LineupFromCenter(
        ReadOnlySpan<Distance> itemSizes,
        LineDistance           availableSpace,
        Distance               paddingBetweenItems = default
    ) {
        if (itemSizes.IsEmpty) {
            return [];
        }

        var sizeFromItems        = itemSizes.Sum();
        var sizeFromPadding      = (itemSizes.Length - 1) * paddingBetweenItems;
        var totalComfortableSize = sizeFromItems + sizeFromPadding;

        var builder    = ImmutableArray.CreateBuilder<LineDistance>(itemSizes.Length);
        var startPoint = availableSpace.Center - totalComfortableSize / 2;
        var smushFactor = (availableSpace.Size < totalComfortableSize) switch {
            true  => availableSpace.Size / totalComfortableSize,
            false => 1
        };

        startPoint.blog(
            sizeFromItems,
            sizeFromPadding,
            totalComfortableSize,
            startPoint,
            smushFactor
        );

        for (int i = 0; i < itemSizes.Length; i++) {
            if (i > 0) {
                startPoint += paddingBetweenItems;
            }

            builder.Add(new LineDistance(startPoint * smushFactor, itemSizes[i]));
            startPoint += itemSizes[i];
        }

        return builder.DrainToImmutable();
    }
}