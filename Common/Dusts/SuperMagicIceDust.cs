using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class SuperMagicIceDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(0, Main.rand.Next(2) * 10, 10, 10);
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        dust.alpha = 0;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        Color blue = new Color(0, 0, 150, 0);
        Color newColor = Color.Lerp(Color.Azure, blue, Utils.GetLerpValue(0, 15, dust.alpha, true));
        float alpha = 1 - dust.alpha / 15f;
        alpha = MathF.Sqrt(MathF.Sin(alpha * MathHelper.Pi));
        return newColor * alpha * 1.2f;
    }

    public override bool Update(Dust dust)
    {
        Lighting.AddLight(dust.position, Color.RoyalBlue.ToVector3() * ((15 - dust.alpha) / 15f));

        dust.velocity *= 0.95f;
        if (!dust.noGravity)
        {
            dust.velocity.Y += 0.1f;
        }
        dust.position += dust.velocity;

        dust.scale *= 0.97f;

        dust.alpha++;
        if (dust.alpha > 15)
        {
            dust.active = false;
        }

        return false;
    }
}