#if TOOLS
using Godot;
using maidoc.Scenes;

[Tool]
public partial class MetersToPixelsEditor : EditorProperty {
    private static EditorSpinSlider CreateNumberEntry(
        string label
    ) {
        return new() {
            Suffix     = "m",
            Step       = 0,
            MinValue   = double.NegativeInfinity,
            MaxValue   = double.PositiveInfinity,
            Label      = label,
            HideSlider = true,
        };
    }

    private readonly EditorSpinSlider _xEntry = CreateNumberEntry("x");
    private readonly EditorSpinSlider _yEntry = CreateNumberEntry("y");

    public MetersToPixelsEditor() {
        var vBox = new VBoxContainer()
            .AsChildOf(this);

        vBox.AddChild(_xEntry);
        vBox.AddChild(_yEntry);

        AddFocusable(_xEntry);
        AddFocusable(_yEntry);

        _xEntry.ValueChanged += _ => { _OnValueChanged(); };
        _yEntry.ValueChanged += _ => { _OnValueChanged(); };
    }

    private void _OnValueChanged() {
        var meters = new Vector2((float)_xEntry.Value, (float)_yEntry.Value);
        var pixels = meters * GodotHelpers.GodotPixelsPerMeter;
        EmitChanged(GetEditedProperty(), pixels);
    }

    public override void _UpdateProperty() {
        var pixels = GetEditedObject().Get(GetEditedProperty()).AsVector2();
        var meters = pixels / GodotHelpers.GodotPixelsPerMeter;
        _xEntry.SetValueNoSignal(meters.X);
        _yEntry.SetValueNoSignal(meters.Y);
    }
}

#endif