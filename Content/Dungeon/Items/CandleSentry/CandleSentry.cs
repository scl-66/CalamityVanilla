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

namespace CalamityVanilla.Content.Dungeon.Items.CandleSentry;

public class CandleSentryItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Type] = true;
    }
    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.DamageType = DamageClass.Summon;
        Item.sentry = true;
        Item.mana = 10;
        Item.width = 26;
        Item.height = 40;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.noMelee = true;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 2);
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<CandleSentry>();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        position = Main.MouseWorld;
        player.LimitPointToPlayerReachableArea(ref position);
        int halfProjectileHeight = (int)Math.Ceiling(ContentSamples.ProjectilesByType[type].height / 2f);

        position.Y -= halfProjectileHeight; // Adjust in-air option to spawn with bottom at cursor.

        // Spawn the sentry projectile at the calculated location.
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, Main.myPlayer);

        // Kills older sentry projectiles according to player.maxTurrets
        player.UpdateMaxTurrets();

        return false;
    }
}

public class CandleSentry : ModProjectile
{
    public ref float ShootTimer => ref Projectile.ai[0];

    public bool JustSpawned
    {
        get => Projectile.localAI[0] == 0;
        set => Projectile.localAI[0] = value ? 0 : 1;
    }

    public Vector2 StartPos;
    public static int TargetingRange = 20 * 16;
    public ref float SinTimer => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 26;
        Projectile.height = 60;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.sentry = true; // Sets the weapon as a sentry for sentry accessories to properly work.
        Projectile.timeLeft = Projectile.SentryLifeTime; // Sentries last 10 minutes
        Projectile.ignoreWater = true;
        Projectile.netImportant = true; // Sentries need this so they are synced to newly joining players
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false; // Prevent tile collision from killing the projectile
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

        TargetingRange = 40 * 16;

        SinTimer++;

        int ShootFrequency = 60 * 2; // How long the sentry waits between shots.
        float FireVelocity = 2.5f; // The velocity the sentry's shot projectile will travel.

        // Code to run when spawned
        if (JustSpawned)
        {
            JustSpawned = false;
            ShootTimer = ShootFrequency * 1.5f; // Delay the first shot slightly
            StartPos = Projectile.position;

            // The sound that Frost Hydra, Spider Turret, and Houndius Shootius play when spawned. Optional.
            SoundEngine.PlaySound(SoundID.Item60, Projectile.position);

            // Dust indicating the sentry spawned. Optional.
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(0.85f, 1.15f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.WaterCandle, speed * 4, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        // Move up and down smoothly
        Projectile.position.Y = (float)(StartPos.Y + Math.Sin(SinTimer / 20) * 4);

        // Spawn torch dust
        if (Main.rand.NextBool(10))
        {
            Dust d = Dust.NewDustDirect(Projectile.Top + new Vector2(-4, 0), 8, 8, DustID.DungeonWater);
            d.noGravity = Main.rand.NextBool();
            d.velocity.Y = (float)Main.rand.Next(-10, 6) * 0.2f;
            d.velocity.X *= 0.3f;
            d.scale = 0.8f;
        }

        // Find an enemy to target.
        float closestTargetDistance = TargetingRange;
        NPC targetNPC = null;
        // Prioritize the owner's minion attack target. (Right click or whip feature)
        if (Projectile.OwnerMinionAttackTargetNPC != null)
        {
            TryTargeting(Projectile.OwnerMinionAttackTargetNPC, ref closestTargetDistance, ref targetNPC);
        }

        // If no minion attack target or if it was out of range, find the closest enemy to target.
        if (targetNPC == null)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                TryTargeting(npc, ref closestTargetDistance, ref targetNPC);
            }
        }

        //Main.NewText(ShootTimer);

        if (targetNPC != null)
        {
            if (ShootTimer < 36 && ShootTimer >= 0)
            {
                // Actually spawning the projectile only runs if the local player is the owner
                if (Main.myPlayer == Projectile.owner)
                {
                    if (ShootTimer % 12 == 0)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            Vector2 rand = Main.rand.NextVector2Circular(4, 4);
                            Dust d = Dust.NewDustDirect(Projectile.position + new Vector2(7, -1), 3, 3, DustID.DungeonWater, rand.X, rand.Y);
                            d.noGravity = true;
                            d.scale = Main.rand.NextFloat(0.8f, 1.1f);
                        }

                        // Play a shoot sound
                        SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.8f, PitchRange = (0.6f, 1f), MaxInstances = 1 }, Projectile.Center);

                        // The direction the projectile will fire.
                        Vector2 shootDirection = (targetNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.PiOver2);
                        // The final velocity vector
                        Vector2 shootVelocity = shootDirection * FireVelocity * Main.rand.NextFloat(0.96f, 1.45f);

                        // The type of projectile the sentry will shoot. It is important that sentry shots are included in ProjectileID.Sets.SentryShot, so reusing unrelated vanilla projectiles as-is won't work 100%.
                        int type = ModContent.ProjectileType<CandleSentryFlame>();

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X - 4f, Projectile.Center.Y - 12), shootVelocity, type, Projectile.damage, 3, Projectile.owner);
                        // Note that Projectile.damage will take into account current equipment damage bonuses automatically for sentries and minions, so there is no need to calculate that here to take advantage of current equipment bonuses.
                        // See Projectile.ContinuouslyUpdateDamageStats docs for more information.
                    }
                }
            } else if (ShootTimer <= 0)
            {
                ShootTimer = ShootFrequency;
            }
        }

        // Count down the shoot timer
        ShootTimer--;

        Lighting.AddLight(Projectile.Center + new Vector2(0, -10), new Vector3(0f, 0.35f, 0.8f) * 1.5f);
    }
    private void TryTargeting(NPC npc, ref float closestTargetDistance, ref NPC targetNPC)
    {
        if (npc.CanBeChasedBy(this))
        {
            float distanceToTargetNPC = Vector2.Distance(Projectile.Center, npc.Center);
            // Is this enemy closer than others? Is it in line of sight?
            if (distanceToTargetNPC < closestTargetDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
            {
                closestTargetDistance = distanceToTargetNPC; // Set a new closest distance value
                targetNPC = npc;
            }
        }
    }

    public Vector2 glowOffset;
    public Vector2 glowOffset2;
    public int glowTimer = 0;

    public override void PostDraw(Color lightColor)
    {
        Texture2D tex = ModContent.Request<Texture2D>("CalamityVanilla/Content/Dungeon/Items/CandleSentry/CandleSentry_Glow", AssetRequestMode.ImmediateLoad).Value;
        Rectangle frame = tex.Frame(1, 5, 0, Projectile.frame);

        Main.EntitySpriteDraw(tex, Projectile.position - Main.screenPosition + tex.Size() / 2, frame, Color.White, 0, tex.Size() / 2, 1, SpriteEffects.None);

        glowTimer++;
        if (glowTimer >= 4)
        {
            glowTimer = 0;
            glowOffset = new Vector2(Main.rand.NextFloat(-3, 3) / 2, Main.rand.NextFloat(-3, 3) / 2);
            glowOffset2 = new Vector2(Main.rand.NextFloat(-3, 3) / 2, Main.rand.NextFloat(-3, 3) / 2);
        }

        Main.EntitySpriteDraw(tex, glowOffset + Projectile.position - Main.screenPosition + tex.Size() / 2, frame, Color.White with { A = 0 }, 0, tex.Size() / 2, 1, SpriteEffects.None);
        Main.EntitySpriteDraw(tex, glowOffset2 + Projectile.position - Main.screenPosition + tex.Size() / 2, frame, Color.White with { A = 0 }, 0, tex.Size() / 2, 1, SpriteEffects.None);
    }
}

public class CandleSentryFlame : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.SentryShot[Type] = true;
        ProjectileID.Sets.TrailCacheLength[Type] = 3;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.timeLeft = 60 * 4;
        DrawOffsetX = 3;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (Main.rand.NextBool(3))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater);
            d.velocity *= 0.2f;
            d.noGravity = true;
            d.scale = 0.8f;
        }

        Projectile.ai[0]++;

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

        int target;
        if (Projectile.ai[0] < 60)
        {
            target = -1;
        } else
        {
            target = Projectile.FindTargetWithLineOfSight(CandleSentry.TargetingRange);
        }

        if (target != -1)
        {
            Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 0.5f;
            Projectile.velocity = Projectile.velocity.LengthClamp(5f);
        } else
        {
            Projectile.velocity *= 0.96f;
        }
        if (Projectile.timeLeft < 20)
        {
            Projectile.Opacity -= 0.025f;
            Projectile.velocity *= 0.93f;
        }

        Lighting.AddLight(Projectile.Center, new Vector3(0f, 0.35f, 0.8f) * 0.75f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var tex = TextureAssets.Projectile[Type];
        Rectangle frame = tex.Frame(1, 4, 0, Projectile.frame);

        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = 1 - i / (float)ProjectileID.Sets.TrailCacheLength[Type];
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, color, Projectile.oldRot[i], frame.Size() / 2, 1f + multiply * 0.2f, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() / 2, 1f, SpriteEffects.None);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 rand = Main.rand.NextVector2Circular(2, 2);
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, rand.X, rand.Y);
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.1f);
        }
    }
}