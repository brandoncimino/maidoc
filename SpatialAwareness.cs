using System.Runtime.CompilerServices;
using Godot;
using maidoc.Scenes;

namespace maidoc;

[Tool]
public partial class SpatialAwareness : Node2D {
    [Export]
    public Sprite2D Sprite { get; set; }

    [Export]
    private Sprite2D Subsprite { get; set; }

    [ExportToolButton("!!")]
    public Callable DoStuff => Callable.From(() => {
        GD.Print("Doing");

        Sprite.blog(
            Sprite.Position,
            Sprite.GlobalPosition,
            Sprite.Scale,
            Sprite.GlobalScale
        );

        // Describe(Sprite);
        // GD.Print();
        // Describe(Subsprite);

        GD.Print("Done did");
    });


    private static void Describe(Sprite2D                   sprite2D,
        [CallerArgumentExpression(nameof(sprite2D))] string _sprite2D = "") {
        sprite2D.blog(label: _sprite2D);
        sprite2D.Position.blog();
        sprite2D.GlobalPosition.blog();
        sprite2D.GetGlobalTransformWithCanvas().blog();
    }
}