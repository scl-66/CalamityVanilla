using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.HiveMind.Drops.AdrianHelmet;

[AutoloadEquip(EquipType.Head)]
public class AdrianHelmet : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 20;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2);
        Item.vanity = true;
    }
}