using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Projectiles;

public class CorruptBoulder : ModProjectile
{
    private static SoundStyle _death;
    private static SoundStyle _spawn;
    public override void SetStaticDefaults()
    {
        _death = new SoundStyle(Mod.Name + "/Assets/Sounds/HiveMind_RockDestroy", [1,2]) { PitchVariance = 0.1f, MaxInstances = 10 };
        _spawn = new SoundStyle(Mod.Name + "/Assets/Sounds/HiveMind_RockEmerge", [1, 2]) { PitchVariance = 0.1f, MaxInstances = 10 };
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        if (Projectile.hide)
            behindNPCsAndTiles.Add(index);
    }
    public override void SetDefaults()
    {
        Projectile.scale = 0.1f;
        Projectile.QuickDefaults(true, (int)(48f / 0.1f));
        Projectile.tileCollide = false;
        Projectile.frame = Main.rand.Next(3);
        Projectile.hide = true;
    }
    public override void AI()
    {
        Player target = Main.player[(int)Projectile.ai[0]];
        Projectile.ai[1]++;
        if(Projectile.scale < 1.2f)
            Projectile.scale += 0.06f;

        float LaunchTime = 240;
        if (Projectile.ai[1] == 1)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.position);
        }
        if (Projectile.ai[1] < LaunchTime)
        {
            Projectile.velocity.Y *= 0.98f;
            Projectile.velocity.X *= 0.99f;
            if (Projectile.ai[1] < 60)
            {
                //if (Main.rand.NextBool(3))
                //{
                //    var p2 = VanillaParticles.RequestRandomizedFrameParticle();
                //    int tex = ProjectileID.ScytheWhipProj;
                //    Main.instance.LoadProjectile(tex);
                //    p2.SetBasicInfo(TextureAssets.Projectile[tex], null, Vector2.Zero, Projectile.Top + Main.rand.NextVector2CircularEdge(32, 32));
                //    p2.SetTypeInfo(Main.projFrames[tex], 2, 24f);
                //    p2.Velocity = p2.LocalPosition.DirectionFrom(Projectile.Center) * 3;
                //    p2.Rotation = p2.Velocity.ToRotation();
                //    p2.Scale = new Vector2(0.5f);
                //    p2.FadeInNormalizedTime = 0.01f;
                //    p2.FadeOutNormalizedTime = 0.5f;
                //    p2.ScaleVelocity = new Vector2(0.025f);
                //    p2.ColorTint = Color.Purple with { A = 128 };
                //    Main.ParticleSystem_World_OverPlayers.Add(p2);
                //}

                Projectile.position.Y -= 0.5f;
                Projectile.rotation += MathF.Sin((Projectile.ai[1] * 0.5f) + (Projectile.identity * 7) * 0.1f) * 0.1f;
                if(Main.rand.NextBool(15))
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            }
            else if (Projectile.ai[1] == 61)
            {
                SoundEngine.PlaySound(_spawn, Projectile.position);
                Projectile.velocity = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-6, -3));
                Projectile.netUpdate = true;
                Projectile.hide = false;
                Point center = Projectile.Center.ToTileCoordinates();
                for (int x = center.X - 3; x <= center.X + 3; x++)
                {
                    for (int y = center.Y - 3; y <= center.Y + 3; y++)
                    {
                        if (Main.tile[x, y].HasTile && (Main.tileSolid[Main.tile[x, y].TileType] || Main.tileSolidTop[Main.tile[x, y].TileType]))
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Dust d = Main.dust[WorldGen.KillTile_MakeTileDust(x, y, Main.tile[x, y])];
                                d.velocity.Y -= Main.rand.NextFloat(1, 4);
                                d.velocity.X *= 2;
                                d.scale *= Main.rand.NextFloat(1f, 1.25f);
                                d.noGravity |= Main.rand.NextBool();
                            }
                        }
                    }
                }
            }
            else
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
                d.color = Color.Purple * Utils.Remap(Projectile.ai[1], 30, 60, 0, 1);
                d.noGravity = true;
                d.velocity += Projectile.velocity * Main.rand.NextFloat();

                Projectile.localAI[0] += 0.003f * MathF.Sign(target.Center.X - Projectile.Center.X);
                Projectile.localAI[0] = MathHelper.Clamp(Projectile.localAI[0], -0.15f, 0.15f);
            }
            if ((Projectile.ai[1] > LaunchTime - 31 || Projectile.ai[1] < 60) && (Projectile.ai[1] - 1) % 15 == 0)
            {
                var p = VanillaParticles.RequestFadingParticle();
                p.SetBasicInfo(TextureAssets.Extra[ExtrasID.KeybrandRing], null, Vector2.Zero, Projectile.Center);
                p.SetTypeInfo(30);
                p.ColorTint = Color.Purple with { A = 64 } * 0.5f;
                p.Velocity = Projectile.velocity;
                //p.Scale = Vector2.Zero;
                //p.ScaleVelocity = new Vector2(Main.rand.NextFloat(0.8f, 1f), Main.rand.NextFloat(0.8f, 1f)) * 0.075f;
                p.Scale = new Vector2(Main.rand.NextFloat(0.8f, 1f), Main.rand.NextFloat(0.8f, 1f)) * 1.5f;
                p.ScaleVelocity = p.Scale / -40;
                p.Rotation = Main.rand.NextFloatDirection();
                p.FadeInNormalizedTime = 0.5f;
                p.FadeOutNormalizedTime = 0.5f;
                Main.ParticleSystem_World_OverPlayers.Add(p);
            }
        }
        else if (Projectile.ai[1] == LaunchTime)
        {
            SoundEngine.PlaySound(SoundID.Item69, Projectile.position);
            int time = 40;
            Vector2 adjustedTargetPosition = target.Top + new Vector2(target.velocity.X * time, 0);
            Projectile.velocity = CVUtils.FindVelocityForGravityAffectedThing(Projectile.Bottom, adjustedTargetPosition, 0.2f, time).LengthClamp(32);
            for(int i = 0; i < 25; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
                d.color = Color.Purple;
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(2);
                d.velocity += Projectile.velocity * Main.rand.NextFloat();
            }
        }
        else
        {
            if (Projectile.velocity.Y > 0 && !Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            Projectile.velocity.Y += 0.2f;
        }
        Projectile.rotation += Projectile.localAI[0];
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = tex.Frame(3, 5, Projectile.frame, (int)Projectile.ai[2]);
        Vector2 jitter = Vector2.Zero;
        if (Projectile.ai[1] < 60)
        {
            var randSeed = Main.TileFrameSeed;
            jitter = new Vector2(Utils.RandomInt(ref randSeed, -20, 21) * 0.2f, Utils.RandomInt(ref randSeed, -20, 21) * 0.2f);
        }
        if (Projectile.ai[1] >= 240)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float opacity = (1f - (i / (float)Projectile.oldPos.Length)) * Projectile.Opacity * 0.5f;
                Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, lightColor * opacity, Projectile.oldRot[i], frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + jitter, frame, lightColor * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        if (Projectile.ai[1] < 240)
        {
            //for(int i = 0; i < 4; i++)
            //{
            //    Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0,8 + (float)Math.Sin(Main.timeForVisualEffects * 0.1f) * 4).RotatedBy(i * MathHelper.PiOver2), frame, Color.Purple with { A = 0 } * Projectile.Opacity * 0.5f, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            //}
            float colorMultiply = Projectile.Opacity * Utils.Remap(Projectile.ai[1],30,120,0,2);
            float interval = 60;
            float amount = (float)(Main.timeForVisualEffects % interval) / interval;
            Color c = Color.Purple with { A = 0 } * amount * colorMultiply * (1f - amount);
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, 16 * amount).RotatedBy((i * MathHelper.PiOver2) + MathHelper.PiOver4), frame, c, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            amount = (float)((Main.timeForVisualEffects + (interval / 2)) % interval) / interval;
            c = Color.Purple with { A = 0 } * amount * colorMultiply * (1f - amount);
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, 16 * amount).RotatedBy(i * MathHelper.PiOver2), frame, c, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
        }
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        if (Main.expertMode && Main.rand.NextBool(4))
        {
            int[] Types = [NPCID.LittleEater, NPCID.EaterofSouls, NPCID.BigEater, NPCID.Slimer,NPCID.CorruptSlime];
            NPC n = NPC.NewNPCDirect(Projectile.GetSource_FromThis(), Projectile.Center, Types[Main.rand.Next(Types.Length)]);
            n.velocity = -Projectile.oldVelocity.RotatedByRandom(1) * Main.rand.NextFloat(0.25f,0.5f);
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
        }
        SoundEngine.PlaySound(_death, Projectile.position);
        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 3;
        }
        Point center = Projectile.Center.ToTileCoordinates();
        for (int x = center.X - 3; x <= center.X + 3; x++)
        {
            for (int y = center.Y - 3; y <= center.Y + 3; y++)
            {
                if (Main.tile[x, y].HasTile && (Main.tileSolid[Main.tile[x, y].TileType] || Main.tileSolidTop[Main.tile[x, y].TileType]))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust d = Main.dust[WorldGen.KillTile_MakeTileDust(x, y, Main.tile[x, y])];
                        d.velocity = Vector2.Normalize(-Projectile.velocity).RotatedByRandom(1f) * Main.rand.NextFloat(2, 6);
                    }
                }
            }
        }
    }
}
