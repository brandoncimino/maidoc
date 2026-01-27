using System;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes.GameComponents;

/// <summary>
/// TODO: Rename to something that makes it clear this is for a SINGLE player, not BOTH of them - maybe `PlayerBoardView` or `PlaymatView`
/// </summary>
public partial class BoardView : Node2D, ISceneRoot<BoardView, BoardView.SpawnInput> {
    private readonly Disenfranchised<BoardRows<CellView>> _cells = new();

    private readonly SceneSpawner<CellView, CellView.SpawnInput> _cellSpawner = new();

    private static PackedScene? _packedScene;

    private readonly Disenfranchised<PlayerId> _playerId = new();

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/Board.tscn");

    public static BoardView InstantiateRawScene() {
        return PackedScene.Instantiate<BoardView>();
    }

    private readonly Disenfranchised<Vector2> _boardSizeInMeters = new();
    public           Vector2                  BoardSizeInMeters => _boardSizeInMeters.Value;

    public override void _Ready() { }

    public BoardView InitializeSelf(SpawnInput input) {
        _playerId.Enfranchise(input.PlayerId);

        _cellSpawner.UseGroupNode(this);

        // This positions the cells so that the board's `Node2D.Position` is the top-center of the grid.
        var cellOffset = new Vector2(input.CellSizeInMeters.X * input.LaneCount * -.5f, 0);

        _cells.Enfranchise(() => {
                return new(
                    input.PlayerId,
                    input.LaneCount,
                    address =>
                        this.SpawnChild<CellView, CellView.SpawnInput>(
                            new CellView.SpawnInput() {
                                MyCell  = address,
                                OnClick = input.OnCellClick,
                                RectInMeters =
                                    new Rect2(
                                        cellOffset + input.CellSizeInMeters * new Vector2(
                                            address.Lane,
                                            (int)address.Row
                                        ),
                                        input.CellSizeInMeters
                                    )
                            }
                        )
                );
            }
        );

        _boardSizeInMeters.Enfranchise(input.CellSizeInMeters * _cells.Value.Dimensions);

        return this;
    }

    public readonly record struct SpawnInput() {
        public required PlayerId            PlayerId         { get; init; }
        public required int                 LaneCount        { get; init; }
        public          Vector2             CellSizeInMeters { get; init; } = new Vector2(1.5f, 1.2f);
        public required Action<CellAddress> OnCellClick      { get; init; }
    }
}