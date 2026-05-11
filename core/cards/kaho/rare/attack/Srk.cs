using System.Collections.Generic;
using System.Threading.Tasks;
using RuriMegu.Core.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Cards.Kaho.Rare.Attack;

/// <summary>
/// S.R.K. — Cost 3, Attack, Rare.
/// Deal 3 damage. Gain 3 Block. Increase max ❤️ by 3. Burst 3. Gain {Energy:energyIcons()}. Draw 3 cards. (Innate.) Exhaust. (Upgrades: Remove Exhaust. Add Innate.)
/// </summary>
public class Srk() : KahoCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(3, ValueProp.Move),
    new BlockVar(3, ValueProp.Move),
    new ExpandHeartsVar(3),
    new BurstHeartsVar(3),
    new CardsVar(3),
    new EnergyVar(3)
  ];

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await CommonActions.CardBlock(this, play);
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
    await LinkuraCardActions.BurstHearts(this, ctx);
    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    await CommonActions.Draw(this, ctx);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Innate);
  }
}
