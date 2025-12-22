using System;
using System.Collections.Immutable;
using Godot;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public partial class Deck : Node2D, ISceneRoot<Deck, ImmutableArray<PaperCard>> {
    public Deck InitializeSelf(ImmutableArray<PaperCard> input) {
        throw new NotImplementedException();
    }
}