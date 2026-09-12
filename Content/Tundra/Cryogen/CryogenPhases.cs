using CalamityVanilla.Common.Particles;
using CalamityVanilla.Content.Tundra.Cryogen.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityVanilla.Content.Tundra.Cryogen;

public partial class Cryogen : ModNPC
{
    private ref float _currentAttack => ref NPC.ai[0];
    public override void AI()
    {
        NPC.rotation += (NPC.velocity * new Vector2(0.01f, 0.005f)).Length() * Math.Sign(NPC.velocity.X);
        _snowOverlayOpacity *= 0.96f;
        Lighting.AddLight(NPC.Center, new Vector3(0.8f, 1f, 1f));
        if (Main.rand.NextBool(10))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Snow);
            d.scale = 0.8f;
            d.velocity += NPC.velocity;
        }

        if (_currentAttack < 0)
        {
            NPC.velocity.Y += 0.1f;
            NPC.velocity.X *= 0.98f;
            NPC.EncourageDespawn(60);
            return;
        }

        if (!NPC.HasValidTarget || !target.ZoneSnow)
        {
            NPC.localAI[0]++;
            NPC.TargetClosest(player => !player.ZoneSnow, false);
            if (NPC.localAI[0] == 60 * 15)
            {
                NPC.netUpdate = true;
                _currentAttack = -1;
            }
        }
        ulong seed = Utils.RandomNextSeed((ulong)(NPC.whoAmI + _currentAttack));
        //ulong seed2 = Utils.RandomNextSeed((ulong)(NPC.whoAmI * _currentAttack));
        bool predictableRand = Utils.RandomInt(ref seed, 2) == 0;
        //bool predictableRand2 = Utils.RandomInt(ref seed, 2) == 0;

        NPC.localAI[3]++;
        //if(_currentAttack > 19 && Main.netMode != NetmodeID.MultiplayerClient)
        //{
        //    if (NPC.localAI[3] > 100)
        //    {
        //        for(int i = 0; i < 100; i++)
        //        {
        //            bool valid = true;
        //            Vector2 rand = NPC.Center + new Vector2(Main.rand.NextFloat(40, 100) * (Main.rand.NextBool()? 16 : -16), Main.rand.NextFloat(20, 60) * (Main.rand.NextBool() ? 16 : -16));
        //            foreach(Player p in Main.ActivePlayers)
        //            {
        //                if(p.Center.Distance(rand) < 16 * 20)
        //                {
        //                    valid = false;
        //                    break;
        //                }
        //            }
        //            if (!valid)
        //                continue;
        //            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceMine>(), 0, 2);
        //            break;
        //        }
        //        NPC.localAI[3] = 0;
        //    }
        //}

        switch (_currentAttack)
        {
            default:
                if (_currentAttack > 19 && predictableRand)
                    IcicleDash();
                else
                    Fly();
                break;
            case 1:
                SnowflakesOrBombs(predictableRand);
                break;
            case 3:
                IceRing();
                break;
            case 5:
                SnowflakesOrBombs(predictableRand);
                break;
            case 7:
                SnowflakesOrBombs(predictableRand);
                break;
            case 8:
                IceRing();
                break;

            //phase 2
            case 19:
                Phase2Transition();
                break;
            case 21:
                if (predictableRand)
                    IceRing(true);
                else
                    DeathLaser();
                break;
            case 23:
                //if (predictableRand2)
                SnowflakesOrBombs(predictableRand);
                //else
                //IceMines();
                break;
            case 25:
                SnowflakesOrBombs(predictableRand);
                break;
            case 27:
                if (predictableRand)
                    IceRing(true);
                else
                    DeathLaser();
                break;
            case 29:
                //if (predictableRand2)
                SnowflakesOrBombs(predictableRand);
                //else
                //IceMines();
                break;
            case 30:
                SnowflakesOrBombs(predictableRand);
                break;
        }
    }
    private void CycleAttack()
    {
        NPC.ai[1] = 0;
        NPC.ai[2] = 0;
        NPC.ai[3] = 0;
        NPC.localAI[1] = 0;
        NPC.localAI[2] = 0;
        _currentAttack++;
        if (!IsInPhase2())
        {
            if (_currentAttack == 9)
                _currentAttack = 0;
        }
        else
        {
            if (_currentAttack <= 11)
            {
                _currentAttack = 19;
            }
            else if (_currentAttack == 31)
            {
                _currentAttack = 20;
            }
        }
    }
    private void Phase2Transition()
    {
        NPC.velocity *= 0.9f;
        NPC.rotation += Main.rand.NextFloat(-0.4f, 0.4f) * NPC.ai[1] / 120f;
        _snowOverlaySpinDirection = NPC.direction;
        _snowOverlayOpacity = Math.Max(_snowOverlayOpacity, NPC.ai[1] / 120f);
        NPC.ai[1]++;
        if (NPC.ai[1] > 120)
        {
            CycleAttack();
            SoundEngine.PlaySound(SoundID.Roar, NPC.position);
            int iterations = 200;
            for (int i = 0; i < iterations; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FrostHydra);
                d.velocity = Vector2.UnitY.RotatedBy((i / (float)iterations * MathHelper.TwoPi) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(10, 12);
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(2);
            }
        }
    }
    private void Fly()
    {
        NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
        NPC.directionY = Math.Sign(target.Center.Y - NPC.Center.Y);
        NPC.ai[1] += 0.5f;
        NPC.ai[2]++;

        if (NPC.ai[2] >= 40)
        {
            if (NPC.ai[2] < 100)
            {
                _snowOverlaySpinDirection = -NPC.direction;
                _snowOverlayOpacity = Math.Max(_snowOverlayOpacity, Utils.Remap(NPC.ai[1], 40, 100, 0, 1));
                NPC.velocity *= 0.95f;
                NPC.rotation += NPC.direction * Utils.Remap(NPC.ai[2], 40, 100, 0, 0.3f);
            }
            else if (NPC.ai[2] == 100)
            {
                SoundEngine.PlaySound(SoundID.Item29, NPC.Center);
                NPC.velocity = NPC.Center.DirectionTo(target.Center) * 18;
            }
            else if (NPC.ai[2] > 140 && NPC.ai[2] < 160)
            {
                NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 10, 0.3f);
            }
            else if (NPC.ai[2] > 160)
            {
                NPC.ai[2] = -100;
            }
            else
            {
                _snowOverlayOpacity = 1;
            }
            return;
        }

        bool closeEnoughToSwitchPhase = NPC.Center.Distance(target.Center) < 16 * 50;
        if (closeEnoughToSwitchPhase)
        {
            NPC.ai[1] += 1f;
            NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 6, 0.1f);
            if (NPC.ai[1] > 200)
            {
                CycleAttack();
            }
        }
        else
        {
            NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 24, 0.15f);
        }
    }
    private void SnowflakesOrBombs(bool Bombs)
    {
        int chargeUpTime = 31;
        if (NPC.ai[1] > 0 || NPC.velocity.Length() < 1f)
        {
            NPC.ai[1]++;
            if (NPC.ai[1] < chargeUpTime)
            {
                //float iterations = 1 + (NPC.ai[1] / chargeUpTime * 3);
                //for (int i = 0; i < iterations; i++)
                //{
                //    Vector2 dir = Main.rand.NextVector2Unit();
                //    Dust d = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(100, 160), DustID.RainbowMk2, -dir * Main.rand.NextFloat(10, 15));
                //    d.noGravity = true;
                //    d.velocity += NPC.velocity;
                //    d.scale = 2;
                //    d.color = GetAuroraColor((int)Main.timeForVisualEffects);
                //}
                Vector2 vect = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 160);
                var p = VanillaParticles.RequestFadingParticle();
                p.ColorTint = Color.White;
                float time = Main.rand.NextFloat(25, 45);
                p.SetTypeInfo(time);
                Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
                p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), (-vect / time) + NPC.velocity, vect + NPC.Center);
                p.FadeInNormalizedTime = 0.3f;
                p.FadeOutNormalizedTime = 1f;
                p.Scale = Vector2.One * 1.3f;
                p.ScaleVelocity = Vector2.One / -time;
                p.RotationVelocity = p.Velocity.X * 0.1f;
                p.RotationAcceleration = -p.RotationVelocity / time;
                //p.Rotation = Main.rand.NextFloatDirection();
                Main.ParticleSystem_World_BehindPlayers.Add(p);
            }
        }
        else
        {
            NPC.velocity *= 0.95f;
        }
        if (NPC.ai[1] > chargeUpTime && Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (!Bombs && NPC.ai[1] % 8 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 4f), ModContent.ProjectileType<CryoFlake1>() + Main.rand.Next(3), 15, 1, -1, NPC.target);
            }
            else if ((NPC.ai[1] - chargeUpTime - 1) is 0 or 8 or 16 or 24 or 32)
            {
                Vector2 adjustedTargetPos = target.Center + (NPC.Center.DirectionTo(target.Center) * 128);
                if (target.velocity.Length() > 6f)
                {
                    adjustedTargetPos += target.velocity * 60;
                }
                Vector2 randomPoint = Vector2.Zero;
                for (int i = 0; i < 100; i++)
                {
                    randomPoint = adjustedTargetPos + Main.rand.NextVector2Circular(16 * 20, 16 * 10);
                    if (!Collision.IsWorldPointSolid(randomPoint, true))
                        break;
                }
                Vector2 vel = CVUtils.FindVelocityForGravityAffectedThing(NPC.Center, randomPoint, 0.25f, 90);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.LengthClamp(240), ModContent.ProjectileType<IceBomb>(), 24, 3);
            }
        }
        if (NPC.ai[1] > chargeUpTime + 64)
            CycleAttack();
    }
    private void IceRing(bool phase2 = false)
    {
        float spinSpeed = 0.015f;
        Vector2 targetPos = new Vector2(NPC.ai[2], NPC.ai[3]);
        if (targetPos == Vector2.Zero)
        {
            NPC.netUpdate = true;
            Vector2 adjustedTargetPos = target.Center + (target.velocity / 16 * 45);
            if (target.velocity != Vector2.Zero)
            {
                adjustedTargetPos += Vector2.Normalize(target.velocity) * 15;
            }
            NPC.ai[2] = adjustedTargetPos.X;
            NPC.ai[3] = adjustedTargetPos.Y;

            int ringSize = 16 * (phase2 ? 18 : 23);
            //ringSize += (int)Math.Ceiling(target.velocity.Length() * 45 / 1f);

            NPC.direction = ringSize * (Main.rand.NextBool() ? 1 : -1);
            NPC.ai[1] = NPC.Center.DirectionTo(adjustedTargetPos + new Vector2(16 * 35)).ToRotation() / spinSpeed;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, target.Center, ModContent.ProjectileType<IcePlacingBeam>(), 15, 1, -1, NPC.whoAmI, ringSize, Main.rand.NextBool() ? 1 : -1);
        }
        else
        {
            NPC.localAI[2]++;
            NPC.SimpleFlyMovement(NPC.Center.DirectionTo(targetPos + new Vector2(NPC.direction + (64 * Math.Sign(NPC.direction))).RotatedBy(NPC.ai[1] * spinSpeed)) * 12, 0.2f);
            float dist = NPC.Center.Distance(targetPos);
            int abs = Math.Abs(NPC.direction);
            //Main.NewText($"abs: {abs} | dist: {dist}", dist > abs + 64? Color.Green : Color.Red);
            if (dist > abs + 64)
            {
                NPC.ai[1]++;
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[2] > 60 && (int)NPC.localAI[2] % 30 == 0)
                {
                    NPC.localAI[1]++;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.Center.DirectionTo(targetPos).RotatedByRandom(0.5f) * 8, ModContent.ProjectileType<Icicles>(), 30, 2);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[2] > 30 && (int)NPC.localAI[2] % 70 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 4f), ModContent.ProjectileType<CryoFlake1>() + Main.rand.Next(3), 15, 1, -1, NPC.target);
                }
                if (NPC.localAI[1] > 6)
                {
                    NPC.netUpdate = true;
                    CycleAttack();
                }
            }
        }
    }
    private void IcicleDash()
    {
        if (NPC.ai[1] >= 40)
        {
            NPC.ai[1]++;
            if (NPC.ai[1] < 100)
            {
                NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
                //if (NPC.ai[1] < 100 - 40)
                //{
                //    Vector2 vect = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 160);
                //    var p = VanillaParticles.RequestFadingParticle();
                //    p.ColorTint = Color.White with { A = 0 };
                //    float time = Main.rand.NextFloat(25, 40);
                //    p.SetTypeInfo(time);
                //    Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
                //    p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), (-vect / time), vect + NPC.Center);
                //    p.FadeInNormalizedTime = 0.3f;
                //    p.FadeOutNormalizedTime = 1f;
                //    p.Scale = Vector2.One * 1.3f;
                //    p.ScaleVelocity = Vector2.One / -time;
                //    p.RotationVelocity = p.Velocity.X * 0.1f;
                //    p.RotationAcceleration = -p.RotationVelocity / time;
                //    Main.ParticleSystem_World_BehindPlayers.Add(p);
                //}

                _snowOverlaySpinDirection = -NPC.direction;
                _snowOverlayOpacity = Math.Max(_snowOverlayOpacity, Utils.Remap(NPC.ai[1], 40, 100, 0, 1));
                NPC.velocity *= 0.95f;
                NPC.rotation += NPC.direction * Utils.Remap(NPC.ai[1], 40, 100, 0, 0.3f);
            }
            else if (NPC.ai[1] == 100)
            {
                SoundEngine.PlaySound(SoundID.Item29, NPC.Center);
                NPC.velocity = NPC.Center.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(0.5f, 1f) * (Main.rand.NextBool() ? 1 : -1)) * 18;
                NPC.netUpdate = true;
            }
            else if (NPC.ai[1] == 140)
            {
                for (int i = 0; i < 50; i++)
                {
                    var p = VanillaParticles.RequestFadingParticle();
                    p.ColorTint = Color.White;
                    float time = Main.rand.NextFloat(25, 45);
                    p.SetTypeInfo(time);
                    Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
                    p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), Main.rand.NextVector2Circular(8, 8), NPC.Center);
                    p.FadeInNormalizedTime = 0.3f;
                    p.FadeOutNormalizedTime = 1f;
                    p.Scale = Vector2.One * 1.3f;
                    p.ScaleVelocity = Vector2.One / -time;
                    p.RotationVelocity = p.Velocity.X * 0.1f;
                    p.RotationAcceleration = -p.RotationVelocity / time;
                    Main.ParticleSystem_World_BehindPlayers.Add(p);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float rot = Main.rand.NextFloat(-0.1f, 0.1f);
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY.RotatedBy((i * MathHelper.TwoPi / 6f) + rot + (MathHelper.TwoPi / 12f)) * 12, ModContent.ProjectileType<Icicles>(), 30, 2);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY.RotatedBy((i * MathHelper.TwoPi / 6f) + rot) * 8, ModContent.ProjectileType<Icicles>(), 30, 2);
                    }
                }
            }
            else if (NPC.ai[1] > 140)
            {
                NPC.velocity *= 0.3f;
            }
            if (NPC.ai[1] > 150)
            {
                NPC.ai[1] = 40;
                NPC.ai[2]++;
            }
        }
        else
        {
            NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
            NPC.directionY = Math.Sign(target.Center.Y - NPC.Center.Y);
            bool closeEnoughToSwitchPhase = NPC.Center.Distance(target.Center) < 16 * 50;
            if (closeEnoughToSwitchPhase)
            {
                NPC.ai[1] += 1f;
                NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 6, 0.1f);
            }
            else
            {
                NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 24, 0.15f);
            }
        }
        if (NPC.ai[2] > 3)
        {
            CycleAttack();
        }
    }
    private void IceMines()
    {
        //int chargeUpTime = 31;
        //if (NPC.ai[1] > 0 || NPC.velocity.Length() < 1f)
        //{
        //    NPC.ai[1]++;
        //    if (NPC.ai[1] < chargeUpTime)
        //    {
        //        Vector2 vect = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 160);
        //        var p = VanillaParticles.RequestFadingParticle();
        //        p.ColorTint = Color.White;
        //        float time = Main.rand.NextFloat(25, 45);
        //        p.SetTypeInfo(time);
        //        Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
        //        p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), (-vect / time) + NPC.velocity, vect + NPC.Center);
        //        p.FadeInNormalizedTime = 0.3f;
        //        p.FadeOutNormalizedTime = 1f;
        //        p.Scale = Vector2.One * 1.3f;
        //        p.ScaleVelocity = Vector2.One / -time;
        //        p.RotationVelocity = p.Velocity.X * 0.1f;
        //        p.RotationAcceleration = -p.RotationVelocity / time;
        //        Main.ParticleSystem_World_BehindPlayers.Add(p);
        //    }
        //}
        //else
        //{
        //    NPC.velocity *= 0.95f;
        //}
        //if (NPC.ai[1] == chargeUpTime)
        //{
        //    if (Main.netMode != NetmodeID.MultiplayerClient)
        //    {
        //        for (int i = 0; i < 6; i++)
        //        {
        //            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(12), ModContent.ProjectileType<IceMine>(), 0, 2);
        //        }
        //    }
        //}
        //else if (NPC.ai[1] == chargeUpTime + 60)
        //{

        //}
    }
    private void DeathLaser()
    {
        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
        {
            NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
            NPC.directionY = Math.Sign(target.Center.Y - NPC.Center.Y);
            NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 6, 0.1f);
            return;
        }

        NPC.velocity *= 0.98f;
        int chargeUpTime = 31;
        int bigLaserDelay = 150;
        NPC.rotation += Utils.Remap(NPC.ai[1] - chargeUpTime - bigLaserDelay, 0, 60, 0, 0.1f * NPC.direction);
        _snowOverlaySpinDirection = NPC.direction;
        _snowOverlayOpacity = Utils.Remap(NPC.ai[1] - chargeUpTime - bigLaserDelay, 0, 60, 0, 1);
        if (NPC.ai[1] > 0 || NPC.velocity.Length() < 1f)
        {
            NPC.ai[1]++;
            if (NPC.ai[1] < chargeUpTime)
            {
                Vector2 vect = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 160);
                var p = VanillaParticles.RequestFadingParticle();
                p.ColorTint = Color.White;
                float time = Main.rand.NextFloat(25, 45);
                p.SetTypeInfo(time);
                Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
                p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), (-vect / time) + NPC.velocity, vect + NPC.Center);
                p.FadeInNormalizedTime = 0.3f;
                p.FadeOutNormalizedTime = 1f;
                p.Scale = Vector2.One * 1.3f;
                p.ScaleVelocity = Vector2.One / -time;
                p.RotationVelocity = p.Velocity.X * 0.1f;
                p.RotationAcceleration = -p.RotationVelocity / time;
                //p.Rotation = Main.rand.NextFloatDirection();
                Main.ParticleSystem_World_BehindPlayers.Add(p);
            }
        }
        if (NPC.ai[1] > chargeUpTime && Main.netMode != NetmodeID.MultiplayerClient)
        {
            if ((NPC.ai[1] - chargeUpTime - 1) is 0 or 8 or 16 or 24 or 32)
            {
                Vector2 randomPoint = Vector2.Zero;
                for (int i = 0; i < 100; i++)
                {
                    randomPoint = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16 * 20, 16 * 40);
                    if (!Collision.IsWorldPointSolid(randomPoint, true))
                        break;
                }
                Vector2 vel = CVUtils.FindVelocityForGravityAffectedThing(NPC.Center, randomPoint, 0.25f, 90);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.LengthClamp(240), ModContent.ProjectileType<IceBomb>(), 24, 3);
            }
            else if (NPC.ai[1] > chargeUpTime + bigLaserDelay && NPC.ai[1] < chargeUpTime + bigLaserDelay + 60)
            {
                Vector2 vect = Main.rand.NextVector2Unit() * Main.rand.NextFloat(128, 160);
                var p = VanillaParticles.RequestFadingParticle();
                p.ColorTint = Color.LightCyan with { A = 0 };
                float time = Main.rand.NextFloat(25, 45);
                p.SetTypeInfo(time);
                Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
                p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), (-vect / time) + NPC.velocity, vect + NPC.Center);
                p.FadeInNormalizedTime = 0.3f;
                p.FadeOutNormalizedTime = 1f;
                p.Scale = Vector2.One * 1.3f;
                p.ScaleVelocity = Vector2.One / -time;
                p.RotationVelocity = p.Velocity.X * 0.1f;
                p.RotationAcceleration = -p.RotationVelocity / time;
                //p.Rotation = Main.rand.NextFloatDirection();
                Main.ParticleSystem_World_BehindPlayers.Add(p);
            }
            if (NPC.ai[1] == chargeUpTime + bigLaserDelay + 60 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceDeathBeam>(), 60, 2, -1, NPC.whoAmI);
            }
        }
        // 100 is the time the laser is alive
        if (NPC.ai[1] > chargeUpTime + bigLaserDelay + 100 + 60)
            CycleAttack();
    }
}