using System;
using System.Collections.Generic;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

public partial class DebugMenu : PanelContainer, ISceneRoot<DebugMenu, PlayerInterface> {
    private readonly Disenfranchised<Container>       _menuContainer   = new();
    private readonly Disenfranchised<PlayerInterface> _playerInterface = new();
    private readonly List<DebugMenuItem>              _menuItems       = [];

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/UI/debug_menu.tscn");

    public static DebugMenu InstantiateRawScene() {
        return PackedScene.Instantiate<DebugMenu>();
    }

    public override void _Process(double delta) {
        _menuItems.ForEach(it => it.Update());
    }

    public DebugMenu InitializeSelf(PlayerInterface playerInterface) {
        _playerInterface.Enfranchise(playerInterface);
        _menuContainer.Enfranchise(this.RequireOnlyChild<Container>());
        AddMenuItem(() => $"Start Game", playerInterface.Referee.StartGame);
        AddMenuItem(() => $"{nameof(playerInterface.Referee.ActivePlayer)}: {playerInterface.Referee.ActivePlayer}");
        AddMenuItem(() => $"{nameof(playerInterface.CurrentAction)}: {playerInterface.CurrentAction}");
        AddMenuItem(() => "Cancel",   playerInterface.Cancel);
        AddMenuItem(() => "Confirm",  () => playerInterface.TryConfirm());
        AddMenuItem(() => "End Turn", () => playerInterface.Referee.EndTurn());

        return this;
    }

    private sealed record DebugMenuItem(
        Func<string> Text,
        Action?      OnClick,
        Button       Button
    ) {
        public DebugMenuItem Update() {
            Button.Text = Text();
            return this;
        }

        public DebugMenuItem Click() {
            OnClick?.Invoke();
            return this;
        }
    }

    private void AddPlayerMenu(DebugMenu debugMenu, PlayerId playerId) {
        debugMenu.AddMenuGroup(menu => {
                menu.AddMenuItem(() => $"Player {playerId}");
                menu.AddMenuItem(() => "Draw", () => _playerInterface.Value.Referee.DrawFromDeck(playerId));
            }
        );
    }

    private void AddMenuGroup(
        Action<DebugMenu> buildMenuGroup
    ) {
        var panel = new DebugMenu();
        buildMenuGroup(panel);
        AddChild(panel);
    }

    private void AddMenuItem(
        Func<string> text,
        Action?      onClick = null
    ) {
        var button = new Button();
        _menuContainer.Value.AddChild(button);
        _menuItems.Add(
            new DebugMenuItem(
                    text,
                    onClick,
                    button
                )
                .Update()
        );
    }
}