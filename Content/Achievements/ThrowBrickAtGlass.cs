using CalamityVanilla.Content.Bosses.Cryogen;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Achievements;

public class ThrowBrickAtGlass : ModAchievement
{
    public override void SetStaticDefaults()
    {
        //Filters this achievement as part of the "Slayer" achievement category.
        Achievement.SetCategory(AchievementCategory.Challenger);
    }
    public override Position GetDefaultPosition() => new Before("VEHICULAR_MANSLAUGHTER");
}