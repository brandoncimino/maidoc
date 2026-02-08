using Godot;

namespace maidoc.Scenes;

public interface IGameNode2D {
    public Node2D AsNode2D { get; }

    public Distance2D UnscaledSize { get; }
}

public static class GameNode2DExtensions {
    extension(IGameNode2D gameNode2D) {
        public Distance2D Size => gameNode2D.UnscaledSize * gameNode2D.AsNode2D.Scale;

        public Distance2D Center {
            get => gameNode2D.AsNode2D.Position.GodotPixels;
            set => gameNode2D.AsNode2D.Position = value.GodotPixels;
        }

        public RectDistance GameRect => RectDistance.ByCenter(gameNode2D.Center, gameNode2D.UnscaledSize);
    }
}