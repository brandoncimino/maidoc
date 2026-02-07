using System.Collections.Immutable;
using System.Linq;
using maidoc.Scenes;

namespace maidoc;

public static class Lineup {
    public static ImmutableArray<LineDistance> LineupFromCenter(
        ImmutableArray<Distance> itemSizes,
        LineDistance             availableSpace,
        Distance                 paddingBetweenItems = default
    ) {
        if (itemSizes.IsDefaultOrEmpty) {
            return [];
        }

        var sizeFromItems        = itemSizes.Sum(it => it.Meters).Meters;
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
            if (i > 1) {
                startPoint += paddingBetweenItems;
            }

            // builder.Add(new LineDistance(startPoint, itemSizes[i]));
            builder.Add(new LineDistance(startPoint * smushFactor, itemSizes[i]));
            startPoint += itemSizes[i];
        }

        return builder.DrainToImmutable();
    }
}