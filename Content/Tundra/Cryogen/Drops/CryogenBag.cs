using CalamityVanilla.Content.Vanity.BossMasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen.Drops;

// Basic code for a boss treasure bag
public class CryogenBag : ModItem
{
    public override void SetStaticDefaults()
    {
        // This set is one that every boss bag should have.
        // It will create a glowing effect around the item when dropped in the world.
        // It will also let our boss bag drop dev armor..
        ItemID.Sets.BossBag[Type] = true;

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

        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<PermafrostHook.PermafrostHook>()));
        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CryogenMask>(), 7));
        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<TheSnowman.TheSnowman>(), 5));
        itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Icebreaker.Icebreaker>(), ModContent.ItemType<HoarfrostBow.HoarfrostBow>()));
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Cryogen>()));
    }
}