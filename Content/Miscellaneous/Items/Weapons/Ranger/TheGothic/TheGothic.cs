using CalamityVanilla.Common.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TheGothic;

public class TheGothic : ModItem
{
    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(TextureAssets.MagicPixel);
        shader.UseImage1(TextureAssets.MagicPixel);
        shader.UseImage0(TextureAssets.MagicPixel);
        GameShaders.Misc.Add("Gothic", shader);
    }
    public override void SetDefaults()
    {
        Item.DefaultToBow(26, 2.5f, true);
        Item.damage = 30;
        Item.knockBack = 1;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 10, 0, 0);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float iterations = 2;
        float spread = Main.rand.NextFloat(0.05f, 0.07f);
        for (float i = -iterations; i <= iterations; i++)
        {
            if (i == 0)
                continue;
            Projectile.NewProjectile(source, position, velocity.RotatedBy(i * spread), ModContent.ProjectileType<TheGothicToothSmall>(), (int)(damage * 0.75f), knockback, player.whoAmI);
        }
        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TheGothicTooth>(), damage, knockback, player.whoAmI);
        return false;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, 0);
    }
}
public struct GothicVertexStrip
{
    private static VertexStrip _vertexStrip = new VertexStrip();
    private float _width;
    public void Draw(Projectile proj, float width)
    {
        _width = width;
        MiscShaderData miscShaderData = GameShaders.Misc["Gothic"];
        miscShaderData.UseOpacity(1f - proj.localAI[0]);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    public static Color StripColors(float progressOnStrip)
    {
        float offsetProgress = MathF.Min((1f - progressOnStrip) * 1.25f, 1);
        return new Color(1 - (1f - offsetProgress), 0f, 0f, 0.7f) * offsetProgress * 0.8f;
    }
    private float StripWidth(float progressOnStrip)
    {
        return _width;
    }
}
public class TheGothicToothSmall : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 50;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.arrow = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 2;
    }
    public override void AI()
    {
        Projectile.ai[2]++;
        if (Projectile.ai[2] > 40 * Projectile.extraUpdates)
        {
            Projectile.velocity.Y += 0.17f / (Projectile.extraUpdates + 1);
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<GothicToothDust>();
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.noGravity = !Main.rand.NextBool(7);
        }
        type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = GothicVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length);
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            d.scale *= 0.75f;
            d.noGravity = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        default(GothicVertexStrip).Draw(Projectile, 1.5f);
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None);
        return false;
    }
}
public class TheGothicTooth : ModProjectile // Example Mod Jumpscare
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 50;
        ProjectileID.Sets.TrailingMode[Type] = 0;
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.arrow = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 2;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 2;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        return base.OnTileCollide(oldVelocity);
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0) // Flying
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] > 40 * Projectile.extraUpdates)
            {
                Projectile.velocity.Y += 0.17f / (Projectile.extraUpdates + 1);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        else
        {
            if (Projectile.localAI[0] < 1)
                Projectile.localAI[0] += 0.01f;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + Projectile.velocity * 0.8f;
            if (Main.rand.NextBool(13))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(6, 0).RotatedBy(Projectile.rotation), DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f)).ToRotationVector2() * Main.rand.NextFloat(-4, -1));
                d.noGravity = Main.rand.NextBool();
            }
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
            }
        }
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            if (Projectile.oldPos[i] != Projectile.oldPos[i - 1])
                Projectile.oldRot[i] = Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]).ToRotation();
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return Projectile.ai[0] == 0;
    }
    private readonly Point[] stickingTeeth = new Point[15];
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.timeLeft = 60 * 12 * Projectile.extraUpdates;
        Projectile.damage = 0;
        Projectile.ai[0] = 1;
        Projectile.ai[2] = 0;
        Projectile.ai[1] = target.whoAmI;
        Projectile.velocity = Projectile.Center - target.Center;
        Projectile.netUpdate = true;
        Projectile.KillOldestJavelin(Projectile.whoAmI, Type, (int)Projectile.ai[1], stickingTeeth);
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        if (Projectile.ai[0] == 1)
        {
            int npcIndex = (int)Projectile.ai[1];
            if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
            {
                if (Main.npc[npcIndex].behindTiles)
                {
                    behindNPCsAndTiles.Add(index);
                }
                else
                {
                    behindNPCs.Add(index);
                }
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<GothicToothDust>();
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.noGravity = !Main.rand.NextBool(7);
            if (Projectile.localAI[0] == 0)
                d.velocity += Projectile.oldVelocity * -0.2f;
        }
        if (Projectile.localAI[0] == 1)
            return;
        type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = GothicVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length) * (1f - Projectile.localAI[0]);
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            //d.scale = Utils.Remap(i, 0, Projectile.oldPos.Length, 1f, 0.3f);
            d.noGravity = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];

        if (Projectile.localAI[0] < 1)
        {
            default(GothicVertexStrip).Draw(Projectile, 3);
            //for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            //{
            //    float multiply = 1 - i / 6f;
            //    Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(1f, 0f, 0f, 0f) * multiply, Projectile.rotation, tex.Size() / 2, 0.8f + multiply * 0.2f, SpriteEffects.None);
            //}
        }

        //Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, Projectile.ai[0] == 0 ? lightColor : Color.Lerp(lightColor, Color.Red, Main.masterColor), Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None);
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, tex.Frame(1, 2, 0, 0), lightColor, Projectile.rotation, new Vector2(13, 7), 1f, SpriteEffects.None);
        if (Projectile.ai[0] == 1)
        {
            float interval = 100;
            float amount = (float)(Main.timeForVisualEffects % interval) / interval;
            Color c = new Color(amount * (1f - amount) * 0.75f, 0, 0, 0);
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, amount * 8).RotatedBy(Projectile.rotation + (i * MathHelper.PiOver2)), tex.Frame(1, 2, 0, 1), c, Projectile.rotation, new Vector2(13, 7), 1f, SpriteEffects.None);
            }
            amount = (float)((Main.timeForVisualEffects + (interval / 2)) % interval) / interval;
            c = new Color(amount * (1f - amount) * 0.5f, 0, 0, 0);
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, amount * 8).RotatedBy(Projectile.rotation + MathHelper.PiOver4 + (i * MathHelper.PiOver2)), tex.Frame(1, 2, 0, 1), c, Projectile.rotation, new Vector2(13, 7), 1f, SpriteEffects.None);
            }
        }
        return false;
    }
}
public class GothicToothRegen : ModPlayer
{
    public int GothicToothRegenCounter = 0;
    public override void PostUpdateBuffs()
    {
        GothicToothRegenCounter++;
        if (GothicToothRegenCounter > 60)
        {
            if (!Player.moonLeech)
            {
                int lifeRegen = 0;
                foreach (Projectile tooth in Main.ActiveProjectiles)
                {
                    if (tooth.type == ModContent.ProjectileType<TheGothicTooth>() && tooth.owner == Player.whoAmI && tooth.ai[0] == 1 && Main.npc[(int)tooth.ai[1]].type != NPCID.TargetDummy)
                    {
                        lifeRegen++;
                    }
                }
                if (lifeRegen == 0)
                    return;
                if (lifeRegen > 15)
                    lifeRegen = 15;
                Player.statLife += lifeRegen;
                CombatText.NewText(Player.Hitbox, CombatText.HealLife, lifeRegen);
                for (int i = 0; i < lifeRegen * 2; i++)
                {
                    Vector2 rotation = Main.rand.NextVector2Circular(1, 1);
                    Dust d = Dust.NewDustPerfect(Player.Center + rotation * 40, DustID.VampireHeal);
                    d.velocity = -rotation * 3 + Player.velocity;
                    d.scale *= 1.3f;
                    d.noGravity = true;
                }
            }
            GothicToothRegenCounter = 0;
        }
    }
}