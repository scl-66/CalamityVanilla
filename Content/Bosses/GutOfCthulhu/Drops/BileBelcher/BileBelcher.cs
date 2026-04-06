using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.GutOfCthulhu.Drops.BileBelcher;

public class BileBelcher : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<IchorBlob>(), 16, 12, 6);
        Item.UseSound = SoundID.NPCHit1;
        Item.damage = 50;
    }
    public override float UseSpeedMultiplier(Player player)
    {
        return 0.5f + player.statMana / (float)player.statManaMax2;
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.9f, 1.1f);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.rand.NextBool(4))
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13, player.position);
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<BileBone>(), damage / 3 * 5, knockback, player.whoAmI);
        }
        return true;
    }
}

public class BileBone : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.DamageType = DamageClass.Magic;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
        }
        if (Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X = -oldVelocity.X * 0.75f;
        }
        Projectile.ai[1]++;

        if (Projectile.ai[1] > 3)
        {
            Projectile.Kill();
        }
        SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.position);

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Bone);
            d.velocity = Main.rand.NextVector2Circular(4, 4);
            d.noGravity = !Main.rand.NextBool(5);
        }
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            d.velocity = Main.rand.NextVector2Circular(4, 4);
            d.noGravity = !Main.rand.NextBool(5);
        }
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
            Projectile.frame = Main.rand.Next(5);
            Projectile.ai[0]++;
        }
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Projectile.velocity.Y += 0.3f;
    }
}

public class IchorBlob : ModProjectile
{
    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(1f, 1f, 1f, 0.85f) * Projectile.Opacity;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 22);
        Projectile.DamageType = DamageClass.Magic;
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.velocity.Y += 0.2f;
        if (Main.rand.NextBool(4))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor);
            d.velocity += Projectile.velocity;
            d.noGravity = true;
        }
        Lighting.AddLight(Projectile.position, 0.2f, 0.15f, 0);

        Projectile.ai[0]++;
        Projectile.scale = 0.9f + MathF.Sin(Projectile.ai[0] * 0.2f) * 0.1f;
        Projectile.Opacity = 0.9f + MathF.Sin(Projectile.ai[0] * 0.2f) * 0.1f;
        Projectile.Opacity *= MathHelper.Clamp(Projectile.ai[0] * 0.1f, 0, 1);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(5))
            target.AddBuff(BuffID.Ichor, 60 * 6);
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.noGravity = true;
        }
    }
}