#if TOOLS
using Godot;

[Tool]
public partial class myplugin : EditorPlugin {
    private MyInspectorPlugin _plugin;

    public override void _EnterTree() {
        _plugin = new MyInspectorPlugin();
        AddInspectorPlugin(_plugin);
    }

    public override void _ExitTree() {
        RemoveInspectorPlugin(_plugin);
    }
}
#endif