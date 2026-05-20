using CalamityVanilla.Content.Particles;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        if (Main.rand.NextBool(5))
        {
            var dustType = Main.rand.NextBool() ? DustID.Shadowflame : DustID.ShadowbeamStaff;
            Vector2 vel = Main.rand.NextVector2Circular(Projectile.width / 7, Projectile.height / 7);
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, vel.X, vel.Y);
            d.scale = Main.rand.NextFloat(0.8f, 1.5f);
            d.noGravity = true;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        bounceAmount++;
        if (bounceAmount >= 6)
        {
            Projectile.Kill();
        }

        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
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
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, vel, 0, new Color(60, 0, 150), 0.7f);
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
        Vector2 oldSize = Projectile.Size;
        // Resize the projectile hitbox to be bigger.
        Projectile.position = Projectile.Center;
        Projectile.Size += new Vector2(explosionArea);
        Projectile.Center = Projectile.position;

        Projectile.tileCollide = false;
        Projectile.velocity *= 0.01f;
        // Damage enemies inside the hitbox area
        Projectile.Damage();
        Projectile.scale = 0.01f;

        //Resize the hitbox to its original size
        Projectile.position = Projectile.Center;
        Projectile.Size = new Vector2(10);
        Projectile.Center = Projectile.position;

        //SoundEngine.PlaySound(SoundID.Item4, Projectile.position);
        SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(2f, 1f);
        sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = Main.rand.NextBool() ? new Color(106, 0, 255) : new Color(153, 0, 255);
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 8;
        sparkle.FadeOutEnd = 20;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);

        for (int i = 0; i < 30; i++)
        {
            var dustType = Main.rand.NextBool() ? DustID.Shadowflame : DustID.FireworksRGB;
            Vector2 vector = Projectile.rotation.ToRotationVector2();
            Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, vector.RotatedBy((float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.02f).RotatedByRandom(MathHelper.TwoPi) * 4f * Main.rand.NextFloat(), 0, dustType == DustID.FireworksRGB ? new Color(60, 0, 150) : Color.White, 0.7f);
            dust.noGravity = true;
            dust.noLight = (dust.noLightEmittence = false);
            if (dustType == DustID.Shadowflame)
            {
                dust.scale = Main.rand.NextFloat(0.9f, 2.05f);
            } else
            {
                dust.scale = Main.rand.NextFloat(0.75f, 1.25f);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, 5, 0, Projectile.frame);
        Texture2D tex2 = TextureAssets.Extra[ExtrasID.SharpTears].Value;
        Rectangle frame2 = tex2.Frame();
        if (Main._multiplyBlendState == null)
        {
            Main._multiplyBlendState = new BlendState
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            };
        }
        Main.spriteBatch.End(out var ss);
        Main.spriteBatch.Begin(ss with
        {
            BlendState = Main._multiplyBlendState
        });

        Vector2 origin = frame.Size() / 2 + new Vector2(0, 2);
        float squash = MathF.Sin(Projectile.ai[0] / 10) / 13f + 0.5f;

        Vector2 trailOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = (Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition);
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(tex, drawPos, frame, (color * 0.8f).MultiplyRGBA(Color.Lerp(Color.Cyan, Color.Green, squash)), Projectile.rotation, trailOrigin, Projectile.scale, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

        Main.spriteBatch.End(out var ss2);
        Main.spriteBatch.Begin(ss2 with
        {
            BlendState = BlendState.Additive
        });

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.Indigo * 0.85f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(ss);
        return false;
    }
}
