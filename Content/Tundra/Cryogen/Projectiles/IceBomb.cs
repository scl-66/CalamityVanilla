using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Content.Tundra.Cryogen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen.Projectiles;

public class IceBomb : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 6;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 36);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 90;
        Projectile.Opacity = 0;
    }
    public override void AI()
    {
        if (Projectile.timeLeft == 89)
        {
            SoundEngine.PlaySound(SoundID.Item106, Projectile.position);
        }
        Projectile.Opacity += 0.2f;
        if (Projectile.timeLeft <= 1)
        {
            foreach (Player p in Main.ActivePlayers)
            {
                if (p.Center.Distance(Projectile.Center) < 16 * 12)
                {
                    Projectile.timeLeft+= 2;
                    break;
                }
            }
        }
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
        d.noGravity = true;
        d.velocity += Projectile.velocity;
        if (Projectile.ai[2] == 1)
        {
            Main.tileSolid[ModContent.TileType<CryogenIceTile>()] = false;
            Vector2 vect = Collision.AnyCollisionWithSpecificTiles(Projectile.position - Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height, Main.tileSolid);
            Main.tileSolid[ModContent.TileType<CryogenIceTile>()] = true;
            if (vect.X != Projectile.velocity.X)
            {
                Projectile.velocity.X *= -1;
            }
            if (vect.Y != Projectile.velocity.Y)
                Projectile.velocity.Y *= -1;
        }
        else if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
        {
            Projectile.ai[2] = 1;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var tex = TextureAssets.Projectile[Type].Value;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float percent = i / (float)Projectile.oldPos.Length;
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 2), null, new Color(0.25f, 0.25f, 0.25f, 0.25f) * (1f - percent) * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        float amount = (int)(Main.timeForVisualEffects % 20) / 20f * Projectile.Opacity;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * amount * (1f - amount) * Projectile.Opacity * 2, Projectile.rotation, tex.Size() / 2, Projectile.scale + (amount * 0.5f), SpriteEffects.None, 0);
        
        var ring = TextureAssets.Extra[ExtrasID.KeybrandRing].Value;
        float ringGlow = Math.Max(1f - (Projectile.timeLeft / 90f),0);
        ringGlow *= ringGlow * ringGlow;
        Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, null, Cryogen.GetAuroraColor((int)(Main.timeForVisualEffects + Projectile.identity * 32)) with { A = 64 } * ringGlow, Projectile.rotation, ring.Size() / 2, 1.4f - ringGlow, SpriteEffects.None, 0);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(Assets.Sounds.IceBombExplode.Asset with { MaxInstances = 10, pitchVariance = 0.3f}, Projectile.position);
        SoundEngine.PlaySound(Assets.Sounds.IceBurn.Asset with { pitchVariance = 0.3f }, Projectile.position);
        int iterations = 200;
        int cryonicDust = ModContent.DustType<CryonicSmokeDust>();
        for (int i = 0; i < iterations; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next([DustID.Ice,DustID.Snow, cryonicDust]));
            d.velocity = Vector2.UnitY.RotatedBy((i / (float)iterations * MathHelper.TwoPi) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(5, 12);
            d.noGravity = true;
            if(d.type == cryonicDust)
            {
                d.fadeIn = Main.rand.Next(4, 8);
                d.scale *= Main.rand.NextFloat(2,3);
                d.noLight = true;
            }
            else
            d.fadeIn = Main.rand.NextFloat(2);
        }
        for(int i = 0; i < 5; i++)
        {
            Gore g = Gore.NewGoreDirect(Projectile.position + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(4, 4), 11 + Main.rand.Next(3));
        }

        int radius = Main.rand.Next(5,8);
        Point origin = Projectile.Center.ToTileCoordinates();
        int num = (radius + 1) * (radius + 1);
        for (int i = origin.Y - radius; i <= origin.Y + radius; i++)
        {
            double num2 = (double)radius / (double)radius * (double)(i - origin.Y);
            int num3 = Math.Min(radius, (int)Math.Sqrt((double)num - num2 * num2));
            for (int j = origin.X - num3; j <= origin.X + num3; j++)
            {
                CryogenIceBlockSystem.PlaceIceBlock(j, i, 450);
            }
        }
    }
}