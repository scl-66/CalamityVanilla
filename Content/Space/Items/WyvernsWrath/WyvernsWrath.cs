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
    
namespace CalamityVanilla.Content.Space.Items.WyvernsWrath;

public class WyvernsWrath : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<WyvernsWrathFeather>(), 24, 45, 20);
        Item.UseSound = null;

        // Set damage and knockBack
        Item.SetWeaponValues(35, 8);

        // Set rarity and value
        Item.SetShopValues(ItemRarityColor.Pink5, 20000);
        Item.useTime = 4;
        Item.useLimitPerAnimation = 5;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        //if (Main.rand.NextBool(5))
        //{
        //    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1) * 2f, ModContent.ProjectileType<WyvernsWrathEnergyFeather>(), damage, knockback, ai1: Main.rand.Next(0, 200));
        //}
        //else
        //{
        //    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3) * Main.rand.NextFloat(1f, 1.5f), type, damage, knockback, ai1: Main.rand.Next(0, 200));
        //}
        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1) * Main.rand.NextFloat(1f, 1.5f), type, damage, knockback, ai1: Main.rand.Next(0, 200));
        return false;
    }
}

public class WyvernsWrathFeather : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Projectile.type] = 4; 
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 340;
        Projectile.penetrate = 3;
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
            if (++Projectile.frameCounter >= 6)
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
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(2) && target.CanBeChasedBy())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2Circular(6, 6), ModContent.ProjectileType<WyvernsWrathEnergyFeather>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack);
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
            d.velocity = Main.rand.NextVector2Circular(1, 1);
            d.noGravity = !Main.rand.NextBool(5);
        }
        int type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = WyvernFeatherVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length);
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            d.scale *= 0.75f;
            d.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(WyvernFeatherVertexStrip).Draw(Projectile);
        Rectangle frameBounds = TextureAssets.Projectile[Type].Frame(1, 4, 0, Projectile.frame);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frameBounds, new Color(Projectile.Opacity, Projectile.Opacity * 2, 1f, 0.7f) * Projectile.Opacity * 2, Projectile.rotation, frameBounds.Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(TextureAssets.MagicPixel);
        shader.UseImage0(TextureAssets.MagicPixel);
        shader.UseImage1(ModContent.Request<Texture2D>(Texture + "Shape"));
        GameShaders.Misc.Add("WyvernsWrathFeather", shader);
    }
}

public class WyvernsWrathEnergyFeather : ModProjectile
{
    // Store the target NPC using Projectile.ai[0]
    private NPC HomingTarget
    {
        get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
        set
        {
            Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
        }
    }
    public ref float DelayTimer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Projectile.type] = 4;
        ProjectileID.Sets.CultistIsResistantTo[Type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 80;
        Projectile.penetrate = 4;
        Projectile.tileCollide = false;
        Projectile.alpha = 140;
    }

    public override void AI()
    {
        Projectile.ai[1] += 1f;
        Projectile.rotation = Projectile.velocity.ToRotation(); // projectile sprite faces up

        int type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 0; i < 1; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Scale: 0.75f);
            d.color = WyvernFeatherVertexStrip.StripColors(0);
            d.velocity = Main.rand.NextVector2Circular(1, 1);
            d.noGravity = true;
        }

        //homing
        float maxDetectRadius = 900f; // The maximum radius at which a projectile can detect a target

        // A short delay to homing behavior after being fired
        if (DelayTimer < 10)
        {
            DelayTimer += 1;
            return;
        }

        // First, we find a homing target if we don't have one
        if (HomingTarget == null)
        {
            HomingTarget = FindClosestNPC(maxDetectRadius);
        }

        // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
        if (HomingTarget != null && !IsValidTarget(HomingTarget))
        {
            HomingTarget = null;
        }

        // If we don't have a target, don't adjust trajectory
        if (HomingTarget == null)
        {
            Projectile.velocity *= 0.98f;
            return;
        }

        // If found, we rotate the projectile velocity in the direction of the target.
        // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
        float length = Math.Clamp(Vector2.Distance(Projectile.position, HomingTarget.position), 0, 10);
        float targetAngle = Projectile.AngleTo(HomingTarget.Center);
        Projectile.velocity *= 1.2f;
        Projectile.velocity.LengthClamp(25);
        Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(8)).ToRotationVector2() * length;
    }

    // Finding the closest NPC to attack within maxDetectDistance range
    // If not found then returns null
    public NPC FindClosestNPC(float maxDetectDistance)
    {
        NPC closestNPC = null;

        // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        // Loop through all NPCs
        foreach (var target in Main.ActiveNPCs)
        {
            // Check if NPC able to be targeted.
            if (IsValidTarget(target))
            {
                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestNPC = target;
                }
            }
        }

        return closestNPC;
    }

    public bool IsValidTarget(NPC target)
    {
        // This method checks that the NPC is:
        // 1. active (alive)
        // 2. chaseable (e.g. not a cultist archer)
        // 3. max life bigger than 5 (e.g. not a critter)
        // 4. can take damage (e.g. moonlord core after all it's parts are downed)
        // 5. hostile (!friendly)
        // 6. not immortal (e.g. not a target dummy)
        // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
        return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
    }

    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 0; i < 3; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.color = WyvernFeatherVertexStrip.StripColors(0);
            d.velocity = Main.rand.NextVector2Circular(1, 1);
            d.noGravity = !Main.rand.NextBool(5);
        }
        
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = WyvernFeatherVertexStrip.StripColors((i + 1) / (float)Projectile.oldPos.Length);
            d.noLight = true;
            d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.velocity *= 0.2f;
            d.scale *= 0.75f;
            d.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        default(WyvernFeatherVertexStrip).Draw(Projectile);
        Rectangle frameBounds = TextureAssets.Projectile[Type].Frame(1, 4, 0, Projectile.frame);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frameBounds, new Color(Projectile.Opacity, Projectile.Opacity * 2, 1f, 0.7f) * Projectile.Opacity * 2, Projectile.rotation, frameBounds.Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
}

public struct WyvernFeatherVertexStrip
{
    private static VertexStrip _vertexStrip = new VertexStrip();
    public void Draw(Projectile proj, float saturation = -3)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["WyvernsWrathFeather"];
        miscShaderData.UseOpacity(proj.Opacity);
        miscShaderData.UseSaturation(saturation);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, true);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    public static Color StripColors(float progressOnStrip)
    {
        return Color.SeaGreen with { A = 0 } * (1f - progressOnStrip);
    }
    private float StripWidth(float progressOnStrip)
    {
        return 10;
    }
}