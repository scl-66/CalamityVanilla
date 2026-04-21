using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Projectiles;

public class Spores : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 36);
        Projectile.timeLeft = 600;
        Projectile.alpha = 64;
        Projectile.tileCollide = false;
        Projectile.frame = Main.rand.Next(3);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        Color c = lightColor * Projectile.Opacity;
        c.A /= 2;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, c, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, c * Projectile.Opacity * 0.25f, Projectile.rotation, frame.Size() / 2, Projectile.scale + (Projectile.Opacity * 0.75f), SpriteEffects.None, 0);
        return false;
    }
    public override void AI()
    {
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128 + (Projectile.alpha / 2);
            d.velocity *= 0.4f;
        }

        foreach(Projectile p in Main.ActiveProjectiles)
        {
            if (p.type == Type && p.Center != Projectile.Center && p.Center.Distance(Projectile.Center) < 36)
                p.velocity += Projectile.Center.DirectionTo(p.Center) * 0.01f;
        }

        Projectile.ai[0]++;
            Projectile.velocity *= 0.95f;
        
        Projectile.scale = (0.5f + Projectile.Opacity * 0.5f) + (float)Math.Sin(Projectile.ai[0] * 0.1f) * 0.1f;
        Projectile.rotation += Projectile.velocity.X * 0.02f + Projectile.direction * 0.02f;
        //Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.identity * 7) * 0.04f);
        if (Projectile.timeLeft < 40)
        {
            Projectile.alpha += 6;
        }
    }
}