using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public partial class GraveyardSceneRoot : Node2D, ISceneRoot<GraveyardSceneRoot, GraveyardSceneRoot.SpawnInput>,
    ICardZoneNode {
    private readonly Disenfranchised<PlayerId> _playerId = new();
    public           PlayerId                  PlayerId => _playerId.Value;

    public static GraveyardSceneRoot InstantiateRawScene() {
        return new GraveyardSceneRoot();
    }

    public readonly record struct SpawnInput {
        public required PlayerId   PlayerId     { get; init; }
        public required Distance2D UnscaledSize { get; init; }
    }

    public GraveyardSceneRoot InitializeSelf(SpawnInput input) {
        _playerId.Enfranchise(input.PlayerId);

        return this;
    }

    public           Node2D                      AsNode2D => this;
    private readonly Disenfranchised<Distance2D> _unscaledSize = new();
    public           Distance2D                  UnscaledSize => _unscaledSize.Value;

    public ZoneAddress ZoneAddress => new() {
        PlayerId = PlayerId,
        ZoneId   = DuelDiskZoneId.Graveyard
    };

    public void AddCard(ICardSceneRoot card) {
        throw new NotImplementedException();
    }

    public bool TryGetCard(SerialNumber serialNumber, [NotNullWhen(true)] out ICardSceneRoot? card) {
        throw new NotImplementedException();
    }
}