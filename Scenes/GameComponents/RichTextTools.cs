using Godot;

namespace maidoc.Scenes.GameComponents;

[Tool]
public partial class RichTextTools : RichTextLabel {
    [ExportToolButton(nameof(TextHelpers.Describe))]
    public Callable DescribeTool => Callable.From(this.Describe);
}