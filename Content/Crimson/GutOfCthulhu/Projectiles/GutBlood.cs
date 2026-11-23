using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.GutOfCthulhu.Projectiles;

public class GutBlood : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 18;
        ProjectileID.Sets.TrailingMode[Type] = 1;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 12);
        Projectile.extraUpdates = 1;
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath9 with { MaxInstances = 10}, Projectile.position);
        }
        Projectile.ai[0]++;
        Projectile.velocity.Y += 0.3f;
        if (Projectile.velocity.Y > 6)
            Projectile.velocity.Y = 6;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
        {
            if (Main.rand.NextBool(5))
            {
                Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, DustID.Blood);
                d.noGravity = true;
                d.alpha = 128;
                d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i + 1]);
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath9 with { MaxInstances = 10}, Projectile.position);
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            d.noGravity = true;
            d.alpha = 64;
            d.velocity *= 2;
            d.scale *= 2;
            d.fadeIn = -1;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var tex = TextureAssets.Projectile[Type].Value;
        ulong seed4 = Main.TileFrameSeed;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            float percent = 1f - (i / (float)Projectile.oldPos.Length);
            Vector2 rand = new Vector2(Utils.RandomInt(ref seed4, -6, 7), Utils.RandomInt(ref seed4, -6, 7));
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 2) + (rand * percent), null, Color.Lerp(lightColor,Color.Black,percent / 2) * 0.5f * percent, Projectile.oldRot[i], tex.Size() / 2, 1f, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex,Projectile.Center - Main.screenPosition,null,lightColor,Projectile.rotation,tex.Size() / 2, 1, SpriteEffects.None);
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * 0.25f, Projectile.rotation, tex.Size() / 2, 2f, SpriteEffects.None);
        return false;
    }
}
