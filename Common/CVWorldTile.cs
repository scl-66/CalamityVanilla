using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class CVWorldTile : ModSystem
{
    public static bool Solid(Point p, bool platforms) => Solid(p.X, p.Y, platforms);
    public static bool Solid(Vector2 v, bool platforms) => Solid((int)v.X, (int)v.Y, platforms);
    public static bool Solid(float i, float j, bool platforms) => Solid((int)i, (int)j, platforms);
    public static bool Solid(int i, int j, bool platforms = false)
    {
        Tile tile = Main.tile[i, j];
        return WorldGen.InWorld(i, j) && WorldGen.SolidTile(tile) || Main.tileSolidTop[tile.TileType];
    }
}