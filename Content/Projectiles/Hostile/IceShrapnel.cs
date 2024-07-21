using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Hostile
{
    public class IceShrapnel : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true, 16);
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 3;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0.5f);
        }
        public override void AI()
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
            d.velocity = Projectile.velocity;
            d.noGravity = true;
            d.scale = 1.5f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
