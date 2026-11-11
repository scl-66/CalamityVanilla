using CalamityVanilla.Common.Extensions;
using CalamityVanilla.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen;

public partial class Cryogen : ModNPC
{
    public enum GlowAnimationType
    {
        Nothing,
        Forwards,
        Backwards,
        Laser
    }
    public struct GlowAnimation()
    {
        public GlowAnimationType Type = GlowAnimationType.Nothing;
        private int _frame = -1;
        private int _frameTimer = 0;
        public GlowAnimation(GlowAnimationType type) : this()
        {
            Type = type;
        }

        public void Draw(DrawData d, Color color)
        {
            if (Type == GlowAnimationType.Nothing || _frame == -1)
                return;
            Texture2D tex = AssetReferences.Content.Tundra.Cryogen.CryogenGlow.Asset.Value;
            Main.EntitySpriteDraw(d with { color = color, sourceRect = tex.Frame(1, 11, 0, _frame), texture = tex });
            if (Type == GlowAnimationType.Laser)
            {
                Main.EntitySpriteDraw(d with { color = color * 0.6f, sourceRect = tex.Frame(1, 11, 0, 10), texture = tex });
            }
        }
        public void Update() 
        {
            if (Type == GlowAnimationType.Nothing)
                return;
            switch (Type)
            {
                case GlowAnimationType.Forwards:
                    _frame = (_frameTimer / 2) % 10;
                    if (_frameTimer >= 20)
                        Type = GlowAnimationType.Nothing;
                    break;
                case GlowAnimationType.Backwards:
                    _frame = 9 - (_frameTimer / 2) % 10;
                    if (_frameTimer >= 20)
                        Type = GlowAnimationType.Nothing;
                    break;
                case GlowAnimationType.Laser:
                    _frame = 3 + ((_frameTimer / 2) % 5);
                    break;
            }
            _frameTimer++;
        }
    }
    private GlowAnimation _glowAnimation = new(GlowAnimationType.Nothing);
    private float _snowOverlayOpacity = 0f;
    private int _snowOverlaySpinDirection = 1;
    private Color _glowColor = new Color(1f, 1f, 1f, 0f);
    public static Color[] AuroraColors = [new Color(255, 0, 0), new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 255, 255), new Color(0, 0, 255)];
    public static Color GetAuroraColor(int time)
    {
        Color c = AuroraColors.CycleThroughColors(time, 60);
        byte lightness = (byte)(Utils.Remap(Utils.PingPongFrom01To010((time % 200) / 200f),0,1,25,148));
        return new Color(Math.Max(c.R,lightness), Math.Max(c.G, lightness), Math.Max(c.B, lightness),255);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Asset<Texture2D> backTexture = AssetReferences.Content.Tundra.Cryogen.CryogenFlake.Asset;
        Asset<Texture2D> tex = TextureAssets.Npc[Type];
        Rectangle bigFlake = new Rectangle(0, 0, 270, 270);
        Rectangle smallFlake = new Rectangle(0, 272, 126, 126);
        Rectangle SnowOverlay = new Rectangle(272, 0, 212, 208);
        //Flakes
        Color tint = NPC.GetColorTint(Color.White);
        Color baseColor = Color.Lerp(Color.White, new Color(0.7f, 0.7f, 0.9f), _snowOverlayOpacity).MultiplyRGBA(tint);

        DrawData body = new(tex.Value, NPC.Center - Main.screenPosition, NPC.frame, baseColor, NPC.velocity.X * 0.03f, NPC.frame.Size() / 2, 1, SpriteEffects.None, 0);

        // snow behind
        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, SnowOverlay, tint * _snowOverlayOpacity * 0.75f, (float)Main.timeForVisualEffects * -0.075f * _snowOverlaySpinDirection, SnowOverlay.Size() / 2, 1.4f, SpriteEffects.None, 0);

        // afterimage

        if (_snowOverlayOpacity > 0)
        {
            for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
            {
                float percent = (i + (NPC.localAI[0] / 3f)) / (float)NPC.oldPos.Length;
                Color afterColor = Color.Lerp(Color.White, GetAuroraColor((int)(Main.timeForVisualEffects + (percent * 100))), percent * 2) with { A = 128 } * (Math.Min(_snowOverlayOpacity,1) * 1f - percent);
                //Color afterColor = GetAuroraColor((int)(Main.timeForVisualEffects + (percent * 300))) with { A = 128 } * (Math.Min(_snowOverlayOpacity, 1) * 1f - percent) * 0.5f;
                float sizeMult = (1f - percent * 0.5f);

                Main.EntitySpriteDraw(body with { position = NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition, color = baseColor.MultiplyRGBA(afterColor with { A = (byte)(afterColor.A * 2)}), scale = body.scale * sizeMult });
                spriteBatch.Draw(backTexture.Value, NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition, bigFlake, baseColor.MultiplyRGBA(afterColor) * 0.5f, NPC.oldRot[i], bigFlake.Size() / 2, sizeMult, SpriteEffects.None, 0);
                spriteBatch.Draw(backTexture.Value, NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition, smallFlake, baseColor.MultiplyRGBA(afterColor) * 0.7f, -NPC.oldRot[i], smallFlake.Size() / 2, sizeMult, SpriteEffects.None, 0);
            }
        }

        //for (int i = 0; i < 4; i++)
        //{
        //    spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition + new Vector2((float)Math.Sin(Main.timeForVisualEffects * 0.03f) * 8).RotatedBy(MathHelper.PiOver2 * i), bigFlake, baseColor * 0.2f, NPC.rotation, bigFlake.Size() / 2, 1, SpriteEffects.None, 0);
        //}

        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, bigFlake, baseColor * 0.5f, NPC.rotation, bigFlake.Size() / 2, 1, SpriteEffects.None, 0);

        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, smallFlake, baseColor * 0.7f, -NPC.rotation, smallFlake.Size() / 2, 1, SpriteEffects.None, 0);

        // The Hexagon
        Main.EntitySpriteDraw(body);
        _glowAnimation.Draw(body, _glowColor);

        // snow overlay
        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, SnowOverlay, tint * _snowOverlayOpacity, (float)Main.timeForVisualEffects * -0.1f * _snowOverlaySpinDirection, SnowOverlay.Size() / 2, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, SnowOverlay, tint * _snowOverlayOpacity, (float)Main.timeForVisualEffects * -0.05f * _snowOverlaySpinDirection, SnowOverlay.Size() / 2, 1.2f, SpriteEffects.None, 0);
        return false;
    }
    private void UpdateAfterimages()
    {
        NPC.localAI[0]++;
        if (NPC.localAI[0] < 3)
            return;
        NPC.localAI[0] = 0;
        for (int num6 = NPC.oldPos.Length - 1; num6 > 0; num6--)
        {
            NPC.oldPos[num6] = NPC.oldPos[num6 - 1];
            NPC.oldRot[num6] = NPC.oldRot[num6 - 1];
        }
        NPC.oldPos[0] = NPC.position + NPC.netOffset;
        NPC.oldRot[0] = NPC.rotation;
    }
}
