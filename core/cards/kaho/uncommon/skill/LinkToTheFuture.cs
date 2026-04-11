using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Link to the FUTURE 鈥?Cost 1, Skill, Uncommon.
/// Retain your hand this turn. Next turn gain 1 (2) energy.
/// </summary>
public class LinkToTheFuture() : KahoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
  ];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(CardKeyword.Retain),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<RetainHandPower>(Owner.Creature, 1m, Owner.Creature, this);
    await PowerCmd.Apply<EnergyNextTurnPower>(Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Energy.UpgradeValueBy(1m);
  }
}
