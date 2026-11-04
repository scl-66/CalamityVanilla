using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Dusts;

public class CryonicSmokeDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.rotation = Main.rand.NextFloat(0, 360);
        dust.scale /= 3;
        dust.customData = 0.01f;
        dust.color = Color.White;
    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X * 0.15f;
        dust.scale += (float)dust.customData;
        dust.alpha += (int)dust.fadeIn;
        dust.velocity *= 0.98f;
        if (dust.alpha > 255)
            dust.active = false;
        if (dust.noGravity)
            dust.velocity *= 0.95f;
        return false;
    }
    public override bool PreDraw(Dust dust)
    {
        Color c = dust.color * ((255 - dust.alpha) / 255f) * Math.Min(dust.scale * 2, 1);
        if(!dust.noLight)
            c = c.MultiplyRGBA(new Color(Lighting.GetSubLight(dust.position)));
        c.A = (byte)(c.A * 0.75f);
        Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, c, dust.rotation, new Vector2(4), dust.scale, SpriteEffects.None,0);
        c *= 0.25f;
        Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, c, -dust.rotation, new Vector2(4), dust.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
