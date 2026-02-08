using System;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

/// <summary>
/// TODO: Rename to something that makes it clear this is for a SINGLE player, not BOTH of them - maybe `PlayerBoardView` or `PlaymatView`
/// </summary>
public partial class BoardView : Node2D, ISceneRoot<BoardView, BoardView.SpawnInput>, IGameNode2D {
    private readonly Disenfranchised<BoardRows<CellView>> _cells = new();

    private readonly SceneSpawner<CellView, CellView.SpawnInput> _cellSpawner = new();

    private static PackedScene? _packedScene;

    private readonly Disenfranchised<PlayerId> _playerId = new();

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/Board.tscn");

    public static BoardView InstantiateRawScene() {
        return PackedScene.Instantiate<BoardView>();
    }

    public           Node2D                      AsNode2D => this;
    private readonly Disenfranchised<Distance2D> _boardSizeInMeters = new();
    public           Distance2D                  UnscaledSize => _boardSizeInMeters.Value;

    public override void _Ready() { }

    public BoardView InitializeSelf(SpawnInput input) {
        GD.Print($"Initializing {this} with: {input}");

        _playerId.Enfranchise(input.PlayerId);

        _cellSpawner.UseGroupNode(this);

        // This positions the cells so that the board's `Node2D.Position` is the top-center of the grid.
        var cellOffset = new Distance2D(input.CellSize.X * input.LaneCount * -.5f, default);

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
                                    new RectDistance(
                                        cellOffset + input.CellSize * new Vector2(address.Lane, (int)address.Row),
                                        input.CellSize
                                    ).Meters
                            }
                        )
                );
            }
        );

        _boardSizeInMeters.Enfranchise(input.CellSize * _cells.Value.Dimensions);

        return this;
    }

    public readonly record struct SpawnInput() {
        public required PlayerId            PlayerId     { get; init; }
        public required int                 LaneCount    { get; init; }
        public          Distance2D          CellSize     { get; init; } = new Vector2(1.5f, 1.2f).Meters;
        public required Action<CellAddress> OnCellClick  { get; init; }
    }
}