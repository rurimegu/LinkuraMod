using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Mystery at the Library (涓庝綘鐨勫濡欏浘涔﹂) 鈥?Cost 1, Skill, Uncommon.
/// On play: Gain 8 (11) Block.
/// Backstage: whenever you play a Skill, Burst Hearts 2 (3).
/// </summary>
public class MysteryAtTheLibrary() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(8, ValueProp.Move),
    new BurstHeartsVar(2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await CommonActions.CardBlock(this, play);
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    if (cardPlay.Card == this || cardPlay.Card.Owner != Owner) return;
    if (cardPlay.Card.Type != CardType.Skill) return;
    await TriggerWithAction(context, () => LinkuraCardActions.BurstHearts(this, context));
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(3m);
    DynamicVars.BurstHearts().UpgradeValueBy(1m);
  }
}
