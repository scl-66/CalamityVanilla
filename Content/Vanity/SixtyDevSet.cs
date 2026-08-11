using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Vanity;

[AutoloadEquip(EquipType.Head)]
public class SixtyHead : ModItem
{
    public override string Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyHead.KEY;

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}

[AutoloadEquip(EquipType.Body)]
public class SixtyBody : ModItem
{
    public override string Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyBody.KEY;

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}

public class SixtyLegsLongCoat : PlayerDrawLayer
{
    public static Asset<Texture2D> Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyBody_LongCoat.Asset;

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmorLongCoat);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.body == ContentSamples.ItemsByType[ModContent.ItemType<SixtyBody>()].bodySlot;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.invis)
            return;
        if (!drawInfo.isSitting)
        {
            DrawData item = new DrawData(Texture.Value, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.legFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.legFrame.Height + 4f)) + drawInfo.drawPlayer.legPosition + drawInfo.legVect, drawInfo.drawPlayer.legFrame, drawInfo.colorArmorBody, drawInfo.drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect);
            item.shader = drawInfo.cBody;
            drawInfo.DrawDataCache.Add(item);
        }
        else
        {
            PlayerDrawLayers.DrawSittingLongCoats(ref drawInfo, 0, Texture.Value, drawInfo.colorArmorBody, drawInfo.cBody, false);
        }
    }
}

[AutoloadEquip(EquipType.Legs)]
public class SixtyLegs : ModItem
{
    public override string Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyLegs.KEY;

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.vanity = true;
        Item.value = Item.sellPrice(0, 5);
    }
}

public class SixtyLegsBuckle : PlayerDrawLayer
{
    public static Asset<Texture2D> Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyLegs_Buckle.Asset;

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.legs == ContentSamples.ItemsByType[ModContent.ItemType<SixtyLegs>()].legSlot;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.invis)
            return;
        if (!drawInfo.isSitting)
        {
            DrawData item = new DrawData(Texture.Value, drawInfo.legsOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.legFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.legFrame.Height + 4f)) + drawInfo.drawPlayer.legPosition + drawInfo.legVect, drawInfo.drawPlayer.legFrame, drawInfo.colorArmorLegs, drawInfo.drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect);
            item.shader = drawInfo.cLegs;
            drawInfo.DrawDataCache.Add(item);
        }
        //else
        //{
        //    PlayerDrawLayers.DrawSittingLegs(ref drawInfo, Texture.Value, drawInfo.colorArmorLegs, drawInfo.cLegs, false);
        //}
    }
}
[AutoloadEquip(EquipType.Wings)]
public class SixtyWings : ModItem
{
    public override string Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyWings.KEY;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = ArmorIDs.Wing.Sets.Stats[ArmorIDs.Wing.RedsWings];
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 8;
        Item.accessory = true;
        Item.rare = ItemRarityID.Cyan;
        Item.value = 400000;
    }

    public override void UpdateEquip(Player player)
    {
        player.flapSound = true;
    }

    public override void UpdateVanity(Player player)
    {
        player.flapSound = true;
    }
}
public class SixtyWingsLayer : PlayerDrawLayer
{
    public static Asset<Texture2D> Texture => Assets.Textures.Vanity.SixtyDevSet.SixtyWings.Asset;

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.wings == ContentSamples.ItemsByType[ModContent.ItemType<SixtyWings>()].wingSlot;
    }
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.invis)
            return;

        Vector2 drawPos = new Vector2((int)drawInfo.Center.X, (int)drawInfo.Center.Y + drawInfo.seatYOffset) - Main.screenPosition;
        Vector2 flip = new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? -1 : 1, drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? -1 : 1);
        if (drawInfo.drawPlayer.head == ContentSamples.ItemsByType[ModContent.ItemType<SixtyHead>()].headSlot)
            drawPos += new Vector2(-9, -27) * flip;
        else
            drawPos += new Vector2(-8, -24) * flip;

        drawPos.X += (float)Math.Sin(Main.timeForVisualEffects * 0.009f) * flip.X * 2 - 2;
        drawPos.Y += (float)Math.Sin(Main.timeForVisualEffects * 0.01f) * flip.Y * 1 - 1;
        drawPos.Y += drawInfo.mountOffSet / 2; // why is this needed???
        DrawData item = new DrawData(Texture.Value, drawPos, null, drawInfo.colorArmorBody, 0, Texture.Size() / 2, 1, drawInfo.playerEffect);
        item.shader = drawInfo.cWings;
        drawInfo.DrawDataCache.Add(item);
    }
}