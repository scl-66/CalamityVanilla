using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using System.Transactions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TerraLongbow;

public class TerraLongbow : ModItem
{
    public const int HoldoutDistance = 16;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        Item.DefaultToBow(70, 6f, true);
        Item.noUseGraphic = true;
        Item.damage = 65;
        Item.knockBack = 2;
        Item.rare = ItemRarityID.Yellow;
        Item.shoot = ModContent.ProjectileType<TerraLongbowHeld>();
        Item.value = Item.sellPrice(0, 20, 0, 0);
        Item.channel = true;
        Item.UseSound = null;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int bow = ModContent.ProjectileType<TerraLongbowHeld>();
        // The velocity value provided is not correct, so we need to calculate a new velocity since velocity for held projectiles is actually the holdout offset.
        Vector2 bowVelocity = Vector2.Normalize(velocity) * HoldoutDistance;
        Projectile.NewProjectile(source, position, bowVelocity, bow, damage, knockback, Main.myPlayer);

        return false;
    }

    public override bool CanConsumeAmmo(Item ammo, Player player)
    {
        if (Main.rand.NextBool(4))
        {
            return false;
        }
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.MythrilAnvil)
            .AddIngredient(ModContent.ItemType<TheGothic.TheGothic>())
            .AddIngredient(ModContent.ItemType<Twiflight.Twiflight>())
            .AddIngredient(ItemID.ShroomiteBar, 12)
            .Register();
    }
}
public class TerraLongbowHeld : ModProjectile
{
    public ref float HoldTimer => ref Projectile.ai[0];
    public ref float ShootTimer => ref Projectile.ai[1];

    public int ShootCount = 0;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
    }
    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.ignoreWater = true;

        // Adjust the drawing to change how it appears when held
        DrawOffsetX = -15;
        DrawOriginOffsetY = -25;
    }
    public override bool? CanDamage() => false;

    public override void AI()
    {
        #region chargeup_version
        /*
        Player player = Main.player[Projectile.owner];
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        Item heldItem = player.HeldItem;

        // HoldTimer counts how long the weapon has been used. It helps control how fast the weapon animates and shoots arrows.
        HoldTimer += 1f;
        int initialShootDelay = heldItem.useTime;
        int usetime = initialShootDelay;
        bool shouldShootArrow = false;
        int maxShootCount = 3;

        ShootTimer += 1f;
        if (ShootTimer >= initialShootDelay - 18)
        {
            if (ShootTimer % 6 == 0)
            {
                shouldShootArrow = true;
            }
            if (ShootTimer >= initialShootDelay)
            {
                ShootTimer = 0f;
            }
        }

        int ShootIndex = ShootCount % 3 + 1;

        //Main.chatMonitor.Clear();
        //Main.NewText("ShootTimer: " + ShootTimer);
        //Main.NewText("HoldTimer: " + HoldTimer);
        //Main.NewText("ShootCount: " + ShootCount);
        //Main.NewText("shouldShootArrow: " + shouldShootArrow);
        //Main.NewText("ShootIndex: " + ShootIndex);

        float holdoutDistance = TerraLongbow.HoldoutDistance * Projectile.scale;
        Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
        #region shoot arrows
        if (shouldShootArrow && Main.myPlayer == Projectile.owner)
        {
            if ((player.channel || ShootCount < maxShootCount) && player.HasAmmo(heldItem) && !player.noItems && !player.CCed)
            {
                PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
                sparkle.LocalPosition = Projectile.Center;
                sparkle.Velocity = player.velocity;
                sparkle.Scale = new Vector2(3f, 0.6f);
                sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
                sparkle.DrawVerticalAxis = true;
                sparkle.ColorTint = Main.rand.NextBool() ? new Color(177, 255, 75) : new Color(255, 211, 47);
                sparkle.FadeInEnd = 5;
                sparkle.FadeOutStart = 5;
                sparkle.FadeOutEnd = 20;
                Main.ParticleSystem_World_OverPlayers.Add(sparkle);


                if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                {
                    Projectile.netUpdate = true;
                }

                // Set the projectile velocity, which is actually the holdout offset for held projectiles.
                Projectile.velocity = holdoutOffset;
                var spawnLocation = playerCenter + holdoutOffset;
                bool ammoConsumed = player.PickAmmo(heldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId);
                if (ammoConsumed)
                {
                    var source = player.GetSource_ItemUse_WithPotentialAmmo(heldItem, usedAmmoItemId);
                    Vector2 velocity = Vector2.Normalize(Projectile.velocity) * 25f;
                    double rot = Projectile.velocity.SafeNormalize(Vector2.UnitX).ToRotation();
                    for (int i = -2; i < 3; i += 2)
                    {
                        int proj = projToShoot;

                        if (Math.Abs(i) == 2)
                        {
                            proj = ModContent.ProjectileType<TerraBolt>();
                            damage = (int)(damage * 0.85f);
                        }
                        else
                        {
                            if (projToShoot == ProjectileID.WoodenArrowFriendly)
                            {
                                proj = ModContent.ProjectileType<TerraBolt>();
                                damage = (int)(damage * 0.85f);
                            }
                            else
                            {
                                velocity *= 0.8f;
                            }
                        }
                        float spread = Main.rand.NextFloat(0.025f, 0.05f) * ShootIndex / 2;
                        Vector2 a = (Vector2.UnitX * -Math.Abs(i)).RotatedBy(Projectile.velocity.ToRotation());
                        Projectile.NewProjectile(source, spawnLocation, velocity.RotatedBy(i * spread) + a, proj, damage, heldItem.knockBack, player.whoAmI, ai2: ShootIndex);
                    }

                    //for (int i = -1; i < 2; i += 1)
                    //{
                    //    float spread = Main.rand.NextFloat(0.025f, 0.05f);
                    //    float clamp = Math.Clamp(2.5f - Math.Abs(i) - 0.15f, 1.25f, 1.5f);
                    //    Projectile.NewProjectile(source, spawnLocation, velocity.RotatedBy(i * spread) * clamp, projToShoot, damage, knockBack, player.whoAmI);
                    //}
                    ShootCount++;
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        #endregion

        #region frame animation
        if (++Projectile.frameCounter == 6)
        {
            ++Projectile.frame;
            Projectile.frameCounter = 0;
            if (Projectile.frame >= Main.projFrames[Type])
            {
                Projectile.frame = 0;
            }
        }
        #endregion

        #region sound and dust
        // Sound and dust are separate from the code shooting arrows because they need to run on other clients as well.
        if (ShootTimer == initialShootDelay - 12 && HoldTimer != 1f && (player.channel || ShootCount < maxShootCount))
        {
            Projectile.soundDelay = initialShootDelay - 12;
            // Prevents a shoot sound from playing when the projectile initially spawns
            if (HoldTimer != 1f)
            {
                SoundEngine.PlaySound(SoundID.Item102, Projectile.position);
            }
        }

        if (ShootTimer < initialShootDelay - 24 && (player.channel || ShootCount < maxShootCount))
        {
            for (int i = 0; i < (int)Utils.Remap(ShootTimer, 0, initialShootDelay - 20, 1, 3); i++)
            {
                if (ShootTimer % (int)Utils.Remap(ShootTimer, 0, initialShootDelay - 20, 2, 1) == 0)
                {
                    Vector2 dir = Main.rand.NextVector2Unit(MathHelper.Pi / 4, MathHelper.Pi).RotatedBy(-MathHelper.PiOver4 * 3).RotatedBy(Projectile.rotation - (Projectile.direction == 1 ? 0 : MathHelper.Pi));
                    Vector2 pos = Main.rand.NextFloat(1f, 1.5f) * dir * Utils.Remap(ShootTimer, 0, initialShootDelay - 20, 35f, 75f) + Projectile.position;
                    Vector2 speed = (pos.DirectionTo(Projectile.position) * pos.Distance(Projectile.position) / 10f);
                    Dust d = Dust.NewDustDirect(pos, 16, 16, DustID.Terra, speed.X, speed.Y);
                    d.velocity += player.velocity;
                    d.noGravity = true;
                    d.scale = Utils.Remap(ShootTimer, 0, initialShootDelay - 20, 0.5f, 1.25f);
                }
            }
        }

        if (ShootTimer == initialShootDelay - 18 && HoldTimer != 1f && (player.channel || ShootCount < maxShootCount))
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit(MathHelper.Pi / 4, MathHelper.Pi).RotatedBy(-MathHelper.PiOver4 * 3).RotatedBy(Projectile.rotation - (Projectile.direction == 1 ? 0 : MathHelper.Pi));
                Vector2 speed = dir * 4f;
                Dust d = Dust.NewDustDirect(Projectile.position, 8, 8, DustID.Terra, speed.X, speed.Y);
                d.noGravity = true;
                //d.velocity *= 0.85f;
                d.scale = Main.rand.NextFloat(0.5f, 1.2f);
            }
        }
        #endregion
        */
        #endregion

        #region normal_fire_version
        Player player = Main.player[Projectile.owner];
        Item heldItem = player.HeldItem;

        bool shouldShootProj = false;
        int maxWaitTime = heldItem.useTime;
        int delayBetweenShot = 6;
        int maxShootCount = 3;
        int ShootIndex = ShootCount % maxShootCount + 1;

        HoldTimer++;
        ShootTimer++;

        #region shooting timer
        if (ShootTimer <= delayBetweenShot * maxShootCount)
        {
            if (ShootTimer % delayBetweenShot == 0)
            {
                shouldShootProj = true;
            }
        }
        if (ShootTimer >= maxWaitTime)
        {
            ShootTimer = 0f;
        }
        #endregion

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        Vector2 toMouse = Vector2.Normalize(Main.MouseWorld - playerCenter);
        float holdoutDist = TerraLongbow.HoldoutDistance * Projectile.scale;
        Vector2 holdoutOffset = holdoutDist * toMouse;

        #region projectile shooting code
        if (shouldShootProj && Main.myPlayer == Projectile.owner)
        {
            if ((player.channel || ShootCount < maxShootCount) && player.HasAmmo(heldItem) && !player.noItems && !player.CCed)
            {
                if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                {
                    Projectile.netUpdate = true;
                }

                // Set the projectile velocity, which is actually the holdout offset for held projectiles.
                Projectile.velocity = holdoutOffset;
                var spawnLocation = playerCenter + holdoutOffset;
                bool ammoConsumed = player.PickAmmo(heldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId);

                //shoot projectiles
                if (ammoConsumed)
                {
                    PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
                    sparkle.LocalPosition = Projectile.Center + holdoutOffset/2;
                    sparkle.Velocity = player.velocity;
                    sparkle.Scale = new Vector2(1.5f, 0.8f);
                    sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
                    sparkle.DrawVerticalAxis = true;
                    sparkle.ColorTint = Main.rand.NextBool() ? new Color(177, 255, 75) : new Color(255, 211, 47);
                    sparkle.FadeInEnd = 5;
                    sparkle.FadeOutStart = 5;
                    sparkle.FadeOutEnd = 20;
                    Main.ParticleSystem_World_OverPlayers.Add(sparkle);

                    float baseSpeed = 35f;
                    var source = player.GetSource_ItemUse_WithPotentialAmmo(heldItem, usedAmmoItemId);
                    Vector2 velocity = Vector2.Normalize(Projectile.velocity) * baseSpeed;
                    double rotation = Projectile.velocity.ToRotation();
                    var terraBolt = ModContent.ProjectileType<TerraBolt>();
                    int damageNerf = (int)(damage * 0.5f);

                    //bolts
                    for (int i = -2; i < 3; i += 2)
                    {
                        int thisProj = projToShoot;
                        if (Math.Abs(i) == 2)
                        {
                            thisProj = terraBolt;
                            damage = damageNerf;
                        }
                        else
                        {
                            if (projToShoot == ProjectileID.WoodenArrowFriendly)
                            {
                                thisProj = terraBolt;
                                damage = damageNerf;
                            }
                        }
                        float spread = Main.rand.NextFloat(0.025f, 0.05f) * ShootIndex / 2;
                        Vector2 forwardOffset = (Vector2.UnitX * -Math.Abs(i)).RotatedBy(rotation);
                        Vector2 finalVelocity = velocity.RotatedBy(i * spread / 2) + forwardOffset;
                        Projectile.NewProjectile(source, spawnLocation, finalVelocity, thisProj, damage, heldItem.knockBack, player.whoAmI, ai2: ShootIndex);
                    }
                    //arrows
                    for (int i = -1; i < 2; i++)
                    {
                        float spread = Main.rand.NextFloat(0.025f, 0.05f);
                        float clamp = Math.Clamp(2.5f - Math.Abs(i) - 0.15f, 1.25f, 1.5f);
                        Vector2 finalVelocity = velocity.RotatedBy(i * spread) * clamp;

                        int thisProj = projToShoot;
                        if (projToShoot == ProjectileID.WoodenArrowFriendly)
                        {
                            thisProj = terraBolt;
                            damage = damageNerf;
                            finalVelocity *= 0.75f;
                        }

                        Projectile.NewProjectile(source, spawnLocation, finalVelocity, thisProj, damage, knockBack, player.whoAmI);
                    }
                    ShootCount++;
                }
            } else {
                Projectile.Kill();
            }
        }
        #endregion

        #region frame animation
        if (++Projectile.frameCounter == 6)
        {
            ++Projectile.frame;
            Projectile.frameCounter = 0;
            if (Projectile.frame >= Main.projFrames[Type])
            {
                Projectile.frame = 0;
            }
        }
        #endregion

        #region sound and dust
        if (ShootTimer == 7)
        {
            SoundEngine.PlaySound(SoundID.Item102, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item109 with { Volume = 0.7f }, Projectile.position);

            for (int i = 0; i < 20; i++)
            {
                float projRotation = Projectile.rotation - (Projectile.direction == 1 ? 0 : MathHelper.Pi);
                Vector2 arc = Main.rand.NextVector2Unit(MathHelper.Pi / 4, MathHelper.Pi).RotatedBy(-MathHelper.PiOver4 * 3);
                Vector2 rotatedArc = arc.RotatedBy(projRotation);
                float rand = Main.rand.NextFloat(0.5f, 1f);
                Vector2 speed = rotatedArc * 8f * rand + player.velocity;
                Dust d = Dust.NewDustDirect(Projectile.position, 8, 8, DustID.Terra, speed.X, speed.Y);
                d.noGravity = true;
                //d.velocity *= 0.85f;
                d.scale = rand + 0.25f + Main.rand.NextFloat(-0.15f, 0.15f);
            }
        }
        #endregion

        //Main.chatMonitor.Clear();
        //Main.NewText("ShootTimer: " + ShootTimer);
        //Main.NewText("HoldTimer: " + HoldTimer);
        //Main.NewText("ShootCount: " + ShootCount);
        //Main.NewText("shouldShootArrow: " + shouldShootProj);
        //Main.NewText("ShootIndex: " + ShootIndex);
        #endregion

        Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
        Projectile.spriteDirection = Projectile.direction;
        player.ChangeDir(Projectile.direction);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        Projectile.Center = playerCenter;
        float rotationOffset = Projectile.spriteDirection == -1 ? MathHelper.Pi : 0;
        Projectile.rotation = Projectile.velocity.ToRotation() + rotationOffset;
        player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        Projectile.timeLeft = 2;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}

public class TerraBolt : ModProjectile
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
        Projectile.arrow = false;
        Projectile.penetrate = 2;
        Projectile.timeLeft = 40;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }
    public override void OnKill(int timeLeft)
    {
        //for (int i = 0; i < 15; i++)
        //{
        //    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
        //    d.velocity *= 2.3f;
        //    d.noGravity = true;
        //    if (Projectile.ai[0] == 0)
        //        d.color = Color.Lerp(new Color(0, 255, 255), new Color(128, 0, 200), Main.rand.NextFloat());
        //    else
        //        d.color = Color.Lerp(new Color(255, 160, 0), new Color(128, 0, 160), Main.rand.NextFloat());
        //    d.color.A = 0;
        //}

        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(4f, 1.3f);
        sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = Main.rand.NextBool() ? new Color(177, 255, 75) : new Color(255, 211, 47);
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 20;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Terra, new Vector2(0, Main.rand.NextFloat(-5, 5)).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.3f, 0.3f)));
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            //d.color = sparkle.ColorTint;
            d.fadeIn = Main.rand.NextFloat(1.5f);
            Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.Terra, new Vector2(Main.rand.NextFloat(-7, 7), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.3f, 0.3f)));
            d2.scale = Main.rand.NextFloat(0.8f, 1.2f);
            d2.noGravity = true;
            //d2.color = sparkle.ColorTint;
            d2.fadeIn = Main.rand.NextFloat(1.5f);
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Main.rand.NextBool(3))
        {
            PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
            sparkle.LocalPosition = Projectile.Center;
            sparkle.Scale = new Vector2(1.5f, 0.8f);
            sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
            sparkle.DrawVerticalAxis = true;
            sparkle.ColorTint = Main.rand.NextBool() ? new Color(177, 255, 75) : new Color(255, 211, 47);
            sparkle.FadeInEnd = 5;
            sparkle.FadeOutStart = 5;
            sparkle.FadeOutEnd = 20;
            Main.ParticleSystem_World_OverPlayers.Add(sparkle);
        }

        int target = Projectile.FindTargetWithLineOfSight(500);

        if (target == -1)
        {
            if (oldVelocity.Y != Projectile.velocity.Y)
                Projectile.velocity.Y = -Projectile.oldVelocity.Y;
            if (oldVelocity.X != Projectile.velocity.X)
                Projectile.velocity.X = -Projectile.oldVelocity.X;
        }
        else
        {
            Projectile.velocity = Projectile.DirectionTo(Main.npc[target].Center).RotatedByRandom(0.1f) * oldVelocity.Length();
        }

        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

        Projectile.damage = (int)(Projectile.damage * 0.95f);

        Projectile.penetrate--;
        if (Projectile.penetrate == 0)
        {
            Projectile.Kill();
        }
        return false;
    }
    public override void AI()
    {
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 4)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
            if (Projectile.frame == 4)
            {
                Projectile.frame = 0;
            }
        }
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        if (Main.rand.NextBool(4))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
            d.velocity *= 0.3f;
            d.noGravity = true;
            d.color = Color.Lerp(new Color(50, 90, 200), new Color(50, 200, 30), Main.rand.NextFloat());
            d.color.A = 0;
        }

        int target = Projectile.FindTargetWithLineOfSight(400);

        if (target != -1)
        {
            Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 1f;
            Projectile.velocity = Projectile.velocity.LengthClamp(Projectile.oldVelocity.Length(), 12f);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        Rectangle frame = tex.Frame(1, 4, 0, Projectile.frame);
        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = 1 - i / (float)ProjectileID.Sets.TrailCacheLength[Type];
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, Color.Lerp(new Color(30, 150, 255, 0), Color.Lerp(new Color(255, 210, 50, 64), new Color(180, 255, 55, 64), Projectile.ai[2]/3), multiply * multiply) * multiply, Projectile.oldRot[i], frame.Size() / 2, 1f + multiply * 0.2f, SpriteEffects.FlipVertically);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 1f), Projectile.rotation, frame.Size() / 2, 1f, SpriteEffects.FlipVertically);
        return false;
    }
}