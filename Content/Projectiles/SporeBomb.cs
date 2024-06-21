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

namespace CalamityVanilla.Content.Projectiles
{
    public class SporeBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true, 16);
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            if(Projectile.timeLeft == 200)
            {
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            }
            Projectile.ai[0] *= 1.001f;
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] * 0.01f);

            Projectile.frameCounter++;
            if(Projectile.frameCounter == 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if(Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for(int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4, 4), ModContent.ProjectileType<Spores>(), 23, 2);
                }
            }
        }
    }
}
