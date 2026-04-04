using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// At the start of your turn, choose at most 1 (2) Backstage cards from your draw pile to put into your hand.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.WelcomeIshikawa"/>.
/// </summary>
public class WelcomeIshikawaPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState) {
    await base.BeforeSideTurnStart(choiceContext, side, combatState);
    if (side != Owner.Side) return;

    var player = Owner.Player;
    if (player == null) return;

    var drawPile = PileType.Draw.GetPile(player);
    if (drawPile == null) return;

    var backstageCards = drawPile.Cards.Where(c => c.Keywords.Contains(LinkuraKeywords.Backstage)).ToList();
    if (backstageCards.Count == 0) return;

    var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount);

    var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, backstageCards, player, prefs);
    if (selected != null) {
      foreach (var card in selected) {
        await CardPileCmd.Add(card, PileType.Hand);
      }
    }
  }
}
