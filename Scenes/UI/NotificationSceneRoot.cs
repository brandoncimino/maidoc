using System;
using System.Linq;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes.UI;

/// <summary>
/// This is structured like a pseudo "button-container": The whole thing is clickable like a <see cref="Button"/>, but also expands to contain its full contents, like a <see cref="PanelContainer"/>.
///
/// This requires some fanciness:
/// <ul>
/// <li> In order for the notification's size to be taken into account by parents, its root must be something that tells its parents about its size - like a <see cref="Container"/> or a <see cref="Button"/> - and NOT a plain <see cref="Control"/>.</li>
/// <li> We want the auto-sizing behavior of a <see cref="PanelContainer"/>, which expands to fit its contents.</li>
/// <li> We want the interactive behavior of a <see cref="Button"/>, which only expands to fit its <see cref="Button.Text"/> and <see cref="Button.Icon"/>.</li>
/// <li> We make the root node a <see cref="PanelContainer"/>.</li>
/// <li> We make the <see cref="PanelContainer"/> have no background (via the "theme overrides")</li>
/// <li> We set up the <see cref="_button"/> to expand to fit the root <see cref="PanelContainer"/>.</li>
/// <li> We put the <see cref="_button"/> BEHIND (i.e., first in the sibling order) everything else, to make sure it doesn't cover up the other stuff.</li>
/// <li> We make sure the other stuff all has <see cref="Control.MouseFilter"/> set to <see cref="Control.MouseFilterEnum.Ignore"/>.</li>
/// </ul>
/// </summary>
public partial class NotificationSceneRoot : Control,
    ISceneRoot<NotificationSceneRoot, NotificationSceneRoot.SpawnInput> {
    private readonly LazyChild<Button>      _button      = new();
    private readonly LazyChild<Label>       _message     = new("Message");
    private readonly LazyChild<ProgressBar> _progressBar = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/UI/NotificationScene.tscn");

    public override void _Ready() { }

    public static NotificationSceneRoot InstantiateRawScene() {
        return PackedScene.Instantiate<NotificationSceneRoot>();
    }

    /// <summary>
    /// This ensures that ONLY my <see cref="_button"/> is interactive.
    /// That way, the <see cref="_button"/> can appear "behind" everything else, but still be clickable.
    /// </summary>
    private void MakeNonButtonNonInteractive() {
        this.EnumerateChildren()
            .OfType<Control>()
            .Where(it => it != _button.Get(this))
            .ForEach(it => it.MouseFilter = MouseFilterEnum.Ignore);
    }

    public NotificationSceneRoot InitializeSelf(SpawnInput input) {
        _message.Get(this).Text   =  input.Notification.Message;
        _button.Get(this).Pressed += Dismiss;

        MakeNonButtonNonInteractive();

        if (input.DurationBeforeFade is { TotalSeconds: var seconds }) {
            var tween = CreateTween();

            tween.TweenMethod(
                Callable.From((float amount) => { _progressBar.Get(this).Value = 100f * amount; }),
                0f,
                1f,
                seconds
            );

            tween.TweenProperty(
                this,
                "modulate:a" /*I think this actually means `this.Modulate.A`*/,
                0,
                input.FadeDuration.TotalSeconds
            );

            tween.TweenCallback(Callable.From(Dismiss));
        }

        return this;
    }

    public void Dismiss() {
        GD.Print($"Dismissed: {_button.Get(this).Text}");
        QueueFree();
    }

    public readonly record struct SpawnInput() {
        public required Notification Notification       { get; init; }
        public          TimeSpan?    DurationBeforeFade { get; init; } = TimeSpan.FromSeconds(3);
        public          TimeSpan     FadeDuration       { get; init; } = TimeSpan.FromSeconds(.1);
    }
}