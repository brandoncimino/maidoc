using Godot;

namespace maidoc.Scenes;

public interface IGameNode2D {
    public Node2D AsNode2D { get; }

    public Distance2D LocalPosition { get => AsNode2D.Position.Meters(); set => AsNode2D.Position = value.Meters; }

    public Distance2D UnscaledSize { get; }
}