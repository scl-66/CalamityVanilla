using CalamityVanilla.Common.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen;

public class CryogenIceTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = false;
        DustType = ModContent.DustType<CryogenIceBlockDust>();
        HitSound = SoundID.Item27 with { MaxInstances = 10, volume = 0.5f, pitch = 0.2f};
        TileID.Sets.ClearedOnWorldLoad[Type] = true;
    }
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num /= 3;
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r += 0.2f;
        g += 0.4f;
        b += 0.5f;
    }
    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        //frameXOffset = i % 4 * 234;
        //frameYOffset = j % 4 * 90;
        frameYOffset = j % 2 * 90;
    }
    public override void PlaceInWorld(int i, int j, Item item)
    {
        base.PlaceInWorld(i, j, item);
    }
}