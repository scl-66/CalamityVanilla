using CalamityVanilla.Content.Tundra.Cryogen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.World;

public class BiomeTileCounter : ModSystem
{
    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        Main.SceneMetrics.SnowTileCount += tileCounts[ModContent.TileType<CryogenIceTile>()] * 5;
    }
}
