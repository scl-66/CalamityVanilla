using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen.Projectiles;

public class Icicles : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 4;
    }
    public override void SetDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        Projectile.QuickDefaults(true, 30);
        Projectile.timeLeft = 180;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        if(Projectile.timeLeft == 179)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
        }
        Projectile.frame = Projectile.identity % 3;
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        //if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
        //    Projectile.tileCollide = true;

        if (Main.rand.NextBool())
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 30, 30, DustID.FrostStaff);
            d.noGravity = true;
            d.velocity *= 0.2f;
            d.fadeIn = Main.rand.NextFloat(1.4f);
            d.velocity += Projectile.velocity;
        }
        if(Projectile.timeLeft < 30)
        {
            Projectile.velocity *= 1f - (1f / 20f);
            Dust d = Dust.NewDustDirect(Projectile.position, 30, 30, DustID.FrostStaff);
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(1.4f);
            d.velocity += Projectile.velocity;
        }

        List<Point> tiles = Collision.GetTilesIn(Projectile.position + Projectile.velocity - new Vector2(4), Projectile.BottomRight + Projectile.velocity + new Vector2(4));
        for (int i = 0; i < tiles.Count; i++)
        {
            if (Main.tile[tiles[i]].HasTile && Main.tile[tiles[i]].TileType == ModContent.TileType<CryogenIceTile>())
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    WorldGen.KillTile(tiles[i].X, tiles[i].Y, false, false, true);
                    NetMessage.SendTileSquare(-1, tiles[i].X, tiles[i].Y);
                }
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float percent = i / (float)Projectile.oldPos.Length;
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 2), sourceRect, lightColor * (1f - percent) * 0.5f, Projectile.oldRot[i], sourceRect.Size() / 2, Projectile.scale - (i * 0.075f), SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, lightColor, Projectile.rotation, sourceRect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        //Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, Color.White with { A = 0} * 0.25f, Projectile.rotation, sourceRect.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        //SoundEngine.PlaySound(SoundID.Item27);
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.FrostStaff);
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(2f);
            d.velocity = Main.rand.NextVector2Circular(5, 5);
        }
    }
}