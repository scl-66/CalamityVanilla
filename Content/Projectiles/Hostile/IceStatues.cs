using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Hostile
{
    public class IceStatues : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0.8f, 0.8f, 0.8f, 0.5f) * Projectile.Opacity;
        }
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.QuickDefaults(true, 64);
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.frame = Projectile.whoAmI % 5;
            if(Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            Projectile.ai[2]++;
            if (Projectile.ai[2] < 0)
            {
                Projectile.rotation += Projectile.velocity.X * 0.01f;
                Projectile.velocity += Projectile.Center.DirectionTo(Main.npc[(int)Projectile.ai[1]].Center);
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
                d.velocity = Projectile.velocity;
                d.noGravity = true;
            }
            else if(Projectile.ai[2] < 30)
            {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.Center.DirectionTo(Main.player[(int)Projectile.ai[0]].Center).ToRotation() + MathHelper.PiOver2, 0.2f);
                Projectile.velocity *= 0.9f;
            }
            else if (Projectile.ai[2] == 30)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                Projectile.tileCollide = true;
                Projectile.extraUpdates = 1;
                Projectile.velocity = Projectile.Center.DirectionTo(Main.player[(int)Projectile.ai[0]].Center) * 20;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Projectile.ai[2] > 200)
                Projectile.velocity.Y += 0.2f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            for(int i = 0; i < 40; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.noGravity = !Main.rand.NextBool(3);
            }
        }
    }
}
