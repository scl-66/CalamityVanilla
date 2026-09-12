using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class PerfectDarkMagicDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(0, Main.rand.Next(2) * 10, 10, 10);
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        dust.alpha = 0;
    }

    public override bool MidUpdate(Dust dust)
    {
        dust.velocity *= 0.94f;
        dust.scale -= dust.fadeIn;

        float lightStrength = Math.Min(1f, dust.scale);
        Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), lightStrength * 0.6f, lightStrength * 0.2f, lightStrength);

        return true;
    }
}