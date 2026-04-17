using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
        Projectile.alpha = 0;
        Projectile.tileCollide = false;
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
        
        Projectile.scale = 1f + (float)Math.Sin(Projectile.ai[0] * 0.1f) * 0.1f;
        Projectile.rotation += Projectile.velocity.X * 0.02f + Projectile.direction * 0.02f;
        //Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.identity * 7) * 0.04f);
        if (Projectile.timeLeft < 40)
        {
            Projectile.alpha += 6;
        }
    }
}