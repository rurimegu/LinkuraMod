using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards.Kaho.Basic.Attack;
using RuriMegu.Core.Cards.Kaho.Basic.Skill;
using RuriMegu.Core.Relics.Kaho.Starter;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Hinoshita Kaho
/// </summary>
public class HinoshitaKaho : LinkuraCharacterModel {
  public const string CHARACTER_ID = "kaho";
  public const string CHARACTER_NAME = "Hinoshita Kaho";
  public override string CharacterId => CHARACTER_ID;
  public override string CharacterName => CHARACTER_NAME;

  public static readonly Color Color = new("f8b400");

  public override Color NameColor => Color;
  public override Color MapDrawingColor => Color;

  public override int StartingHp => 80;

  public override IEnumerable<CardModel> StartingDeck => [
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<LinkuraEnergy>(),
    ModelDb.Card<WideHeart>(),
  ];

  public override IReadOnlyList<RelicModel> StartingRelics => [
    ModelDb.Relic<LinkuraSystem>(),
  ];

  public override CardPoolModel CardPool => ModelDb.CardPool<HinoshitaKahoCardPool>();
  public override RelicPoolModel RelicPool => ModelDb.RelicPool<KahoRelicPool>();
  public override PotionPoolModel PotionPool => ModelDb.PotionPool<KahoPotionPool>();
}
