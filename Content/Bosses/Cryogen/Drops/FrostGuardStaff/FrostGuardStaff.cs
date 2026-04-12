using CalamityVanilla.Common;
using CalamityVanilla.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen.Drops.FrostGuardStaff;

public class FrostGuardStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Type] = true;

        ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.Size = new Vector2(24, 24);
        Item.damage = 10;
        Item.DamageType = DamageClass.Summon;
        Item.mana = 10;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.knockBack = 2f;

        Item.shoot = ModContent.ProjectileType<FrostShieldCounter>();
        Item.buffType = ModContent.BuffType<FrostShieldBuff>();
        Item.shootSpeed = 10f;

        Item.UseSound = SoundID.Item66;
        Item.autoReuse = true;
        Item.reuseDelay = 2;

        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 70, 0);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
        player.LimitPointToPlayerReachableArea(ref position);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);

        return true;
    }
}

public class FrostShieldBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        var frostShieldCounterId = ModContent.ProjectileType<FrostShieldCounter>();
        var frostShieldId = ModContent.ProjectileType<FrostShield>();

        if (player.ownedProjectileCounts[frostShieldCounterId] > 0)
        {
            player.GetModPlayer<FrostShieldModPlayer>().HasFrostShieldBuff = true;
            player.buffTime[buffIndex] = 18000;
        }
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}

public class FrostShieldModPlayer : ModPlayer
{
    public bool HasFrostShieldBuff { get; set; }
    public int HighestFrostShieldCounterOriginalDamage { get; set; }

    public override void ResetEffects()
    {
        HasFrostShieldBuff = false;
    }

    public override void PostUpdateBuffs()
    {
        var frostShieldCounterId = ModContent.ProjectileType<FrostShieldCounter>();
        var frostShieldId = ModContent.ProjectileType<FrostShield>();

        if (Main.myPlayer == Player.whoAmI)
        {
            var shieldAmountTarget = 0;
            for (int i = 0; i < Player.ownedProjectileCounts[frostShieldCounterId]; i++)
            {
                if (i < 3)
                {
                    shieldAmountTarget += 2;
                }
                else
                {
                    shieldAmountTarget += 1;
                }
            }

            var shieldsTargetDifference = int.Abs(Player.ownedProjectileCounts[frostShieldId] - shieldAmountTarget);

            if (Player.ownedProjectileCounts[frostShieldId] > shieldAmountTarget)
            {
                var shieldsLeftToAdd = shieldsTargetDifference;
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.owner == Player.whoAmI && projectile.type == frostShieldId)
                    {
                        projectile.Kill();
                        shieldsLeftToAdd--;

                        if (shieldsLeftToAdd == 0)
                        {
                            break;
                        }
                    }
                }
            }
            else if (Player.ownedProjectileCounts[frostShieldId] < shieldAmountTarget)
            {
                for (int i = 0; i < shieldsTargetDifference; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, frostShieldId, 0, 0f, Player.whoAmI);
                }
            }
        }

        HighestFrostShieldCounterOriginalDamage = 0;
        foreach (var projectile in Main.ActiveProjectiles)
        {
            if (projectile.owner == Player.whoAmI && projectile.type == frostShieldCounterId)
            {
                int originalDamage = projectile.originalDamage;
                if (HighestFrostShieldCounterOriginalDamage < originalDamage)
                {
                    HighestFrostShieldCounterOriginalDamage = originalDamage;
                }
            }
        }
    }
}

public class FrostShieldCounter : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 6;

        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        ProjectileID.Sets.MinionSacrificable[Type] = true;

        Main.projPet[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(10, 10);

        Projectile.timeLeft = 60;
        Projectile.minion = true;
        Projectile.minionSlots = 1f;
        Projectile.netImportant = true;

        Projectile.DamageType = DamageClass.Summon;
        Projectile.friendly = true;
        Projectile.penetrate = -1;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;

        Projectile.hide = true;
    }

    public override void AI()
    {
        var player = Main.player[Projectile.owner];
        var modPlayer = player.GetModPlayer<FrostShieldModPlayer>();

        if (player.dead)
        {
            modPlayer.HasFrostShieldBuff = false;
        }
        if (modPlayer.HasFrostShieldBuff)
        {
            Projectile.timeLeft = 2;
        }

        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 6)
            {
                Projectile.frame = 0;
            }
        }

        Projectile.GetGroupIndex(out var index, out var totalIndexesInGroup);
        Projectile.Center = Projectile.AI_164_GetHomeLocation(player, index, totalIndexesInGroup);
    }

    public override bool? CanDamage() => false;
    public override bool? CanCutTiles() => false;
}

public class FrostShield : ModProjectile
{
    public ref float CooldownTimer => ref Projectile.ai[0];
    public ref float FinishedCooldownTimer => ref Projectile.localAI[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        ProjectileID.Sets.MinionCannotBeFreed[Type] = true;

        Main.projPet[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(38, 38);

        Projectile.timeLeft *= 5;
        Projectile.minion = true;
        Projectile.netImportant = true;

        Projectile.DamageType = DamageClass.Summon;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        var previousCooldownTimer = CooldownTimer;
        CooldownTimer--;
        if (CooldownTimer < 0)
            CooldownTimer = 0;

        FinishedCooldownTimer--;
        if (FinishedCooldownTimer < 0)
            FinishedCooldownTimer = 0;

        if (CooldownTimer == 0 && previousCooldownTimer != 0)
        {
            FinishedCooldownTimer = 30;
            SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
        }

        var player = Main.player[Projectile.owner];
        var modPlayer = player.GetModPlayer<FrostShieldModPlayer>();

        Projectile.GetGroupIndex(out var index, out var totalIndexesInGroup);

        if (player.dead)
        {
            modPlayer.HasFrostShieldBuff = false;
        }
        if (modPlayer.HasFrostShieldBuff)
        {
            Projectile.timeLeft = 2;
        }

        var (shieldCircleNumber, fractionInShieldCircle) = GetCurrentCircleNumber();

        var shieldRotationSpeed = 0.025f + (shieldCircleNumber - 1) * 0.005f;
        var shieldDistance = shieldCircleNumber * 50 + 25;

        Projectile.Center = player.Center + Vector2.UnitY * player.gfxOffY + (fractionInShieldCircle * MathF.Tau - MathF.PI * 0.5f + player.GetModPlayer<PlayerStats>().TimeInWorld * shieldRotationSpeed).ToRotationVector2() * shieldDistance;
        Projectile.rotation = (player.Center - Projectile.Center).ToRotation() - MathF.PI * 0.5f;

        Projectile.originalDamage = modPlayer.HighestFrostShieldCounterOriginalDamage;

        var hitboxes = GetHitboxes();
        if (CooldownTimer <= 0)
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                var isProjectileDeadly = projectile.hostile || (projectile.friendly && projectile.owner != -1 && Main.player[projectile.owner].InOpposingTeam(Main.player[Projectile.owner]));
                if (!(projectile.whoAmI != Projectile.whoAmI && isProjectileDeadly)) continue;

                if (CanProjectileBeReflected(projectile)) continue;

                var colliding = projectile.Colliding(projectile.Hitbox, hitboxes[0]);
                colliding |= projectile.Colliding(projectile.Hitbox, hitboxes[1]);
                colliding |= projectile.Colliding(projectile.Hitbox, hitboxes[2]);
                if (!colliding) continue;

                ReflectProjectile(projectile);
                Break();

                break;
            }
        }
    }

    public override bool MinionContactDamage() => CooldownTimer <= 0;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var hitboxes = GetHitboxes();

        var colliding = targetHitbox.Intersects(hitboxes[0]);
        colliding |= targetHitbox.Intersects(hitboxes[1]);
        colliding |= targetHitbox.Intersects(hitboxes[2]);

        return colliding;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (target.knockBackResist == 0f) return;

        var directionToReflectTo = (Projectile.rotation - MathF.PI * 0.5f).ToRotationVector2() * 5f * target.knockBackResist;
        target.velocity = directionToReflectTo;
        target.netUpdate = true;
    }

    private (int, float) GetCurrentCircleNumber()
    {
        Projectile.GetGroupIndex(out var index, out var totalIndexesInGroup);

        var shieldAmountsLeft = totalIndexesInGroup;
        var shieldCircleNumber = 0;
        var fractionInShieldCircle = 0f;
        var shieldAmountInCurrentCircle = 6;
        var accumulatedPreviousShieldAmount = 0;

        while (shieldAmountsLeft > 0)
        {
            fractionInShieldCircle = (float)(index - accumulatedPreviousShieldAmount) / int.Min(shieldAmountsLeft, shieldAmountInCurrentCircle);
            if (fractionInShieldCircle >= 0 && fractionInShieldCircle < 1)
            {
                break;
            }

            accumulatedPreviousShieldAmount += shieldAmountInCurrentCircle;
            shieldAmountsLeft -= shieldAmountInCurrentCircle;
            shieldCircleNumber++;
            shieldAmountInCurrentCircle += 2;
        }

        shieldCircleNumber++;

        return (shieldCircleNumber, fractionInShieldCircle);
    }

    private Rectangle[] GetHitboxes()
    {
        int centerHitboxSize = 38;
        int edgeHitboxSize = 32;

        var edgeOffset = Projectile.rotation.ToRotationVector2() * 24;

        Rectangle[] hitboxes =
        [
            new Rectangle((int)Projectile.Center.X - centerHitboxSize / 2, (int)Projectile.Center.Y - centerHitboxSize / 2, centerHitboxSize, centerHitboxSize),

            new Rectangle((int)(Projectile.Center.X + edgeOffset.X) - edgeHitboxSize / 2, (int)(Projectile.Center.Y + edgeOffset.Y) - edgeHitboxSize / 2, edgeHitboxSize, edgeHitboxSize),
            new Rectangle((int)(Projectile.Center.X - edgeOffset.X) - edgeHitboxSize / 2, (int)(Projectile.Center.Y - edgeOffset.Y) - edgeHitboxSize / 2, edgeHitboxSize, edgeHitboxSize),
        ];

        return hitboxes;
    }

    private static readonly HashSet<int> _projectileAiStylesThatCanBeReflected = 
    [
        ProjAIStyleID.Arrow,
        ProjAIStyleID.ThrownProjectile,
        ProjAIStyleID.Bounce,
        ProjAIStyleID.MusicNote,
        ProjAIStyleID.CrystalShard,
        ProjAIStyleID.ColdBolt,
        ProjAIStyleID.GemStaffBolt,
        ProjAIStyleID.DD2FlameBurstShot
    ];

    private static bool CanProjectileBeReflected(Projectile projectile)
    {
        if (!projectile.active) return false;

        if (CVItemSets.CanBeReflected[projectile.type] != null) return CVItemSets.CanBeReflected[projectile.type].Value;

        if (_projectileAiStylesThatCanBeReflected.Contains(projectile.aiStyle)) return true;

        return false;
    }

    private void ReflectProjectile(Projectile projectileToReflect)
    {
        SoundEngine.PlaySound(in SoundID.Item150, projectileToReflect.position);

        for (int i = 0; i < 3; i++)
        {
            int dust = Dust.NewDust(projectileToReflect.position, projectileToReflect.width, projectileToReflect.height, DustID.Smoke);
            Main.dust[dust].velocity *= 0.3f;
        }

        projectileToReflect.reflected = true;
        projectileToReflect.hostile = false;
        projectileToReflect.friendly = true;
        projectileToReflect.owner = Projectile.owner;

        projectileToReflect.damage = Projectile.damage;

        Vector2 directionToReflectTo = (Projectile.rotation - MathF.PI * 0.5f).ToRotationVector2() * projectileToReflect.oldVelocity.Length();
        projectileToReflect.velocity = directionToReflectTo;
    }

    private void Break()
    {
        CooldownTimer = 60 * 5;
        SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

        var hitboxes = GetHitboxes();

        for (int i = 0; i < hitboxes.Length; i++)
        {
            var hitbox = hitboxes[i];
            for (int j = 0; j < 12; j++)
            {
                var dust = Dust.NewDustDirect
                (
                    new Vector2(hitbox.X, hitbox.Y),
                    hitbox.Width, hitbox.Height,
                    DustID.Ice,
                    Scale: 1.25f
                );
            }
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        var baseColor = new Color(250, 250, 250, 150);

        if (CooldownTimer <= 0)
            return baseColor;

        return baseColor * Utils.Remap(CooldownTimer, 0, 60, 0.5f, 0.2f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = TextureAssets.Projectile[Type].Value;

        SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Main.spriteBatch.Draw
        (
            texture,
            Projectile.Center - Main.screenPosition,
            new Rectangle(0, 0, texture.Width, texture.Height / 2),
            GetAlpha(lightColor).Value,
            Projectile.rotation,
            new Vector2(texture.Width / 2, texture.Height / 4),
            Projectile.scale,
            spriteEffects,
            0
        );

        Main.spriteBatch.Draw
        (
            texture,
            Projectile.Center - Main.screenPosition,
            new Rectangle(0, texture.Height / 2, texture.Width, texture.Height / 2),
            Color.White * Utils.GetLerpValue(0, 30, FinishedCooldownTimer),
            Projectile.rotation,
            new Vector2(texture.Width / 2, texture.Height / 4),
            Projectile.scale,
            spriteEffects,
            0
        );

        return false;
    }
}