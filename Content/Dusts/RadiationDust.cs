using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Dusts;

public class UraniumRadDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.velocity *= 0.9f; // Multiply the dust's start velocity by 0.4, slowing it down
        dust.noGravity = true; // Makes the dust have no gravity.
        dust.noLight = false; // Makes the dust emit no light.
        dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // Multiplies the dust's initial scale by 1.5.
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return Color.White * dust.scale;
    }

    public override bool Update(Dust dust)
    { // Calls every frame the dust is active
        if (Main.rand.Next(0, 4) == 0)
        {
            dust.velocity = dust.velocity.RotatedByRandom(MathHelper.PiOver2);
        }
        dust.position += dust.velocity/1.05f;
        dust.rotation += dust.velocity.X * 0.15f;
        dust.scale *= 0.95f;

        float light = 0.35f * dust.scale;

        Lighting.AddLight(dust.position, 0.66f*light, 0.8f*light, 0f);

        if (dust.scale < 0.05f)
        {
            dust.active = false;
        }

        return false; // Return false to prevent vanilla behavior.
    }
}

public class PlutoniumRadDust : UraniumRadDust
{
    public override bool Update(Dust dust)
    { // Calls every frame the dust is active
        if (Main.rand.Next(0, 4) == 0)
        {
            dust.velocity = dust.velocity.RotatedByRandom(MathHelper.PiOver2);
        }
        dust.position += dust.velocity / 1.05f;
        dust.rotation += dust.velocity.X * 0.15f;
        dust.scale *= 0.95f;

        float light = 0.35f * dust.scale;

        Lighting.AddLight(dust.position, 0.85f * light, 0.66f * light, 0.33f * light);

        if (dust.scale < 0.05f)
        {
            dust.active = false;
        }

        return false; // Return false to prevent vanilla behavior.
    }
}