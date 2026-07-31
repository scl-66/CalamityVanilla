using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Achievements;

public class PriestGetBlessing : ModAchievement
{
    public CustomFlagCondition Condition { get; private set; }
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Explorer);

        Condition = AddCondition("ActivatePriestBlessing");
    }
    public override Position GetDefaultPosition() => new Before("THE_GREAT_SOUTHERN_PLANTKILL");
    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<CryogenKilled>());
    }
}