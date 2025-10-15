using CalamityVanilla.Content.Bosses.Cryogen;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Achievements;

public class ThrowBrickAtGlass : ModAchievement
{
    public CustomFlagCondition Condition { get; private set; }
    public override void SetStaticDefaults()
    {
        //Filters this achievement as part of the "Slayer" achievement category.
        Achievement.SetCategory(AchievementCategory.Challenger);

        Condition = AddCondition("BrickBreaksGlass");
    }
    public override Position GetDefaultPosition() => new Before("VEHICULAR_MANSLAUGHTER");
}