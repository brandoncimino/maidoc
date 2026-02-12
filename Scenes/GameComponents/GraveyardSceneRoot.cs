namespace maidoc.Scenes.GameComponents;

public partial class GraveyardSceneRoot : CardZoneNode {
    public static GraveyardSceneRoot InstantiateRawScene() {
        return new GraveyardSceneRoot();
    }

    public sealed record SpawnInput : ZoneSpawnInput;

    protected override void InitializeSubtype(SpawnInput input) { }
}