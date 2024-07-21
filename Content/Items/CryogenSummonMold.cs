using CalamityVanilla.Content.NPCs.Bosses.Cryogen;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items
{
    public class CryogenSummonMold : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            // If this would be for a vanilla boss that has no summon item, you would have to include this line here:
            // NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

            // Otherwise the UseItem code to spawn it will not work in multiplayer
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.maxStack = 1;
            Item.value = 100;
            Item.rare = ItemRarityID.Pink;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.EverythingElse;
        }
    }
}