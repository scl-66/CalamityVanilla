using CalamityVanilla.Content.Particles;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityVanilla.Content.Corruption.Items.DarkPrismStaff;

public class DarkPrismStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<DarkPrism>(), 6f, 30, 8);

        // Set damage and knockBack
        Item.SetWeaponValues(20, 4);

        // Set rarity and value
        Item.SetShopValues(ItemRarityColor.Blue1, 2000);
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils)
            .AddIngredient(ItemID.DemoniteBar, 8)
            .AddIngredient(ItemID.ShadowScale, 6)
            .Register();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
}

public class DarkPrism : ModProjectile
{
    private static SoundStyle _impact = new SoundStyle(CalamityVanilla.AssetPath + "Sounds/DarkPrismImpact", 3) { pitchVariance = 0.4f, pitch = -0.2f };
    private static SoundStyle _death = new SoundStyle(CalamityVanilla.AssetPath + "Sounds/DarkPrismBreak", 2) { pitchVariance = 0.4f, volume = 0.6f, pitch = -0.3f };
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
    }

    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 22);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = (int)(60 * 3.5f);
        Projectile.penetrate = 2;
        Projectile.tileCollide = true;
        Projectile.alpha = 30;
        Projectile.hide = true;
    }

    public float startSpeed;
    public int bounceAmount;

    public override void AI()
    {
        // if prism is slow enough, make pierce infinite
        if (Projectile.velocity.Length() < 0.2f)
        {
            Projectile.penetrate = -1;
        }

        //sprite animation 
        if (++Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        Projectile.ai[0]++;

        if (Projectile.ai[0] == 0)
        {
            startSpeed = Projectile.velocity.Length();
        }

        if (Projectile.ai[0] > 30)
        {
            Projectile.velocity *= 0.973f;
        }
        Projectile.rotation += 0.1f * Projectile.direction * Utils.Remap(Projectile.velocity.Length(), 0.1f, startSpeed, 0.75f, 5f);

        //if (Main.rand.NextBool(5))
        //{
        //    var dustType = Main.rand.NextBool() ? DustID.Shadowflame : DustID.ShadowbeamStaff;
        //    Vector2 vel = Main.rand.NextVector2Circular(Projectile.width / 7, Projectile.height / 7);
        //    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, vel.X, vel.Y);
        //    d.scale = Main.rand.NextFloat(0.8f, 1.5f);
        //    d.noGravity = true;
        //}
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB);
            d.color = new Color(80, 0, 150) * Utils.Remap(Projectile.velocity.Length(), 0, 3, 0, 1);
            d.velocity = Projectile.velocity;
            d.noGravity = true;
            d.scale /= 2;
            d.noLight = d.noLightEmittence = true;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        bounceAmount++;
        if (bounceAmount >= 6)
        {
            Projectile.Kill();
        }

        SoundEngine.PlaySound(_impact, Projectile.position);
        // If the projectile hits the left or right side of the tile, reverse the X velocity
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2 vector = Projectile.rotation.ToRotationVector2();
            Vector2 vel = oldVelocity.RotatedBy((float)Math.PI * Main.rand.NextFloatDirection() * 0.05f).RotatedByRandom(MathHelper.PiOver2).RotatedBy(MathHelper.Pi) * 0.45f * Main.rand.NextFloat();
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, vel, 0, new Color(80, 0, 150), 0.7f);
            dust.noGravity = true;
            dust.noLight = (dust.noLightEmittence = true);
            dust.scale = Main.rand.NextFloat(0.7f, 1f);
        }

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        // If the projectile dies without hitting an enemy, crate a small explosion that hits all enemies in the area.
        // Makes the projectile hit all enemies as it circunvents the penetrate limit.
        Projectile.maxPenetrate = -1;
        Projectile.penetrate = -1;

        int explosionArea = 30;
        Projectile.Resize(explosionArea, explosionArea);

        Projectile.tileCollide = false;
        // Damage enemies inside the hitbox area
        Projectile.Damage();

        SoundEngine.PlaySound(_death, Projectile.position);

        //for (int i = 0; i < 30; i++)
        //{
        //    var dustType = Main.rand.NextBool() ? DustID.Shadowflame : DustID.FireworksRGB;
        //    Vector2 vector = Projectile.rotation.ToRotationVector2();
        //    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, vector.RotatedBy((float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.02f).RotatedByRandom(MathHelper.TwoPi) * 4f * Main.rand.NextFloat(), 0, dustType == DustID.FireworksRGB ? new Color(60, 0, 150) : Color.White, 0.7f);
        //    dust.noGravity = true;
        //    dust.noLight = (dust.noLightEmittence = false);
        //    if (dustType == DustID.Shadowflame)
        //    {
        //        dust.scale = Main.rand.NextFloat(0.9f, 2.05f);
        //    } else
        //    {
        //        dust.scale = Main.rand.NextFloat(0.75f, 1.25f);
        //    }
        //}
        for (int i = 0; i < 5; i++)
        {
            var p = VanillaParticles.RequestFadingParticle();
            p.SetBasicInfo(TextureAssets.Extra[ExtrasID.ThePerfectGlow], null, new Vector2(0, Main.rand.NextFloat(1.5f, 3f)).RotatedBy((i * MathHelper.TwoPi / 5f) + Main.rand.NextFloat(-0.2f * MathHelper.Pi, 0.2f * MathHelper.Pi)), Projectile.Center);
            p.SetTypeInfo(30);
            p.AccelerationPerFrame = -p.Velocity / 30f;
            p.Scale = new Vector2(0.25f, 0.5f) * Main.rand.NextFloat(1, 2);
            p.ScaleVelocity = -p.Scale / new Vector2(30, 60);
            p.Rotation = p.Velocity.ToRotation() + MathHelper.PiOver2;
            p.ColorTint = Color.Lerp(Color.Black, Color.Purple, Main.rand.NextFloat(0.5f));
            p.FadeInNormalizedTime = 0.15f;
            p.FadeOutNormalizedTime = 0.5f;
            Main.ParticleSystem_World_BehindPlayers.Add(p);
            for (int y = 0; y < 6; y++)
            {
                Dust d = Dust.NewDustPerfect(p.LocalPosition, DustID.Stone, p.Velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(3));
                d.noGravity = true;
                d.color = Color.Black;

            }
        }
        for (int i = 0; i < 5; i++)
        {
            Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Square(-2, 2));
            d2.noGravity = true;
            d2.color = new Color(80, 0, 150);
            d2.noLight = d2.noLightEmittence = true;
        }
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.End(out var ss);
        Main.spriteBatch.Begin(ss with
        {
            BlendState = new()
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            }
        });

        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, 5, 0, Projectile.frame);
        Vector2 origin = frame.Size() / 2 + new Vector2(0, 2);

        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            float percent = 1f - (k / (float)Projectile.oldPos.Length);
            Vector2 drawPos = (Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition);
            Main.EntitySpriteDraw(tex, drawPos, frame, Color.Lerp(Color.White, Color.Cyan, percent) * percent * Utils.Remap(Projectile.velocity.Length(), 0, 3, 0, 1), Projectile.oldRot[k], origin, Projectile.scale - (1f - percent), SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(ss);
        Color c = Color.Purple with { A = 0 } * Utils.Remap((float)Math.Sin(Main.timeForVisualEffects * 0.1f), -1, 1, 0, 0.75f);
        if (Projectile.timeLeft < 20)
        {
            float percent = Projectile.timeLeft / 20f;
            c = Color.Lerp(Color.Black, c, percent);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, c * (1f - percent) * 0.5f, Projectile.rotation, origin, Projectile.scale * (1f + (1f - percent) * 0.5f), SpriteEffects.None);
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, c, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        return false;
    }
}