using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace maidoc.Core.Cards;

public class PaperCardGroup(IEnumerable<PaperCard> cards) {
    /// <summary>
    /// Decided to go with <see cref="ImmutableArray"/> over <see cref="ImmutableList{T}"/> mostly because of <see cref="ImmutableArray{T}.Slice"/>,
    /// which is the only tangible benefit - though in theory:
    /// <ul>
    /// <li><see cref="ImmutableList{T}"/>s are better when you're going to do lots of <b><i>mutation</i></b>, because they are allowed to contain bits and pieces of other <see cref="ImmutableList{T}"/>s;</li>
    /// <li><see cref="ImmutableArray{T}"/>s are better when you're doing lots of <b><i>iteration</i></b>, because they are always supposed to have their elements in "contiguous memory",
    /// which makes computers happy (especially in contexts where <see cref="System.Numerics.Vector"/>ization can be used).</li>
    /// </ul>
    /// </summary>
    private ImmutableArray<PaperCard> _cards = cards.ToImmutableArray().RequireDistinct();

    [MustUseReturnValue]
    public PaperCard DrawAt(Index index) {
        var offset = index.GetOffset(_cards.Length);
        var drawn  = _cards[offset];
        _cards             = _cards.RemoveAt(offset);
        return drawn;
    }

    [MustUseReturnValue]
    public ReadOnlySpan<PaperCard> DrawRange(Range range) {
        var drawn = _cards.AsSpan(range);
        var (offset, length) = range.GetOffsetAndLength(_cards.Length);
        _cards               = _cards.RemoveRange(offset, length);
        return drawn;
    }

    public void Insert(Index index, PaperCard card) {
        if (_cards.Contains(card)) {
            throw new ArgumentException(
                $"Can't insert {card} into the deck at index {index} because it is ALREADY in the deck!");
        }

        _cards = _cards.Insert(index.GetOffset(_cards.Length), card);
    }

    public void Shuffle(
        Random? random = null
    ) {
        random ??= Random.Shared;

        var shuffled = _cards.ToBuilder();
        for (int a = shuffled.Count - 1; a >= 0; a--) {
            var b = random.Next(a + 1);

            (shuffled[a], shuffled[b]) = (shuffled[b], shuffled[a]);
        }

        _cards = shuffled.DrainToImmutable();
    }

    public void Remove(
        PaperCard card
    ) {
        var beforeRemove = _cards;
        _cards = _cards.Remove(card);
        if (beforeRemove.Length == _cards.Length) {
            throw new InvalidOperationException($"Couldn't remove the card {card} because it wasn't here!");
        }
    }

    public void Add(PaperCard paperCard) {
        if (_cards.Contains(paperCard)) {
            throw new ArgumentException($"Can't add {paperCard} because it is already here!");
        }

        _cards = _cards.Add(paperCard);
    }

    public void AddRange(ReadOnlySpan<PaperCard> cards) {
        foreach (var card in cards) {
            if (_cards.Contains(card)) {
                throw new ArgumentException($"Can't add {card} because it is already here!");
            }
        }

        _cards = _cards.AddRange(cards);
    }
}