using System.Collections.Generic;
using System.Threading.Tasks;
using RuriMegu.Core.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Cards.Kaho.Basic.Skill;

/// <summary>
/// Defend card for Hinoshita Kaho.
/// Basic skill: Gain 5 block. Upgrade: Gain 8 block.
/// </summary>
public class KahoDefend() : KahoCard(1, CardType.Skill, CardRarity.Basic, TargetType.None) {
  protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(5, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await CommonActions.CardBlock(this, play);
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(3m);
  }
}
