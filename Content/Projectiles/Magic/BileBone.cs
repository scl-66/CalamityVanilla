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
    public class BileBone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(false,20);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.75f;
            }
            Projectile.ai[1]++;

            if (Projectile.ai[1] > 3)
            {
                Projectile.Kill();
            }
            SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.position);

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Bone);
                d.velocity = Main.rand.NextVector2Circular(4, 4);
                d.noGravity = !Main.rand.NextBool(5);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
                d.velocity = Main.rand.NextVector2Circular(4, 4);
                d.noGravity = !Main.rand.NextBool(5);
            }
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
                Projectile.frame = Main.rand.Next(5);
                Projectile.ai[0]++;
            }
            Projectile.rotation += Projectile.velocity.X * 0.05f;
            Projectile.velocity.Y += 0.3f;
        }
    }
}
