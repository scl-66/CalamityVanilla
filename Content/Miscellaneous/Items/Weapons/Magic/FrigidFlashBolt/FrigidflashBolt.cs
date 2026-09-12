using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Particles;
using CalamityVanilla.Content.Tundra.Items.Frostbolt;
using CalamityVanilla.Content.Underworld.Items.FlareBolt;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.FrigidFlashBolt;

public class FrigidflashBolt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.damage = 49;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 16;
        Item.useTime = 7;
        Item.useAnimation = 7;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 5f;
        Item.UseSound = SoundID.Item20 with
        {
            Pitch = 1f,
            Volume = 0.85f,
            PitchVariance = 0.5f,
            MaxInstances = 0,
        };
        Item.autoReuse = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 8f;

        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 6);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = Main.rand.Next(new int[] { ModContent.ProjectileType<FrigidflashColdBoltProjectile>(), ModContent.ProjectileType<FrigidflashHotBoltProjectile>() });
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.2), type, damage, knockback, player.whoAmI,
            ai0: Main.rand.NextFloat(0.1f, 3f));

        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.MythrilAnvil)
            .AddIngredient<FrostBolt>()
            .AddIngredient<FlareBolt>()
            .AddIngredient(ItemID.SoulofFright, 10)
            .AddIngredient(ItemID.SoulofMight, 10)
            .AddIngredient(ItemID.SoulofSight, 10)
            .Register();
    }
}

public abstract class FrigidflashBoltProjectile : ModProjectile
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
        ProjectileID.Sets.TrailCacheLength[Type] = 75;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 3;
        Projectile.alpha = 255;
        Projectile.penetrate = 10;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        float maxRange = 800f;

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
        Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(HomingStrength)).ToRotationVector2() * length;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.penetrate--;
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
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }
        return false;
    }
}

public class FrigidflashColdBoltProjectile : FrigidflashBoltProjectile
{
    private static Asset<Texture2D> _explosion;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

        if (!Main.dedServ)
            _explosion = ModContent.Request<Texture2D>(Texture + "Effect");
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.coldDamage = true;
    }

    public override void AI()
    {
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 13;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.timeLeft > 115)
            return;
        base.AI();

        if (Main.rand.NextBool(3))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(14 * Projectile.scale, 14 * Projectile.scale), Main.rand.NextBool() ? DustID.Snow : DustID.IceRod);
            d.velocity = Projectile.velocity * 0.8f;
            d.alpha = Projectile.alpha;
            d.scale *= Main.rand.NextFloat(1f, 1.25f);
            d.noGravity = true;
        }

        Projectile.localAI[0] += Projectile.velocity.X * 0.02f;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath15 with { MaxInstances = 5, volume = 0.5f }, Projectile.position);
        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.Snow : DustID.IceRod);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.alpha = Projectile.alpha;
            d.scale *= Main.rand.NextFloat(1f, 1.5f);
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
        for (int i = 0; i < 5; i++)
        {
            var p = VanillaParticles.RequestFadingParticle();
            p.ColorTint = Color.White;
            float time = Main.rand.NextFloat(25, 45);
            p.SetTypeInfo(time);
            Main.instance.LoadProjectile(ProjectileID.NorthPoleSnowflake);
            p.SetBasicInfo(TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake], TextureAssets.Projectile[ProjectileID.NorthPoleSnowflake].Frame(1, 3, 0, Main.rand.Next(3)), Main.rand.NextVector2Circular(6, 6), Projectile.Center);
            p.FadeInNormalizedTime = 0.2f;
            p.FadeOutNormalizedTime = 0.5f;
            p.Scale = Vector2.One * 1.3f;
            p.ScaleVelocity = Vector2.One / -time;
            p.AccelerationPerFrame = p.Velocity / -time;
            p.RotationVelocity = p.Velocity.X * 0.1f;
            p.RotationAcceleration = -p.RotationVelocity / time;
            //p.Rotation = Main.rand.NextFloatDirection();
            Main.ParticleSystem_World_BehindPlayers.Add(p);

            //var p = VanillaParticles.RequestPrettySparkleParticle();
            //p.TimeToLive = Main.rand.Next(20, 45);
            //p.FadeInNormalizedTime = 0.2f;
            //p.FadeOutNormalizedTime = 0.8f;
            //p.LocalPosition = Projectile.Center;
            //p.DrawHorizontalAxis = false;
            //p.Scale = new Vector2(2, 1.3f);
            //p.ColorTint = Color.Lerp(new Color(0f, 1f, 1f, 0.5f), new Color(0.2f, 0.2f, 1f,0.5f), Main.rand.NextFloat());
            //p.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4, 7);
            //p.AccelerationPerFrame = -p.Velocity / p.TimeToLive;
            //p.Rotation = p.Velocity.ToRotation() + MathHelper.PiOver2;
            //Main.ParticleSystem_World_BehindPlayers.Add(p);
        }

        var p2 = AnimatedParticle.RequestAnimatedParticle();
        p2.SetTypeInfo(5, Main.rand.Next(10, 15), _explosion, Color.White);
        p2.LocalPosition = Projectile.Center;
        p2.ScaleVelocity = Vector2.One * Main.rand.NextFloat(-0.01f, 0.01f);
        p2.Scale = Vector2.One * Main.rand.NextFloat(1f, 1.3f);
        Main.ParticleSystem_World_OverPlayers.Add(p2);
        //var p2 = VanillaParticles.RequestFadingParticle();
        //p2.SetBasicInfo(TextureAssets.Projectile[Type], null, Vector2.Zero, Projectile.Center);
        //p2.SetTypeInfo(Main.rand.Next(20, 35));
        //p2.ColorTint = new Color(Projectile.Opacity, Projectile.Opacity * 2, 1f, 0f) * Projectile.Opacity * 0.75f;
        //p2.FadeInNormalizedTime = 0.1f;
        //p2.FadeOutNormalizedTime = 0.5f;
        //p2.Scale = Vector2.One * Projectile.scale;
        //p2.ScaleVelocity = Vector2.One * 0.05f;
        //p2.Rotation = Projectile.localAI[0];
        //p2.RotationVelocity = Projectile.velocity.X * 0.02f;
        //Main.ParticleSystem_World_BehindPlayers.Add(p2);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn2, 360);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Frostburn2, 360, false, false);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        default(FrostboltVertexStrip).Draw(Projectile, -7);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, new Color(Projectile.Opacity, Projectile.Opacity * 2, 1f, 0.7f) * Projectile.Opacity * 2, Projectile.localAI[0], TextureAssets.Projectile[Type].Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
}

public class FrigidflashHotBoltProjectile : FrigidflashBoltProjectile
{
    public override void AI()
    {
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 13;
        }

        Projectile.frameCounter++;
        if (Projectile.frameCounter > 12)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
            if (Projectile.frame > 2)
                Projectile.frame = 0;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.timeLeft > 115)
            return;
        if (Main.rand.NextBool(4))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            d.velocity += Projectile.velocity * 0.25f;
            if (!Main.rand.NextBool(3))
            {
                d.scale = Main.rand.NextFloat(1, 2);
                d.noGravity = true;
            }
        }
        if (Main.rand.NextBool(8))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            d.velocity += Projectile.velocity * 0.25f;
            d.noGravity = true;
        }
        base.AI();
    }
    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = FrigidFlashFlareBoltVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length) * Projectile.Opacity * 0.5f;
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            d.noGravity = true;
        }
        for (int i = 0; i < 7; i++)
        {
            Gore g = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2Circular(3, 2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
            g.velocity += Projectile.velocity * 0.25f;
            g.scale = Main.rand.NextFloat(0.5f, 1f);
        }

        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(3, 3));
            d.scale += Main.rand.NextFloat();
            if (Main.rand.NextBool())
            {
                d.velocity *= 3;
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(2);
            }
            else
            {
                d.customData = 1;
            }
            d.velocity += Projectile.velocity * 0.25f;
        }

        var p = AnimatedParticle.RequestAnimatedParticle();
        p.SetTypeInfo(5, Main.rand.Next(10, 15), TextureAssets.Projectile[ProjectileID.Volcano], Color.White);
        p.LocalPosition = Projectile.Center;
        p.ScaleVelocity = Vector2.One * Main.rand.NextFloat(-0.01f, 0.01f);
        p.Scale = Vector2.One * Main.rand.NextFloat(1f, 1.3f);
        Main.ParticleSystem_World_OverPlayers.Add(p);
        SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Custom/meteor_shower_", [1, 2, 3]) with { MaxInstances = 15 }, Projectile.position);
        //SoundEngine.PlaySound(SoundID.Item62 with
        //{
        //    PitchVariance = 0.2f,
        //    MaxInstances = 10,
        //}, Projectile.Center);
        //SoundEngine.PlaySound(SoundID.Item38 with
        //{
        //    Pitch = 0.8f,
        //    PitchVariance = 0.2f,
        //    Volume = 0.8f,
        //    MaxInstances = 0,
        //}, Projectile.Center);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 360);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.OnFire3, 360, false, false);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        default(FrigidFlashFlareBoltVertexStrip).Draw(Projectile);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, TextureAssets.Projectile[Type].Frame(1, 3, 0, Projectile.frame), new Color(1f, Projectile.Opacity * 2, Projectile.Opacity, 0.7f) * Projectile.Opacity * 2, Projectile.rotation - MathHelper.PiOver2, new Vector2(10, 32), Projectile.scale, SpriteEffects.None);
        return false;
    }
    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(ModContent.Request<Texture2D>(Texture + "Erosion"));
        shader.UseImage1(ModContent.Request<Texture2D>(Texture + "Shape"));
        shader.UseImage0(ModContent.Request<Texture2D>(Texture + "Gradient"));
        GameShaders.Misc.Add("FrigidFlashFlareBolt", shader);
    }
}
public struct FrigidFlashFlareBoltVertexStrip
{
    private static VertexStrip _vertexStrip = new VertexStrip();
    public void Draw(Projectile proj)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["FrigidFlashFlareBolt"];
        miscShaderData.UseSaturation(-7);
        miscShaderData.UseOpacity(proj.Opacity);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    public static Color StripColors(float progressOnStrip)
    {
        return Color.Lerp(new Color(1f, 1f, 0.5f, 0f), new Color(1f, 0f, 0f, 0f), progressOnStrip) * (1f - progressOnStrip * progressOnStrip);
    }
    private float StripWidth(float progressOnStrip)
    {
        return 40;
    }
}