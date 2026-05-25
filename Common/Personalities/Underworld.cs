using Terraria;
using Terraria.GameContent.Personalities;

namespace CalamityVanilla.Common.Personalities;

public class Underworld : AShoppingBiome
{
    public Underworld()
    {
        base.NameKey = "Underworld";
    }

    public override bool IsInBiome(Player player)
    {
        return player.ZoneUnderworldHeight;
    }
}