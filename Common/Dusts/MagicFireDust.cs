using Microsoft.Xna.Framework;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class MagicFireDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(0, Main.rand.Next(2) * 10, 10, 10);
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        dust.alpha = 0;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        Color gray = new Color(25, 25, 25);
        Color newColor = Color.Lerp(Color.Lerp(Color.White, Color.Orange, dust.alpha / 15f), gray, dust.alpha / 25f);
        float alpha = 1 - dust.alpha / 15f;
        alpha = MathF.Sqrt(MathF.Sin(alpha * MathHelper.Pi));
        return newColor * alpha * 1.2f;
    }

    public override bool Update(Dust dust)
    {
        Lighting.AddLight(dust.position, Color.Red.ToVector3() * 1.2f * ((30 - dust.alpha) / 30f));

        dust.velocity *= 0.98f;
        if (!dust.noGravity)
        {
            dust.velocity.Y += 0.1f;
        }
        dust.position += dust.velocity;

        dust.scale *= 0.97f;

        dust.alpha++;
        if (dust.alpha > 30)
        {
            dust.active = false;
        }

        return false;
    }
}