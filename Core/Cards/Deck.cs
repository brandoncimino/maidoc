using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Godot;
using Range = System.Range;

namespace maidoc.Core.Cards;

public sealed class Deck(IEnumerable<PaperCard> cards) : PaperCardGroup(cards) {

}