using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Fan Service (楗拻) 鈥?Cost 1, Skill, Common.
/// Collect. The damage from Collect also triggers on 1(2) additional random enemy.
/// </summary>
public class FanService() : KahoCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new RepeatVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.CollectHearts(this, ctx, null, triggers: DynamicVars.Repeat.IntValue + 1);
  }

  protected override void OnUpgrade() {
    DynamicVars.Repeat.UpgradeValueBy(1m);
  }
}
