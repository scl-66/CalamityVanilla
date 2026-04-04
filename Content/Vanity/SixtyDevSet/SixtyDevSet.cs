using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Vanity.SixtyDevSet;

[AutoloadEquip(EquipType.Head)]
public class SixtyHead : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}
