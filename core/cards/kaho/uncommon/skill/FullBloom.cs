using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Full Bloom! (盛开！) — Cost 0, Skill, Uncommon.
/// If your ❤️ is at max, gain 1 (2) Energy and draw 2 card(s). Collect.
/// </summary>
public class FullBloom() : KahoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
    new CardsVar(2),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect),
  ];

  protected override bool ShouldGlowGoldInternal => HeartsState.ReachedMaxHearts(Owner);

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (ShouldGlowGoldInternal) {
      await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
      await CommonActions.Draw(this, ctx);
    }
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Energy.UpgradeValueBy(1m);
  }
}
