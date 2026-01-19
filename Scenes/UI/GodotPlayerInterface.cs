using Godot;
using maidoc.Core;

namespace maidoc.Scenes.UI;

public partial class GodotPlayerInterface : CanvasLayer,
    ISceneRoot<GodotPlayerInterface, GodotPlayerInterface.SpawnInput> {
    private readonly LazyChild<Container> _notificationContainer = new(uniqueName: "NotificationContainer");

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/UI/godot_player_interface.tscn");

    public static GodotPlayerInterface InstantiateRawScene() {
        return PackedScene.Instantiate<GodotPlayerInterface>();
    }

    public override void _Ready() { }

    public GodotPlayerInterface InitializeSelf(SpawnInput spawnInput) {
        return this;
    }

    public void NotifyPlayer(Notification notification) {
        _notificationContainer.Get(this)
            .SpawnChild<NotificationSceneRoot, NotificationSceneRoot.SpawnInput>(
                new NotificationSceneRoot.SpawnInput() {
                    Notification = notification
                }
            );
    }

    public readonly record struct SpawnInput { }
}