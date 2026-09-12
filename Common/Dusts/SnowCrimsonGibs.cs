using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityVanilla.Common.Dusts;

public class SnowCrimsonGibs : ModDust
{
    public override bool Update(Dust dust)
    {
        UpdateType = DustID.Blood;
        return base.Update(dust);
    }
}