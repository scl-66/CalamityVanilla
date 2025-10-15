using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TheGothic;
using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.Twiflight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Jungle.Items.BlossomFlux;

public class BlossomFlux : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToBow(7, 14, true);
        Item.useAnimation *= 5;
        Item.consumeAmmoOnFirstShotOnly = true;
        Item.damage = 28;
        Item.knockBack = 2;
        Item.UseSound = null;

        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.MythrilAnvil)
            .AddIngredient(ModContent.ItemType<TheGothic>())
            .AddIngredient(ModContent.ItemType<Twiflight>())
            .AddIngredient(ItemID.ChlorophyteBar, 10)
            .AddIngredient(ItemID.Ectoplasm, 8)
            .Register();
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-10, 0);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item5, player.position);

        if (player.ItemAnimationJustStarted)
        {
            SoundEngine.PlaySound(SoundID.Item102, player.position);
            Projectile.NewProjectile(source, position, velocity * 0.7f + new Vector2(0, -1), ModContent.ProjectileType<BlossomFluxBomb>(), damage * 2, knockback * 2, player.whoAmI);
        }

        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedByRandom(0.1f);
        if (type == ProjectileID.WoodenArrowFriendly)
        {
            type = ModContent.ProjectileType<BlossomFluxLeaf>();
        }
    }
}

public class BlossomFluxLeaf : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.extraUpdates = 1;
        Projectile.arrow = true;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 20)
        {
            Projectile.velocity.Y += 0.15f;
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 1f, 0) * 0.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];

        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = (1 - (i / 10f));
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(0.8f * multiply, 0.8f, 0.6f * multiply, 0.3f) * multiply, Projectile.rotation, tex.Size() / 2, 0.2f + multiply, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, tex.Size() / 2, 1.2f, SpriteEffects.None);
        return false;
    }
}

public class BlossomFluxBomb : ModProjectile
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
        Projectile.extraUpdates = 1;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 20; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(0, 2f);
        }

        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
        if (Main.myPlayer == Projectile.owner)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4, 4), ModContent.ProjectileType<BlossomFluxSpores>(), Projectile.damage / 3, 2, Projectile.owner);
            }
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

        Projectile.velocity.Y += 0.1f;
        Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 1f, 0) * 0.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];

        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = (1 - (i / 5f));
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, new Rectangle(0, tex.Height() / 4 * Projectile.frame, tex.Width(), tex.Height() / 4), new Color(1f, 1f, 1f, 0f) * multiply, Projectile.rotation, tex.Size() / new Vector2(2, 8), 1f + (multiply * 0.2f), SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, tex.Height() / 4 * Projectile.frame, tex.Width(), tex.Height() / 4), new Color(1f, 1f, 1f, 0.5f), Projectile.rotation, tex.Size() / new Vector2(2, 8), 1f, SpriteEffects.None);
        return false;
    }
}

public class BlossomFluxSpores : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 36);
        Projectile.timeLeft = 60 * 3;
        Projectile.alpha = 128;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 10)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
            if (Projectile.frame == 5)
            {
                Projectile.frame = 0;
            }
        }

        Projectile.ai[0]++;
        Projectile.velocity *= 0.99f;
        Projectile.scale = 1f + (float)Math.Sin(Projectile.ai[0] * 0.1f) * 0.1f;

        if (Projectile.timeLeft < 20)
        {
            Projectile.alpha += 6;
        }
    }
}