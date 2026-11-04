using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Tundra.Cryonic.Bar;
using CalamityVanilla.Content.Tundra.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen;

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
        return !NPC.AnyNPCs(ModContent.NPCType<Cryogen>()) && player.ZoneSnow;
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
            .AddIngredient(AutoContent.ItemType<CryonicBar>(), 3)
            .AddIngredient(ModContent.ItemType<EleumSoul>(), 6)
            .AddRecipeGroup("CalamityVanillaAnyIceBlock", 25)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}