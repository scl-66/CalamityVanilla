using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 480);
        Projectile.tileCollide = false;
        Projectile.frame = Main.rand.Next(3);
        Projectile.scale = 0.1f;
    }
    public override void AI()
    {
        Player target = Main.player[(int)Projectile.ai[0]];
        Projectile.ai[1]++;
        if(Projectile.scale < 1f)
            Projectile.scale += 0.1f;

        float PrepareTime = 120;
        if (Projectile.ai[1] == 1)
        {
            SoundEngine.PlaySound(_spawn, Projectile.position);
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
                            d.scale *= Main.rand.NextFloat(1f,1.25f);
                            d.noGravity |= Main.rand.NextBool();
                        }
                    }
                }
            }
        }
        if (Projectile.ai[1] < PrepareTime)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
            d.color = Color.Purple;
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(2);
            d.velocity += Projectile.velocity;

            Projectile.localAI[0] += 0.004f * MathF.Sign(target.Center.X - Projectile.Center.X);
            Projectile.localAI[0] = MathHelper.Clamp(Projectile.localAI[0], -0.15f, 0.15f);
            Projectile.velocity.Y *= 0.98f;
            Projectile.velocity.X *= 0.99f;

            if (Projectile.ai[1] > PrepareTime - 31 && (Projectile.ai[1] - 1) % 15 == 0)
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
        else if (Projectile.ai[1] == PrepareTime)
        {
            SoundEngine.PlaySound(SoundID.Item69, Projectile.position);
            int time = 40;
            Projectile.velocity = CVUtils.FindVelocityForGravityAffectedThing(Projectile.Bottom,target.Top + (target.velocity * time),0.1f, time).LengthClamp(24,4);
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
            if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            Projectile.velocity.Y += 0.1f;
        }
        Projectile.rotation += Projectile.localAI[0];
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = tex.Frame(3, 5, Projectile.frame, (int)Projectile.ai[2]);

        if (Projectile.ai[1] >= 120)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float opacity = (1f - (i / (float)Projectile.oldPos.Length)) * Projectile.Opacity * 0.5f;
                Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, lightColor * opacity, Projectile.oldRot[i], frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
        }
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        if (Projectile.ai[1] < 120)
        {
            for(int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0,8 + (float)Math.Sin(Main.timeForVisualEffects * 0.15f) * 4).RotatedBy(i * MathHelper.PiOver2 + Projectile.rotation), frame, Color.Purple with { A = 0 } * Projectile.Opacity * 0.5f, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
        }
        return false;
    }
    public override void OnKill(int timeLeft)
    {
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
