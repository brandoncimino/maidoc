using System;
using Godot;
using maidoc.Core;
using Side = Godot.Side;

namespace maidoc.Scenes.GameComponents;

[Tool]
public partial class PlaymatSceneRoot : Node2D, ISceneRoot<PlaymatSceneRoot, PlaymatSceneRoot.SpawnInput> {
    private readonly Disenfranchised<Deck>               _deck      = new();
    private readonly Disenfranchised<HandView>           _hand      = new();
    private readonly Disenfranchised<BoardView>          _board     = new();
    private readonly Disenfranchised<GraveyardSceneRoot> _graveyard = new();

    private readonly PlaymatLayout _layout = PlaymatLayout.Default();

    public BoardView          Board     => _board.Value;
    public HandView           Hand      => _hand.Value;
    public Deck               Deck      => _deck.Value;
    public GraveyardSceneRoot Graveyard => _graveyard.Value;

    private readonly Disenfranchised<PlayerId> _playerId = new();
    public           PlayerId                  PlayerId => _playerId.Value;

    [Export]
    public bool DrawInGame { get; set; } = false;

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/Playmat.tscn");

    public static PlaymatSceneRoot InstantiateRawScene() {
        return PackedScene.Instantiate<PlaymatSceneRoot>();
    }

    public override void _EnterTree() {
        // if (!_initialized) {
        //     InitializeSelf(
        //         new SpawnInput() {
        //             PlayerId    = PlayerId.Red,
        //             LaneCount   = 4,
        //             OnCellClick = _ => { },
        //             OnZoneClick = _ => { },
        //         }
        //     );
        // }
    }

    public readonly record struct PlaymatLayout {
        public required RectDistance BoardRect     { get; init; }
        public required RectDistance DeckRect      { get; init; }
        public required RectDistance GraveyardRect { get; init; }
        public required RectDistance HandRect      { get; init; }

        public static PlaymatLayout Default() {
            return Create(
                new Vector2(0.4f, 0.4f).Meters(),
                .6f.ScreenWidths(),
                new Vector2(.7f, .35f).Screens()
            );
        }

        public static PlaymatLayout Create(
            Distance2D boardPadding,
            Distance   handWidth,
            Distance2D boardSize
        ) {
            var boardRect = GodotHelpers.Rect2BySide(
                Side.Top,
                default(Distance2D),
                boardSize
            );

            var deckRect = GodotHelpers.Rect2BySide(
                Side.Right,
                boardRect.GetSidePoint(Side.Left) + (Vector2.Left * boardPadding),
                new Distance2D(1.Meters(), boardRect.Height() / 2)
            );

            var graveyardRect = deckRect.Meters.Mirror(Vector2.Axis.Y).Meters();

            var belowBoardSpace = GodotHelpers.GetProjectScreenHeight() / 2 - boardRect.GetSide(Side.Bottom);
            var handHeight      = belowBoardSpace                           - boardPadding.Y;

            var handRect = GodotHelpers.Rect2BySide(
                Side.Bottom,
                new Vector2(0, .5f).Screens(),
                new Distance2D(
                    handWidth,
                    handHeight
                )
            );

            return new() {
                BoardRect     = boardRect,
                DeckRect      = deckRect,
                GraveyardRect = graveyardRect,
                HandRect      = handRect
            };
        }
    }

    public readonly record struct SpawnInput {
        public required PlayerId            PlayerId    { get; init; }
        public required int                 LaneCount   { get; init; }
        public required Action<CellAddress> OnCellClick { get; init; }
        public required Action<ZoneAddress> OnZoneClick { get; init; }
    }

    public PlaymatSceneRoot InitializeSelf(SpawnInput input) {
        _playerId.Enfranchise(input.PlayerId);

        SpawnBoardCells(input, _layout.BoardRect);
        SpawnHand(input, _layout.HandRect);
        SpawnDeck(input, _layout.DeckRect);
        SpawnGraveyard(input, _layout.GraveyardRect);

        return this;
    }

    private void SpawnBoardCells(SpawnInput spawnInput, RectDistance boardRect) {
        GD.Print($"Spawning board cells: {_layout}");

        _board.Enfranchise(() => SpawnBoard(this, spawnInput, boardRect));

        GD.Print($"spawned board rotation: {_board.Value.Rotation}");
    }

    private static BoardView SpawnBoard(Node2D parent, SpawnInput spawnInput, RectDistance boardRect) {
        var board = parent.SpawnChild<BoardView, BoardView.SpawnInput>(
            new BoardView.SpawnInput() {
                PlayerId    = spawnInput.PlayerId,
                LaneCount   = spawnInput.LaneCount,
                OnCellClick = spawnInput.OnCellClick,
                CellSize    = boardRect.Size / new Vector2(spawnInput.LaneCount, 2)
            }
        ).AtPosition(boardRect.GetSidePoint(Side.Top));
        board.Position = boardRect.GetSidePoint(Side.Top).GodotPixels;
        return board;
    }

    private void SpawnHand(SpawnInput spawnInput, RectDistance handRect) {
        _hand.Enfranchise(() =>
            this.SpawnChild<HandView, HandView.SpawnInput>(
                new HandView.SpawnInput() {
                    PlayerId     = spawnInput.PlayerId,
                    UnscaledSize = handRect.Size
                }
            ).AdjustSizeAndPosition(handRect.Meters)
        );
    }

    private void SpawnDeck(SpawnInput spawnInput, RectDistance deckRect) {
        _deck.Enfranchise(() =>
            this.SpawnChild<Deck, Deck.SpawnInput>(
                    new Deck.SpawnInput() {
                        PlayerId             = spawnInput.PlayerId,
                        OnZoneClick          = spawnInput.OnZoneClick,
                        UnscaledSizeInMeters = deckRect.Size.Meters
                    }
                )
                .Named($"{spawnInput.PlayerId} Deck")
                .AtPosition(deckRect.GetCenter().GodotPixels)
        );
    }

    private void SpawnGraveyard(SpawnInput spawnInput, RectDistance graveyardRect) {
        _graveyard.Enfranchise(() =>
            this.SpawnChild<GraveyardSceneRoot, GraveyardSceneRoot.SpawnInput>(
                new() {
                    PlayerId     = spawnInput.PlayerId,
                    UnscaledSize = graveyardRect.Size
                }
            ).AtPosition(graveyardRect.Position)
        );
    }

    public ICardZoneNode GetZoneNode(DuelDiskZoneId duelDiskZoneId) {
        return duelDiskZoneId switch {
            DuelDiskZoneId.Deck => _deck.Value,
            DuelDiskZoneId.Hand => _hand.Value,
            DuelDiskZoneId.Graveyard => _graveyard.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(duelDiskZoneId), duelDiskZoneId, null)
        };
    }

    public int GetCellNode(CellAddress cellAddress) {
        throw new NotImplementedException();
    }

    public override void _Draw() {
        if (DrawInGame || Engine.IsEditorHint()) {
            DrawLayout(_layout);
        }
    }

    private void DrawLayout(PlaymatLayout playmatLayout) {
        DrawRect(playmatLayout.BoardRect.GodotPixels,     Colors.Purple);
        DrawRect(playmatLayout.DeckRect.GodotPixels,      Colors.Blue);
        DrawRect(playmatLayout.GraveyardRect.GodotPixels, Colors.Green);
        DrawRect(playmatLayout.HandRect.GodotPixels,      Colors.DeepPink);
    }
}