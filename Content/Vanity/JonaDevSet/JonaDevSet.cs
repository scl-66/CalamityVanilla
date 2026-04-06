using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Vanity.JonaDevSet;

[AutoloadEquip(EquipType.Head)]
public class JonaWig : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.FrontToBackID[Item.headSlot] = EquipLoader.GetEquipSlot(Mod, "JonaWigBack", EquipType.Head);
        ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
    }
    public override void Load()
    {
        EquipLoader.AddEquipTexture(Mod, Texture + "_HeadBack", EquipType.Head, null, "JonaWigBack");
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}
public class JonaWigEyesAndHairTip : PlayerDrawLayer
{
    private static Asset<Texture2D> _tex;
    public override bool IsHeadLayer => true;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.head == ContentSamples.ItemsByType[ModContent.ItemType<JonaWig>()].headSlot;
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    public override void Load()
    {
        _tex = ModContent.Request<Texture2D>(ModContent.GetInstance<JonaWig>().Texture + "_Extra");
    }
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Rectangle earFrame = new(20, 12, 8, 8);
        Rectangle hairFrame = new(20, 22, 8, 2);
        Rectangle frame = new(0, 12 * (int)drawInfo.drawPlayer.eyeHelper.CurrentEyeFrame, 8, 10);

        Vector2 offset = new Vector2(0, -12);
        offset += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        offset *= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1, drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1);
        Vector2 center = drawInfo.drawPlayer.MountedCenter + new Vector2(0, drawInfo.drawPlayer.gfxOffY);
        DrawData face = new(_tex.Value, new Vector2((int)center.X, (int)center.Y) - Main.screenPosition + offset, frame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, frame.Size() / 2, 1f, drawInfo.playerEffect);
        face.position = drawInfo.drawPlayer.RotatedRelativePoint(face.position, addGfxOffY: false);
        drawInfo.DrawDataCache.Add(face);

        DrawData faceGlow = face with { sourceRect = frame with { X = 10 } };
        faceGlow.color = new Color(Vector3.One - (Lighting.GetSubLight(drawInfo.drawPlayer.Center))) with { A = 0};
        drawInfo.DrawDataCache.Add(faceGlow);

        offset = new Vector2(-10, -13);
        offset += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        offset *= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1, drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1);
        DrawData ear = new(_tex.Value, new Vector2((int)center.X, (int)center.Y) - Main.screenPosition + offset, earFrame, drawInfo.colorBodySkin, drawInfo.drawPlayer.headRotation, earFrame.Size() / 2, 1f, drawInfo.playerEffect);
        ear.position = drawInfo.drawPlayer.RotatedRelativePoint(ear.position, addGfxOffY: false);
        drawInfo.DrawDataCache.Add(ear);


        offset = new Vector2(2, -34);
        offset += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        offset *= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1, drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1);
        DrawData hair = new(_tex.Value, new Vector2((int)center.X, (int)center.Y) - Main.screenPosition + offset, hairFrame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, hairFrame.Size() / 2, 1f, drawInfo.playerEffect);
        hair.position = drawInfo.drawPlayer.RotatedRelativePoint(hair.position, addGfxOffY: false);
        hair.shader = drawInfo.cHead;
        drawInfo.DrawDataCache.Add(hair);

        //offset = new Vector2(3, -11);
        //offset += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        //offset *= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1, drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1);

        //float brightness = (faceGlow.color.R + faceGlow.color.G + faceGlow.color.B) / 765f;

        //for (int i = 0; i < 2; i++)
        //{
        //    DrawData eyeSparkle = new(TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value, new Vector2((int)center.X, (int)center.Y) - Main.screenPosition + offset, null, faceGlow.color.MultiplyRGBA(new Color(1f,0f,0f,0f)) * brightness, drawInfo.drawPlayer.headRotation + (MathHelper.PiOver2 * i), TextureAssets.Extra[ExtrasID.ThePerfectGlow].Size() / 2, new Vector2(0.3f,0.4f - (i * 0.2f)) * brightness, drawInfo.playerEffect);
        //    eyeSparkle.position = drawInfo.drawPlayer.RotatedRelativePoint(eyeSparkle.position, addGfxOffY: false);
        //    eyeSparkle.scale *= new Vector2(1f + MathF.Sin(((float)Main.timeForVisualEffects * 0.01f) + i * 8) * 0.3f, 0.9f + MathF.Sin(((float)Main.timeForVisualEffects * 0.075f) + i * 7) * 0.4f);
        //    drawInfo.DrawDataCache.Add(eyeSparkle);

        //    DrawData face4 = eyeSparkle with { color = new Color(brightness, brightness, brightness, 0) * 0.5f };
        //    face4.scale *= new Vector2(0.4f,0.8f);
        //    drawInfo.DrawDataCache.Add(face4);
        //}

        //for (int i = 0; i < 4; i++)
        //{
        //    DrawData face3 = face2;
        //    face3.sourceRect = new Rectangle(16,frame.Y,2,10);
        //    float amount = (int)(Main.timeForVisualEffects % 60) / 60f;
        //    face3.color *= 1f - amount;
        //    face3.position += new Vector2(0,amount * 2).RotatedBy(i * MathHelper.PiOver2) * new Vector2(1,2);
        //    drawInfo.DrawDataCache.Add(face3);
        //}
    }
}
public class JonaWigOverShoulderHair : PlayerDrawLayer
{
    private static Asset<Texture2D> _tex;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.head == ContentSamples.ItemsByType[ModContent.ItemType<JonaWig>()].headSlot && drawInfo.drawPlayer.bodyFrame.Y is > 335 or 0;
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);
    public override void Load()
    {
        _tex = ModContent.Request<Texture2D>(ModContent.GetInstance<JonaWig>().Texture + "_Extra");
    }
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Rectangle frame = new(20, 0, 10, drawInfo.drawPlayer.bodyFrame.Y == 0 ? 10 : 6);

        Vector2 headOffset = new Vector2(-3, -4);
        headOffset += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        headOffset *= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1, drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1);

        Vector2 center = drawInfo.drawPlayer.MountedCenter + new Vector2(0, drawInfo.drawPlayer.gfxOffY);
        DrawData face = new(_tex.Value, new Vector2((int)center.X, (int)center.Y) - Main.screenPosition + headOffset, frame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, new Vector2(5), 1f, drawInfo.playerEffect);
        face.position = drawInfo.drawPlayer.RotatedRelativePoint(face.position, addGfxOffY: false);
        face.shader = drawInfo.cHead;
        drawInfo.DrawDataCache.Add(face);
    }
}

[AutoloadEquip(EquipType.Body)]
public class JonaBody : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}

[AutoloadEquip(EquipType.Legs)]
public class JonaLegs : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}
public class JonaLegsLayer : PlayerDrawLayer
{
    private static Asset<Texture2D> _tex;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.legs == ContentSamples.ItemsByType[ModContent.ItemType<JonaLegs>()].legSlot;
    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Leggings);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.isSitting)
        {
            DrawSittingLegs(ref drawInfo, TextureAssets.Players[0, 10].Value, drawInfo.colorLegs);
            return;
        }

        DrawData item = new DrawData(TextureAssets.Players[0, 10].Value, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2), drawInfo.drawPlayer.legFrame, drawInfo.colorLegs, drawInfo.drawPlayer.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(item);
    }
    private static void DrawSittingLegs(ref PlayerDrawSet drawinfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex = 0, bool glowmask = false)
    {
        Vector2 legsOffset = drawinfo.legsOffset;
        Vector2 vector = new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.legFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.legFrame.Height + 4f)) + drawinfo.drawPlayer.legPosition + drawinfo.legVect;
        Rectangle legFrame = drawinfo.drawPlayer.legFrame;
        vector.Y -= 2f;
        vector.Y += drawinfo.seatYOffset;
        vector += legsOffset;
        int num = 2;
        int num2 = 42;
        int num3 = 2;
        int num4 = 2;
        int num5 = 0;
        int num6 = 0;
        int num7 = 0;
        bool flag = drawinfo.drawPlayer.legs == 101 || drawinfo.drawPlayer.legs == 102 || drawinfo.drawPlayer.legs == 118 || drawinfo.drawPlayer.legs == 99;
        if (drawinfo.drawPlayer.wearsRobe && !flag)
        {
            num = 0;
            num4 = 0;
            num2 = 6;
            vector.Y += 4f;
            legFrame.Y = legFrame.Height * 5;
        }
        for (int num8 = num3; num8 >= 0; num8--)
        {
            Vector2 position = vector + new Vector2(num, 2f) * new Vector2(drawinfo.drawPlayer.direction, 1f);
            Rectangle value = legFrame;
            value.Y += num8 * 2;
            value.Y += num2;
            value.Height -= num2;
            value.Height -= num8 * 2;
            if (num8 != num3)
                value.Height = 2;

            position.X += drawinfo.drawPlayer.direction * num4 * num8 + num6 * drawinfo.drawPlayer.direction;
            if (num8 != 0)
                position.X += num7 * drawinfo.drawPlayer.direction;

            position.Y += num2;
            position.Y += num5;
            DrawData item = new DrawData(textureToDraw, position, value, matchingColor, drawinfo.drawPlayer.legRotation, drawinfo.legVect, 1f, drawinfo.playerEffect);
            item.shader = shaderIndex;
            drawinfo.DrawDataCache.Add(item);
        }
    }
}
