using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class EyeballBloodDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        //dust.velocity *= 0.4f; // Multiply the dust's start velocity by 0.4, slowing it down
        dust.noGravity = false; // Makes the dust have no gravity.
        dust.noLight = true; // Makes the dust emit no light.
        dust.scale = Main.rand.NextFloat(0.5f, 1.5f); // Multiplies the dust's initial scale by 1.5.
    }

    public override bool Update(Dust dust)
    { // Calls every frame the dust is active
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X * 0.15f;
        dust.velocity.Y += 0.2f;
        dust.scale *= 0.95f;

        //float light = 0.35f * dust.scale;

        //Lighting.AddLight(dust.position, light, light, light);

        if (dust.scale < 0.05f)
        {
            dust.active = false;
        }

        return false; // Return false to prevent vanilla behavior.
    }
}