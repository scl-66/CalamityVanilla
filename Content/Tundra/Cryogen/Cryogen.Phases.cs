using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Extensions;
using CalamityVanilla.Common.Particles;
using CalamityVanilla.Content.Tundra.Cryogen.Projectiles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
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
        UpdateAfterimages();
        _glowAnimation.Update();
        NPC.rotation += (NPC.velocity * new Vector2(0.01f, 0.005f)).Length() * Math.Sign(NPC.velocity.X);
        _snowOverlayOpacity *= 0.96f;
        Lighting.AddLight(NPC.Center, new Vector3(0.8f, 1f, 1f));

        if(_snowOverlayOpacity > 0 && NPC.velocity.Length() > 1)
        {
            int type = ModContent.DustType<CryonicSmokeDust>();
            for(int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, type);
                d.velocity += NPC.velocity * Main.rand.NextFloat();
                d.fadeIn = Main.rand.Next(2,5);
                d.scale = Main.rand.NextFloat(1.5f);
                d.noGravity = false;
                d.color = Color.White * _snowOverlayOpacity * Main.rand.NextFloat(2);
            }
        }

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
            NPC.localAI[3]++;
            NPC.TargetClosest(player => !player.ZoneSnow, false);
            if (NPC.localAI[3] >= 60 * 5)
            {
                NPC.netUpdate = true;
                _currentAttack = -1;
            }
        }
        ulong seed = Utils.RandomNextSeed((ulong)(NPC.whoAmI + _attacksCycled));
        bool predictableRand = Utils.RandomInt(ref seed, 2) == 0;

        NPC.rotation += 0.05f;
        //if (Main.timeForVisualEffects % 70 == 0)
        //    _glowAnimation = new(GlowAnimationType.Backwards);
        //return;

        //if (_currentAttack % 2 == 0)
        //    Fly();
        //else
        //    DeathLaser();
        //return;

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
                SnowflakesOrBombs(predictableRand);
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
                SnowflakesOrBombs(predictableRand);
                break;
            case 30:
                SnowflakesOrBombs(false);
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
        _attacksCycled++;
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
            _snowOverlayOpacity = Math.Max(_snowOverlayOpacity, Utils.Remap(NPC.ai[2], 40, 100, 0, 1));
            if (NPC.ai[2] < 100)
            {
                _snowOverlaySpinDirection = -NPC.direction;
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

        if (_glowAnimation.Type == GlowAnimationType.Nothing)
        {
            _glowAnimation = new GlowAnimation(Bombs ? GlowAnimationType.Forwards : GlowAnimationType.Backwards);
        }
        _glowColor = Bombs ? Color.White with { A = 0 } : Color.Cyan with { A = 0 };
        _glowColor *= Utils.Remap(NPC.ai[1], chargeUpTime, chargeUpTime + 64, 1, 0);
        if (NPC.ai[1] > 0 || NPC.velocity.Length() < 3f)
        {
            NPC.ai[1]++;
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
            else if (Bombs && (NPC.ai[1] - chargeUpTime - 1) is 0 or 8 or 16 or 24 or 32)
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
        int bombType = ModContent.ProjectileType<IceBomb>();
        foreach (Projectile p in Main.ActiveProjectiles)
        {
            if (p.type == bombType)
            {
                NPC.velocity *= 0.9f;
                return;
            }
        }
        float spinSpeed = 0.015f;
        int beamType = ModContent.ProjectileType<IcePlacingBeam>();
        Vector2 targetPos = new Vector2(NPC.ai[2], NPC.ai[3]);
        if (targetPos == Vector2.Zero)
        {
            NPC.netUpdate = true;
            Vector2 adjustedTargetPos = target.Center + (target.velocity / 16 * 60);
            if (target.velocity != Vector2.Zero)
            {
                adjustedTargetPos += Vector2.Normalize(target.velocity) * 15;
            }
            NPC.ai[2] = adjustedTargetPos.X;
            NPC.ai[3] = adjustedTargetPos.Y;

            int ringSize = 16 * (phase2 ? 18 : 23);

            NPC.direction = ringSize * (Main.rand.NextBool() ? 1 : -1);
            NPC.ai[1] = NPC.Center.DirectionTo(adjustedTargetPos + new Vector2(16 * 35)).ToRotation() / spinSpeed;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, target.Center, beamType, 15, 1, -1, NPC.whoAmI, ringSize, Main.rand.NextBool() ? 1 : -1);
        }
        else
        {
            NPC.localAI[2]++;
            NPC.SimpleFlyMovement(NPC.Center.DirectionTo(targetPos + new Vector2(NPC.direction + (64 * Math.Sign(NPC.direction))).RotatedBy(NPC.ai[1] * spinSpeed)) * 12, 0.2f);
            bool laserActive = false;
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if(p.type == beamType)
                {
                    NPC.velocity *= 0.5f;
                    laserActive = true;
                    break;
                }
            }
            if (laserActive)
            {
                if (_glowAnimation.Type == GlowAnimationType.Nothing)
                {
                    _glowAnimation = new(GlowAnimationType.Laser);
                    _glowColor = Color.White with { A = 0 } * 0.5f;
                }
            }
            else
            {
                _glowColor.R -= 2;
                _glowColor.G -= 2;
                _glowColor.B -= 2;
                if (_glowColor.R <= 5)
                    _glowAnimation.Type = GlowAnimationType.Nothing;
            }
            //float dist = NPC.Center.Distance(targetPos);
            //int abs = Math.Abs(NPC.direction);
            //if (dist > abs + 64)
            //{

                NPC.ai[1]++;
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[2] > 60 && (int)NPC.localAI[2] % (phase2? 20 : 30) == 0)
                {
                    NPC.localAI[1]++;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.Center.DirectionTo(targetPos).RotatedByRandom(0.5f) * 8, ModContent.ProjectileType<Icicles>(), 30, 2);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[2] > 30 && (int)NPC.localAI[2] % 70 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 4f), ModContent.ProjectileType<CryoFlake1>() + Main.rand.Next(3), 15, 1, -1, NPC.target);
                }
                if ((NPC.localAI[1] > 6 && ! phase2) || NPC.localAI[1] > 9)
                {
                    NPC.netUpdate = true;
                    CycleAttack();
                }
            //}
        }
    }
    private void IcicleDash()
    {
        if (NPC.ai[1] >= 40)
        {
            NPC.ai[1]++;
            _snowOverlayOpacity = Math.Max(_snowOverlayOpacity, Utils.Remap(NPC.ai[1], 40, 100, 0, 1));
            if (NPC.ai[1] < 100)
            {
                NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
                _snowOverlaySpinDirection = -NPC.direction;
                NPC.velocity *= 0.95f;
                NPC.rotation += NPC.direction * Utils.Remap(NPC.ai[1], 40, 100, 0, 0.3f);
            }
            else if (NPC.ai[1] == 100)
            {
                _glowAnimation = new(GlowAnimationType.Backwards);
                _glowColor = Color.White with { A = 0 };
                SoundEngine.PlaySound(SoundID.Item29, NPC.Center);
                NPC.velocity = NPC.Center.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(0.5f, 1f) * (Main.rand.NextBool() ? 1 : -1)) * 18;
                NPC.netUpdate = true;
            }
            else if (NPC.ai[1] == 140)
            {
                _glowAnimation = new(GlowAnimationType.Forwards);
                _glowColor = Color.White with { A = 0 };
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float rot = Main.rand.NextFloat(-0.1f, 0.1f);
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity, Vector2.UnitY.RotatedBy((i * MathHelper.TwoPi / 6f) + rot + (MathHelper.TwoPi / 12f)) * 12, ModContent.ProjectileType<Icicles>(), 30, 2);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity, Vector2.UnitY.RotatedBy((i * MathHelper.TwoPi / 6f) + rot) * 8, ModContent.ProjectileType<Icicles>(), 30, 2);
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
    SlotId _chargeSound;
    private void DeathLaser()
    {
        int chargeUpTime = 31;
        int bigLaserDelay = 130;
        int laserTelegraphTime = 100;
        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
        {
            NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
            NPC.directionY = Math.Sign(target.Center.Y - NPC.Center.Y);
            NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY) * 6, 0.1f);
            if (NPC.ai[1] > 0 && NPC.ai[1] < chargeUpTime + 64)
                NPC.ai[1]--;
            return;
        }
        NPC.velocity *= 0.98f;
        NPC.rotation += Utils.Remap(NPC.ai[1] - chargeUpTime - bigLaserDelay, 0, laserTelegraphTime, 0, 0.1f * NPC.direction);
        _snowOverlaySpinDirection = NPC.direction;
        _snowOverlayOpacity = Utils.Remap(NPC.ai[1] - chargeUpTime - bigLaserDelay, 0, laserTelegraphTime, 0, 1);

        if (NPC.ai[1] > 0 || NPC.velocity.Length() < 3f)
        {
            NPC.ai[1]++;
            if (NPC.ai[1] < chargeUpTime + 64)
            {
                if (_glowAnimation.Type == GlowAnimationType.Nothing)
                {
                    _glowAnimation = new GlowAnimation(GlowAnimationType.Forwards);
                }
                _glowColor = Color.White with { A = 0 };
                _glowColor *= Utils.Remap(NPC.ai[1], chargeUpTime, chargeUpTime + 64, 1, 0);
            }
            else if (NPC.ai[1] == chargeUpTime + 64)
            {
                _chargeSound = SoundEngine.PlaySound(Assets.Sounds.CryogenDeathLaserCharge.Asset, NPC.Center);
                _glowAnimation = new GlowAnimation(GlowAnimationType.Laser);
            }
            else if (NPC.ai[1] > chargeUpTime + 64)
            {
                _glowColor = GetAuroraColor((int)(Main.timeForVisualEffects * 3)) with { A = 0 };
                if (NPC.ai[1] < chargeUpTime + bigLaserDelay)
                    _glowColor *= Utils.Remap(NPC.ai[1], chargeUpTime + 64, chargeUpTime + bigLaserDelay, 0, 1);
                else
                    _glowColor *= Utils.Remap(NPC.ai[1], chargeUpTime + bigLaserDelay + 100 + laserTelegraphTime / 2, chargeUpTime + bigLaserDelay + 100 + laserTelegraphTime, 1, 0);
            }
        }
        if (NPC.ai[1] > chargeUpTime)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && (NPC.ai[1] - chargeUpTime - 1) is 0 or 8 or 16 or 24 or 32)
            {
                Vector2 randomPoint = Vector2.Zero;
                for (int i = 0; i < 100; i++)
                {
                    randomPoint = NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(16 * 20, 16 * 40)) * new Vector2(1, 0.75f);
                    randomPoint.X += ((NPC.ai[1] - chargeUpTime - 1) - 16) * 20;
                    if (!Collision.IsWorldPointSolid(randomPoint, true) && randomPoint.Distance(NPC.Center) > 16 * 30)
                        break;
                }
                Vector2 vel = CVUtils.FindVelocityForGravityAffectedThing(NPC.Center, randomPoint, 0.25f, 90);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.LengthClamp(240), ModContent.ProjectileType<IceBomb>(), 24, 3);
            }
            else if (NPC.ai[1] > chargeUpTime + 64 && NPC.ai[1] < chargeUpTime + bigLaserDelay + laserTelegraphTime + 100)
            {
                for (int i = 0; i < 2; i++)
                {
                    float multi = Main.rand.NextFloat(32, 220);
                    Vector2 vect = Main.rand.NextVector2Unit() * multi;
                    var p = VanillaParticles.RequestFadingParticle();
                    p.ColorTint = _glowColor * Utils.Remap(multi, 100, 220, 1, 0.25f);
                    float time = Main.rand.NextFloat(15, 23);
                    p.SetTypeInfo(time);
                    Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
                    p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), (-vect / time), vect.RotatedBy(MathHelper.PiOver2 * -_snowOverlaySpinDirection) + NPC.Center);
                    p.AccelerationPerFrame = (p.Velocity.RotatedBy(MathHelper.PiOver2) / time) * -_snowOverlaySpinDirection;
                    p.FadeInNormalizedTime = 0.5f;
                    p.FadeOutNormalizedTime = 1f;
                    p.Scale = Vector2.One * Main.rand.NextFloat(0.8f, 1.5f);
                    p.ScaleVelocity = Vector2.One / -time;
                    p.RotationVelocity = p.Velocity.X * 0.1f;
                    p.RotationAcceleration = -p.RotationVelocity / time;
                    Main.ParticleSystem_World_BehindPlayers.Add(p);
                }
            }
            if (NPC.ai[1] == chargeUpTime + bigLaserDelay + laserTelegraphTime)
            {
                if(SoundEngine.TryGetActiveSound(_chargeSound, out ActiveSound sound) && sound != null && sound.IsPlaying)
                {
                    sound.Stop();
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceDeathBeam>(), 60, 2, -1, NPC.whoAmI);
            }
        }
        // 100 is the time the laser is alive
        if (NPC.ai[1] > chargeUpTime + bigLaserDelay + 100 + laserTelegraphTime)
        {
            _glowAnimation.Type = GlowAnimationType.Nothing;
            CycleAttack();
        }
    }
}