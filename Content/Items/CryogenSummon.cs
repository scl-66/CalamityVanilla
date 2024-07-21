using CalamityVanilla.Content.NPCs.Bosses.Cryogen;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items
{
    public class CryogenSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            // If this would be for a vanilla boss that has no summon item, you would have to include this line here:
            // NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

            // Otherwise the UseItem code to spawn it will not work in multiplayer
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool CanUseItem(Player player)
        {
            // If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
            // If you want more constraints for the summon item, combine them as boolean expressions:
            //    return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"
            return !NPC.AnyNPCs(ModContent.NPCType<Cryogen>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitly excluded serverside here)
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<Cryogen>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()

                .AddIngredient(ModContent.ItemType<CryogenSummonMold>())
                .AddIngredient(ItemID.IceBlock, 10)
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddIngredient(ItemID.SoulofMight, 3)
                .AddConsumeItemCallback((Recipe recipe, int type, ref int amount) =>
                {
                    if (type == ModContent.ItemType<CryogenSummonMold>())
                    {
                        amount = 0;
                    }
                })
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}