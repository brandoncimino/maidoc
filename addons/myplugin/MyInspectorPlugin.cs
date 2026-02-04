// MyInspectorPlugin.cs

#if TOOLS
using Godot;
using maidoc.addons.myplugin;

[Tool]
public partial class MyInspectorPlugin : EditorInspectorPlugin {
    public override bool _CanHandle(GodotObject @object) {
        return @object is IMetricNode2D;
    }

    private static bool IsPosition(GodotObject godotObject, string propertyName) {
        return propertyName == Node2D.PropertyName.Position
               || propertyName is "PositionInPixels";
    }

    private static readonly Vector2 PixelsPerMeter = new Vector2(100, 100);

    public override bool _ParseProperty(
        GodotObject        @object,
        Variant.Type       type,
        string             name,
        PropertyHint       hintType,
        string             hintString,
        PropertyUsageFlags usageFlags,
        bool               wide
    ) {
        var obj = @object;

        // AddPropertyEditor(
        //     name,
        //     new MetersToPixelsEditor()
        // );

        if (type == Variant.Type.Vector2) {
            if (IsPosition(obj, name)) {
                this.AddRatio2DPropertyEditor(
                    name,
                    "meters",
                    "m",
                    () => PixelsPerMeter,
                    addToEnd: true
                );

                // this.AddRatio2DPropertyEditor(
                //     name,
                //     "screens",
                //     "screens",
                //     () => new Vector2(
                //         GodotHelpers.GetProjectScreenWidth(),
                //         GodotHelpers.GetProjectScreenHeight()
                //         ),
                //     addToEnd: true
                //     );
            }
        }


        //
        //     // BUG: This seems to be the culprit! Not only that, but once it's been called, the `.tcsn` file becomes TAINTED!
        //     // AddPropertyEditor(
        //     //     name,
        //     //     EditorInspector.InstantiatePropertyEditor(
        //     //         canvasItem,
        //     //         type,
        //     //         name,
        //     //         hintType,
        //     //         hintString+",suffix:m",
        //     //         (uint)usageFlags,
        //     //         wide
        //     //         )
        //     //     );
        // }

        // We handle properties of type integer.
        // if (type == Variant.Type.Int)
        // {
        // Create an instance of the custom property editor and register
        // it to a specific property path.
        // AddPropertyEditor(name, new RandomIntEditor());
        // Inform the editor to remove the default property editor for
        // this property type.
        // return true;
        // }

        // return false;
        return base._ParseProperty(@object, type, name, hintType, hintString, usageFlags, wide);
    }
}
#endif