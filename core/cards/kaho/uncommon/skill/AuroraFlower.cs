using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Aurora Flower (极光花) — Cost 1, Skill, Uncommon.
/// Trigger 6 (9) [gold]Auto Burst[/gold]. Collect after each trigger.
/// </summary>
public class AuroraFlower() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {

  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new TriggerAutoBurstVar(6),
  ];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int triggers = DynamicVars.TriggerAutoBurst().IntValue;
    for (int i = 0; i < triggers; i++) {
      await LinkuraCardActions.AutoBurst(this, ctx, 1);
      await LinkuraCardActions.CollectHearts(this, ctx);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.TriggerAutoBurst().UpgradeValueBy(3m);
  }
}
