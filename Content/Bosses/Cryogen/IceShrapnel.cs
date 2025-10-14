using CalamityVanilla.Content.Bosses.Cryogen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class IceShrapnel : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 16);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60 * 9;
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(1f, 1f, 1f, 0.5f);
    }
    public override void AI()
    {
        //Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
        //d.velocity = Projectile.velocity;
        //d.noGravity = true;
        //d.scale = 1.5f;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        for (int i = 0; i < 6; i++)
        {
            Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition - (Projectile.velocity * i * 0.5f), null, (Cryogen.GetAuroraColor((int)(Projectile.whoAmI * 25 + (Projectile.timeLeft * 5)) + (i * 15)) * (1f - (i / 6f))) with { A = 0 }, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}