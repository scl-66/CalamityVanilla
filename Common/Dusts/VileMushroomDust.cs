using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class VileMushroomDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.frame = new Rectangle(Main.rand.Next(2) * 10, Main.rand.Next(2) * 10, 10, 10);
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    }
    public override bool Update(Dust dust)
    {
        if (!dust.noGravity)
        {
            if (Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10) && dust.fadeIn == 0f)
            {
                dust.scale *= 0.9f;
                dust.velocity *= 0.25f;
            }
        }
        return base.Update(dust);
    }
}