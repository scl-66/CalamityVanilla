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
    public override bool PreDrawExtras()
    {
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