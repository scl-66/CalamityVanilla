using CalamityVanilla.Common;
using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Bosses.Cryogen;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class IceBomb : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 32);
        Projectile.timeLeft = 60 * 3;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        if (Projectile.timeLeft == (60 * 3) - 1)
        {
            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
        }

        Projectile.velocity *= 0.99f;
        Projectile.rotation += 0.1f;
        Projectile.scale = 1f + MathF.Sin(Projectile.timeLeft * 0.1f) * 0.1f;

        if (Projectile.timeLeft < 60)
            Projectile.ai[2] += 1f / 60;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        int type = ModContent.DustType<SimpleColorableGlowyDust>();

        for (int i = 0; i < 20; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.velocity = Main.rand.NextVector2Circular(12, 12);
            d.noGravity = true;
            d.scale = 1.2f;
            //d.fadeIn = Main.rand.NextFloat(2);
            d.color = Cryogen.GetAuroraColor((int)Main.timeForVisualEffects + Main.rand.Next(30) + Projectile.whoAmI * 25) with { A = 0 };

            Dust d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
            d2.velocity = Main.rand.NextVector2Circular(6, 6);
            d2.scale = 1.5f;
            d2.noGravity = Main.rand.NextBool();
        }

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, Projectile.ai[1]).RotatedBy(MathHelper.PiOver2 * i).RotatedBy(Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver4), ModContent.ProjectileType<IceShrapnel>(), 25, 0);
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        Asset<Texture2D> glow = TextureAssets.Extra[ExtrasID.KeybrandRing];
        Color glowColor = Cryogen.GetAuroraColor((int)Main.timeForVisualEffects + Projectile.whoAmI * 25) with { A = 64 } * Projectile.ai[2];
        Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, glowColor, 0f, glow.Size() / 2, 1f - Projectile.ai[2], SpriteEffects.None);
        glowColor.A = 0;
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(tex.Height(), 0, tex.Height(), tex.Height()), new Color(1f, 1f, 1f, 0.5f), Projectile.rotation, new Vector2(tex.Height() / 2) - new Vector2(1), Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, tex.Height(), tex.Height()), new Color(1f, 1f, 1f, 0.5f), Projectile.ai[0] != 0 ? 0 : MathHelper.PiOver4, new Vector2(tex.Height() / 2) - new Vector2(1), Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(tex.Height(), 0, tex.Height(), tex.Height()), glowColor, Projectile.rotation, new Vector2(tex.Height() / 2) - new Vector2(1), Projectile.scale * 1.3f, SpriteEffects.None);
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, tex.Height(), tex.Height()), glowColor, Projectile.ai[0] != 0 ? 0 : MathHelper.PiOver4, new Vector2(tex.Height() / 2) - new Vector2(1), Projectile.scale * 1.3f, SpriteEffects.None);
        return false;
    }
}