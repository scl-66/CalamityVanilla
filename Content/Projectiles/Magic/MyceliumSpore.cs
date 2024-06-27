using Microsoft.CodeAnalysis.CSharp.Syntax;
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

namespace CalamityVanilla.Content.Projectiles.Magic
{
    public class MyceliumSpore : ModProjectile
    {
        NPC target = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(false, 18);
            Projectile.penetrate = 5;
            Projectile.timeLeft = 60 * 10;
        }

        public override void AI()
        {
            target = CVUtils.FindClosestNPC(700f, Projectile.Center);

            if (Projectile.velocity.Y > 0f && target == null) Projectile.velocity.X = (float)Math.Sin(Projectile.ai[0] / 15) * 1f;
            else if (Projectile.velocity.Y > 0f && target != null)
            {
                if (Math.Abs(target.Center.X - Projectile.Center.X) > 2f)
                {
                    if ((target.Center.X - Projectile.Center.X) / (Math.Abs(target.Center.X - Projectile.Center.X)) == Projectile.velocity.X/Math.Abs(Projectile.velocity.X))
                    {
                        Projectile.velocity.X += (target.Center.X - Projectile.Center.X) / (Math.Abs(target.Center.X - Projectile.Center.X)) * 0.08f;
                    }
                    else Projectile.velocity.X += (target.Center.X - Projectile.Center.X) / (Math.Abs(target.Center.X - Projectile.Center.X)) * 0.2f;
                }
            }
            else Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y + 0.1f, -10f, 2f);
            Projectile.ai[0]++;

            Projectile.rotation = Projectile.velocity.X/5f;

            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Corruption, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.2f, 0.2f));
                Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Cloud, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.2f, 0.2f));
            }
        }
    }
}
