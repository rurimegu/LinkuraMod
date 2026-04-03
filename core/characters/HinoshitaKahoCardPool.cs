using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards.Kaho.Basic.Attack;
using RuriMegu.Core.Cards.Kaho.Basic.Skill;
using RuriMegu.Core.Cards.Kaho.Common.Attack;
using RuriMegu.Core.Cards.Kaho.Common.Power;
using RuriMegu.Core.Cards.Kaho.Common.Skill;
using RuriMegu.Core.Cards.Kaho.Rare.Attack;
using RuriMegu.Core.Cards.Kaho.Uncommon.Attack;
using RuriMegu.Core.Cards.Kaho.Uncommon.Skill;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters;

/// <summary>
/// Card pool for Hinoshita Kaho-colored cards.
/// </summary>
public class HinoshitaKahoCardPool : CustomCardPoolModel {
  public override string Title => HinoshitaKaho.CharacterName;

  public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
  public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

  public override float H => 0.017f;
  public override float S => 1.0f;
  public override float V => 0.745f;

  public override Color DeckEntryCardColor => new("be1400");

  public override bool IsColorless => false;

  protected override CardModel[] GenerateAllCards() {
    return [
      ModelDb.Card<KahoStrike>(),
      ModelDb.Card<KahoDefend>(),
      ModelDb.Card<WideHeart>(),
      // Common attack cards
      ModelDb.Card<BuddingPremonition>(),
      ModelDb.Card<FinalAct>(),
      ModelDb.Card<TrainingOutcomes>(),
      ModelDb.Card<KahoSmash>(),
      ModelDb.Card<Fantasy375>(),
      ModelDb.Card<StepUp>(),
      ModelDb.Card<BunnyPyonPyon>(),
      ModelDb.Card<CeriseBouquet>(),
      // Common skill cards
      ModelDb.Card<SpringBreezeSwing>(),
      ModelDb.Card<BunnyDefend>(),
      ModelDb.Card<GenyouYakou>(),
      ModelDb.Card<Zanyou>(),
      ModelDb.Card<NewBlack>(),
      ModelDb.Card<CurtainCall>(),
      ModelDb.Card<FullOfPower>(),
      ModelDb.Card<AgileSteps>(),
      ModelDb.Card<LoveIt>(),
      ModelDb.Card<MixShake>(),
      ModelDb.Card<FanService>(),
      // Common power cards
      ModelDb.Card<GlowingRoutine>(),
      // Uncommon attack cards
      ModelDb.Card<UnfulfilledWishes>(),
      ModelDb.Card<Variations>(),
      ModelDb.Card<Afterglow>(),
      ModelDb.Card<BuildUp>(),
      ModelDb.Card<SuddenInspiration>(),
      ModelDb.Card<FullBloomSprint>(),
      ModelDb.Card<EphemeralYouth>(),
      ModelDb.Card<HeartStrike>(),
      ModelDb.Card<BackstagePreparation>(),
      ModelDb.Card<OverflowingEmotion>(),
      // Uncommon skill cards
      ModelDb.Card<SpecialThanks>(),
      ModelDb.Card<MysteryAtTheLibrary>(),
      ModelDb.Card<DreamBelievers>(),
      ModelDb.Card<AuroraFlower>(),
      ModelDb.Card<OnYourMark>(),
      ModelDb.Card<Soulmate>(),
      ModelDb.Card<MentalGuard>(),
      ModelDb.Card<HalfConveyedFeelings>(),
      ModelDb.Card<FallBackAsleep>(),
      ModelDb.Card<Encore>(),
      ModelDb.Card<Celebration>(),
      ModelDb.Card<TrueFacePixel>(),
      ModelDb.Card<FullBloom>(),
      ModelDb.Card<ChoreographyMemo>(),
      ModelDb.Card<LinkToTheFuture>(),
      ModelDb.Card<SukiSukiClub>(),
      // Rare attack cards
      ModelDb.Card<Srk>(),
      ModelDb.Card<EmbracingPetals>(),
      ModelDb.Card<SayoShigure>(),
    ];
  }
}
