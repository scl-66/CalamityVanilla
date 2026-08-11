using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Items.Frostbolt;

public class FrostBolt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.damage = 21;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 15;
        Item.useTime = 7;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 5f;
        //Item.UseSound = SoundID.Item30 with { Volume = 0.25f, MaxInstances = 10, PitchRange = (-0.2f,0.2f) };
        Item.UseSound = SoundID.Item28 with { Volume = 0.5f, MaxInstances = 5 };
        Item.autoReuse = true;
        Item.shootSpeed = 12f;
        Item.shoot = ModContent.ProjectileType<FrostBoltProjectile>();

        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(0, 4);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.4) * Main.rand.NextFloat(0.8f, 1.2f), type, damage, knockback, player.whoAmI,
            ai0: Main.rand.NextFloat(0.01f, 0.05f));

        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Bookcases)
            .AddIngredient(ItemID.SpellTome, 1)
            .AddRecipeGroup("CalamityVanillaAnyIceBlock", 20)
            .AddIngredient<EleumSoul>(15)
            .Register();
    }
}

public class FrostBoltProjectile : ModProjectile
{
    private ref float HomingStrength => ref Projectile.ai[0];
    private NPC HomingTarget
    {
        get => Projectile.ai[1] == 0 ? null : Main.npc[(int)Projectile.ai[1] - 1];
        set
        {
            Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1;
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.penetrate = 5;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.coldDamage = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
        Projectile.alpha = 260;
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.localAI[0] += Projectile.velocity.X * 0.02f;
        Projectile.velocity.Y += 0.2f;
        if (Main.rand.NextBool(3))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(14 * Projectile.scale, 14 * Projectile.scale), Main.rand.NextBool() ? DustID.Snow : DustID.IceRod);
            d.velocity = Projectile.velocity * 0.8f;
            d.alpha = Projectile.alpha;
            d.scale *= Main.rand.NextFloat(1f, 1.25f);
            d.noGravity = true;
        }

        if (Projectile.alpha == 260)
        {
            Projectile.scale = Main.rand.NextFloat(0.8f, 1f);
        }
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 13;
        }

        if (Projectile.timeLeft > 50)
            return;

        float maxRange = 400f;

        if (HomingTarget != null &&
            (
            HomingTarget.CanBeChasedBy(Projectile, false) ||
            Projectile.DistanceSQ(HomingTarget.Center) < maxRange * maxRange ||
            Collision.CanHit(Projectile.Center, 1, 1, HomingTarget.position, HomingTarget.width, HomingTarget.height
            )
        ))
        {
            HomingTarget = null;
        }

        if (HomingTarget == null)
        {
            int target = Projectile.FindTargetWithLineOfSight(maxRange);
            if (target >= 0)
            {
                HomingTarget = Main.npc[target];
            }
        }

        if (HomingTarget == null)
            return;

        float length = Projectile.velocity.Length();
        float targetAngle = Projectile.AngleTo(HomingTarget.Center);
        Projectile.velocity = Utils.AngleTowards(Projectile.velocity.ToRotation(), targetAngle, HomingStrength).ToRotationVector2() * length;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn2, 180);
        Projectile.ai[0] /= 2;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        Projectile.ai[0] /= 2;
        target.AddBuff(BuffID.Frostburn2, 180, false, false);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.penetrate--;
        Projectile.ai[0] /= 2;
        if (Projectile.penetrate <= 0)
        {
            Projectile.Kill();
        }
        else
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            SoundEngine.PlaySound(SoundID.Item50 with { Volume = 0.25f, MaxInstances = 10 }, Projectile.Center);
        }
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27 with
        {
            PitchRange = (0.2f, 0.5f),
            Volume = 0.4f,

            MaxInstances = 0,
        }, Projectile.Center);
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.Snow : DustID.IceRod);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.alpha = Projectile.alpha;
            d.scale *= Main.rand.NextFloat(1f, 1.25f);
            d.noGravity = true;
        }
        int type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = FrostboltVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length) * Projectile.Opacity * 0.5f;
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            d.noGravity = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        default(FrostboltVertexStrip).Draw(Projectile);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, new Color(Projectile.Opacity, Projectile.Opacity * 2, 1f, 0.7f) * Projectile.Opacity * 2, Projectile.localAI[0], TextureAssets.Projectile[Type].Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(ModContent.Request<Texture2D>(Texture + "Erosion"));
        shader.UseImage1(ModContent.Request<Texture2D>(Texture + "Shape"));
        shader.UseImage0(ModContent.Request<Texture2D>(Texture + "Gradient"));
        GameShaders.Misc.Add("FrostBolt", shader);
    }
}
public struct FrostboltVertexStrip
{
    private static VertexStrip _vertexStrip = new VertexStrip();
    public void Draw(Projectile proj, float saturation = -3)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["FrostBolt"];
        miscShaderData.UseOpacity(proj.Opacity);
        miscShaderData.UseSaturation(saturation);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    public static Color StripColors(float progressOnStrip)
    {
        return Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color(0f, 0.5f, 1f, 0f), progressOnStrip) * (1f - progressOnStrip * progressOnStrip);
    }
    private float StripWidth(float progressOnStrip)
    {
        return 20;
    }
}