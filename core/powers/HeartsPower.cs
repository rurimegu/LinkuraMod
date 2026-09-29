using RuriMegu.Core.Characters.Kaho;

namespace RuriMegu.Core.Powers;

public class HeartsPower : LinkuraPower {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
  protected override bool IsVisibleInternal => false;
}
