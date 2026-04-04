using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Sugar Melt (甜蜜融化) — Cost 2, Power, Uncommon.
/// Gain 2 (3) stacks of Auto Burst. Whenever you Auto Burst, gain Block equal to the non-overflowing burst amount.
/// </summary>
public class SugarMelt() : LinkuraCard(2, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new AutoBurstVar(2),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    HoverTipFactory.Static(StaticHoverTip.Block)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCmd.GainAutoBurst(Owner.Creature, DynamicVars.AutoBurst().IntValue, Owner.Creature, this);
    await PowerCmd.Apply<SugarMeltPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.AutoBurst().UpgradeValueBy(1m);
  }
}
