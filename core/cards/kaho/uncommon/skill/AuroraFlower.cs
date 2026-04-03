using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Aurora Flower (极光花) — Cost 1, Skill, Uncommon.
/// Trigger 4 (6) [gold]Auto Burst[/gold]. Collect after each trigger.
/// </summary>
public class AuroraFlower() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int BASE_BURST_AMOUNT = 1;
  private const string TRIGGERS_VAR = "RURIMEGU-AURORA_TRIGGERS";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(TRIGGERS_VAR, 4),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int triggers = DynamicVars[TRIGGERS_VAR].IntValue;
    for (int i = 0; i < triggers; i++) {
      await LinkuraCmd.TriggerAutoBurst(Owner, ctx, this);
      await LinkuraCardActions.CollectHearts(this, ctx);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[TRIGGERS_VAR].UpgradeValueBy(2m);
  }
}
