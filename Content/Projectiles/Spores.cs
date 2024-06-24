using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles
{
    public class Spores : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true,36);
            Projectile.timeLeft = 600;
            Projectile.alpha = 128;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.alpha = 128;
                d.velocity *= 0.4f;
            }

            Projectile.ai[0]++;
            Projectile.velocity *= 0.99f;
            Projectile.scale = 1f + (float)(Math.Sin(Projectile.ai[0] * 0.1f)) * 0.1f;
            Projectile.rotation += (Projectile.velocity.X * 0.02f) + Projectile.direction * 0.02f;

            if(Projectile.timeLeft < 20)
            {
                Projectile.alpha+= 6;
            }
        }
    }
}
