using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.SeafoamBomb;

public class SeafoamBomb : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<SeafoamBombProjectile>(), 15, 10, true);
        Item.noUseGraphic = true;
        Item.damage = 20;
        Item.knockBack = 1;
        Item.rare = ItemRarityID.White;
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.UseSound = SoundID.Item1;
        Item.value = 15;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils).AddIngredient(ItemID.Grenade).AddIngredient(ItemID.Coral).Register();
    }
}

public class SeafoamBombProjectile : ModProjectile
{
    public override string Texture => ModContent.GetModItem(ModContent.ItemType<SeafoamBomb>()).Texture;
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.timeLeft = 4 * 60;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.5f, PitchVariance = 0.6f }, Projectile.position);
        for (int i = 0; i < 60; i++)
        {
            int speed = 5;
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width * 2, Projectile.height * 2, DustID.Water, Main.rand.Next(-speed, speed), Main.rand.Next(-speed * 2, 1), 0, new Color(1f, 1f, 0f, 0f));
            d.scale = Main.rand.NextFloat(0.8f, 1.5f);
        }

        for (int i = 0; i < Main.rand.Next(3, 6); i++)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5, 2), ModContent.ProjectileType<Seafoam2>(), Projectile.damage / 15, Projectile.knockBack, ai1: Main.rand.NextFloat(0, 3), ai2: Main.rand.NextFloat(0, 2));
        }
    }
    public override void AI()
    {

        Projectile.velocity.Y += 0.3f;
        Projectile.velocity.X *= 0.99f;

        Projectile.rotation += (float)Projectile.velocity.X / 16f;
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -10).RotatedBy(Projectile.rotation), DustID.Water, new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-5, -2)), 0, new Color(1f, 1f, 0f, 0f));
            d.noGravity = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition + new Vector2(0, 4), null, lightColor, Projectile.rotation, new Vector2(9, 17), Projectile.scale, SpriteEffects.None);
        return false;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = Projectile.oldVelocity.Y * -0.6f;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = Projectile.oldVelocity.X * -0.6f;
        return false;
    }
}

public class Seafoam : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.NoLiquidDistortion[Type] = true;
    }

    public ref float Size => ref Projectile.ai[1];
    public ref float FrameNum => ref Projectile.ai[2];

    public int rand = Main.rand.Next(80, 120);

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
    }
    public override void AI()
    {
        Projectile.velocity.Y += 0.3f;
        Projectile.velocity.X *= 0.95f;
        if (Projectile.wet)
        {
            Projectile.velocity.Y = MathF.Sin(Projectile.ai[0] * 0.1f) * 0.1f;
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] % 8 == 0)
        {
            FrameNum++;
        }

        if (FrameNum > 2)
        {
            FrameNum = 0;
        }

        if (Projectile.ai[0] % rand == 0)
        {
            Size++;
        }

        if (Size > 3)
        {
            Size = 3;
        }

        if (Projectile.ai[0] > rand * 4f)
        {
            Projectile.timeLeft = 0;
        }
        if (Projectile.ai[0] > rand * 3.7f)
        {
            Projectile.Opacity *= 0.9f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(12, 10);
        Vector2 drawPos = Projectile.Bottom - Main.screenPosition;
        float sinVal = ((float)Math.Sin(Projectile.ai[0] / 15f) / 5f + 0.75f);
        Color color = Projectile.GetAlpha(lightColor) * sinVal * Projectile.Opacity;
        Main.EntitySpriteDraw(texture, drawPos, texture.Frame(3, 4, (int)FrameNum, (int)Size), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -Projectile.oldVelocity.X;
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.ai[0] = (float)Math.Round(Projectile.ai[0] / rand * 4f, 0, MidpointRounding.AwayFromZero) * rand;
    }
}

public class Seafoam2 : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.NoLiquidDistortion[Type] = true;
    }
    public ref float Size => ref Projectile.ai[1];
    public ref float FrameNum => ref Projectile.ai[2];

    public int rand = Main.rand.Next(80, 120);
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
    }
    public override void AI()
    {
        Projectile.velocity.Y += -0.1f;
        if (Projectile.velocity.Y < -2f * rand / 60f)
            Projectile.velocity.Y = -2f * rand / 60f;
        Projectile.velocity.X *= 0.98f;
        if (!Projectile.wet)
        {
            Projectile.Kill();
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, Projectile.velocity * Vector2.UnitX, ModContent.ProjectileType<Seafoam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], Projectile.ai[1], Projectile.ai[2]);
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] % 8 == 0)
        {
            FrameNum++;
        }

        if (FrameNum > 2)
        {
            FrameNum = 0;
        }

        if (Projectile.ai[0] % rand == 0)
        {
            Size++;
        }

        if (Size > 3)
        {
            Size = 3;
        }

        if (Projectile.ai[0] > rand * 4f)
        {
            Projectile.timeLeft = 0;
        }
        if (Projectile.ai[0] > rand * 3.7f)
        {
            Projectile.Opacity *= 0.9f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(texture.Width / 6, texture.Height / 8);
        Vector2 drawPos = (Projectile.Center - Main.screenPosition);
        float sinVal = ((float)Math.Sin(Projectile.ai[0] / 15f) / 5f + 0.75f);
        Color color = Projectile.GetAlpha(lightColor) * sinVal * Projectile.Opacity;
        Main.EntitySpriteDraw(texture, drawPos, texture.Frame(3, 4, (int)FrameNum, (int)Size), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -Projectile.oldVelocity.X;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -Projectile.oldVelocity.Y;
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.ai[0] = (float)Math.Round(Projectile.ai[0] / rand * 4f, 0, MidpointRounding.AwayFromZero) * rand;
    }
}