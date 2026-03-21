using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TheGothic;

public class TheGothic : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToBow(14, 2.5f, true);
        Item.damage = 35;
        Item.knockBack = 1;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 10, 0, 0);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float iterations = Main.rand.NextBool() ? 0.5f : 1f;
        for (float i = -iterations; i <= iterations; i++)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(i * Main.rand.NextFloat(0.05f, 0.1f)), ModContent.ProjectileType<TheGothicTooth>(), damage, knockback, player.whoAmI);
        }
        return false;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, 0);
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
        Projectile.hide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 2;
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
            Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + Projectile.velocity * 0.8f;
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
            }
        }
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            if(Projectile.oldPos[i] != Projectile.oldPos[i - 1])
            Projectile.oldRot[i] = Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]).ToRotation();
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return Projectile.ai[0] == 0;
    }
    private readonly Point[] stickingTeeth = new Point[12];
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.timeLeft = 60 * 6 * Projectile.extraUpdates;
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
            d.noGravity = true;
            d.velocity += Projectile.oldVelocity * -0.2f;
        }
        if (Projectile.localAI[0] == 1)
            return;
        type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = new Color(1, 0f, 0f, 0.7f) * MathF.Pow(Utils.Remap(i,0,Projectile.oldPos.Length,1,0), 3) * (1f - Projectile.localAI[0]);
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            d.scale = Utils.Remap(i, 0, Projectile.oldPos.Length, 1f, 0.3f);
            d.noGravity = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];

        if (Projectile.localAI[0] < 1)
        {
            default(GothicVertexStrip).Draw(Projectile);
            //for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            //{
            //    float multiply = 1 - i / 6f;
            //    Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(1f, 0f, 0f, 0f) * multiply, Projectile.rotation, tex.Size() / 2, 0.8f + multiply * 0.2f, SpriteEffects.None);
            //}
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, Projectile.ai[0] == 0 ? lightColor : Color.Lerp(lightColor, Color.Red, Main.masterColor), Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None);
        return false;
    }
}
public struct GothicVertexStrip
{
    private static VertexStrip _vertexStrip = new VertexStrip();
    public void Draw(Projectile proj)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["LightDisc"];
        miscShaderData.UseOpacity(1f - proj.localAI[0]);
        miscShaderData.UseImage1(TextureAssets.MagicPixel);
        miscShaderData.UseImage2(TextureAssets.MagicPixel);
        miscShaderData.Apply();

        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    private Color StripColors(float progressOnStrip)
    {
        return new Color(1, 0f, 0f, 0.7f) * MathF.Pow((1f - progressOnStrip),3);
    }
    private float StripWidth(float progressOnStrip)
    {
        return 3;
    }
}
public class GothicToothRegen : ModPlayer
{
    public int GothicToothRegenCounter = 0;
    public override void PostUpdateBuffs()
    {
        GothicToothRegenCounter++;
        if (GothicToothRegenCounter > 120)
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
                if (lifeRegen > 30)
                    lifeRegen = 30;
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