using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class IceChunks : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 16);
        Projectile.timeLeft = 600;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 1 / 120f;
        if (Projectile.ai[0] > 1f)
            Projectile.ai[0] = 1f;

        Projectile.velocity.Y += 0.2f;
        Projectile.frame = Projectile.whoAmI % 3;
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.rotation += Projectile.velocity.X * 0.02f;

        if (Projectile.localAI[0] > 9)
            Projectile.Kill();

        if (Projectile.timeLeft < 590)
        {
            List<Point> tiles = Collision.GetTilesIn(Projectile.position, Projectile.BottomRight);
            for (int i = 0; i < tiles.Count; i++)
            {
                if (Main.tile[tiles[i]].HasTile && Main.tile[tiles[i]].TileType == ModContent.TileType<CryogenIceTile>())
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        WorldGen.KillTile(tiles[i].X, tiles[i].Y, false, false, true);
                        NetMessage.SendTileSquare(-1, tiles[i].X, tiles[i].Y);
                        Projectile.localAI[0]++;
                    }
                }
                else if (Main.tile[tiles[i]].HasTile && Main.tileSolid[Main.tile[tiles[i]].TileType] && !Main.tileSolidTop[Main.tile[tiles[i]].TileType])
                {
                    Projectile.Kill();
                    break;
                }
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
            d.velocity = -Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat();
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        for (int i = 0; i < 4; i++)
        {
            Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 4).RotatedBy(i * MathHelper.PiOver2), tex.Frame(1, 3, 0, Projectile.frame), (Cryogen.GetAuroraColor(Projectile.timeLeft + Projectile.whoAmI * 5) * (1f - Projectile.ai[0])) with { A = 0 }, Projectile.rotation, new Vector2(16), Projectile.scale, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition, tex.Frame(1, 3, 0, Projectile.frame), lightColor, Projectile.rotation, new Vector2(16), Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}