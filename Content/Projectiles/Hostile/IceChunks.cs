using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Hostile
{
    public class IceChunks : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true,16);
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.frame = Projectile.whoAmI % 3;
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
