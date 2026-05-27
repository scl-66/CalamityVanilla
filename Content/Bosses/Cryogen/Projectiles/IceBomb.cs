using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen.Projectiles;

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
            SoundEngine.PlaySound(SoundID.Item131, Projectile.position);
        }
        Projectile.Opacity += 0.2f;
        if (Projectile.timeLeft <= 1)
        {
            foreach (Player p in Main.ActivePlayers)
            {
                if (p.Center.Distance(Projectile.Center) < 16 * 12)
                {
                    Projectile.timeLeft++;
                    break;
                }
            }
        }
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
        d.noGravity = true;
        d.velocity += Projectile.velocity;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float percent = i / (float)Projectile.oldPos.Length;
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 2), null, new Color(0.25f, 0.25f, 0.25f, 0.25f) * (1f - percent) * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        float amount = (int)(Main.timeForVisualEffects % 20) / 20f * Projectile.Opacity;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f,1f,1f,0f) * amount * (1f - amount) * Projectile.Opacity * 2, Projectile.rotation, tex.Size() / 2, Projectile.scale + (amount * 0.5f), SpriteEffects.None, 0);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
        int iterations = 200;
        for(int i = 0; i < iterations; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowSpray);
            d.velocity = Vector2.UnitY.RotatedBy((i / (float)iterations * MathHelper.TwoPi) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(10,12);
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(2);
        }

        int radius = 6;
        Point origin = Projectile.Center.ToTileCoordinates();
        int num = (radius + 1) * (radius + 1);
        for (int i = origin.Y - radius; i <= origin.Y + radius; i++)
        {
            double num2 = (double)radius / (double)radius * (double)(i - origin.Y);
            int num3 = Math.Min(radius, (int)Math.Sqrt((double)num - num2 * num2));
            for (int j = origin.X - num3; j <= origin.X + num3; j++)
            {
                CryogenIceBlockSystem.PlaceIceBlock(j, i,1800 / 4);
            }
        }
    }
}
