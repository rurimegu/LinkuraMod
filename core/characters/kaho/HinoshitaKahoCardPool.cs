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
      ModelDb.Card<SectionChange>(),
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
      // Uncommon power cards
      ModelDb.Card<SugarMelt>(),
      ModelDb.Card<SpecialAppeal>(),
      ModelDb.Card<UnadornedBeauty>(),
      ModelDb.Card<AutoCollectOn>(),
      ModelDb.Card<HolidayHoliday>(),
      ModelDb.Card<WelcomeIshikawa>(),
      ModelDb.Card<TrainingCamp>(),
      ModelDb.Card<CareerSurvey>(),
      // Rare attack cards
      ModelDb.Card<Srk>(),
      ModelDb.Card<EmbracingPetals>(),
      ModelDb.Card<SayoShigure>(),
      // Rare skill cards
      ModelDb.Card<AThousandChanges>(),
      ModelDb.Card<SummeryPain>(),
      ModelDb.Card<PresentPastFuture>(),
      ModelDb.Card<IsThatCloudAWhale>(),
      ModelDb.Card<ExcessiveTraining>(),
      ModelDb.Card<EnduringTradition>(),
      ModelDb.Card<TwoSecondsOfEternity>(),
      ModelDb.Card<BallOfTrails>(),
      // Rare power cards
      ModelDb.Card<AutoClickerOn>(),
      ModelDb.Card<FlowerKnot>(),
      ModelDb.Card<ReflectionInTheMirror>(),
      ModelDb.Card<Prologue>(),
      ModelDb.Card<MayDreamsBloom>(),
      ModelDb.Card<BloomGardenParty>(),
    ];
  }
}
