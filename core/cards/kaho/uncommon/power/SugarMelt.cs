using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Sugar Melt (甜蜜融化) — Cost 2, Power, Uncommon.
/// Gain 2 (3) stacks of Auto Burst. Whenever you Auto Burst, gain 1 Block.
/// </summary>
public class SugarMelt() : KahoCard(2, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new AutoBurstVar(2),
    new BlockVar(1, ValueProp.Unpowered),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCmd.GainAutoBurst(Owner.Creature, ctx, DynamicVars.AutoBurst().IntValue, Owner.Creature, this);
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<SugarMeltPower>(Owner.Creature, DynamicVars.Block.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.AutoBurst().UpgradeValueBy(1m);
  }
}
