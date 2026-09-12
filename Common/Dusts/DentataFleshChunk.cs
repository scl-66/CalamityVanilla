using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public sealed class DentataFleshChunk : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(0, 12 * Main.rand.Next(5), 12, 12);
    }

    public override bool PreDraw(Dust dust)
    {
        Main.EntitySpriteDraw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, dust.GetAlpha(Lighting.GetColor(dust.position.ToTileCoordinates())).MultiplyRGBA(Color.DarkRed with { A = 100 }), dust.rotation, dust.frame.Size() / 2, dust.scale * 1.2f, 0, 0);
        Main.EntitySpriteDraw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, dust.GetAlpha(Lighting.GetColor(dust.position.ToTileCoordinates())), dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);

        return false;
    }
}