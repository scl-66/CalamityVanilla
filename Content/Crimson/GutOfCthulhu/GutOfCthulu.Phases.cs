using CalamityVanilla.Common.Extensions;
using CalamityVanilla.Common.Interfaces;
using CalamityVanilla.Content.Crimson.GutOfCthulhu.Projectiles;
using CalamityVanilla.Content.Crimson.GutOfCthulhu.Worms;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.GutOfCthulhu;

public partial class GutOfCthulhu : ModNPC, ICustomTrailingMode
{
    private ref float _currentAttack => ref NPC.ai[0];
    private int _wormsSummoned = 0;
    public override void AI()
    {
        if (!NPC.HasValidTarget || !target.ZoneCrimson)
        {
            NPC.TargetClosest(player => !player.ZoneCrimson, false);
        }

        if(NPC.justHit && Main.netMode != NetmodeID.MultiplayerClient)
        {
            var seed = Utils.RandomNextSeed((ulong)NPC.whoAmI);
            int ingestoid = ModContent.NPCType<Ingestoid>();
            int[] worms = {ingestoid,ingestoid,ingestoid};
            int randomSlot = Utils.RandomInt(ref seed, 3);
            worms[randomSlot] = ModContent.NPCType<Hematode>();
            worms[(randomSlot + Utils.RandomInt(ref seed, 1,3)) % 3] = ModContent.NPCType<Malarasite>();

            int threshold = (int)(((NPC.lifeMax - NPC.life) / (float)NPC.lifeMax) * 6);
            while(_wormsSummoned < threshold)
            {
                _wormsSummoned++;
                Vector2 spawnPos = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)spawnPos.X, (int)spawnPos.Y, worms[_wormsSummoned % 3], NPC.whoAmI);
            }
        }

        NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.1f, -0.2f, 0.2f);
        switch (_currentAttack)
        {
            case 0:
                if (!NPC.HasValidTarget || !target.ZoneCrimson)
                {
                    NPC.target = -1;
                    NPC.velocity.Y -= 0.2f;
                    NPC.EncourageDespawn(60);
                }
                else
                    Fly();
                break;
            case 1:
                Blood();
                break;
            case 2:
                Slam();
                break;
            case 3:
                Ichor();
                break;
        }
    }
    private Vector2 TargetPos()
    {
        return target.Center + new Vector2(0, -250);
    }
    private void Cycle()
    {
        NPC.ai[1] = 0;
        NPC.ai[2] = 0;
        NPC.ai[3] = 0;

        if (_currentAttack != 0)
            _currentAttack = 0;
        else
            _currentAttack = Main.rand.Next(2, 3);
        NPC.netUpdate = true;
    }
    private void Fly()
    {
        Vector2 targetPos = TargetPos();
        float speed = 1.5f;
        float acceleration = 0.02f;
        float distance = NPC.Center.Distance(targetPos);

        if (distance > 900)
        {
            speed = 32;
            acceleration = 0.08f;
            NPC.direction = Math.Sign(targetPos.X - NPC.Center.X);
            NPC.directionY = Math.Sign(targetPos.Y - NPC.Center.Y);
            NPC.ai[2] -= 0.75f;
        }
        else
        {
            if (NPC.ai[1] % 60 == 0)
            {
                NPC.PointDirectionToVector(targetPos);
            }
            if (NPC.ai[2] > 60 * 5)
            {
                Cycle();
                return;
            }
        }
        NPC.ai[1]++;
        NPC.ai[2]++;
        NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY * 0.75f) * speed, acceleration);
    }
    private void Blood()
    {
        Vector2 targetPos = TargetPos();
        NPC.ai[3]++;
        if (NPC.ai[1] == 0)
        {
            NPC.direction = Math.Sign(target.Center.X - NPC.Center.X);
            NPC.ai[2] = MathHelper.Lerp(targetPos.X, NPC.Center.X, 0.5f);
            NPC.ai[1]++;
        }
        if (NPC.ai[3] % 20 == 0)
            NPC.directionY = Math.Sign(targetPos.Y - NPC.Center.Y);
        int halfAreaWidth = 600;
        if (NPC.ai[1] == 1)
        {
            NPC.SimpleFlyMovement(new Vector2(-NPC.direction, NPC.directionY * 0.5f) * 10, 0.1f);
            if (Math.Abs(NPC.Center.X - NPC.ai[2]) > halfAreaWidth)
                NPC.ai[1]++;
        }
        else
        {
            if (NPC.ai[1] == 2)
            {
                NPC.velocity.X = NPC.direction * 10;
                SoundEngine.PlaySound(SoundID.NPCDeath10 with { pitch = 0.33f }, NPC.Center);
                for (int i = 0; i < 35; i++)
                {
                    Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                    d.velocity.X += NPC.direction * Main.rand.NextFloat(6, 18);
                    d.scale++;
                    d.alpha = 128;
                    d.frame.X = 150;
                    d.frame.Y = Main.rand.Next(3, 6) * 10;
                }
            }
            NPC.ai[1]++;
            _afterImageActive = true;
            _afterImageOpacity = 1;
            NPC.SimpleFlyMovement(new Vector2(NPC.velocity.X, NPC.directionY * 3), 0.2f);
            NPC.velocity.X *= 0.998f;
            if (NPC.ai[3] % (Main.expertMode ? 15 : 25) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<GutBlood>();
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(-94, -70) + NPC.velocity, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f) + NPC.direction, Main.rand.NextFloat(-12, -10)), type, 24, 1);
            }

            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                d.velocity += NPC.velocity;
                d.alpha = 128;
                d.frame.X = 150;
                d.scale += 0.5f;
                d.frame.Y = Main.rand.Next(3, 6) * 10;
            }

            if (Math.Abs(NPC.Center.X - NPC.ai[2]) > halfAreaWidth * 1.5f)
            {
                Cycle();
                return;
            }
        }
    }
    private void Ichor()
    {

    }
    private void Slam()
    {
        Vector2 targetPos = TargetPos();
        float speed = 6;
        float acceleration = 0.04f;
        float distance = NPC.Center.Distance(targetPos);
        NPC.ai[1]++;
        if (NPC.ai[2] == 0)
        {
            if (distance > 900)
            {
                speed = 32;
                acceleration = 0.08f;
                NPC.direction = Math.Sign(targetPos.X - NPC.Center.X);
                NPC.directionY = Math.Sign(targetPos.Y - NPC.Center.Y);
            }
            else
            {
                if (NPC.ai[1] % 20 == 0)
                {
                    NPC.PointDirectionToVector(targetPos);
                }
                if (Math.Abs(NPC.Center.X - target.Center.X) < 150)
                {
                    NPC.ai[2]++;
                    NPC.ai[1] = 0;
                }
            }
        }
        else if (NPC.ai[2] == 1)
        {
            NPC.velocity.X *= 0.9f;
            float ySpeed = Utils.Remap(NPC.ai[1], 0, 120, -0.75f, 2);
            NPC.velocity.Y += ySpeed;
            if (NPC.velocity.Y > 32)
                NPC.velocity.Y = 32;
            if (ySpeed > 0 && NPC.velocity.Y > 0)
            {
                if (_afterImageActive == false)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath10 with { pitch = 0.33f }, NPC.Center);
                    for (int i = 0; i < 35; i++)
                    {
                        Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                        d.velocity *= Main.rand.NextFloat(2, 5);
                        d.scale++;
                        d.alpha = 128;
                        d.frame.X = 150;
                        d.frame.Y = Main.rand.Next(3, 6) * 10;
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                    d.velocity += NPC.velocity;
                    d.alpha = 128;
                    d.frame.X = 150;
                    d.scale += 0.5f;
                    d.frame.Y = Main.rand.Next(3, 6) * 10;
                }
                _afterImageActive = true;
                _afterImageOpacity = 1;
                if (Collision.SolidCollision(NPC.position + NPC.velocity + (Vector2.UnitX * NPC.width / 4), NPC.width / 4, 180))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<GutShockwave>();
                        for (int i = -1; i < 2; i += 2)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), CVUtils.FindNearbySpotOnGround(NPC.Center + Vector2.UnitX * 90 * i, 32) - (Vector2.UnitY * ContentSamples.ProjectilesByType[type].height / 2), Vector2.Zero, type, 30, 1, -1, 10 * i);
                    }
                    NPC.ai[2]++;
                    NPC.ai[1] = 0;
                }
            }
        }
        else
        {
            if (NPC.ai[1] == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.position);
            }
            NPC.velocity = Vector2.Zero;
            NPC.rotation = MathF.Sin(NPC.ai[1] * 0.5f) * NPC.ai[1] / 300;
            if (NPC.ai[1] > 60)
            {
                NPC.velocity.Y = -5;
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.position);
                Cycle();
            }
            return;
        }
        NPC.SimpleFlyMovement(new Vector2(NPC.direction, NPC.directionY * 0.75f) * speed, acceleration);
    }
}
