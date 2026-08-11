using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.Crystaline;

public class Crystaline : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<CrystalineProjectile>(), 23, 12, true);
        Item.noUseGraphic = true;
        Item.damage = 16;
        Item.knockBack = 2;
        Item.rare = ItemRarityID.Blue;
        Item.consumable = false;
        Item.maxStack = 1;
        Item.UseSound = SoundID.Item1;
        Item.value = 800;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils).AddIngredient(ItemID.ThrowingKnife, 150).AddIngredient(ItemID.FallenStar, 5).AddIngredient(ItemID.Diamond, 7).Register();
    }
}

public class CrystalineProjectile : ModProjectile
{
    public override string Texture => ModContent.GetModItem(ModContent.ItemType<Crystaline>()).Texture;
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 16);
        Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.ai[1] == 0 ? DustID.GemDiamond : DustID.Glass, Main.rand.NextVector2Circular(3, 3));
            d.noGravity = !Main.rand.NextBool(3);
            d.velocity -= Projectile.velocity * 0.2f;
        }
        if (Projectile.ai[1] < 1f)
        {
            int length = ProjectileID.Sets.TrailCacheLength[Type];
            int t = ModContent.DustType<SimpleColorableGlowyDust>();
            for (int i = 1; i < length; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, t);
                d.color = new Color(0.2f, 0.6f, 0.8f, 0f) * (1f - Projectile.ai[1]) * Projectile.Opacity * (1f - i / (float)length);
                d.noGravity = d.noLight = true;
                d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * 3;
                d.velocity *= 0.4f;
            }
        }
    }
    public override void AI()
    {

        Projectile.ai[0]++;

        if (Projectile.ai[0] == 30)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(i * MathHelper.TwoPi / 3 + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(1.3f, 1.6f), ModContent.ProjectileType<CrystalineShard>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack / 3, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.Item110 with { Pitch = 0.5f, PitchVariance = 0.6f }, Projectile.position);

            PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
            sparkle.LocalPosition = Projectile.Center;
            sparkle.Scale = new Vector2(Main.rand.NextFloat(2.7f, 3.3f), Main.rand.NextFloat(0.9f, 1.1f));
            sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f);
            sparkle.DrawVerticalAxis = true;
            sparkle.ColorTint = new Color(0.3f, 0.9f, 1f, 0f);
            sparkle.FadeInEnd = 5;
            sparkle.FadeOutStart = 5;
            sparkle.FadeOutEnd = 40;
            Main.ParticleSystem_World_OverPlayers.Add(sparkle);

            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, new Vector2(0, Main.rand.NextFloat(-5, 5)).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
                Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, new Vector2(Main.rand.NextFloat(-7, 7), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
                d2.scale = Main.rand.NextFloat(0.8f, 1.2f);
                d2.noGravity = true;

                Dust d3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UltraBrightTorch);
                d3.color = Color.Cyan;
                d3.velocity *= 2;
                d3.noGravity = true;
                d3.fadeIn = Main.rand.NextFloat(1.3f);
            }
        }
        if (Projectile.ai[0] > 30)
        {
            Projectile.ai[1] += 0.04f;
            Projectile.velocity.Y += 0.3f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f * Projectile.direction;
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond);
                d.velocity = Projectile.velocity.RotatedByRandom(0.2f);
                d.noGravity = true;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.ai[1] < 1f)
        {
            int length = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 1; i < length; i++)
            {
                Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(0.05f, 0.15f, 0.2f, 0f) * (1f - Projectile.ai[1]) * Projectile.Opacity * (1f - i / (float)length), Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]).ToRotation() + MathHelper.PiOver2, TextureAssets.Extra[ExtrasID.ThePerfectGlow].Size() / 2, new Vector2(1f - Projectile.ai[1], 1) * Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            }
        }
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        if (Projectile.ai[1] < 1f)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * Projectile.Opacity * (1f - Projectile.ai[1]) * 2, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, new Color(0.5f, 0.5f, 0.5f, 0.3f) * Projectile.Opacity * (1f - Projectile.ai[1]), Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2, Projectile.scale * 1.5f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        }
        return false;
    }
}

public class CrystalineShard : ModProjectile
{
    public override string Texture => ModContent.GetModItem(ModContent.ItemType<Crystaline>()).Texture;
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 16);
        Projectile.timeLeft = 20;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.Opacity = Projectile.timeLeft / 20f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        int length = ProjectileID.Sets.TrailCacheLength[Type];
        for (int i = 1; i < length; i++)
        {
            Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(0.2f, 0.6f, 0.8f, 0f) * Projectile.Opacity * (1f - i / (float)length), Projectile.oldRot[i], TextureAssets.Extra[ExtrasID.ThePerfectGlow].Size() / 2, new Vector2(0.5f, 1f) * Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        }
        Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value, Projectile.Center - Main.screenPosition, null, new Color(0.8f, 1f, 1f, 0f) * Projectile.Opacity * 2f, Projectile.rotation, TextureAssets.Extra[ExtrasID.ThePerfectGlow].Size() / 2, 0.75f * Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        //for (int i = 0; i < 2; i++)
        //{
        //    Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value, Projectile.Center - Main.screenPosition, null, new Color(0.8f, 1f, 1f, 0f) * Projectile.Opacity * 0.1f, (i * MathHelper.PiOver2), TextureAssets.Extra[ExtrasID.ThePerfectGlow].Size() / 2, new Vector2(0.4f,3f) * Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        //}
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        if (timeLeft > 0)
        {

        }
    }
}