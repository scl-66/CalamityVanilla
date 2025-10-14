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

public class IceChunks : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 16);
        Projectile.timeLeft = 600;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 1 / 120f;
        if (Projectile.ai[0] > 1f)
            Projectile.ai[0] = 1f;

        Projectile.velocity.Y += 0.2f;
        Projectile.frame = Projectile.whoAmI % 3;
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        for (int i = 0; i < 4; i++)
        {
            Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 4).RotatedBy(i * MathHelper.PiOver2), tex.Frame(1, 3, 0, Projectile.frame), (Cryogen.GetAuroraColor(Projectile.timeLeft + Projectile.whoAmI * 5) * (1f - Projectile.ai[0])) with { A = 0 }, Projectile.rotation, new Vector2(16), Projectile.scale, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition, tex.Frame(1, 3, 0, Projectile.frame), lightColor, Projectile.rotation, new Vector2(16), Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}