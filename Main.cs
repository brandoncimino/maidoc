using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes;
using maidoc.Scenes.UI;

namespace maidoc;

public partial class Main : Node {
	[Export]
	public bool DebugMenuEnabled { get; set; } = true;

	private readonly LazyChild<GodotPlayerInterface> _godotPlayerInterface = new();

	private DuelRunner? _duelRunner;

	public override void _Ready() {
		SpawnDuelRunner(Decklist.DevPlaceholder, new Ruleset());

		GD.Print("YOLO");

		GD.Print(_duelRunner);
	}

	private void SpawnDuelRunner(
		// TODO: Early placeholder - eventually, the scene will be instantiated externally and the `Referee` will be provided by someone else.
		Decklist decklist,
		Ruleset  ruleset
	) {
		var paperPusher = new PaperPusher(ruleset.LaneCount);
		var referee = Referee.PrepareFreshGame(
			new Dictionary<PlayerId, Decklist> {
				[PlayerId.Red]  = decklist,
				[PlayerId.Blue] = decklist
			},
			PlayerId.Red,
			ruleset,
			paperPusher
		);

		var playerInterface = new ActionManager() {
			Referee = referee
		};

		var godotBetween = new GodotBetween() {
			Referee              = referee,
			GodotPlayerInterface = _godotPlayerInterface.Get(this),
		};

		var duelRunnerInput = new DuelRunner.SpawnInput() {
			PaperPusher          = paperPusher,
			GodotBetween         = godotBetween,
			GodotPlayerInterface = _godotPlayerInterface.Get(this),
			LaneCount            = 4,
			OnCellClick          = playerInterface.ClickCell,
			OnZoneClick          = playerInterface.ClickZone
		};

		_duelRunner = this.SpawnChild<DuelRunner, DuelRunner.SpawnInput>(duelRunnerInput);

		if (DebugMenuEnabled) {
			_godotPlayerInterface.Get(this).SpawnChild<DebugMenu, DebugMenu.SpawnInput>(
				new DebugMenu.SpawnInput() {
					GodotBetween = godotBetween,
					PaperView    = paperPusher,
					DuelRunner   = _duelRunner
				}
			);
		}
	}
}
