using CalamityVanilla.Content.Particles;
using CalamityVanilla.Content.Space.Items.Cloudfall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.Twiflight;

public class Twiflight : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToBow(24, 12, true);
        Item.consumeAmmoOnFirstShotOnly = true;
        Item.damage = 20;
        Item.knockBack = 2;
        Item.UseSound = null;

        Item.UseSound = SoundID.Item102;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 4);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.DemonAltar)
            .AddIngredient(ItemID.BeesKnees)
            .AddIngredient(ItemID.HellwingBow)
            .AddIngredient(ItemID.BloodRainBow)
            .AddIngredient(ModContent.ItemType<Cloudfall>())
            .Register();
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-10, 0);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = position + Vector2.Normalize(velocity) * 10;
        sparkle.Scale = new Vector2(6f, 1.3f);
        sparkle.Rotation = velocity.ToRotation();
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = new Color(1f, 0.5f, 0f, 0f);
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 20;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustPerfect(sparkle.LocalPosition, DustID.GemTopaz, new Vector2(0, Main.rand.NextFloat(-5, 5)).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            Dust d2 = Dust.NewDustPerfect(sparkle.LocalPosition, DustID.GemTopaz, new Vector2(Main.rand.NextFloat(-7, 7), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d2.scale = Main.rand.NextFloat(0.8f, 1.2f);
            d2.noGravity = true;
        }
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(sparkle.LocalPosition, 1, 1, DustID.RainbowMk2);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.noGravity = true;
            d.color = Color.Lerp(new Color(0.6f, 1f, 1f), Color.MediumVioletRed, Main.rand.NextFloat()) with { A = 0 };
            d.fadeIn = Main.rand.NextFloat(0, 1.5f);
        }
        for (int i = -1; i < 2; i += 2)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(i * 0.2f) * 0.6f, ModContent.ProjectileType<TwiflightFeather>(), damage, knockback, player.whoAmI);
        }
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (type == ProjectileID.WoodenArrowFriendly)
            type = ModContent.ProjectileType<TwiflightFeather>();
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }
}

public class TwiflightFeather : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 4;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 320;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.noGravity = true;
            d.color = Color.Lerp(new Color(0.6f, 1f, 1f), Color.MediumVioletRed, Main.rand.NextFloat()) with { A = 0 };
            d.fadeIn = Main.rand.NextFloat(0, 1.5f);
        }

        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(6f, 1.3f);
        sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = new Color(1f, 0.5f, 0f, 0f);
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 20;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, new Vector2(0, Main.rand.NextFloat(-5, 5)).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, new Vector2(Main.rand.NextFloat(-7, 7), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d2.scale = Main.rand.NextFloat(0.8f, 1.2f);
            d2.noGravity = true;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (oldVelocity.Y != Projectile.velocity.Y)
            Projectile.velocity.Y = -Projectile.oldVelocity.Y;
        if (oldVelocity.X != Projectile.velocity.X)
            Projectile.velocity.X = -Projectile.oldVelocity.X;

        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

        Projectile.penetrate--;
        if (Projectile.penetrate == 0)
        {
            Projectile.Kill();
        }
        else
        {
            PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
            sparkle.LocalPosition = Projectile.Center;
            sparkle.Scale = new Vector2(3f, 1f);
            sparkle.Rotation = Projectile.rotation;
            sparkle.ColorTint = new Color(1f, 0.5f, 0f, 0f);
            sparkle.FadeInEnd = 5;
            sparkle.FadeOutStart = 5;
            sparkle.FadeOutEnd = 20;
            sparkle.DrawVerticalAxis = false;
            Main.ParticleSystem_World_OverPlayers.Add(sparkle);
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, new Vector2(Main.rand.NextFloat(-5, 5), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            }
        }
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(3f, 1f);
        sparkle.Rotation = Projectile.rotation + MathHelper.PiOver2;
        sparkle.ColorTint = new Color(1f, 0.5f, 0f, 0f);
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 20;
        sparkle.DrawVerticalAxis = false;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, new Vector2(Main.rand.NextFloat(-5, 5), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
        }
    }
    public override void AI()
    {
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 6)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
            if (Projectile.frame == 4)
            {
                Projectile.frame = 0;
            }
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Main.rand.NextBool())
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
            d.velocity *= 0.3f;
            d.noGravity = true;
            d.color = Color.Lerp(new Color(0.6f, 1f, 1f), Color.MediumVioletRed, Main.rand.NextFloat()) with { A = 0 };
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];

        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = 1 - i / 5f;
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, new Rectangle(0, tex.Height() / 4 * Projectile.frame, tex.Width(), tex.Height() / 4), new Color(0.6f * multiply, 0f, 1f, 0f) * multiply * 0.5f, Projectile.rotation, tex.Size() / new Vector2(2, 8), 1f + multiply * 0.2f, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, tex.Height() / 4 * Projectile.frame, tex.Width(), tex.Height() / 4), new Color(1f, 1f, 1f, 1f), Projectile.rotation, tex.Size() / new Vector2(2, 8), 1f, SpriteEffects.None);
        return false;
    }
}