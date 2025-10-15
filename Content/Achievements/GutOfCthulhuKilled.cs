using CalamityVanilla.Content.Bosses.Cryogen;
using CalamityVanilla.Content.Bosses.GutOfCthulhu;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Achievements;

public class GutOfCthulhuKilled : ModAchievement {
    public override void SetStaticDefaults() {
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition("GutOfCthulhuKillCondition", ModContent.NPCType<GutOfCthulhu>());
    }

    public override Position GetDefaultPosition() => new Before("BUCKETS_OF_BOLTS");
}