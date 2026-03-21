using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityVanilla.Content.Underground.Items.GraniteTome;

public class GraniteTome : ModItem
{
    public static readonly SoundStyle UseSound = new("CalamityVanilla/Assets/Sounds/ItemElectricZap");

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.damage = 4;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 8;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 5f;
        Item.UseSound = UseSound with
        {
            Pitch = -0.3f,
            PitchVariance = 0.2f,
            Volume = 0.1f,
            MaxInstances = 0,
        };
        Item.autoReuse = true;
        Item.shootSpeed = 8f;
        Item.shoot = ModContent.ProjectileType<GraniteTomeBolt>();

        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 30);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1), type, damage, knockback, player.whoAmI, 0, Main.rand.NextBool() ? 1 : -1);

        return false;
    }
}

public class GraniteTomeBolt : ModProjectile
{
    private static VertexStrip _vertexStrip = new VertexStrip();

    // to ensure that the trail mesh fades out smoothly, the projectile lives twice as long and after its timeLeft goes below TotalTimeLeft the mesh length starts to shrink
    private int ActualTotalTimeleft => ProjectileID.Sets.TrailCacheLength[Projectile.type] * 2;
    private int TotalTimeLeft => ProjectileID.Sets.TrailCacheLength[Projectile.type];

    private ref float SplittingLikelihood => ref Projectile.ai[0];

    private int _randomSeed;
    private UnifiedRandom _random;
    private ref float TimeUntilRotate => ref Projectile.localAI[0];
    private ref float CurrentDirection => ref Projectile.ai[1];
    private ref float TotalAngle => ref Projectile.localAI[1];
    private ref float TimeUntiSplit => ref Projectile.localAI[2];

    public override void Load()
    {
#pragma warning disable CS0618 // we're not in 1.4.5 yet
        GameShaders.Misc["GraniteTome"] = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(true);
#pragma warning restore CS0618
        GameShaders.Misc["GraniteTome"].UseImage0("Images/Extra_197");
        GameShaders.Misc["GraniteTome"].UseImage1("Images/Extra_195");
        GameShaders.Misc["GraniteTome"].UseImage2("Images/Extra_197");
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Projectile.type] = -1; // caching positions is done manually for control
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 64;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = ActualTotalTimeleft;
        Projectile.extraUpdates = 7;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _randomSeed = Main.rand.Next(int.MaxValue);
        _random = new UnifiedRandom(_randomSeed);
    }

    private void Rotate()
    {
        float angle = _random.NextFloat(0.1f, 0.2f) * CurrentDirection;
        TotalAngle += angle;
        if (MathF.Abs(TotalAngle) > 0.4f)
        {
            TotalAngle -= angle;
            angle = -angle * 2;
            TotalAngle += angle;
        }
        CurrentDirection *= -1;
        Projectile.velocity = Projectile.velocity.RotatedBy(angle);
    }

    private void Split()
    {
        SplittingLikelihood += 2;
        if (SplittingLikelihood > 4)
        {
            SplittingLikelihood = -1;
            return;
        }
        double rotationRadians = Main.rand.NextFloat(0.2f, 0.6f) * (Main.rand.NextBool() ? 1 : -1);
        Vector2 velocity = Projectile.velocity.RotatedBy(rotationRadians);
        Projectile newProj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, SplittingLikelihood * 2, Main.rand.NextBool() ? 1 : -1);
        newProj.timeLeft = Projectile.timeLeft - 1;
        newProj.netUpdate = true;
    }

    public override void AI()
    {
        if (Projectile.timeLeft < TotalTimeLeft)
        {
            Projectile.friendly = false;
            return;
        }

        TimeUntilRotate++;
        if (TimeUntilRotate > 8 && (TimeUntilRotate > 32 || _random.NextBool((int)TimeUntilRotate, 32)))
        {
            Rotate();
            TimeUntilRotate = 0;

            Projectile.damage += 2;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (Main.rand.NextBool(8))
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.5f);
            if (Main.rand.NextBool(3))
            {
                dust.scale *= 1.5f;
                dust.velocity *= 0.5f;
            }
        }

        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            Projectile.oldRot[i] = Projectile.oldRot[i - 1];
        }
        Projectile.oldPos[0] = Projectile.position;
        Projectile.oldRot[0] = Projectile.rotation;

        TimeUntiSplit++;
        int splitChance = (int)(32 + 6 * SplittingLikelihood);
        if (
            Projectile.owner == Main.myPlayer &&
            TimeUntiSplit > 16 &&
            (!(SplittingLikelihood < 0 || SplittingLikelihood > 2)) &&
            (TimeUntiSplit > splitChance || Main.rand.NextBool((int)TimeUntiSplit, splitChance))
        )
        {
            Split();
            TimeUntiSplit = 0;

            if (Main.rand.NextBool(2))
            {
                Rotate();
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SplittingLikelihood = -1;
        Projectile.velocity = Vector2.Zero;

        if (Projectile.timeLeft < TotalTimeLeft)
            return false;

        for (int i = 0; i < 7 * Projectile.timeLeft / (float)TotalTimeLeft; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, oldVelocity.X * 0.5f, oldVelocity.Y * 0.5f, 0, default, 0.5f);
            if (Main.rand.NextBool(2, 3))
            {
                dust.noGravity = true;
                dust.scale *= 1.5f;
                dust.velocity *= 0.5f;
            }
        }

        return false;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        _randomSeed = Main.rand.Next(int.MaxValue);
        _random = new UnifiedRandom(_randomSeed);

        writer.Write(_randomSeed);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _randomSeed = reader.ReadInt32();

        _random = new(_randomSeed);
    }

    public static void SpawnParticles(Vector2 position)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f);
            Dust dust = Dust.NewDustPerfect(position, DustID.Electric, velocity, 0, default, 0.5f);
            if (Main.rand.NextBool(2, 3))
            {
                dust.noGravity = true;
                dust.scale *= 1.5f;
                dust.velocity *= 0.5f;
            }
        }

        PrettySparkleParticle prettySparkleParticle = VanillaParticles.RequestPrettySparkleParticle();
        float rotation = MathHelper.PiOver2;
        Vector2 scale = new Vector2(Main.rand.NextFloat() * 0.2f + 0.4f);
        Vector2 offset = Main.rand.NextVector2Circular(4f, 4f) * scale;
        prettySparkleParticle.ColorTint = new Color(0.3f, 1f, 1f, 0.3f);
        prettySparkleParticle.LocalPosition = position + offset;
        prettySparkleParticle.Rotation = rotation;
        prettySparkleParticle.Scale = scale;
        prettySparkleParticle.FadeInNormalizedTime = 0.05f;
        prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
        prettySparkleParticle.FadeInEnd = 10f;
        prettySparkleParticle.FadeOutStart = 20f;
        prettySparkleParticle.FadeOutEnd = 30f;
        prettySparkleParticle.TimeToLive = 30f;
        Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SpawnParticles(Projectile.position);
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityVanilla.PacketType.SpawnGraniteTomeBoltSparks);
            packet.WriteVector2(Projectile.Center);
            packet.Send();
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SpawnParticles(Projectile.position);
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityVanilla.PacketType.SpawnGraniteTomeBoltSparks);
            packet.Write(Main.myPlayer);
            packet.WriteVector2(Projectile.Center);
            packet.Send();
        }
    }

    public override void PostDraw(Color lightColor)
    {
        Color StripColors(float p) => new Color(0.4f, 0.85f, 1f, 0f) * MathF.Min(1f, p * 4);// * (1f - MathF.Abs(p - 0.5f) * 2f);
        float StripWidth(float p) => 16f;

        MiscShaderData miscShaderData = GameShaders.Misc["GraniteTome"];
        miscShaderData.UseSaturation(-2.8f);
        miscShaderData.UseOpacity(MathF.Min(2f, 4f * Projectile.timeLeft / TotalTimeLeft));
        miscShaderData.Apply(null);
        _vertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2f, (Projectile.timeLeft > TotalTimeLeft) ? (ActualTotalTimeleft - Projectile.timeLeft) : Projectile.timeLeft);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}