using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

public partial class SceneFactory {
    private readonly PackedSceneSpawner<CellView, CellView.SpawnInput> _cellSpawner = new() {
        PackedScenePath = "res://Scenes/GameComponents/Cell.tscn"
    };

    public void SpawnBoardCells(
        CellView.BoardSpawnInput input
    ) {
        Helpers.ForEachCellIndex(
            input.BoardGrid.Width,
            input.BoardGrid.Height,
            (x, y) => {
                SpawnCell(
                    new CellView.SpawnInput() {
                        MyCell  = input.BoardGrid[x, y],
                        OnClick = input.OnClick,
                        RectInMeters = new Rect2(
                            input.CellSizeInMeters * new Vector2(x, y),
                            input.CellSizeInMeters
                        )
                    }
                );
            }
        );
    }

    private CellView SpawnCell(CellView.SpawnInput spawnInput) => _cellSpawner.Spawn(spawnInput);
}