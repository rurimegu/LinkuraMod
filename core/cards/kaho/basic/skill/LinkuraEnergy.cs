using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Basic.Skill;

public class LinkuraEnergy() : KahoCard(1, CardType.Skill, CardRarity.Basic, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(8),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, choiceContext);
    await LinkuraCardActions.CollectHearts(this, choiceContext);
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(4m);
  }
}
