using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Magic
{
    public class IchorBlob : ModProjectile
    {
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0.85f) * Projectile.Opacity;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(false,22);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.2f;
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor);
                d.velocity += Projectile.velocity;
                d.noGravity = true;
            }
            Lighting.AddLight(Projectile.position, 0.2f, 0.15f, 0);

            Projectile.ai[0]++;
            Projectile.scale = 0.9f + MathF.Sin(Projectile.ai[0] * 0.2f) * 0.1f;
            Projectile.Opacity = 0.9f + MathF.Sin(Projectile.ai[0] * 0.2f) * 0.1f;
            Projectile.Opacity *= MathHelper.Clamp(Projectile.ai[0] * 0.1f, 0, 1);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(5))
                target.AddBuff(BuffID.Ichor, 60 * 6);
        }
        public override void OnKill(int timeLeft)
        {
            for(int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.noGravity = true;
            }
        }
    }
}
