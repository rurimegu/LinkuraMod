using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Special Appeal (特别演出) — Cost 1, Power, Uncommon.
/// Whenever you Collect Hearts, deal damage to all enemies.
/// Upgrade: Innate.
/// </summary>
public class SpecialAppeal() : KahoCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None) {

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Collect),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<SpecialAppealPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Innate);
  }
}
