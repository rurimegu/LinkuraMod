using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Curtain Call (演出落幕) — Cost 0, Skill, Common.
/// Gain 5 (8) block. Collect.
/// </summary>
public class CurtainCall() : KahoCard(0, CardType.Skill, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<string> RegisteredKeywordIds => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(5, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardBlock(this, play);
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(3m);
  }
}
