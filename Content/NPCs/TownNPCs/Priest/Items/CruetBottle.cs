using CalamityVanilla.Common;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items;

public class CruetBottleEntity : ModTileEntity
{
    public int potionType;
    public bool filled;
    public Color? juiceColor;
    public float timer = 0;
    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        //The MyTile class is shown later
        return tile.HasTile && tile.TileType == ModContent.TileType<CruetBottleTile>();
    }

    public override void Update()
    {
        if (!filled)
        {
            juiceColor = null;
        }
        else
        {
            FruitPunchItemSet.GetPotionColor(potionType, out Color? color1, out Color? color2);
            timer += 0.01f;
            float siner = Utils.Remap((float)Math.Sin(timer * 5f) / 4, -0.6f, 0.6f, 0, 1);

            juiceColor = Color.Lerp((Color)color1, (Color)color2, siner);
        }
    }
}
public class CruetBottleTile : ModTile
{
    public bool heldItemIsPotionable;

    public override void SetStaticDefaults()
    {
        HitSound = SoundID.Shatter;
        DustType = DustID.Glass;
        AddMapEntry(new Color(219, 226, 225), Language.GetText("Mods.CalamityVanilla.Items.CruetBottle.DisplayName"));
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.Style1x2.Width, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<CruetBottleEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<CruetBottleEntity>().Kill(i, j);
    }

    public override void MouseOver(int i, int j)
    {
        if (TileEntity.TryGet(i, j, out CruetBottleEntity entity))
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;
            heldItemIsPotionable = FruitPunchItemSet.PunchAccepts[item.type];
            //Main.NewText(itemName);
            player.cursorItemIconEnabled = true;
            if (!entity.filled)
            {
                if (!heldItemIsPotionable)
                {
                    player.cursorItemIconID = ItemID.BottledWater;
                }
                else
                {
                    player.cursorItemIconID = item.type;
                }
            }
            else
            {
                player.cursorItemIconID = ItemID.Bottle;
            }
        }
    }

    public override bool RightClick(int i, int j)
    {
        if (TileEntity.TryGet(i, j, out CruetBottleEntity entity))
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;

            //fill with potion
            if (!entity.filled)
            {
                if (heldItemIsPotionable)
                {
                    //Main.NewText(player.HeldItem.Name);
                    entity.potionType = item.type;
                    Vector2 tileCoords = new Vector2(i, j).ToWorldCoordinates();
                    SoundEngine.PlaySound(SoundID.SplashWeak with { MaxInstances = 0 }, tileCoords);
                    for (int k = 0; k < Main.rand.Next(3, 7); k++)
                    {
                        FruitPunchItemSet.GetPotionColor(entity.potionType, out Color? color1, out Color? color2);
                        Dust d = Dust.NewDustDirect(tileCoords - new Vector2(0, 16), 16, 16, DustID.Water, 0, Main.rand.NextFloat(-5f, -1.5f));
                        d.scale = Main.rand.NextFloat(0.5f, 0.75f);
                        d.color = (Color)color1;
                    }
                    entity.filled = true;
                    if (--item.stack < 0)
                        item.TurnToAir();
                    player.QuickSpawnItem(player.GetSource_FromThis(), ItemID.Bottle);
                    //FruitPunchItemSet.GetPotionColor(entity.potionType, out Color? color1, out Color? color2);
                    return true;
                }
            }
            else
            {
                //empty potion into bottle
                Vector2 tileCoords = new Vector2(i, j).ToWorldCoordinates();
                if (item.type == ItemID.Bottle)
                {
                    SoundEngine.PlaySound(SoundID.SplashWeak with { MaxInstances = 0 }, tileCoords);
                    for (int k = 0; k < Main.rand.Next(3, 7); k++)
                    {
                        FruitPunchItemSet.GetPotionColor(entity.potionType, out Color? color1, out Color? color2);
                        Dust d = Dust.NewDustDirect(tileCoords - new Vector2(0, 16), 16, 16, DustID.Water, 0, Main.rand.NextFloat(-5f, -1.5f));
                        d.scale = Main.rand.NextFloat(0.5f, 0.75f);
                        d.color = (Color)color1;
                    }
                    entity.filled = false;
                    if (--item.stack < 0)
                        item.TurnToAir();
                    player.QuickSpawnItem(player.GetSource_FromThis(), entity.potionType);
                    return true;
                }
                //drink potion
                else if (!heldItemIsPotionable && !(new Item(entity.potionType).potion && player.potionDelay > 0))
                {
                    if (player.inventory[58] == null || player.inventory[58].IsAir && player.ItemTimeIsZero)
                    {
                        entity.filled = false;
                        Item drunkItem = new Item(entity.potionType)
                        {
                            noUseGraphic = true
                        };
                        SoundEngine.PlaySound(SoundID.Item3 with { MaxInstances = 0 }, tileCoords);

                        //dust
                        for (int k = 0; k < Main.rand.Next(3, 7); k++)
                        {
                            FruitPunchItemSet.GetPotionColor(entity.potionType, out Color? color1, out Color? color2);
                            Dust d = Dust.NewDustDirect(tileCoords - new Vector2(0, 16), 16, 16, DustID.Water, 0, Main.rand.NextFloat(-5f, -1.5f));
                            d.scale = Main.rand.NextFloat(0.5f, 0.75f);
                            d.color = (Color)color1;
                        }

                        int previousSelected = player.selectedItem;
                        player.inventory[58] = drunkItem;
                        player.selectedItem = 58;
                        player.controlUseItem = true;
                        player.ItemCheck();
                        player.controlUseItem = false;

                        //Main.NewText((drunkItem.ToString() + " " + player.ToString()));
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (TileEntity.TryGet(i, j, out CruetBottleEntity entity) && entity.filled)
        {
            var tex = ModContent.Request<Texture2D>("CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CruetBottleTile_Juice").Value;
            if (entity.juiceColor == null)
            {
                entity.juiceColor = Color.White;
            }
            Tile tile = Framing.GetTileSafely(i, j);
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            Rectangle frame = new(frameX, frameY, tex.Width, tex.Height);

            Vector2 offsetZero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Vector2 location = new Vector2(i, j).ToWorldCoordinates(0, 0);
            Vector2 offsets = -Main.screenPosition + offsetZero;
            Vector2 drawCoords = location + offsets;

            spriteBatch.Draw
                (
                    tex, drawCoords, frame, Lighting.GetColor(i, j).MultiplyRGBA((Color)entity.juiceColor), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                );
            spriteBatch.End(out var sbSnapshot);
            spriteBatch.Begin(sbSnapshot);
        }
    }
}

public class CruetBottle : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<CruetBottleTile>());
        Item.value = Item.buyPrice(0, 0, 15, 0);
    }
}