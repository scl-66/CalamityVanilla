using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Projectiles;

public class HiveVineSpawner : ModProjectile
{
    private static SoundStyle _sound;
    public override void SetStaticDefaults()
    {
        _sound = new SoundStyle(Mod.Name + "/Assets/Sounds/HiveMind_VineEmerge", [1, 2]) { PitchVariance = 0.1f, MaxInstances = 10 };
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 28);
        Projectile.tileCollide = false;
        Projectile.Opacity = 0;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value;
        Vector2 scale = new Vector2(1f, 2f) * (0.9f + (MathF.Sin(Projectile.timeLeft * 0.1f + Projectile.identity * 7) * 0.1f));
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(0.25f, 0.5f, 0f, 0.5f) * Projectile.Opacity, MathHelper.PiOver2, tex.Size() / 2, scale * new Vector2(scale.Y, 1f), SpriteEffects.None);
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(0.4f, 0.4f, 0.4f, 0f) * Projectile.Opacity, MathHelper.PiOver2, tex.Size() / 2, scale * new Vector2(1f, 0.6f), SpriteEffects.None);

        tex = TextureAssets.Extra[ExtrasID.PortalGateHalo2].Value;
        scale.X *= Math.Min(1, Projectile.timeLeft * 0.1f);
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height / 2), new Color(0.25f, 0.5f, 0f, 0f) * Projectile.Opacity * 0.5f, 0, tex.Size() / 2, new Vector2(scale.X, 4 * Projectile.Opacity), SpriteEffects.None);
        return false;
    }
    public override void AI()
    {
        Projectile.Opacity += 0.035f;
        if (Main.rand.NextBool(3))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-25, 25), 0), DustID.RainbowMk2, Vector2.UnitY * Main.rand.NextFloat(-24, -3));
            d.color = new Color(0.25f, 0.5f, 0f, 0.8f) * Projectile.Opacity;
            d.noGravity = true;
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 120)
            Projectile.Kill();
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(_sound, Projectile.position);
        Projectile.Opacity += 0.075f;
        for (int i = 0; i < 20; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-25, 25), 0), DustID.RainbowMk2, Main.rand.NextVector2Circular(6, 5));
            d.color = new Color(0.25f, 0.5f, 0f, 0.8f) * Projectile.Opacity;
            d.noGravity = true;
            d.scale += Main.rand.NextFloat();
            d.fadeIn = Main.rand.NextFloat(2);
        }

        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-25, 25), 0), DustID.Corruption);
            d.velocity += Vector2.UnitY * Main.rand.NextFloat(-5, -2);
            d.noGravity = Main.rand.NextBool();
            d.alpha = 128;
        }
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-25, 25), 0), DustID.CorruptGibs);
            d.velocity += Vector2.UnitY * Main.rand.NextFloat(-12, -2);
            d.fadeIn = Main.rand.NextFloat(1.5f);
            d.noGravity = true;
        }
        Point center = Projectile.Center.ToTileCoordinates();
        for (int x = center.X - 1; x <= center.X + 1; x++)
        {
            for (int y = center.Y - 2; y <= center.Y + 2; y++)
            {
                if (Main.tile[x, y].HasTile && (Main.tileSolid[Main.tile[x, y].TileType] || Main.tileSolidTop[Main.tile[x, y].TileType]))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Dust d = Main.dust[WorldGen.KillTile_MakeTileDust(x, y, Main.tile[x, y])];
                        d.velocity.Y -= Main.rand.NextFloat(2, 8);
                        d.velocity.X *= 2;
                        d.scale += Main.rand.NextFloat(0.5f);
                        d.noGravity = Main.rand.NextBool();
                    }
                }
            }
        }

        if (Projectile.owner != Main.myPlayer)
            return;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, 4), Vector2.Zero, ModContent.ProjectileType<HiveVine>(), Projectile.damage, 1, -1, 0, Projectile.ai[1]);
    }
    public override string Texture => ModContent.GetInstance<HiveVine>().Texture;
    public override bool? CanDamage()
    {
        return false;
    }
}
public class HiveVine : ModProjectile
{
    private static SoundStyle _death;
    private static Asset<Texture2D> _platform;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        _platform = ModContent.Request<Texture2D>(Texture + "Platform");
        _death = new SoundStyle(Mod.Name + "/Assets/Sounds/HiveMind_VineDestroy", [1, 2]) { PitchVariance = 0.1f, MaxInstances = 10 };
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 32);
        Projectile.height = 0;
        Projectile.hide = true;
        Projectile.tileCollide = false;
        Projectile.frame = Main.rand.Next(2);
        Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        int adjustedHeight = (Projectile.height / 2) * 2;
        Rectangle rect = new Rectangle(0, 0, 42, Math.Min(adjustedHeight, 54));

        Color colorTint = Color.White;
        Vector2 jitter = Vector2.Zero;
        ulong randSeed = Main.TileFrameSeed ^ (ulong)(((long)Projectile.position.X << 32) | (uint)Projectile.position.Y);
        if (Projectile.ai[0] > 200)
        {
            jitter.X = Utils.RandomInt(ref randSeed, -20, 21) * 0.1f;
            jitter.Y = Utils.RandomInt(ref randSeed, -20, 21) * 0.1f;
            colorTint = Color.Lerp(Color.White, new Color(0.8f, 0.7f, 1f), (Projectile.ai[0] - 200) / 40f);
        }
        Main.EntitySpriteDraw(tex.Value, Projectile.Top - Main.screenPosition + jitter, rect, new Color(Lighting.GetSubLight(Projectile.Top + new Vector2(rect.Height / 2))).MultiplyRGB(colorTint) * Projectile.Opacity, 0, new Vector2(rect.Width / 2, 0), 1, SpriteEffects.None);
        for (int i = 1; i < Math.Ceiling((adjustedHeight - 22) / 32f); i++)
        {
            if (Projectile.ai[0] > 200)
            {
                jitter.X = Utils.RandomInt(ref randSeed, -20, 21) * 0.1f;
            }

            rect = new Rectangle(0, i % 2 == 0 ? 56 : 90, 42, Math.Min((adjustedHeight - 18) - (i * 32), 32));
            Vector2 drawPos = Projectile.Top + new Vector2(0, (i * 32) + 22);
            Main.EntitySpriteDraw(tex.Value, drawPos - Main.screenPosition + jitter, rect, new Color(Lighting.GetSubLight(drawPos + new Vector2(rect.Height / 2))).MultiplyRGB(colorTint) * Projectile.Opacity, 0, new Vector2(rect.Width / 2, 0), 1, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(_platform.Value, Projectile.Bottom - Main.screenPosition, _platform.Frame(1, 2, 0, Projectile.frame), new Color(Lighting.GetSubLight(Projectile.Bottom)) * Projectile.Opacity, 0, new Vector2(_platform.Width() / 2, 4), 1, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(_death, Projectile.position);
        for (int i = 0; i < Projectile.height / 8; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 0.4f;
        }
        for (int i = 0; i < Projectile.height / 2; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptionThorns);
        }
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Bottom + new Vector2(Main.rand.NextFloat(-24, 24), Main.rand.NextFloat(16)), DustID.Dirt);
            Dust d2 = Dust.NewDustPerfect(Projectile.Bottom + new Vector2(Main.rand.NextFloat(-24, 24), Main.rand.NextFloat(16)), DustID.Corruption);
        }
    }
    public override void AI()
    {
        for (int i = 0; i < Projectile.height / 32; i++)
        {
            if (Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.alpha = 128;
                d.velocity *= 0.4f;
            }
        }
        if (Projectile.height > Projectile.ai[1] * 32)
        {
            if (Projectile.ai[0] <= 0)
            {
                int howHighAbove = -9999;
                foreach (var p in Main.player)
                {
                    if (Math.Abs(p.Top.X - Projectile.Top.X) < 16 * 15)
                    {
                        howHighAbove = (int)Math.Max(howHighAbove, Projectile.Top.Y - p.Top.Y);
                        if (howHighAbove > -16 * 15)
                        {
                            Projectile.ai[0]--;
                            break;
                        }
                    }
                }
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 240)
            {
                Projectile.Kill();
            }
        }
        if (Projectile.ai[0] == 0)
        {
            int speed = (int)Utils.Remap(Projectile.timeLeft, 3600 - 30, 3600, 4, 48);
            //int speed = 1;
            Projectile.height += speed;
            Projectile.position.Y -= speed;
        }
    }
}
/*
public class HiveVine : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 32);
        Projectile.alpha = 255;
        Projectile.timeLeft = 60 * 3;
        Projectile.tileCollide = false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        Rectangle rect = Projectile.frame == 0 ? new Rectangle(0, 0, 42, 50) : new Rectangle(0, 52 + 32 * (Projectile.frame - 1), 42, 32);
        Main.EntitySpriteDraw(tex.Value, Projectile.Bottom - Main.screenPosition, rect, lightColor * Projectile.Opacity, 0, new Vector2(rect.Width / 2, rect.Height), 1, SpriteEffects.None);

        return false;
    }
    public override void AI()
    {
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 15;
        }

        if (Projectile.timeLeft <= 17)
        {
            Projectile.alpha += 30;
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.alpha = 128;
                d.velocity *= 0.4f;
            }
        }

        if (Main.rand.NextBool(15))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 0.4f;
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] == 5 && Projectile.ai[1] > 0)
        {
            Projectile.frame = Projectile.ai[1] % 2 == 0 ? 1 : 2;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Projectile.ai[1] == 1)
                {
                    int howHighAbove = -9999;
                    foreach (var p in Main.player)
                    {
                        if (Math.Abs(p.Center.X - Projectile.Center.X) < 16 * 15)
                        {
                            howHighAbove = (int)Math.Max(howHighAbove, Projectile.Center.Y - p.Center.Y);
                            if (howHighAbove > -16 * 5)
                            {
                                Projectile.ai[1]++;
                                break;
                            }
                        }
                    }
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, -30), Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Projectile.ai[1] - 1);
            }
        }
    }
}
 */