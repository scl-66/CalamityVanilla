using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.Items.CursedDagger;

public class CursedDagger : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<CursedDaggerProjectile>(), 15, 15, true);
        Item.noUseGraphic = true;
        Item.damage = 30;
        Item.knockBack = 4;
        Item.rare = ItemRarityID.LightRed;
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.UseSound = SoundID.Item1;
        Item.value = 12;
    }
    public override void AddRecipes()
    {
        CreateRecipe(66).AddTile(TileID.Anvils).AddIngredient(ItemID.RottenChunk, 1).AddIngredient(ItemID.CursedFlame, 1).Register();
    }
}

public class CursedDaggerExplosion : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 7;
    }
    public override void SetDefaults()
    {
        //Projectile.arrow = true;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.timeLeft = 21;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        if (Projectile.timeLeft % 3 == 0)
        {
            Projectile.frame++;
        }
        Lighting.AddLight(Projectile.Center, new Vector3(0.9f, 1f, 0f) * Projectile.timeLeft / 14f); // R G B values from 0 to 1f.
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(1f, 1f, 1f, 1f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.CursedInferno, 4 * 60);
    }
}

public class CursedDaggerProjectile : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 16);
        //Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 4;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.frame == 0 ? DustID.CursedTorch : DustID.Stone, Main.rand.NextVector2Circular(3, 3));
            if (Projectile.frame == 0)
            {
                d.noGravity = true;
            }
            d.velocity *= 0.95f;
        }
    }
    public override void AI()
    {
        Projectile.ai[0]++;

        if (Projectile.ai[0] == 30)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CursedDaggerExplosion>(), (int)(Projectile.damage * 2.5f), Projectile.knockBack);
            }
            SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.5f, PitchVariance = 0.6f, Volume = 0.6f }, Projectile.position);
            Projectile.frame = 1;
            for (int i = 0; i < 40; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, speed);
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
                d.fadeIn = Main.rand.NextFloat(1.4f);
            }
        }
        if (Projectile.ai[0] > 30)
        {
            Projectile.ai[1] += 0.04f;
            Projectile.velocity.Y += 0.5f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f * Projectile.direction;
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            for (int i = 0; i < 1; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, 15, 15, DustID.CursedTorch, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                d.noGravity = true;
                d.velocity *= 0.95f;
                d.scale = 1.2f;
            }
        }

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.frame == 0)
        {
            target.AddBuff(BuffID.CursedInferno, 80);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.25f);
        if (Projectile.frame == 0)
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, 2, 0, Projectile.frame), color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
        }
        Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Projectile.Size / 2, texture.Frame(1, 2, 0, Projectile.frame), lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}