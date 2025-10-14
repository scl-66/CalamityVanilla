using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Vanity.BossMasks;

[AutoloadEquip(EquipType.Head)]
public class HiveMindMask : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 20;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 75);
        Item.vanity = true;
    }
}
[AutoloadEquip(EquipType.Head)]
public class PerforatorsMask : HiveMindMask;
[AutoloadEquip(EquipType.Head)]
public class CryogenMask : HiveMindMask;