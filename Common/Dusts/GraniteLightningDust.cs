using Microsoft.Xna.Framework;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class GraniteLightningDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(0, Main.rand.Next(2) * 10, 10, 10);
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        dust.alpha = 0;
        dust.noLight = true;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale *= 0.75f;
        dust.velocity *= 0.90f;
        Lighting.AddLight(dust.position, Color.Cyan.ToVector3() * 1.2f * ((10 - dust.alpha) / 10f));
        dust.alpha++;
        if (dust.alpha > 10)
        {
            dust.active = false;
        }
        return false;
    }
}