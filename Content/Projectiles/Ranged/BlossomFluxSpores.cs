using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Ranged
{
    public class BlossomFluxSpores : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(false, 36);
            Projectile.timeLeft = 600;
            Projectile.alpha = 128;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 10)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame == 5)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.ai[0]++;
            Projectile.velocity *= 0.99f;
            Projectile.scale = 1f + (float)Math.Sin(Projectile.ai[0] * 0.1f) * 0.1f;

            if (Projectile.timeLeft < 20)
            {
                Projectile.alpha += 6;
            }
        }
    }
}
