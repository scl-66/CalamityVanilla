using CalamityVanilla.Content.Vanity.BossMasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops;

// Basic code for a boss treasure bag
public class HiveMindBag : ModItem
{
    public override void SetStaticDefaults()
    {
        // This set is one that every boss bag should have.
        // It will create a glowing effect around the item when dropped in the world.
        // It will also let our boss bag drop dev armor..
        ItemID.Sets.BossBag[Type] = true;

        // This prevents our boss bag from dropping developer armor.
        // Since Queen Slime does not drop developer armor, it only makes sense
        // That The Hive Mind does not drop dev armor, as they are on the same tier.
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;

        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Purple;
        Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        // We have to replicate the expert drops from MinionBossBody here

        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<HiveMindMask>(), 7));
        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<FilthyGrip.FilthyGrip>(), 3));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkinBoilHat.SkinBoilHat>()));
        itemLoot.Add(ItemDropRule.OneFromOptions(1,
            ModContent.ItemType<PerfectDark.PerfectDark>(),
            ModContent.ItemType<MushroomBomber.MushroomBomber>(),
            ModContent.ItemType<MyceliumStaff.MyceliumStaff>(),
            ModContent.ItemType<SinisterIncubator.SinisterIncubator>()));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AdrianHelmet.AdrianHelmet>(), 50));
        itemLoot.Add(ItemDropRule.Common(ItemID.SoulofNight, 1, 10, 15));
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<HiveMind>()));
    }
}