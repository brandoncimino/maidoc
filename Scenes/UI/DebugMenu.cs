using System;
using System.Collections.Generic;
using System.Linq;
using BSharp.Core;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.Navigation;
using Side = Godot.Side;

namespace maidoc.Scenes;

public partial class DebugMenu : PanelContainer, ISceneRoot<DebugMenu, DebugMenu.SpawnInput> {
    private readonly Disenfranchised<Container> _menuContainer = new();
    private readonly List<DebugMenuItem>        _menuItems     = [];

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/UI/debug_menu.tscn");

    public static DebugMenu InstantiateRawScene() {
        return PackedScene.Instantiate<DebugMenu>();
    }

    public override void _Process(double delta) {
        _menuItems.ForEach(it => it.Update());
    }

    public DebugMenu InitializeSelf(SpawnInput spawnInput) {
        _menuContainer.Enfranchise(this.RequireOnlyChild<Container>());

        var playerInterface = spawnInput.GodotBetween;
        AddMenuItem(() => "Start Game", playerInterface.Referee.StartGame);
        AddMenuItem(() =>
            $"{nameof(playerInterface.Referee.ActivePlayer)}: {playerInterface.Referee.ActivePlayer.DisplayName()}"
        );
        // AddMenuItem(() => $"{nameof(playerInterface.CurrentAction)}: {playerInterface.CurrentAction}");
        // AddMenuItem(() => "Cancel",   playerInterface.Cancel);
        // AddMenuItem(() => "Confirm",  () => playerInterface.TryConfirm());
        AddMenuItem(() => "End Turn", () => playerInterface.Referee.EndTurn());

        AddMenuItem(
            foldableTitle: () =>
                $"🎯 Focus {GetViewport().GuiGetFocusOwner().OrNullPlaceholder(static it => $"{it.Name}")}",
            text: () => GetViewport().GuiGetFocusOwner()?.FocusWrapper().DescribeFocus() ?? "",
            initiallyFolded: true
        );

        AddMenuItem(() => "🔍 Grab initial focus", () => spawnInput.DuelRunner.FocusOnHand(default));
        AddMenuItem(() => "❌ Lose focus",          () => GetViewport().GuiReleaseFocus());

        foreach (var playerId in Players.Ids) {
            BuildPlayerPanel(playerId, spawnInput);
        }

        BuildEventQueuePanel(spawnInput);

        return this;
    }

    private void BuildEventQueuePanel(SpawnInput spawnInput) {
        var eventQueuePanel = new PanelContainer() {
            Name = "Event Queue Panel"
        }.AsChildOf(_menuContainer.Value);
        var eventQueueContainer = new VBoxContainer().AsChildOf(eventQueuePanel);
        new Label() {
            Text = "Event Queue",
            Name = "Event Queue Header"
        }.AsChildOf(eventQueueContainer);

        _menuItems.Add(
            new Label() {
                    Name = "Event List",
                }
                .AsChildOf(eventQueueContainer),
            eventList => {
                eventList.Text = spawnInput.GodotBetween.CurrentEvents switch {
                    []     => "(empty)",
                    var ls => ls.JoinString("\n")
                };
            }
        );
    }

    private void BuildPlayerPanel(PlayerId playerId, SpawnInput spawnInput) {
        var playerPanel = new FoldableContainer() {
            Name           = playerId.DisplayName(),
            Title          = playerId.DisplayName(),
            TitleAlignment = HorizontalAlignment.Center,
        }.AsChildOf(_menuContainer.Value);

        var vFlow = new HFlowContainer().AsChildOf(playerPanel);

        AddMenuItem(() => "Draw",    () => spawnInput.GodotBetween.DrawFromDeck(playerId), vFlow);
        AddMenuItem(() => "Shuffle", () => spawnInput.GodotBetween.ShuffleDeck(playerId),  vFlow);

        foreach (var duelDiskZoneId in Enum.GetValues<DuelDiskZoneId>()) {
            var zoneAddress = new ZoneAddress() {
                PlayerId = playerId,
                ZoneId   = duelDiskZoneId
            };

            AddMenuItem(
                foldableTitle: () => $"{duelDiskZoneId} ({spawnInput.PaperView.GetZoneSnapshot(zoneAddress).Length})",
                text: () => spawnInput.PaperView.GetZoneSnapshot(zoneAddress)
                                      .Select(it => it.CanonicalName)
                                      .JoinString("\n"),
                container: vFlow
            );
        }
    }

    public void AddMenuItem(
        Func<string>  text,
        Action?       onClick         = null,
        Container?    container       = null,
        Func<string>? foldableTitle   = null,
        bool          initiallyFolded = true
    ) {
        var parent = container ?? _menuContainer.Value;

        if (foldableTitle is not null) {
            var foldableContainer = new FoldableContainer() {
                    Folded                   = initiallyFolded,
                    TitleTextOverrunBehavior = TextServer.OverrunBehavior.TrimWordEllipsis
                }
                .AsChildOf(parent);

            _menuItems.Add(foldableContainer, it => it.Title = foldableTitle());

            parent = foldableContainer;
        }

        if (onClick != null) {
            var button = new Button() {
                    FocusMode = FocusModeEnum.None
                }
                .AsChildOf(parent);
            button.Pressed += onClick;
            _menuItems.Add(new(button, () => button.Text = text()));
        }
        else {
            var label = new Label()
                .AsChildOf(parent);
            _menuItems.Add(new(label, () => label.Text = text()));
        }
    }

    public readonly record struct SpawnInput {
        public required GodotBetween GodotBetween { get; init; }
        public required IPaperView   PaperView    { get; init; }
        public required DuelRunner   DuelRunner   { get; init; }
    }
}

internal record DebugMenuItem(Control Control, Action Update);

internal static class DebugMenuExtensions {
    public static T Add<T>(
        this List<DebugMenuItem> menuItems,
        T                        item,
        Action<T>                update
    ) where T : Control {
        menuItems.Add(new(item, () => update(item)));
        return item;
    }

    public static string Icon(this PlayerId playerId) => playerId switch {
        PlayerId.Red  => "🔴",
        PlayerId.Blue => "🔵",
        _             => throw new ArgumentOutOfRangeException(nameof(playerId), playerId, null)
    };

    public static string DisplayName(this PlayerId playerId) => $"{playerId.Icon()} {playerId} Player";

    public static string DescribeFocus(this FocusWrapper focusWrapper) {
        return Enum.GetValues<Side>()
                   .Select(side => $"{side}: {focusWrapper.GetFocusNeighbor(side).Describe()}")
                   .Concat(
                       [
                           $"Previous: {focusWrapper.FocusPrevious.Describe()}",
                           $"Next:     {focusWrapper.FocusNext.Describe()}"
                       ]
                   )
                   .JoinString("\n");
    }

    public static string Describe(this NodePath nodePath) {
        return nodePath.IsEmpty
            ? "empty"
            : $"{nodePath.GetName(0)} ->  {nodePath.GetName(nodePath.GetNameCount() - 1)} ({nodePath.GetType().Name})";
    }

    public static string NodeName(this NodePath nodePath) => nodePath.GetName(nodePath.GetNameCount() - 1);

    public static string Describe(this Node node) => $"{node.Name} {node} [{node.GetType().Name}]";
}