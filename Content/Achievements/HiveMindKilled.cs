using CalamityVanilla.Content.Bosses.HiveMind;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Achievements;

public class HiveMindKilled : ModAchievement
{
    public override void SetStaticDefaults()
    {
        //Filters this achievement as part of the "Slayer" achievement category.
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition("HiveMindKillCondition", ModContent.NPCType<HiveMind>());
    }

    // By default a ModAchievement will be placed at the end of the achievement ordering.
    // GetDefaultPosition is used to position a ModAchievement in relation to vanilla achievements.
    // Since MinionBoss is similar to Eye of Cthulhu, we place it after its achievement, "EYE_ON_YOU".
    public override Position GetDefaultPosition() => new Before("BUCKETS_OF_BOLTS");
}