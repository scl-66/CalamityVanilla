using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen.Projectiles;

public class CryoFlake1 : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 24, -1);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60 * 4;
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.Lerp(lightColor, new Color(1f, 1f, 1f, 0f), 0.5f) * Projectile.Opacity;
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void AI()
    {
        if (Projectile.ai[1] == 0)
        {
            Projectile.ai[1]++;
            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
        }

        Player target = Main.player[(int)Projectile.ai[0]];
        Projectile.rotation += Projectile.velocity.X * 0.04f;

        if (Main.rand.NextBool())
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2), DustID.Snow, Projectile.velocity.RotatedByRandom(0.3f));
            d.noGravity = true;
        }
        if (!target.dead && Projectile.ai[1] < 30)
        {
            Projectile.velocity = Utils.rotateTowards(Projectile.Center, Projectile.velocity * 1.01f, target.Center, 0.1f);
        }
        Projectile.velocity = Projectile.velocity.LengthClamp(8);

        //if (Projectile.ai[1] < 1f)
        //{
        //    Projectile.ai[1] += 1 / 150f * Projectile.ai[2] * (Projectile.ai[1] > 0.5f ? 2f : 1f);
        //    Projectile.velocity *= 1.01f;

        //    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2), ModContent.DustType<SimpleColorableGlowyDust>(), Projectile.velocity.RotatedByRandom(0.1f));
        //    d.color = Cryogen.GetAuroraColor(Projectile.timeLeft + Projectile.whoAmI) with { A = 0 } * (1f - Projectile.ai[1]);
        //    d.noGravity = true;
        //}
        //if (Projectile.ai[1] > 1f)
        //    Projectile.ai[1] = 1f;
        //Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utils.AngleLerp(Projectile.velocity.ToRotation(), Projectile.Center.DirectionTo(target.Center).ToRotation(), MathF.Sin(Projectile.ai[1] * MathHelper.Pi) * 0.075f));

        if (Projectile.timeLeft < 30)
        {
            Projectile.Opacity = Projectile.timeLeft / 30f;
        }
    }
}
public class CryoFlake2 : CryoFlake1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 30;
    }
}
public class CryoFlake3 : CryoFlake1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 50;
    }
}