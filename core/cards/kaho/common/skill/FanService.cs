using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Fan Service (饭撒) — Cost 1 (0), Skill, Common.
/// Collect. This Collect deals damage to ALL enemies.
/// </summary>
public class FanService() : KahoCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(LinkuraKeywords.Collect.GetModCardKeyword());

  protected override IEnumerable<DynamicVar> CanonicalVars => [];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.CollectHearts(this, ctx, damageAllEnemies: true);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
