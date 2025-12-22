using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class SceneSpawner : Node2D {
    // TODO: can this be `private`? It should only be accessible from the editor.
    [Export]
    public required PackedScene CardScene { get; set; }

    private readonly InstanceGroup<CardView> _cards = new();
    private readonly PackedSceneSpawner<CardView, CardView.Input> _cardSpawner =
        new() {
            PackedScene         = null,
            InstanceInitializer = (cardView, input) => cardView.Initialize(input)
        };

    public CardView SpawnCard(
        PaperCard paperCard,
        Action?   onClick = null
    ) {
        Require.Argument(
            paperCard,
            _allCards.SingleOrDefault(it => it.MyCard == paperCard) is null,
            $"Can't spawn a {nameof(CardView)} for {paperCard} because one already exists!"
        );

        var cardObj = CardScene.Instantiate<CardView>()
            .Initialize(
                new CardView.Input() {
                    MyCard  = paperCard,
                    OnClick = onClick
                }
            )
            .AsChildOf(CardParent);

        _allCards.Add(cardObj);
        return cardObj;
    }
}

public sealed class InstanceGroup<TNode> where TNode : Node {
    public string GroupName { get; init; } = $"{typeof(TNode).Name} Instances";


    private          Node?        _groupNode;

    public Node GroupNode => _groupNode ??= new() {
        Name = GroupName
    };

    private readonly IList<TNode> _instances = [];

    public TNode Spawn(PackedScene packedScene) {
        var instance = packedScene.Instantiate<TNode>();
        GroupNode.AddChild(instance);
        instance.AddToGroup(GroupName);
        return instance;
    }
}

/// <summary>
/// Instantiated <see cref="TSceneRoot"/>s are tracked in 3 places:
/// <ul>
/// <li>As children of <see cref="GroupNode"/>.</li>
/// <li>As members of the <see cref="GroupName"/> <see cref="Node.AddToGroup">group</see>.</li>
/// <li>As elements of <see cref="Instances"/>.</li>
/// </ul>
/// </summary>
/// <typeparam name="TSceneRoot">The type of the root-level <see cref="Node"/> in the <see cref="PackedScene"/>.</typeparam>
/// <typeparam name="TInput">The </typeparam>
public class PackedSceneSpawner<TSceneRoot, TInput> where TSceneRoot : Node, ISceneRoot<TSceneRoot, TInput> {
    private Node? _parentNode;

    public StringName GroupName { get; init; } = typeof(TSceneRoot).Name;

    public Node GroupNode {
        get => _parentNode ??= new() {
            Name = GroupName
        };

        init => _parentNode = value;
    }

    public ImmutableArray<TInput> Instances { get; private set; } = ImmutableArray<TInput>.Empty;

    public TSceneRoot Spawn(PackedScene packedScene, TInput input) {
        var instance = packedScene.Instantiate<TSceneRoot>()
            .Initialize(input)
            .AsChildOf(GroupNode);

        instance.AddToGroup(GroupName);
        Instances = Instances.Add(input);
        return instance;
    }
}