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

public class IceStatues : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public int TilesBroken = 0;
    public override void SetDefaults()
    {
        Projectile.alpha = 255;
        Projectile.QuickDefaults(true, 64);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60 * 10;
        TilesBroken = 0;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Rectangle frame = TextureAssets.Projectile[Type].Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        for (int i = 0; i < Math.Min(Projectile.ai[2] - 30, ProjectileID.Sets.TrailCacheLength[Type]); i++)
        {
            float percent = 1f - (i / 8f);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, frame, new Color(0.8f, 0.8f, 0.8f, 0.5f) * Projectile.Opacity * percent * 0.5f, Projectile.oldRot[i], frame.Size() / 2, Projectile.scale * (0.8f + (percent * 0.3f)), SpriteEffects.None);
        }
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frame, new Color(0.8f, 0.8f, 0.8f, 0.5f) * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
    public override void AI()
    {
        if (TilesBroken > 9)
        {
            Projectile.Kill();
            return;
        }
        Projectile.frame = Projectile.whoAmI % 5;
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 25;
        }
        Projectile.ai[2]++;
        if (Projectile.ai[2] < 0)
        {
            Projectile.rotation += Projectile.velocity.X * 0.01f;
            Projectile.velocity += Projectile.Center.DirectionTo(Main.npc[(int)Projectile.ai[1]].Center);
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
            d.velocity = Projectile.velocity;
            d.noGravity = true;
        }
        else if (Projectile.ai[2] < 30)
        {
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.Center.DirectionTo(Main.player[(int)Projectile.ai[0]].Center).ToRotation() + MathHelper.PiOver2, 0.2f);
            Projectile.velocity *= 0.9f;
        }
        else if (Projectile.ai[2] == 30)
        {
            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
            Projectile.extraUpdates = 2;
            Projectile.velocity = Projectile.Center.DirectionTo(Main.player[(int)Projectile.ai[0]].Center) * 20;
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[2] > 200)
                Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 20)
                Projectile.velocity.Y = 20;

            if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.localAI[0] = 1;

            List<Point> tiles = Collision.GetTilesIn(Projectile.position, Projectile.BottomRight);
            for (int i = 0; i < tiles.Count; i++)
            {
                if (Main.tile[tiles[i]].HasTile && Main.tile[tiles[i]].TileType == ModContent.TileType<CryogenIceTile>())
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        WorldGen.KillTile(tiles[i].X, tiles[i].Y, false, false, true);
                        NetMessage.SendTileSquare(-1, tiles[i].X, tiles[i].Y);
                        TilesBroken++;
                    }
                }
                else if (Projectile.localAI[0] == 1 && Main.tile[tiles[i]].HasTile && Main.tileSolid[Main.tile[tiles[i]].TileType] && !Main.tileSolidTop[Main.tile[tiles[i]].TileType])
                {
                    Projectile.Kill();
                    break;
                }
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
        {
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(14, 14), Mod.Find<ModGore>("IceStatueGoreGeneric_" + $"{i}").Type);
        }

        int goresNumber = 0;
        switch (Projectile.frame)
        {
            case 0:
                goresNumber = 3;
                break;
            case 1:
                goresNumber = 5;
                break;
            case 2:
                goresNumber = 3;
                break;
            case 3:
                goresNumber = 5;
                break;
            case 4:
                goresNumber = 3;
                break;
        }
        for (int i = 0; i < goresNumber + 1; i++)
        {
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(14, 14), Mod.Find<ModGore>("IceStatueGore" + $"{Projectile.frame}_{i}").Type);
        }

        SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
        for (int i = 0; i < 40; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.noGravity = !Main.rand.NextBool(3);
        }
    }
}