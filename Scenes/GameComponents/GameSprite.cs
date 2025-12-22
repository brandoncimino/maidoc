using System.Linq;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

[Tool]
public partial class GameSprite : Sprite2D {
    [Export]
    public bool MaintainAspectRatio {
        get;
        set;
    } = true;

    public override void _Ready() {
        Centered = true;

        this.NormalizeSize(MaintainAspectRatio);

        if (MaintainAspectRatio) {
            this.GetSiblings()
                .OfType<CollisionShape2D>()
                .ForEach(shape => {
                    shape.Scale = Texture.GetSize().NormalizeLargerAxis();
                });

            this.GetSiblings()
                .OfType<Control>()
                .ForEach(secretRectangle => {
                        secretRectangle.Size =
                            Texture.GetSize().NormalizeLargerAxis() * GodotHelpers.GodotUnitsPerMeter;
                    }
                );
        }
    }

    [ExportToolButton("Ready-Up")]
    public Callable ReadyUp => Callable.From(_Ready);
}