using Godot;

namespace maidoc.Scenes;

public interface IGameNode2D {
    public Node2D AsNode2D { get; }

    public Distance2D UnscaledSize { get; }
}

public static class GameNode2DExtensions {
    extension(IGameNode2D gameNode2D) {
        public Distance2D Size => gameNode2D.UnscaledSize * gameNode2D.AsNode2D.Scale;

        public Distance2D LocalPosition {
            get => gameNode2D.AsNode2D.Position.GodotPixels;
            set => gameNode2D.AsNode2D.Position = value.GodotPixels;
        }

        public RectDistance LocalRect() {
            return new RectDistance(gameNode2D.LocalPosition, gameNode2D.Size);
        }
    }
}