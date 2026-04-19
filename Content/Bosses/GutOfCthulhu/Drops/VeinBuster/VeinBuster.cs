using CalamityVanilla.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityVanilla.Content.Bosses.GutOfCthulhu.Drops.VeinBuster;

public class VeinBusterPlayer : ModPlayer
{
    public bool SpawnedShards = false;
}

public class VeinBuster : ModItem, ISyncedOnHitEffect
{
    public override void SetDefaults()
    {
        Item.Size = new Vector2(24, 28);
        Item.damage = 61;
        Item.knockBack = 6f;
        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 10, 0);
    }

    public override bool? UseItem(Player player)
    {
        if (player.ItemAnimationJustStarted)
            player.GetModPlayer<VeinBusterPlayer>().SpawnedShards = false;
        return base.UseItem(player);
    }

    public void SyncedOnHitNPC(Player player, NPC target, int damage, float knockback, bool crit, int hitDirection)
    {
        VeinBusterPlayer modPlayer = player.GetModPlayer<VeinBusterPlayer>();

        if (!modPlayer.SpawnedShards)// && (target == null || target.HittableForOnHitRewards()))
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int shardAmount = Main.rand.Next(3, 4 + 1);

                for (int i = 0; i < shardAmount; i++)
                {
                    var direction = (hitDirection == 1 ? Vector2.UnitX : -Vector2.UnitX).SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3);
                    var projectile = Projectile.NewProjectileDirect
                    (
                        player.GetSource_ItemUse(Item),
                        target.Center + direction * 16,
                        direction * Main.rand.NextFloat(8f, 16f),
                        ModContent.ProjectileType<VeinBusterShard>(),
                        damage / 3, knockback / 2,
                        player.whoAmI
                    );
                    projectile.localNPCImmunity[target.whoAmI] = projectile.localNPCHitCooldown;
                }
            }

            int visualBurstAmount = 4;

            for (int i = 0; i < visualBurstAmount; i++)
            {
                var basePosition = (hitDirection == 1 ? target.Right : target.Left) + Main.rand.NextVector2Circular(8, 8);
                var baseVelocity = (hitDirection == 1 ? Vector2.UnitX : -Vector2.UnitX).RotatedByRandom(1) * Main.rand.NextFloat(8f, 16f); ;/*player.itemRotation.ToRotationVector2()
                    .RotatedBy(player.direction == 1 ? Math.PI : 0)
                    .RotatedBy(Math.PI * 0.75 * (player.direction == 1 ? -1 : 1))
                    /*.RotatedByRandom(1)
                    * Main.rand.NextFloat(8f, 16f);*/

                for (int j = 0; j < 10; j++)
                {
                    var progress = j / (10 - 1f);
                    var scale = float.Lerp(2f, 1.25f, progress);

                    var dust = Dust.NewDustPerfect(
                        basePosition + Main.rand.NextVector2Circular(4, 4),
                        ModContent.DustType<VeinBusterBloodDust>(),
                        baseVelocity.RotatedByRandom(0.2) * progress,
                        Scale: scale
                    );
                    dust.noGravity = true;
                }
            }

            SoundEngine.PlaySound(SoundID.Item171, player.position);
            SoundEngine.PlaySound(SoundID.NPCHit19 with
            {
                Volume = 0.5f
            }, player.position);

            modPlayer.SpawnedShards = true;
        }

	}
}

public class VeinBusterShard : ModProjectile
{
    public ref float Timer => ref Projectile.localAI[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;

        Projectile.penetrate = 8;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Melee;

        Projectile.extraUpdates = 2;
    }

    public override void AI()
    {
        Timer++;
        if (Timer > 60 * Projectile.extraUpdates)
        {
            Projectile.Kill();
        }

        Projectile.Opacity = Utils.GetLerpValue(0, 5 * Projectile.extraUpdates, Timer, true);

        Projectile.rotation = Projectile.velocity.ToRotation();

        for (int i = 0; i < 3; i++)
        {
            var progress = 1f - i / 2f;

            var dust = Dust.NewDustPerfect(
                Projectile.Center - Projectile.rotation.ToRotationVector2() * 42 - Projectile.velocity * Main.rand.NextFloat(),
				ModContent.DustType<VeinBusterBloodDust>(),
                Velocity: Projectile.velocity,
                Scale: 1.25f * Projectile.Opacity
            );
            dust.noGravity = true;
        }
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SourceDamage *= Projectile.penetrate / (float)Projectile.maxPenetrate;
    }
    public override void OnKill(int timeLeft)
    {
        int shardDustType = ModContent.DustType<VeinBusterShardDust>();
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustDirect
            (
                Projectile.position, Projectile.width, Projectile.height,
                shardDustType
            );
        }
        int bloodDustType = ModContent.DustType<VeinBusterBloodDust>();
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustDirect
            (
                Projectile.position, Projectile.width, Projectile.height,
                bloodDustType,
                Scale: 1.25f
            );
        }

        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = TextureAssets.Projectile[Type].Value;

        for (int i = Projectile.oldPos.Length - 1; i >= 1; i--)
        {
            var progress = 1f - i / (Projectile.oldPos.Length - 1f);
            var alpha = float.Lerp(0f, 0.25f, progress);
            Main.EntitySpriteDraw
            (
                texture,
                Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null,
                lightColor.MultiplyRGB(Color.Red) * alpha * Projectile.Opacity,
                Projectile.oldRot[i],
                new Vector2(texture.Width, texture.Height / 2f),
                1f,
                SpriteEffects.None
            );
        }

        Main.EntitySpriteDraw
        (
            texture,
            Projectile.position + Projectile.Size * 0.5f - Main.screenPosition, null,
            lightColor * Projectile.Opacity,
            Projectile.rotation,
            new Vector2(texture.Width, texture.Height / 2f),
            1f,
            SpriteEffects.None
        );

        return false;
    }
}

public class VeinBusterShardDust : ModDust;

public class VeinBusterBloodDust : ModDust
{
	public override bool Update(Dust dust)
	{
		UpdateType = DustID.Blood;
		return base.Update(dust);
	}
}