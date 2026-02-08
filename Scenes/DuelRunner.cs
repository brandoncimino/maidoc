using System;
using System.Collections.Concurrent;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.GameComponents;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class DuelRunner : Node2D, ISceneRoot<DuelRunner, DuelRunner.SpawnInput> {
    private readonly Disenfranchised<SpawnInput>  _spawnInput = new();
    private          PaperPusher                  _paperPusher => _spawnInput.Value.PaperPusher;
    private readonly Disenfranchised<CanvasLayer> _uiRoot = new();

    private readonly Disenfranchised<PlayerMap<PlaymatSceneRoot>> _playmats = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/duel_runner.tscn");

    public static DuelRunner InstantiateRawScene() {
        return PackedScene.Instantiate<DuelRunner>();
    }

    public DuelRunner InitializeSelf(SpawnInput input) {
        _spawnInput.Enfranchise(input);
        _uiRoot.Enfranchise(() => input.GodotPlayerInterface);

        _playmats.Enfranchise(() =>
            PlayerMap.Create(playerId => {
                    var playmatSpawnInput = new PlaymatSceneRoot.SpawnInput() {
                        PlayerId    = playerId,
                        LaneCount   = input.LaneCount,
                        OnCellClick = input.OnCellClick,
                        OnZoneClick = input.OnZoneClick
                    };

                    var playmat = this.SpawnChild<PlaymatSceneRoot, PlaymatSceneRoot.SpawnInput>(playmatSpawnInput)
                                      .Named($"{playerId} Playmat");

                    if (playerId is PlayerId.Blue) {
                        playmat.RotationDegrees = 180;
                    }

                    return playmat;

                    // return this.EnumerateSiblings()
                    // .OfType<PlaymatSceneRoot>()
                    // .Single(it => it.Name == playerId.ToString())
                    // .InitializeSelf(
                    // playmatSpawnInput
                    // );
                }
            )
        );

        return this;
    }

    public override void _Process(double delta) {
        _spawnInput.Value.GodotBetween.ConsumeAllEvents(Consume);
    }

    private readonly ConcurrentDictionary<SerialNumber, CardScene3> _allCards = new();

    private ICardSceneRoot GetOrSpawnCard(SerialNumber serialNumber) {
        return _allCards.GetOrAdd(
            serialNumber,
            cardSerial => {
                var paperCard = _paperPusher.GetCard(cardSerial);

                return CardScene3.InstantiateRawScene()
                                 .Named($"Card {serialNumber}")
                                 .InitializeSelf(
                                     new() {
                                         CardData = paperCard.Data,
                                         OnClick  = number => { GD.Print($"CLICKED {number}!"); },
                                         SerialNumber = serialNumber
                                     }
                                 );
            }
        );
    }

    /// <summary>
    /// TODO: The responsibilities between <see cref="ActionManager"/>, <see cref="GodotBetween"/>, <see cref="Referee"/>, <see cref="PaperPusher"/>...it's a mess
    /// </summary>
    public readonly record struct SpawnInput {
        public required GodotBetween         GodotBetween         { get; init; }
        public required GodotPlayerInterface GodotPlayerInterface { get; init; }
        public required PaperPusher          PaperPusher          { get; init; }

        public required int                 LaneCount   { get; init; }
        public required Action<CellAddress> OnCellClick { get; init; }
        public required Action<ZoneAddress> OnZoneClick { get; init; }
    }

    private ICardZoneNode GetZoneNode(ZoneAddress zoneAddress) {
        return _playmats.Value[zoneAddress.PlayerId].GetZoneNode(zoneAddress.ZoneId);
    }

    #region Event Consumption

    private void Consume(IGameEvent gameEvent) {
        _ = gameEvent switch {
            AdmonitionEvent admonitionEvent => ConsumeAdmonitionEvent(admonitionEvent),
            DeckShuffledEvent deckShuffledEvent => ConsumeDeckShuffledEvent(deckShuffledEvent),
            CardMovedEvent cardMovedEvent => ConsumeCardMovedEvent(cardMovedEvent),
            _ => throw new NotImplementedException($"I don't know how to handle: {gameEvent}")
        };
    }

    private bool ConsumeCardMovedEvent(CardMovedEvent cardMovedEvent) {
        GD.Print($"Consuming: {cardMovedEvent}");

        var originNode = GetZoneNode(cardMovedEvent.From);
        var cardObject = GetOrSpawnCard(cardMovedEvent.Card.SerialNumber);
        cardObject.AsNode2D.AsChildOf(originNode.AsNode2D);

        GetZoneNode(cardMovedEvent.To).AddCard(cardObject);

        return true;
    }

    private bool ConsumeDeckShuffledEvent(DeckShuffledEvent deckShuffledEvent) {
        return NotifyPlayer(
            new() {
                Message = $"{deckShuffledEvent.PlayerId} player deck shuffled."
            }
        );
    }

    private bool ConsumeAdmonitionEvent(AdmonitionEvent admonitionEvent) {
        return NotifyPlayer(
            new Notification() {
                Message = admonitionEvent.Message,
                Tone    = Core.Notification.NotificationTone.Negative
            }
        );
    }

    private bool NotifyPlayer(Notification notification) {
        _spawnInput.Value.GodotPlayerInterface.NotifyPlayer(notification);
        return true;
    }

    #endregion

    public void FocusOnHand(PlayerId playerId) {
        // TODO: Official documentation says to use this with `CallDeferred`, but that's pretty janky
        //       (and the example code on https://docs.godotengine.org/en/stable/tutorials/ui/gui_navigation.html#necessary-code doesn't even compile)
        //       so I'm not bothering with it for the time being.
        // Callable.From(_handContainers.Value[playerId].FindNextValidFocus().GrabFocus).CallDeferred();
        // _hands.Value[playerId].FocusOnFirstCard();
        _playmats.Value[playerId].Hand.FocusOnFirstCard();
    }
}