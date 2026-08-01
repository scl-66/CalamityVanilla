using CalamityVanilla.Common.Interfaces;
using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.TerraGlobe;

public class TerraGlobe : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TerraGlobeProjectile>(), 30, 9, true);
        Item.mana = 24;
        Item.damage = 78;
        Item.knockBack = 5;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 20, 0, 0);
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.UseSound = SoundID.Item67 with
        {
            Volume = 0.75f
        };
    }

    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation.X -= 29f * player.direction;
        player.itemLocation.Y -= 28f;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        int rand = Main.rand.Next(new int[] { -1, 1 });
        velocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(5, 12) * rand));
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddTile(TileID.MythrilAnvil)
            .AddIngredient<BlacklightScepter.BlacklightScepter>()
            .AddIngredient<FrigidFlashBolt.FrigidflashBolt>()
            .AddIngredient(ItemID.VenomStaff)
            .AddIngredient(ItemID.SpectreBar, 12)
            .Register();
    }
}

public class TerraGlobeProjectile : ModProjectile, ISyncedOnHitEffect
{
    public int maxTimeLeft = (int)(60 * 2.23f);
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 24);
        Projectile.timeLeft = maxTimeLeft;
        Projectile.penetrate = 5;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Projectile.ai[1]++;
        if (Projectile.ai[1] == 1) // summon orbiting orbs
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Vector2 rand = Main.rand.NextVector2CircularEdge(Projectile.width * 2, Projectile.height * 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + rand, rand.SafeNormalize(Vector2.UnitX) * 5f,
                    ModContent.ProjectileType<TerraGlobeOrb>(), 0, 0.5f, Projectile.owner, Projectile.whoAmI, Projectile.damage / 3);
                }
            }
        }

        if (Projectile.ai[1] == 3) // spawn dust when first fired (this code is messy i knowww)
        {
            for (int i = 0; i < 30; i++)
            {
                float projRotation = Projectile.rotation - (Projectile.direction == 1 ? 0 : MathHelper.Pi);
                Vector2 arc = Main.rand.NextVector2Unit(MathHelper.Pi / 4, MathHelper.Pi).RotatedBy(-MathHelper.PiOver4 * 3);
                Vector2 rotatedArc = arc.RotatedBy(projRotation);
                float rand = Main.rand.NextFloat(0.5f, 1f);
                Vector2 speed = rotatedArc * 2f * rand + Main.player[Projectile.owner].velocity;
                Dust d2 = Dust.NewDustDirect(Projectile.position, 8, 8, DustID.Terra, speed.X, speed.Y);
                d2.noGravity = !Main.rand.NextBool(3);
                d2.scale = rand + 0.25f + Main.rand.NextFloat(-0.15f, 0.15f);
            }
        }

        if (Projectile.timeLeft > 40 && Projectile.penetrate > 1)
        {
            Vector2 dir = Projectile.Center.DirectionTo(Main.MouseWorld);
            Projectile.velocity += (dir);
            Projectile.velocity = Projectile.velocity.LengthClamp(15, 2);
        }
        else
        {
            Projectile.velocity *= 0.93f;
        }

        if (Projectile.penetrate <= 1 && Projectile.timeLeft > 41)
        {
            Projectile.timeLeft = 40;
        }

        if (Main.rand.NextBool(4))
        {
            Dust d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d2.velocity *= 0.3f;
            d2.velocity += Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f);
            d2.noGravity = !Main.rand.NextBool(8);
            d2.scale = 0.25f;
            d2.fadeIn = 0.3f;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.damage = (int)(Projectile.damage * 0.8f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        Projectile.damage = (int)(Projectile.damage * 0.8f);
        Projectile.penetrate--;
        if (Projectile.penetrate > 0)
        {
            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.5f;
            return false;
        }
        return true;
    }

    public override void OnKill(int timeLeft)
    {
        if (timeLeft > 40)
        {
            int length = ProjectileID.Sets.TrailCacheLength[Type];
            int t = DustID.Terra;
            for (int i = 1; i < length; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, t);
                d.color = Color.White * Projectile.Opacity * (1f - i / (float)length);
                d.noGravity = d.noLight = true;
                d.scale = 1.5f;
                d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * 3;
                d.velocity *= 0.4f;
            }
        }
        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(Main.rand.NextFloat(2.7f, 3.3f), Main.rand.NextFloat(0.9f, 1.1f));
        sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f);
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = Color.Lime;
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 40;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);

        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d.velocity += Projectile.velocity * 0.1f;
            d.noGravity = true;
        }

        if (Main.myPlayer == Projectile.owner)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerraGlobeExplosion>(), (int)(Projectile.damage * 2.5f), Projectile.knockBack);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        float glowOpacity = MathF.Pow(Projectile.Opacity, 8);
        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = 1 - i / (float)ProjectileID.Sets.TrailCacheLength[Type];
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, Color.Lerp(new Color(30, 150, 255, 0), Color.Lerp(new Color(255, 210, 50, 64), new Color(180, 255, 55, 64), Projectile.ai[2] / 3), multiply * multiply) * multiply * glowOpacity, Projectile.oldRot[i], TextureAssets.Projectile[Type].Size() / 2, 1f + multiply * 0.2f, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f) * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2, 1f, SpriteEffects.None);
        return false;
    }

    public void SyncedOnHitNPC(Player player, NPC target, int damage, float knockback, bool crit, int hitDirection)
    {
        for (int i = -1; i < 2; i += 2)
        {
            var p = VanillaParticles.RequestPrettySparkleParticle();
            p.ColorTint = new Color(0.2f, 0.85f, 0.4f, 0.5f);
            p.LocalPosition = Projectile.Center;
            p.Rotation = Projectile.velocity.ToRotation();
            p.Scale = new Vector2(3, 0.75f);
            p.FadeInNormalizedTime = 0.95f;
            p.FadeOutNormalizedTime = 0.95f;
            p.TimeToLive = 20;
            p.AdditiveAmount = 0.35f;
            p.DrawVerticalAxis = false;
            p.Velocity = Projectile.velocity * 0.05f * (i + 0.25f);
            Main.ParticleSystem_World_OverPlayers.Add(p);

            var p2 = VanillaParticles.RequestPrettySparkleParticle();
            p2.ColorTint = p.ColorTint;
            p2.LocalPosition = p.LocalPosition;
            p2.Rotation = p.Rotation + MathHelper.PiOver2;
            p2.Scale = new Vector2(3, 0.5f);
            p2.FadeInNormalizedTime = p.FadeInNormalizedTime;
            p2.FadeOutNormalizedTime = p.FadeOutNormalizedTime;
            p2.TimeToLive = p.TimeToLive;
            p2.AdditiveAmount = p.AdditiveAmount;
            p2.DrawVerticalAxis = false;
            p2.Velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.02f * i;
            Main.ParticleSystem_World_OverPlayers.Add(p2);

            for (int y = 0; y < 2; y++)
            {
                Dust d = Dust.NewDustPerfect(p.LocalPosition, DustID.Terra, p.Velocity.RotatedByRandom(1) * Main.rand.NextFloat(4));
                d.noGravity = true;
                Dust d2 = Dust.NewDustPerfect(p2.LocalPosition, DustID.Terra, p2.Velocity.RotatedByRandom(1) * Main.rand.NextFloat(4));
                d2.noGravity = true;
            }
        }
    }
}

public class TerraGlobeOrb : ModProjectile
{
    private Projectile ParentProjectile
    {
        get => Main.projectile[(int)Projectile.ai[0]];
        set => Projectile.ai[0] = value.whoAmI;
    }
    public ref float BaseDamage => ref Projectile.ai[1];
    public Vector2 startOffset;
    public Vector2 offsetVel;
    public bool canFollow = true;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.timeLeft = (int)(60 * 3f);
        Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
        Projectile.ignoreWater = true;
        //Projectile.extraUpdates = 1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        startOffset = new(Projectile.Center.X - ParentProjectile.Center.X, Projectile.Center.Y - ParentProjectile.Center.Y);
    }
    public override void AI()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;

        if (canFollow)
        {
            //Vector2 dir = Projectile.Center.DirectionTo(ParentProjectile.Center);
            //Projectile.velocity += (dir);
            //Projectile.velocity = Projectile.velocity.LengthClamp(15, 2);
            Projectile.Center = ParentProjectile.Center + startOffset;

            Vector2 dir = (ParentProjectile.Center + startOffset).DirectionTo(ParentProjectile.Center);
            offsetVel += dir;
            offsetVel = offsetVel.RotatedBy(MathHelper.ToRadians(2));
            offsetVel = offsetVel.LengthClamp(8, 2);
            startOffset += offsetVel;

            Projectile.timeLeft = (int)(60 * 3f);

            if (ParentProjectile.timeLeft <= 0)
            {
                canFollow = false;
                Projectile.velocity = (Projectile.Center - ParentProjectile.Center) / 8;
                Projectile.velocity = Projectile.velocity.LengthClamp(8, 4);
            }
        }
        else
        {
            Projectile.damage = (int)BaseDamage;
            int target = Projectile.FindTargetWithLineOfSight(500);

            if (target != -1)
            {
                Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 1f;
                Projectile.velocity = Projectile.velocity.LengthClamp(15, 2);
            }
            else
            {
                NPC targetAttempt2 = Projectile.FindTargetWithinRange(250);
                if (targetAttempt2 != null)
                {
                    Projectile.velocity += Projectile.DirectionTo(targetAttempt2.Center) * 1f;
                    Projectile.velocity = Projectile.velocity.LengthClamp(6, 2);
                }
                else
                {
                    Projectile.velocity *= 0.96f;
                }
            }
        }

        //dust
        if (Main.rand.NextBool(2))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d.noLight = true;
            d.velocity *= 0.2f;
            d.noGravity = true;
            //d.velocity += Projectile.velocity * 0.4f;
            d.scale = Main.rand.NextFloat(0.5f, 1f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(Main.rand.NextFloat(2.7f, 3.3f), Main.rand.NextFloat(0.9f, 1.1f));
        sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f);
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = Color.Lime;
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 40;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);

        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d.velocity += Projectile.velocity * 0.1f;
            d.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        float glowOpacity = MathF.Pow(Projectile.Opacity, 8);

        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            float inbetween = 3;
            for (int j = 0; j < inbetween; j++)
            {
                float percent = 1f - ((MathHelper.Lerp(i, i - 1 >= 0 ? i - 1 : 0, j)) / (float)Projectile.oldPos.Length);
                Vector2 drawPos = (Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[(i - 1) >= 0 ? i - 1 : 0], j/inbetween) + Projectile.Size / 2 - Main.screenPosition);
                Main.EntitySpriteDraw(tex.Value, drawPos, null, Color.Lerp(new Color(30, 150, 255, 0), Color.Lerp(new Color(255, 210, 50, 64), new Color(180, 255, 55, 64), Projectile.ai[2] / 3), percent) * (percent / 2) * glowOpacity, Projectile.oldRot[i], TextureAssets.Projectile[Type].Size() / 2, 1f + percent * 0.2f, SpriteEffects.None);
            }
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f) * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2, 1f, SpriteEffects.None);
        return false;
    }
}

public class TerraGlobeExplosion : ModProjectile, ISyncedOnHitEffect
{
    public override string Texture => ModContent.GetModProjectile(ModContent.ProjectileType<TerraGlobeProjectile>()).Texture;
    public override void SetDefaults()
    {
        //Projectile.arrow = true;
        Projectile.QuickDefaults(false, 150);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 9;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.Opacity = 0f;
    }
    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, new Vector3(0.9f, 1f, 0f) * Projectile.timeLeft / 14f); // R G B values from 0 to 1f.
    }
    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.3f, PitchVariance = 0.2f, Volume = 0.6f }, Projectile.position);
        for (int i = 0; i < 50; i++)
        {
            Vector2 speed = Main.rand.NextVector2Circular(15f, 15f);
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Terra, speed);
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            d.fadeIn = Main.rand.NextFloat(1.4f);
        }
    }

    public void SyncedOnHitNPC(Player player, NPC target, int damage, float knockback, bool crit, int hitDirection)
    {
        for (int i = -3; i < 4; i += 2)
        {
            var p = VanillaParticles.RequestPrettySparkleParticle();
            p.ColorTint = new Color(0.2f, 0.85f, 0.4f, 0.5f);
            p.LocalPosition = target.Center + Main.rand.NextVector2Circular(15, 15);
            p.Rotation = Projectile.velocity.ToRotation();
            p.Scale = new Vector2(3, 0.75f);
            p.FadeInNormalizedTime = 0.95f;
            p.FadeOutNormalizedTime = 0.95f;
            p.TimeToLive = 20;
            p.AdditiveAmount = 0.35f;
            p.DrawVerticalAxis = false;
            p.Velocity = Projectile.velocity * 0.05f * (i + 0.25f);
            Main.ParticleSystem_World_OverPlayers.Add(p);

            var p2 = VanillaParticles.RequestPrettySparkleParticle();
            p2.ColorTint = p.ColorTint;
            p2.LocalPosition = p.LocalPosition;
            p2.Rotation = p.Rotation + MathHelper.PiOver2;
            p2.Scale = new Vector2(3, 0.5f);
            p2.FadeInNormalizedTime = p.FadeInNormalizedTime;
            p2.FadeOutNormalizedTime = p.FadeOutNormalizedTime;
            p2.TimeToLive = p.TimeToLive;
            p2.AdditiveAmount = p.AdditiveAmount;
            p2.DrawVerticalAxis = false;
            p2.Velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.02f * i;
            Main.ParticleSystem_World_OverPlayers.Add(p2);

            for (int y = 0; y < 2; y++)
            {
                Dust d = Dust.NewDustPerfect(p.LocalPosition, DustID.Terra, p.Velocity.RotatedByRandom(1) * Main.rand.NextFloat(4));
                d.noGravity = true;
                Dust d2 = Dust.NewDustPerfect(p2.LocalPosition, DustID.Terra, p2.Velocity.RotatedByRandom(1) * Main.rand.NextFloat(4));
                d2.noGravity = true;
            }
        }
    }
}
