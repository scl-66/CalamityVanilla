using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Ranged
{
    public class CloudArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.QuickDefaults();
            Projectile.arrow = true;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
            d.velocity *= 0.1f;
            d.alpha = 128;
        }
        public override void OnKill(int timeLeft)
        {
            for(int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
                d.velocity *= 0.5f;
                d.noGravity = !Main.rand.NextBool(3);
            }
        }
    }
}
