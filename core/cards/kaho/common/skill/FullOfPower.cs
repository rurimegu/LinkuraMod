using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Full Of Power (活力全开) — Cost 2, Skill, Common.
/// Gain 11 (14) block. Burst 10 (14).
/// </summary>
public class FullOfPower() : LinkuraCard(2, CardType.Skill, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(11, ValueProp.Move),
    new BurstHeartsVar(10),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardBlock(this, play);
    await LinkuraCardActions.BurstHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(3m);
    DynamicVars.BurstHearts().UpgradeValueBy(4m);
  }
}
