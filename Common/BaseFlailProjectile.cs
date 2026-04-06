using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

// Mostly adapted from Example Mod because I didn't want to manually decipher flail code
public abstract class BaseFlailProjectile : ModProjectile
{
    #region Implementation

    public enum AIState
    {
        Spinning,
        LaunchingForward,
        Retracting,
        UnusedState,
        ForcedRetracting,
        Ricochet,
        Dropping
    }

    public AIState CurrentAIState
    {
        get => (AIState)Projectile.ai[0];
        set => Projectile.ai[0] = (float)value;
    }
    public ref float StateTimer => ref Projectile.ai[1];
    public ref float CollisionCounter => ref Projectile.localAI[0];
    public ref float SpinningStateTimer => ref Projectile.localAI[1];

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
        {
            Projectile.Kill();
            return;
        }
        if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
        {
            Projectile.Kill();
            return;
        }

        Vector2 mountedCenter = player.MountedCenter;
        bool doFastThrowDust = false;
        bool shouldFreelyRotate = ShouldFreelyRotate;
        bool ownerHitCheck = false;
        int launchMaxTime = MaxTimeLaunched;
        float launchSpeed = LaunchSpeed;
        float launchMaxDistance = MaxDistanceLaunched;
        float retractionAcceleration = ManualRetractionAcceleration;
        float maximumRetractionSpeed = MaxManualRetractionSpeed;
        float forcedRetractionAcceleration = ForcedRetractionAcceleration;
        float maximumForcedRetractionSpeed = MaxForcedRetractionSpeed;
        float unusedRetractionAcceleration = 1f;
        float unusedMaximumRetractionSpeed = 14f;
        int unusedChainLength = 60;
        int defaultHitCooldown = DefaultNPCHitCooldown;
        int spinningHitCooldown = SpinningNPCHitCooldown;
        int launchHitCooldown = LaunchedNPCHitCooldown;
        int ricochetMaxTime = 10 + 5;//launchMaxTime + 5; Vanilla calculates this before changing launchMaxTime (which is 10 by default) to flail-specific values

        float meleeSpeed = player.GetTotalAttackSpeed(DamageClass.Melee);
        launchSpeed *= meleeSpeed;
        unusedRetractionAcceleration *= meleeSpeed;
        unusedMaximumRetractionSpeed *= meleeSpeed;
        retractionAcceleration *= meleeSpeed;
        maximumRetractionSpeed *= meleeSpeed;
        forcedRetractionAcceleration *= meleeSpeed;
        maximumForcedRetractionSpeed *= meleeSpeed;
        float launchRange = launchSpeed * launchMaxTime;
        float maxDropRange = launchRange + 160f;

        Projectile.localNPCHitCooldown = defaultHitCooldown;

        switch (CurrentAIState)
        {
            case AIState.Spinning:
                ownerHitCheck = true;

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 origin = mountedCenter;
                    Vector2 mouseWorld = Main.MouseWorld;
                    Vector2 launchDirection = origin.DirectionTo(mouseWorld).SafeNormalize(Vector2.UnitX * player.direction);

                    if (!player.channel)
                    {
                        CurrentAIState = AIState.LaunchingForward;
                        StateTimer = 0f;
                        Projectile.velocity = launchDirection * launchSpeed + player.velocity;
                        Projectile.Center = mountedCenter;
                        Projectile.netUpdate = true;
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.localNPCHitCooldown = launchHitCooldown;
                        break;
                    }
                }

                SpinningStateTimer++;
                Vector2 positionOffsetDirection = new Vector2(player.direction).RotatedBy(MathF.PI * 10f * (SpinningStateTimer / 60f) * player.direction);
                positionOffsetDirection.Y *= 0.8f;
                if (positionOffsetDirection.Y * player.gravDir > 0f)
                {
                    positionOffsetDirection.Y *= 0.5f;
                }
                Projectile.Center = mountedCenter + positionOffsetDirection * 30f;
                Projectile.velocity = Vector2.Zero;
                Projectile.localNPCHitCooldown = spinningHitCooldown;

                break;
            case AIState.LaunchingForward:
                doFastThrowDust = true;
                bool shouldAutomaticallyRetract = StateTimer++ >= launchMaxTime;
                shouldAutomaticallyRetract |= Projectile.Distance(mountedCenter) >= launchMaxDistance;
                if (player.controlUseItem)
                {
                    CurrentAIState = AIState.Dropping;
                    StateTimer = 0f;
                    Projectile.netUpdate = true;
                    Projectile.velocity *= 0.2f;

                    OnDropDuringLaunch();

                    break;
                }
                if (shouldAutomaticallyRetract)
                {
                    CurrentAIState = AIState.Retracting;
                    StateTimer = 0f;
                    Projectile.netUpdate = true;
                    Projectile.velocity *= 0.3f;

                    OnManualRetract();
                }
                player.ChangeDir(player.Center.X < Projectile.Center.X ? 1 : -1);
                Projectile.localNPCHitCooldown = launchHitCooldown;
                break;
            case AIState.Retracting:
                Vector2 retractionDirection = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                if (Projectile.Distance(mountedCenter) <= maximumRetractionSpeed)
                {
                    Projectile.Kill();
                    return;
                }
                if (player.controlUseItem)
                {
                    CurrentAIState = AIState.Dropping;
                    StateTimer = 0f;
                    Projectile.netUpdate = true;
                    Projectile.velocity *= 0.2f;

                    OnDropDuringManualRetract();
                }
                else
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity = Projectile.velocity.MoveTowards(retractionDirection * maximumRetractionSpeed, retractionAcceleration);
                    player.ChangeDir(player.Center.X < Projectile.Center.X ? 1 : -1);
                }
                break;
            case AIState.UnusedState:
                if (!player.controlUseItem)
                {
                    CurrentAIState = AIState.ForcedRetracting;
                    StateTimer = 0f;
                    Projectile.netUpdate = true;
                    break;
                }
                float chainLength = Projectile.Distance(mountedCenter);
                Projectile.tileCollide = StateTimer == 1f;
                bool flag4 = chainLength <= launchRange;
                if (flag4 != Projectile.tileCollide)
                {
                    Projectile.tileCollide = flag4;
                    StateTimer = Projectile.tileCollide ? 1 : 0;
                    Projectile.netUpdate = true;
                }
                if (chainLength > unusedChainLength)
                {
                    if (chainLength >= launchRange)
                    {
                        Projectile.velocity *= 0.5f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaximumRetractionSpeed, unusedMaximumRetractionSpeed);
                    }
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaximumRetractionSpeed, unusedRetractionAcceleration);
                }
                else
                {
                    if (Projectile.velocity.Length() < 6f)
                    {
                        Projectile.velocity.X *= 0.96f;
                        Projectile.velocity.Y += 0.2f;
                    }
                    if (player.velocity.X == 0f)
                    {
                        Projectile.velocity.X *= 0.96f;
                    }
                }
                player.ChangeDir(player.Center.X < Projectile.Center.X ? 1 : -1);
                break;
            case AIState.ForcedRetracting:
                Projectile.tileCollide = false;
                Vector2 forcedRetractionDirection = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                if (Projectile.Distance(mountedCenter) <= maximumForcedRetractionSpeed)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity *= 0.98f;
                Projectile.velocity = Projectile.velocity.MoveTowards(forcedRetractionDirection * maximumForcedRetractionSpeed, forcedRetractionAcceleration);
                Vector2 target = Projectile.Center + Projectile.velocity;
                Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                if (Vector2.Dot(forcedRetractionDirection, value) < 0f)
                {
                    Projectile.Kill();
                    return;
                }
                player.ChangeDir(player.Center.X < Projectile.Center.X ? 1 : -1);
                break;
            case AIState.Ricochet:
                if (StateTimer++ >= ricochetMaxTime)
                {
                    CurrentAIState = AIState.Dropping;
                    StateTimer = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.localNPCHitCooldown = launchHitCooldown;
                    Projectile.velocity.Y += 0.6f;
                    Projectile.velocity.X *= 0.95f;
                    player.ChangeDir(player.Center.X < Projectile.Center.X ? 1 : -1);
                }
                break;
            case AIState.Dropping:
                if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDropRange)
                {
                    CurrentAIState = AIState.ForcedRetracting;
                    StateTimer = 0f;
                    Projectile.netUpdate = true;
                    break;
                }
                if (!Projectile.shimmerWet)
                {
                    Projectile.velocity.Y += 0.8f;
                }
                Projectile.velocity.X *= 0.95f;
                player.ChangeDir(player.Center.X < Projectile.Center.X ? 1 : -1);
                break;
        }

        Projectile.direction = Projectile.velocity.X > 0f ? 1 : -1;
        Projectile.spriteDirection = Projectile.direction;
        Projectile.ownerHitCheck = ownerHitCheck;
        if (shouldFreelyRotate)
        {
            if (Projectile.velocity.Length() > 1f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f;
            }
            else
            {
                Projectile.rotation += Projectile.velocity.X * 0.1f;
            }
        }
        Projectile.timeLeft = 2;
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
        if (Projectile.Center.X < mountedCenter.X)
        {
            player.itemRotation += (float)Math.PI;
        }
        player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

        ShowVisuals(doFastThrowDust);

        if (Projectile.shimmerWet)
        {
            ShimmerBehavior();
        }
    }

    private void ShimmerBehavior()
    {
        if (CurrentAIState == AIState.Retracting || CurrentAIState == AIState.ForcedRetracting)
        {
            return;
        }

        if (Projectile.velocity.Y > 0f)
        {
            Projectile.velocity.Y *= -1f;
            Projectile.netUpdate = true;
        }
        Projectile.velocity.Y -= 0.4f;
        if (Projectile.velocity.Y < -8f)
        {
            Projectile.velocity.Y = -8f;
        }
    }

    public override bool? CanDamage()
    {
        if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f) // Can't damage while spinning and hasn't done one full spin
        {
            return false;
        }
        return null;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (CurrentAIState == AIState.Spinning) // Hit anything in an elipsis around the player
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
            shortestVectorFromPlayerToTarget.Y /= 0.8f;
            float hitRadius = 55f;
            return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
        }
        return null;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        int hitCooldown = 10;
        int impactIntensity = 0;
        Vector2 velocity = Projectile.velocity;
        float bounceAmount = 0.2f;
        if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
        {
            bounceAmount = 0.4f;
        }
        if (CurrentAIState == AIState.Dropping)
        {
            bounceAmount = 0f;
        }

        if (oldVelocity.X != Projectile.velocity.X)
        {
            if (Math.Abs(oldVelocity.X) > 4f)
            {
                impactIntensity = 1;
            }
            Projectile.velocity.X = -oldVelocity.X * bounceAmount;
            CollisionCounter++;
        }

        if (oldVelocity.Y != Projectile.velocity.Y)
        {
            if (Math.Abs(oldVelocity.Y) > 4f)
            {
                impactIntensity = 1;
            }
            Projectile.velocity.Y = -oldVelocity.Y * bounceAmount;
            CollisionCounter++;
        }

        if (CurrentAIState == AIState.LaunchingForward)
        {
            CurrentAIState = AIState.Ricochet;
            Projectile.localNPCHitCooldown = hitCooldown;
            Projectile.netUpdate = true;
            Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
            Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
            impactIntensity = 2;
            Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out var causedShockwaves);
            Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
            Projectile.position -= velocity;
        }

        if (impactIntensity > 0)
        {
            Projectile.netUpdate = true;
            for (int i = 0; i < impactIntensity; i++)
            {
                Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }

        if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
        {
            CurrentAIState = AIState.ForcedRetracting;
            Projectile.netUpdate = true;
        }

        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (CurrentAIState == AIState.Spinning)
        {
            modifiers.SourceDamage *= 1.2f;
        }
        else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
        {
            modifiers.SourceDamage *= 2f;
        }

        modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

        if (CurrentAIState == AIState.Spinning)
        {
            modifiers.Knockback *= 0.25f;
        }
        else if (CurrentAIState == AIState.Dropping)
        {
            modifiers.Knockback *= 0.5f;
        }
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (CurrentAIState == AIState.Spinning)
        {
            modifiers.SourceDamage *= 1.2f;
        }
        else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
        {
            modifiers.SourceDamage *= 2f;
        }

        modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

        if (CurrentAIState == AIState.Spinning)
        {
            modifiers.Knockback *= 0.25f;
        }
        else if (CurrentAIState == AIState.Dropping)
        {
            modifiers.Knockback *= 0.5f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawChains(ref lightColor);

        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        Vector2 drawOrigin = frame.Size() * 0.5f;
        SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Vector2 drawPosition = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
        if (CurrentAIState == AIState.LaunchingForward)
        {
            Color afterimageColor = Projectile.GetAlpha(lightColor);
            afterimageColor.A = 127;
            afterimageColor *= 0.5f;

            int afterimageCount = Math.Min(5, (int)StateTimer);

            for (float i = 1f; i >= 0f; i -= 0.125f)
            {
                float afterimageAlpha = 1f - i;
                Vector2 positionOffset = Projectile.velocity * -afterimageCount * i;
                Main.spriteBatch.Draw(texture, drawPosition + positionOffset, frame, afterimageColor * afterimageAlpha, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, afterimageAlpha), spriteEffects, 0);
            }
        }

        Color mainColor = Projectile.GetAlpha(lightColor);
        Main.spriteBatch.Draw(texture, drawPosition, frame, mainColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

        return false;
    }

    private void DrawChains(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);
        playerArmPosition -= Vector2.UnitY * player.gfxOffY;

        Asset<Texture2D> texture = ModContent.Request<Texture2D>(ChainTexture, AssetRequestMode.ImmediateLoad);
        Rectangle? sourceRectangle = InitialChainSourceRectangle(texture);
        float chainHeightAdjustment = ChainHeightAdjustment;

        Vector2 textureOrigin = (sourceRectangle.HasValue ? (sourceRectangle.Value.Size() / 2f) : (texture.Size() / 2f));
        Vector2 chainDrawPosition = Projectile.Center;
        Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
        Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
        float chainSegmentLength = (sourceRectangle.HasValue ? sourceRectangle.Value.Height : texture.Height()) + chainHeightAdjustment;
        if (chainSegmentLength < 0)
        {
            chainSegmentLength = 10;
        }
        float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
        int chainCount = 0;

        float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

        while (chainLengthRemainingToDraw > 0f)
        {
            Color chainLightColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

            PreDrawChain(chainCount, ref texture, ref sourceRectangle, ref chainLightColor);

            Main.spriteBatch.Draw(texture.Value, chainDrawPosition - Main.screenPosition, sourceRectangle, chainLightColor, chainRotation, textureOrigin, 1f, SpriteEffects.None, 0f);

            chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
            chainCount++;
            chainLengthRemainingToDraw -= chainSegmentLength;
        }
    }

    #endregion

    #region Gameplay properties

    /// <summary>
    /// The maximum allowed time a flail can stay launched.
    /// </summary>
    /// <remarks>
    /// Also controls how far the flail can be from its owner when dropped.
    /// <br/>
    /// Overridden vanilla values:<br/>
    /// - <see cref="ProjectileID.Mace"/>, <see cref="ProjectileID.FlamingMace"/>, <see cref="ProjectileID.TheDaoofPow"/>, <see cref="ProjectileID.DripplerFlail"/>, <see cref="ProjectileID.FlowerPow"/>: 13<br/>
    /// - <see cref="ProjectileID.BallOHurt"/>, <see cref="ProjectileID.TheMeatball"/>, <see cref="ProjectileID.BlueMoon"/>, <see cref="ProjectileID.Sunfury"/>: 15<br/>
    /// </remarks>
    public virtual int MaxTimeLaunched => 10;

    /// <summary>
    /// The speed at which the flail will be launched.
    /// </summary>
    /// <remarks>
    /// Overridden vanilla values:<br/>
    /// - <see cref="ProjectileID.Mace"/>, <see cref="ProjectileID.FlamingMace"/>: 12<br/>
    /// - <see cref="ProjectileID.BallOHurt"/>: 14<br/>
    /// - <see cref="ProjectileID.TheMeatball"/>: 15<br/>
    /// - <see cref="ProjectileID.BlueMoon"/>: 16<br/>
    /// - <see cref="ProjectileID.Sunfury"/>: 17<br/>
    /// - <see cref="ProjectileID.TheDaoofPow"/>: 21<br/>
    /// - <see cref="ProjectileID.DripplerFlail"/>: 22<br/>
    /// - <see cref="ProjectileID.FlowerPow"/>: 23<br/>
    /// </remarks>
    public virtual float LaunchSpeed => 24f;

    /// <summary>
    /// The maximum allowed distance a flail can be from its owner when launched.
    /// </summary>
    /// <remarks>
    /// Also controls how far the flail can be from its owner when dropped.
    /// </remarks>
    public virtual float MaxDistanceLaunched => 800f;

    /// <summary>
    /// The speed the flail will attempt to reach when manually retracting the flail.
    /// </summary>
    /// <remarks>
    /// Also controls the distance from the player at which the flail disappears when retracting, but this shouldn't matter.
    /// <br/>
    /// Overridden vanilla values:<br/>
    /// - <see cref="ProjectileID.Mace"/>, <see cref="ProjectileID.FlamingMace"/>: 8<br/>
    /// - <see cref="ProjectileID.BallOHurt"/>: 10<br/>
    /// - <see cref="ProjectileID.TheMeatball"/>: 11<br/>
    /// - <see cref="ProjectileID.BlueMoon"/>: 12<br/>
    /// - <see cref="ProjectileID.Sunfury"/>: 14<br/>
    /// - <see cref="ProjectileID.TheDaoofPow"/>: 20<br/>
    /// - <see cref="ProjectileID.DripplerFlail"/>: 22<br/>
    /// </remarks>
    public virtual float MaxManualRetractionSpeed => 16f;

    /// <summary>
    /// How fast the flail will travel when manually retracting.
    /// </summary>
    public virtual float ManualRetractionAcceleration => 3f;

    /// <summary>
    /// The speed the flail will attempt to reach when the flail is being forcibly retracting.
    /// </summary>
    /// <remarks>
    /// Also controls the distance from the player at which the flail disappears when retracting, but this shouldn't matter.
    /// <br/>
    /// Overridden vanilla values:<br/>
    /// - <see cref="ProjectileID.Mace"/>, <see cref="ProjectileID.FlamingMace"/>: 13<br/>
    /// - <see cref="ProjectileID.BallOHurt"/>: 15<br/>
    /// - <see cref="ProjectileID.TheMeatball"/>, <see cref="ProjectileID.BlueMoon"/>: 16<br/>
    /// - <see cref="ProjectileID.Sunfury"/>: 18<br/>
    /// - <see cref="ProjectileID.TheDaoofPow"/>: 24<br/>
    /// - <see cref="ProjectileID.DripplerFlail"/>: 26<br/>
    /// </remarks>
    public virtual float MaxForcedRetractionSpeed => 48f;

    /// <summary>
    /// How fast the flail will travel when being forcibly retracted.
    /// </summary>
    public virtual float ForcedRetractionAcceleration => 6;

    /// <summary>
    /// The default value <see cref="Projectile.localNPCHitCooldown"/> is set to.
    /// </summary>
    public virtual int DefaultNPCHitCooldown => 10;

    /// <summary>
    /// The value <see cref="Projectile.localNPCHitCooldown"/> is set to when spinning.
    /// </summary>
    /// <remarks>
    /// Overridden vanilla values:<br/>
    /// - <see cref="ProjectileID.TheDaoofPow"/>, <see cref="ProjectileID.DripplerFlail"/>, <see cref="ProjectileID.FlowerPow"/>: 12<br/>
    /// </remarks>
    public virtual int SpinningNPCHitCooldown => 15;

    /// <summary>
    /// The value <see cref="Projectile.localNPCHitCooldown"/> is set to when being launched.
    /// </summary>
    public virtual int LaunchedNPCHitCooldown => 10;

    /// <summary>
    /// Whether the flail should freely rotate.
    /// </summary>
    public virtual bool ShouldFreelyRotate => true;

    #endregion

    #region Gameplay functions

    /// <summary>
    /// Called when the flail goes from being launched to being manually retracted.
    /// </summary>
    public virtual void OnManualRetract()
    {

    }

    /// <summary>
    /// Called when the flail goes from being launched to being dropped.
    /// </summary>
    public virtual void OnDropDuringLaunch()
    {

    }

    /// <summary>
    /// Called when the flail goes from being retracted to being dropped.
    /// </summary>
    public virtual void OnDropDuringManualRetract()
    {

    }

    /// <summary>
    /// Spawn effects that should appear near the flail.
    /// </summary>
    /// <param name="doFastThrowDust">Whether the flail is being launched. Flails being launched should generate bigger effects.</param>
    public virtual void ShowVisuals(bool doFastThrowDust)
    {

    }

    #endregion

    #region Rendering properties & functions

    /// <summary>
    /// The file name of the chain texture file in the file space.
    /// </summary>
    public virtual string ChainTexture => Texture + "_Chain";

    /// <summary>
    /// The initial source rectangle of the chain texture the chain will use.
    /// </summary>
    /// <param name="texture">The chain texture, provided for convenience.</param>
    /// <returns>The source rectangle. Return null to use the whole texture.</returns>
    public virtual Rectangle? InitialChainSourceRectangle(Asset<Texture2D> texture) => null;

    /// <summary>
    /// How much the chain's height should be offset. Use this to adjust the chain overlap.
    /// </summary>
    public virtual int ChainHeightAdjustment => 0;

    /// <summary>
    /// Lets you modify chain drawing before a chain gets drawn.
    /// </summary>
    /// <param name="chainCount">The amount of chains drawn. Used to alternate between or change frames/textures.</param>
    /// <param name="texture">The chain texture.</param>
    /// <param name="sourceRectangle">The source rectangle.</param>
    /// <param name="lighColor">The light color at the chain's position.</param>
    public virtual void PreDrawChain(int chainCount, ref Asset<Texture2D> texture, ref Rectangle? sourceRectangle, ref Color lighColor)
    {

    }

    #endregion
}