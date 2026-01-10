using Godot;

namespace maidoc.Scenes;

/// <remarks>
/// Separate <see cref="PackedSceneSpawner{TSceneRoot,TInput}"/>s are in their own files,
/// but are combined into a single <c>partial class</c> so that all of the <see cref="PackedScene"/> fields can be assigned
/// to one <see cref="Node"/> in the editor.
/// <br/>
/// <br/>
/// Note that there must be at least one file whose name is exactly "<see cref="SceneFactory"/>.cs" for Godot to be able to open from the editor.
/// </remarks>
public partial class SceneFactory;