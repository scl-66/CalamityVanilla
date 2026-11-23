using CalamityVanilla.Common.Extensions;
using CalamityVanilla.Common.Interfaces;
using CalamityVanilla.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.GutOfCthulhu;

public partial class GutOfCthulhu : ModNPC, ICustomTrailingMode
{
    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Width = TextureAssets.Npc[Type].Width() / 2;
    }
    private static Vector2 FindSpotOnBody(Vector2 vector, DrawData body)
    {
        return (body.position + vector - body.origin).RotatedBy(body.rotation, body.position);
    }
    private bool _afterImageActive = false;
    private float _afterImageOpacity = 0f;

    private void DrawChoppedAndLighted(DrawData drawData)
    {
        Rectangle rect = drawData.sourceRect.Value;
        int segmentSize = (int)(24 - 16 * Main.gfxQuality);
        int xSegments = rect.Width / segmentSize;
        int ySegments = rect.Height / segmentSize;
        Color effectColor = NPC.GetColorTint(Color.White) * NPC.Opacity;
        for (int x = 0; x < xSegments; x++)
        {
            for (int y = 0; y < ySegments; y++)
            {
                DrawData d = drawData with
                {
                    sourceRect = new Rectangle(rect.X + x * segmentSize, rect.Y + y * segmentSize, segmentSize, segmentSize),
                    position = drawData.position + new Vector2(x * segmentSize, y * segmentSize).RotatedBy(drawData.rotation),
                };
                d.color = new Color(Lighting.GetSubLight(d.position + Main.screenPosition - d.origin.RotatedBy(drawData.rotation))).MultiplyRGBA(effectColor);
                Main.EntitySpriteDraw(d);
            }
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var bodyTex = TextureAssets.Npc[Type].Value;
        var extraTex = AssetReferences.Content.Crimson.GutOfCthulhu.GutOfCthuluExtra.Asset.Value;

        // eye positions are from top to bottom. eyeball rects is the rectangle around the eyeball with 2 pixel margins
        Vector2[] eyePositions = { new(160, 28), new(166, 68), new(154, 118) };
        int[] eyeMoveDistance = { 6, 11, 9 };
        Rectangle[] eyeballRects = { new Rectangle(422, 12, 50, 34), new Rectangle(420, 46, 66, 46), new Rectangle(414,98,54,40) };
        DrawData body = new(bodyTex, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, new Vector2(138,136), 1, SpriteEffects.None);

        Color effectColor = NPC.GetColorTint(Color.White) * NPC.Opacity;

        Color afterimageColor = Color.Lerp(drawColor, Color.Black, 0.25f).MultiplyRGBA(effectColor) * 0.25f * _afterImageOpacity;

        if (_afterImageActive)
        {
            for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
            {
                float percent = 1f - (i / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(body with { position = NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition, rotation = NPC.oldRot[i], color = afterimageColor * percent });
            }
        }
        //Main.EntitySpriteDraw(body);
        for (int i = 0; i < 3; i++)
        {
            DrawData eyeball = body with { sourceRect = eyeballRects[i], position = FindSpotOnBody(eyeballRects[i].TopLeft() + new Vector2(-150, 136), body) };
            Vector2 eyePos = FindSpotOnBody(eyePositions[i], body);
            eyeball.color = new Color(Lighting.GetSubLight(eyePos + Main.screenPosition)).MultiplyRGBA(effectColor);
            int eyeFrame = (int)((Main.GlobalTimeWrappedHourly * 10) + i * 5) % 3;
            if (NPC.HasValidTarget)
                eyePos += ((target.Center - (eyePos + Main.screenPosition)) * 0.1f).LengthClamp(eyeMoveDistance[i]);
            Main.EntitySpriteDraw(eyeball);
            Main.EntitySpriteDraw(extraTex, eyePos, new Rectangle(0, 20 * eyeFrame, 18, 20), eyeball.color, body.rotation, new Vector2(9), 1, SpriteEffects.None);
        }
        //Main.EntitySpriteDraw(body with { sourceRect = NPC.frame with { X = NPC.frame.Width } });
        DrawChoppedAndLighted(body);
        return false;
    }
    public void UpdateTrails(NPC npc)
    {
        if (!_afterImageActive)
        {
            Array.Fill(npc.oldPos, Vector2.Zero);
            Array.Fill(npc.oldRot, 0);
            return;
        }
        npc.localAI[0]++;
        if (npc.localAI[0] < 3)
            return;
        npc.localAI[0] = 0;
        for (int num6 = npc.oldPos.Length - 1; num6 > 0; num6--)
        {
            npc.oldPos[num6] = npc.oldPos[num6 - 1];
            npc.oldRot[num6] = npc.oldRot[num6 - 1];
        }
        npc.oldPos[0] = npc.position + npc.netOffset;
        npc.oldRot[0] = npc.rotation;
        _afterImageOpacity -= 0.1f;
        if (_afterImageOpacity <= 0)
            _afterImageActive = false;
    }
}
