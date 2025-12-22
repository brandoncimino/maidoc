using System;
using Godot;

namespace maidoc.Scenes.GameComponents;

[Tool]
public partial class TextTools : Label {
    [Export]
    public float FontSizeInMeters { get; set; } = .1f;

    [Export]
    public float FontSharpenScale { get; set; } = 10f;

    [ExportToolButton(nameof(TextHelpers.Describe))]
    public Callable DescribeTool => Callable.From(this.Describe);
}