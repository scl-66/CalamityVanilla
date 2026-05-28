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

public class IceMine : ModProjectile
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
        Projectile.timeLeft = 1200;
        Projectile.Opacity = 0;
    }
    public override void AI()
    {
        Projectile.velocity *= 0.98f;
        Projectile.Opacity += 0.1f;
        if (Projectile.timeLeft < 10)
            Projectile.Opacity -= 0.2f;
        foreach(Player p in Main.ActivePlayers)
        {
            if(p.Center.Distance(Projectile.Center) < 36)
            {
                Projectile.Kill();
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.1f, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        float amount = (float)(Main.timeForVisualEffects % 60) / 60f;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Blue with { A = 0 } * 0.5f * Projectile.Opacity * amount * (1f - amount), Projectile.rotation, tex.Size() / 2, Projectile.scale + (amount * 0.5f), SpriteEffects.None, 0);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        if (timeLeft <= 1)
            return;
        SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
        int iterations = 200;
        for(int i = 0; i < iterations; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowSpray);
            d.velocity = Vector2.UnitY.RotatedBy((i / (float)iterations * MathHelper.TwoPi) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(10,12);
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(2);
        }

        Point origin = Projectile.Center.ToTileCoordinates();

        int radius = 12;
        int innerRadius = 9;

        int radiusSquare = (radius + 1) * (radius + 1);
        int innerRadiusSquare = (innerRadius + 1) * (innerRadius + 1);

        for (int i = -radius; i <= radius; i++)
        {
            double num2 = (double)radius / (double)radius * (double)(i);
            int num3 = Math.Min(radius, (int)Math.Sqrt((double)radiusSquare - num2 * num2));

            for (int j = -num3; j <= num3; j++)
            {
                if((i*i) + (j*j) > innerRadiusSquare)
                    CryogenIceBlockSystem.PlaceIceBlock(j + origin.X, i + origin.Y,60 * 3);
            }
        }
    }
}
