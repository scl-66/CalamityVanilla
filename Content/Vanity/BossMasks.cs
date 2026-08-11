using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Vanity;

public abstract class BossMask : ModItem
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
public class HiveMindMask : BossMask
{
    public override string Texture => Assets.Textures.Vanity.BossMasks.HiveMindMask.KEY;
}
[AutoloadEquip(EquipType.Head)]
public class PerforatorsMask : BossMask
{
    public override string Texture => Assets.Textures.Vanity.BossMasks.PerforatorsMask.KEY;
}
[AutoloadEquip(EquipType.Head)]
public class CryogenMask : BossMask
{
    public override string Texture => Assets.Textures.Vanity.BossMasks.CryogenMask.KEY;
}