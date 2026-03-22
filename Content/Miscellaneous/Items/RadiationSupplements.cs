using CalamityVanilla.Content.Buffs.Debuffs;
using CalamityVanilla.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items;

public class RadiationSupplements : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 36;
        Item.maxStack = 1;
        Item.value = Item.sellPrice(45, 0);
        Item.accessory = true;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.buffImmune[ModContent.BuffType<IrradiatedDebuff>()] = true;
    }
}
