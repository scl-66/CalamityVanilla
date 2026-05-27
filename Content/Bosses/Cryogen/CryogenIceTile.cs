using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class CryogenIceTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = false;
        DustType = ModContent.DustType<CryogenIceBlockDust>();
        HitSound = SoundID.Item27;
        TileID.Sets.ClearedOnWorldLoad[Type] = true;
    }
    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        //if (Main.rand.NextBool(60))
        //{
        //    Dust d = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, DustType, 0, 0, 128);
        //    d.noGravity = false;
        //    d.velocity *= 0.2f;
        //}
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