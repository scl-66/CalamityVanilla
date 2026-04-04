using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Dusts;
public class SnowCorruptGibs : ModDust
{
    public override bool Update(Dust dust)
    {
        UpdateType = DustID.CorruptGibs;
        return base.Update(dust);
    }
}