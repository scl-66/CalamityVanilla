using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Projectiles;

public class SporeBomb : ModProjectile
{
    private static SoundStyle _death;
    private static SoundStyle _spawn;
    public override void SetStaticDefaults()
    {
        _death = new SoundStyle(Mod.Name + "/Assets/Sounds/HiveMind_SporeBombPop", [1,2]) { PitchVariance = 0.1f };
        _spawn = new SoundStyle(Mod.Name + "/Assets/Sounds/HiveMind_SporeBombLaunch") { PitchVariance = 0.1f };
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 3;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 30);
        Projectile.timeLeft = 120;
        Projectile.Opacity = 0;
    }
    public override void AI()
    {
        Projectile.Opacity += 0.1f;
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 0.4f;
        }

        if (Projectile.ai[0] == 0)
        {
            SoundEngine.PlaySound(_spawn, Projectile.position);
        }
        Projectile.ai[0]++;
        Projectile.frameCounter++;
        if (Projectile.frameCounter == 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame > Main.projFrames[Type] - 1)
            {
                Projectile.frame = 0;
            }
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = tex.Frame(2, Main.projFrames[Type], 0, Projectile.frame);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float opacity = (1f - (i / (float)Projectile.oldPos.Length)) * Projectile.Opacity;
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, lightColor * opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        if (Projectile.timeLeft < 60)
        {
            Color glow = Color.Lerp(new Color(1f, 0.5f, 0.5f, 0f), new Color(0.5f, 0.25f, 1f, 0f), Main.masterColor) * Utils.Remap(Projectile.timeLeft, 30, 60, 1, 0);
            frame.X += frame.Width;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, glow * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin((float)Main.timeForVisualEffects * 0.03f) * 8, 0).RotatedBy(Projectile.rotation + i * MathHelper.PiOver2), frame, glow * Projectile.Opacity * 0.15f, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
        }
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(_death, Projectile.position);
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 9; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(1, 1), ModContent.ProjectileType<Spores>(), 23, 2, ai0: Main.rand.NextFloat(MathF.PI * 10));
            }
        }
        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 3;
        }
        int type = ModContent.DustType<VileMushroomDust>();
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.velocity *= 2.5f;
            d.fadeIn = Main.rand.NextFloat(0.2f, 0.5f);
            d.noGravity = Main.rand.NextBool();
        }
    }
}
public class SporeBombLarge : SporeBomb
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 3;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 50);
        Projectile.timeLeft = 120;
        Projectile.Opacity = 0;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 15; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(7, 7), ModContent.ProjectileType<Spores>(), 23, 2, ai0: Main.rand.NextFloat(MathF.PI * 10));
            }
        }
        for (int i = 0; i < 40; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 3;
        }
        int type = ModContent.DustType<VileMushroomDust>();
        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.velocity *= 2.5f;
            d.fadeIn = Main.rand.NextFloat(0.2f, 0.5f);
            d.noGravity = Main.rand.NextBool();
        }
    }
}