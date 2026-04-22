using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TheGothic;
using CalamityVanilla.Content.Tundra.Items.Frostbolt;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Space.Items.FeatherDicer;

public class FeatherDicer : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<DicerFeather>(), 20, 38, 8);
        Item.UseSound = null;

        // Set damage and knockBack
        Item.SetWeaponValues(18, 2);

        // Set rarity and value
        Item.SetShopValues(ItemRarityColor.Green2, 2000);
        Item.useTime = 6;
        Item.useLimitPerAnimation = 3;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3) * Main.rand.NextFloat(1f, 1.3f), type, damage, knockback, ai1: Main.rand.Next(0, 200));
        return false;
    }
}

public class DicerFeather : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Projectile.type] = 8; 
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 280;
        Projectile.penetrate = 3;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        Projectile.ai[0] += 1f; // Use a timer to wait 15 ticks before applying gravity.
        int timeToStartFalling = 60;
        float sinMult = Utils.Remap(Projectile.ai[0], timeToStartFalling, timeToStartFalling + 120, 0, 1);
        float siner = MathF.Sin((Projectile.ai[0] + Projectile.ai[1]) * MathHelper.TwoPi / 60f) * sinMult;
        if (Projectile.ai[0] == 1)
        {
            SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 10, Pitch = 0.5f, PitchVariance = 0.2f }, Projectile.position);
            Projectile.spriteDirection = Projectile.direction;
        }
        if (Projectile.ai[0] < timeToStartFalling)
        {
            Projectile.velocity *= 0.96f;
            Projectile.frame = 0;
            Projectile.rotation = Projectile.velocity.ToRotation(); // projectile sprite faces up
        }
        else 
        {
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
            Projectile.velocity.X *= 0.99f;
            Projectile.velocity.X += siner / 5f;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, (siner / 2) + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0), sinMult / 2);
            if (Projectile.velocity.Y > 4f)
            {
                Projectile.velocity.Y = 4f;
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Harpy);
            d.velocity = Main.rand.NextVector2Circular(1, 1);
            d.noGravity = !Main.rand.NextBool(5);
        }
        if (Projectile.ai[0] < 75)
        {
            int type = ModContent.DustType<SimpleColorableGlowyDust>();
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
                d.color = DicerFeatherVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length);
                d.noLight = true;
                d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
                d.velocity *= 0.2f;
                d.scale *= 0.75f;
                d.noGravity = true;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(DicerFeatherVertexStrip).Draw(Projectile, Utils.Remap(Projectile.ai[0], 75, 95, 1, 0));

        Rectangle frameBounds = TextureAssets.Projectile[Type].Frame(1, 8, 0, Projectile.frame);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frameBounds, new Color(Projectile.Opacity, Projectile.Opacity * 2, 1f, 0.7f) * Projectile.Opacity * 2, Projectile.rotation, frameBounds.Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(TextureAssets.MagicPixel);
        shader.UseImage0(TextureAssets.MagicPixel);
        shader.UseImage1(ModContent.Request<Texture2D>(Texture + "Shape"));
        GameShaders.Misc.Add("DicerFeather", shader);
    }
}

public struct DicerFeatherVertexStrip
{
    private static VertexStrip _vertexStrip = new VertexStrip();
    public void Draw(Projectile proj, float opacity)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["DicerFeather"];
        miscShaderData.UseOpacity(proj.Opacity * opacity);
        miscShaderData.UseSaturation(-3);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, true);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    public static Color StripColors(float progressOnStrip)
    {
        return Color.SkyBlue with { A = 128 } * (1f - progressOnStrip);
    }
    private float StripWidth(float progressOnStrip)
    {
        return 5;
    }
}