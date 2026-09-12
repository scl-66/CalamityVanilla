using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Interfaces;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.HiveMind.Drops.PerfectDark;

public class PerfectDark : ModItem
{
    public override void SetDefaults()
    {
        Item.Size = new Vector2(28, 34);
        Item.damage = 59;
        Item.knockBack = 6f;
        Item.useTime = 21;
        Item.useAnimation = 21;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 10, 0);
        Item.shoot = ModContent.ProjectileType<PerfectDarkCloud>();
        Item.shootSpeed = 5;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int cloudAmount = Main.rand.Next(2, 5 + 1);
        for (int i = 0; i < cloudAmount; i++)
        {
            Projectile.NewProjectile
            (
                source,
                position + velocity.SafeNormalize(Vector2.UnitX) * 16f,
                velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.66f, 1.25f),
                type,
                (int)(damage * 0.5f),
                (int)(knockback * 0.33f),
                player.whoAmI,
                ai0: Main.rand.Next(90, 150)
            );
        }
        return false;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        for (int i = 0; i < 4; i++)
        {
            float pointProgress = Main.rand.NextFloat(0.6f);
            float pointProgressOffset = Main.rand.NextFloat(-0.2f, 0.2f);
            float savedItemRotation = player.itemRotation;
            player.itemRotation += Main.rand.NextFloat(-0.2f, 0.2f);
            CVUtils.GetPointOnSwungItemPath(60f, 74f, 0.4f + pointProgress + pointProgressOffset, player.GetAdjustedItemScale(Item), out var location, out var outwardDirection, player);
            Vector2 velocity = outwardDirection.RotatedBy(MathHelper.PiOver2 * player.direction * player.gravDir);
            player.itemRotation = savedItemRotation;

            float dustInterpolator = Utils.GetLerpValue(0.2f, 0.6f, pointProgress, true);
            float dustScale = MathHelper.Lerp(1f, 2f, dustInterpolator);//MathHelper.Lerp(1.5f, 2.5f, dustInterpolator);
            dustScale *= Main.rand.NextFloat(0.75f, 1.25f);
            Color dustColor = Color.Lerp(Color.White, Color.Black, dustInterpolator);
            int dustAlpha = 0;// (int)MathHelper.Lerp(100, 0, dustInterpolator);
            int dustType = ModContent.DustType<PerfectDarkMagicDust>();

            Dust dust = Dust.NewDustPerfect(
                location,
                dustType,
                velocity * 4f,
                Alpha: dustAlpha,
                newColor: dustColor,
                Scale: dustScale
            );
            dust.noGravity = true;
            dust.fadeIn = Main.rand.NextFloat(0.01f, 0.1f);
        }
    }
}

public class PerfectDarkCloud : ModProjectile, ISyncedOnHitEffect
{
    public float TotalTime => Projectile.ai[0];
    public ref float Timer => ref Projectile.localAI[0];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;

        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 40;
        Projectile.friendly = true;
        Projectile.hostile = false;

        //Projectile.tileCollide = false;
        Projectile.ignoreWater = true;

        Projectile.frameCounter = Main.rand.Next(5 + 1);
        Projectile.frame = Main.rand.Next(3);
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    }

    public override void AI()
    {
        Timer++;

        Projectile.Opacity = Utils.GetLerpValue(0, 30, Timer, true) * Utils.GetLerpValue(TotalTime, TotalTime - 30, Timer, true);
        if (Timer > TotalTime)
        {
            Projectile.Kill();
            return;
        }

        Projectile.frameCounter++;
        if (Projectile.frameCounter > 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame > 2)
                Projectile.frame = 0;
        }

        Projectile.velocity *= 0.98f;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
    }

    public void SyncedOnHitNPC(Player player, NPC target, int damage, float knockback, bool crit, int hitDirection)
    {
        if (Timer < TotalTime - 30)
        {
            Timer += 15;
            Timer = float.Min(Timer, TotalTime - 30);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = oldVelocity;
        Timer += 5;

        return false;
    }
}