using CalamityVanilla.Common;
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

namespace CalamityVanilla.Content.Tundra.Cryogen.Drops.TheSnowman;

public class TheSnowman : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
    }

    public override void SetDefaults()
    {
        Item.Size = new Vector2(30, 30);
        Item.damage = 60;
        Item.knockBack = 6f;
        Item.useTime = 21;
        Item.useAnimation = 21;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 3, 10, 0);
        Item.shoot = ModContent.ProjectileType<TheSnowmanProjectile>();
        Item.shootSpeed = 15.9f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }
}


public class TheSnowmanProjectile : BaseFlailProjectile
{
    private static Asset<Texture2D> _hat;

    public ref float HatRotation => ref Projectile.localAI[2];

    public override void Load()
    {
        _hat = ModContent.Request<Texture2D>(Texture + "_Hat");
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;

        if (Main.dedServ)
            return;

        for (int i = 0; i < 5; i++)
        {
            int goreType = Mod.Find<ModGore>($"TheSnowmanChain{i}").Type;

            ChildSafety.SafeGore[goreType] = true;
            GoreID.Sets.DisappearSpeedAlpha[goreType] = 25;
        }

        string[] snowmallBallGores = ["TheSnowmanCarrot", "TheSnowmanHat", "TheSnowmanBigButton", "TheSnowmanSmallButton"];
        foreach (string goreName in snowmallBallGores)
        {
            int goreType = Mod.Find<ModGore>(goreName).Type;

            ChildSafety.SafeGore[goreType] = true;
            GoreID.Sets.DisappearSpeedAlpha[goreType] = 25;
        }
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(26, 26);
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.netImportant = true;
    }

    public override int MaxTimeLaunched => 13;
    public override float LaunchSpeed => 21;
    public override float MaxManualRetractionSpeed => 22;
    public override float MaxForcedRetractionSpeed => 25;
    public override int SpinningNPCHitCooldown => 12;

    public override bool ShouldFreelyRotate => false;


    public override void PostAI()
    {
        float intendedRotation = -Projectile.velocity.X / 20;
        HatRotation = MathHelper.Lerp(HatRotation, intendedRotation, 0.25f);
        HatRotation = Math.Clamp(HatRotation, -1f, 1f);

        float rotationAmount = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f;
        Projectile.rotation += rotationAmount * Math.Sign(Projectile.velocity.X);
        if (CurrentAIState == AIState.Spinning)
        {
            Projectile.rotation += MathHelper.TwoPi / 15f * Main.player[Projectile.owner].direction;
        }
    }

    private void GenerateChainGores()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);
        playerArmPosition -= Vector2.UnitY * player.gfxOffY;

        float chainHeightAdjustment = ChainHeightAdjustment;

        Vector2 chainDrawPosition = Projectile.Center;
        Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
        Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
        float chainSegmentLength = 14 + chainHeightAdjustment;
        if (chainSegmentLength < 0)
        {
            chainSegmentLength = 10;
        }
        float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
        int chainCount = 0;

        float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

        while (chainLengthRemainingToDraw > 0f)
        {
            int frameNumber = Math.Clamp(chainCount - 1, 0, 4);
            int goreType = Mod.Find<ModGore>($"TheSnowmanChain{frameNumber}").Type;

            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), chainDrawPosition, Vector2.Zero, goreType);
            gore.velocity.X *= 0.2f;
            gore.rotation = chainRotation + Main.rand.NextFloat(-0.2f, 0.2f);
            gore.timeLeft = 10 + chainCount;

            chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
            chainCount++;
            chainLengthRemainingToDraw -= chainSegmentLength;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (CurrentAIState == AIState.Dropping)
        {
            GenerateChainGores();

            int snowballAmount = Main.rand.Next(6, 15);
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < snowballAmount; i++)
                {
                    int projectileType = i switch
                    {
                        < 2 => ModContent.ProjectileType<TheSnowmanSnowballBig>(),
                        < 5 => ModContent.ProjectileType<TheSnowmanSnowballMedium>(),
                        < 9 => ModContent.ProjectileType<TheSnowmanSnowballSmall>(),
                        _ => ModContent.ProjectileType<TheSnowmanSnowballTiny>(),
                    };

                    Vector2 snowballVelocity = -oldVelocity.SafeNormalize(-Vector2.UnitY).RotatedByRandom(1f) * Main.rand.NextFloat(4f, 8f);

                    Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        snowballVelocity,
                        projectileType,
                        (int)(Projectile.damage * 0.5f),
                        Projectile.knockBack * 0.5f,
                        Projectile.owner,
                        ai1: Main.rand.Next(2)
                    );
                }
            }

            Vector2[] pileOffsets = [new Vector2(-16, 0), new Vector2(16, 0), new Vector2(0, 0)];
            Vector2 pileAnchor = Projectile.BottomLeft + Projectile.velocity - new Vector2(2, 0);
            foreach (Vector2 offset in pileOffsets)
            {
                const int collisionWidthConstraint = 8;
                if (Collision.SolidCollision(pileAnchor + offset + new Vector2(collisionWidthConstraint, 0), 34 - collisionWidthConstraint * 2, 8))
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectileDirect(
                            Projectile.GetSource_FromThis(),
                            pileAnchor + offset + new Vector2(15, -4),
                            Vector2.Zero,
                            ModContent.ProjectileType<TheSnowmanPile>(),
                            0,
                            0,
                            Projectile.owner,
                            ai1: Main.rand.Next(180, 240)
                        );
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        Dust dust = Dust.NewDustDirect(pileAnchor + offset + new Vector2(0, -8), 34, 12, DustID.Snow, Scale: 1.5f);
                        dust.noGravity = true;
                    }
                }
            }

            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, Scale: 2f);
                dust.noGravity = true;
            }

            int hatGoreType = Mod.Find<ModGore>("TheSnowmanHat").Type;
            Vector2 hatDirection = (HatRotation - MathHelper.PiOver2).ToRotationVector2();
            Gore hatGore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.Center + hatDirection * 15, Vector2.Zero, hatGoreType);
            hatGore.velocity *= 1.2f;
            hatGore.timeLeft = 90;

            string[] snowmanBallGores = [
                "TheSnowmanBigButton", "TheSnowmanBigButton",
                "TheSnowmanCarrot",
                "TheSnowmanSmallButton", "TheSnowmanSmallButton", "TheSnowmanSmallButton", "TheSnowmanSmallButton", "TheSnowmanSmallButton", "TheSnowmanSmallButton",
            ];
            foreach (string goreName in snowmanBallGores)
            {
                int goreType = Mod.Find<ModGore>(goreName).Type;
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, goreType);
                gore.timeLeft = 60 + Main.rand.Next(-10, 11);
            }

            SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.Center);

            return true;
        }

        return base.OnTileCollide(oldVelocity);
    }

    public override Rectangle? InitialChainSourceRectangle(Asset<Texture2D> texture) => texture.Frame(1, 5, 0, 0);
    public override int ChainHeightAdjustment => -2;
    public override void PreDrawChain(int chainCount, ref Asset<Texture2D> texture, ref Rectangle? sourceRectangle, ref Color lighColor)
    {
        int frameNumber = Math.Clamp(chainCount - 1, 0, 4);
        sourceRectangle = texture.Frame(1, 5, 0, frameNumber);
    }

    public override void PostDraw(Color lightColor)
    {
        Vector2 hatDirection = (HatRotation - MathHelper.PiOver2).ToRotationVector2();

        Main.spriteBatch.Draw(_hat.Value, Projectile.Center + hatDirection * 12 - Main.screenPosition, null, Projectile.GetAlpha(lightColor), HatRotation, new Vector2(_hat.Width() * 0.5f, _hat.Height() - 2), 1f, SpriteEffects.None, 0);
    }
}


public class TheSnowmanPile : ModProjectile
{
    public ref float Timer => ref Projectile.ai[0];
    public float TotalTime => Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(34, 12);
        Projectile.tileCollide = false;
        Projectile.hide = true;
    }

    public override void AI()
    {
        Timer++;
        Projectile.Opacity = Utils.GetLerpValue(TotalTime, TotalTime - 30, Timer, true);
        if (Timer > TotalTime)
        {
            Projectile.Kill();
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);
    }
}


public abstract class TheSnowmanSnowball : ModProjectile
{
    public ref float Timer => ref Projectile.ai[0];
    public bool CanRoll
    {
        get => Projectile.ai[1] == 1;
        set => Projectile.ai[1] = value ? 1 : 0;
    }

    private bool ShouldSpawnDustOnKill { get; set; } = true;

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(DefaultWidth, DefaultHeight);
        Projectile.friendly = true;
        Projectile.penetrate = PenetrationAmount;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        Timer++;

        if (Timer > 5)
        {
            Projectile.velocity.Y += 0.2f;

            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        if (Projectile.shimmerWet)
        {
            Projectile.velocity.Y -= 0.5f;
        }

        //if (Projectile.velocity.Y > 0)
        //{
        //   Collision.StepDown(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        //}

        if (CanRoll)
        {
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (CanRoll)
        {
            Projectile.scale -= 0.01f;
            Projectile.width = (int)(DefaultWidth * Projectile.scale);
            Projectile.height = (int)(DefaultHeight * Projectile.scale);

            if (Projectile.scale <= 0f)
            {
                ShouldSpawnDustOnKill = false;
                Projectile.Kill();
            }

            return false;
        }

        return true;
    }

    public override void OnKill(int timeLeft)
    {
        if (!ShouldSpawnDustOnKill)
        {
            return;
        }

        SoundEngine.PlaySound(SoundID.Item51 with
        {
            PitchRange = (0.1f, 0.4f),
            Volume = 0.6f,
            MaxInstances = 0,
        }, Projectile.Center);
        for (int i = 0; i < 8; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
            dust.noGravity = true;
            dust.velocity -= Projectile.oldVelocity * 0.25f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> asset = TextureAssets.Projectile[Type];
        Color color = Projectile.GetAlpha(lightColor);

        Main.spriteBatch.Draw(asset.Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, asset.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

        return false;
    }

    public abstract int DefaultWidth { get; }
    public abstract int DefaultHeight { get; }
    public abstract int PenetrationAmount { get; }
}

public class TheSnowmanSnowballBig : TheSnowmanSnowball
{
    public override int DefaultWidth => 18;
    public override int DefaultHeight => 18;
    public override int PenetrationAmount => 4;
}

public class TheSnowmanSnowballMedium : TheSnowmanSnowball
{
    public override int DefaultWidth => 14;
    public override int DefaultHeight => 14;
    public override int PenetrationAmount => 2;
}

public class TheSnowmanSnowballSmall : TheSnowmanSnowball
{
    public override int DefaultWidth => 12;
    public override int DefaultHeight => 12;
    public override int PenetrationAmount => 1;
}

public class TheSnowmanSnowballTiny : TheSnowmanSnowball
{
    public override int DefaultWidth => 8;
    public override int DefaultHeight => 8;
    public override int PenetrationAmount => 0;
}