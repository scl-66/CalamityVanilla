using CalamityVanilla.Content.Dusts;
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
public class CryoFlake1 : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 24, -1);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 500;
    }
    public override void AI()
    {
        if (Projectile.ai[1] == 0)
        {
            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
        }

        Player target = Main.player[(int)Projectile.ai[0]];
        Projectile.rotation += Projectile.velocity.X * 0.04f;

        if (Projectile.ai[1] < 1f)
        {
            Projectile.ai[1] += 1 / 150f * Projectile.ai[2] * (Projectile.ai[1] > 0.5f? 2f : 1f);
            Projectile.velocity *= 1.01f;
        }
        if (Projectile.ai[1] > 1f)
            Projectile.ai[1] = 1f;
        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utils.AngleLerp(Projectile.velocity.ToRotation(), Projectile.Center.DirectionTo(target.Center).ToRotation(), MathF.Sin(Projectile.ai[1] * MathHelper.Pi) * 0.1f));
        Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2), ModContent.DustType<SimpleColorableGlowyDust>(), Projectile.velocity.RotatedByRandom(0.1f));
        d.color = Cryogen.GetAuroraColor(Projectile.timeLeft + Projectile.whoAmI) with { A = 0 };
        d.noGravity = true;
    }
}
public class CryoFlake2 : CryoFlake1
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 30, -1);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
    }
}
public class CryoFlake3 : CryoFlake1
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 50, -1);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
    }
}
