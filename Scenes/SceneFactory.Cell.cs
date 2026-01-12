using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

public partial class SceneFactory {
    private readonly SceneSpawner<CellView, CellView.SpawnInput> _cellSpawner = new() {
        NamingConvention = (input, i) => $"Cell {input.MyCell.Coord}"
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

    public CellView SpawnCell(CellView.SpawnInput spawnInput) {
        _cellSpawner.UseGroupNode(this);

        return _cellSpawner.Spawn(spawnInput);
    }
}