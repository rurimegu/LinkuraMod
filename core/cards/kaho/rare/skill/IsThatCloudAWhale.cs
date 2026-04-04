using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Is That Cloud A Whale? — Cost 2, Skill, Rare.
/// For every 20 (15) Max ❤️, gain 1 Intangible.
/// Reset Max ❤️ to 9. Exhaust.
/// </summary>
public class IsThatCloudAWhale() : LinkuraCard(2, CardType.Skill, CardRarity.Rare, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Exhaust,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new MaxHeartsThresholdVar(20m),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    int threshold = DynamicVars.MaxHeartThreshold().IntValue;
    int amount = maxHearts / threshold;
    if (amount > 0) {
      await PowerCmd.Apply<IntangiblePower>(Owner.Creature, amount, Owner.Creature, this);
    }
    await HeartsState.SetMaxHearts(Owner, ctx, 9, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.MaxHeartThreshold().UpgradeValueBy(-5m);
  }
}
