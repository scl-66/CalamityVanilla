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

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class Icicles : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 16);
        Projectile.timeLeft = 600;
    }
    public override void AI()
    {
        Projectile.velocity.Y += 0.2f;
        Projectile.frame = Projectile.whoAmI % 3;
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        if (Projectile.velocity.Y > 8)
        {
            Projectile.velocity.Y = 8;
        }

        if (Main.rand.NextBool())
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.FrostStaff);
            d.noGravity = true;
            d.velocity += Projectile.velocity;
        }
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27);
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.FrostStaff);
            d.noGravity = Main.rand.NextBool();
            d.velocity = Main.rand.NextVector2Circular(3, 3);
        }
    }
}