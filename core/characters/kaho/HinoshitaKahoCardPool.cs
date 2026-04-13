using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards.Kaho.Basic.Attack;
using RuriMegu.Core.Cards.Kaho.Basic.Skill;
using RuriMegu.Core.Cards.Kaho.Common.Attack;
using RuriMegu.Core.Cards.Kaho.Common.Power;
using RuriMegu.Core.Cards.Kaho.Common.Skill;
using RuriMegu.Core.Cards.Kaho.Rare.Attack;
using RuriMegu.Core.Cards.Kaho.Rare.Power;
using RuriMegu.Core.Cards.Kaho.Rare.Skill;
using RuriMegu.Core.Cards.Kaho.Uncommon.Attack;
using RuriMegu.Core.Cards.Kaho.Uncommon.Power;
using RuriMegu.Core.Cards.Kaho.Uncommon.Skill;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Card pool for Hinoshita Kaho-colored cards.
/// </summary>
public class HinoshitaKahoCardPool : CustomCardPoolModel {
  public override string Title => HinoshitaKaho.CHARACTER_NAME;

  public override string BigEnergyIconPath => "big_energy.png".CharacterUiPath(HinoshitaKaho.CHARACTER_ID);
  public override string TextEnergyIconPath => "text_energy.png".CharacterUiPath(HinoshitaKaho.CHARACTER_ID);

  public override float H => 0.017f;
  public override float S => 1.0f;
  public override float V => 0.745f;

  public override Color DeckEntryCardColor => HinoshitaKaho.Color;

  public override bool IsColorless => false;
}
