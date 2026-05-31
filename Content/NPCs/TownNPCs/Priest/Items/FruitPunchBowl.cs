using CalamityVanilla.Content.Dusts;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items;

public class FruitPunchBowlEntity : ModTileEntity
{
    public int potionType;
    public bool filled;
    public Color juiceColor;
    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        //The MyTile class is shown later
        return tile.HasTile && tile.TileType == ModContent.TileType<FruitPunchBowlTile>();
    }
}
public class FruitPunchBowlTile : ModTile
{
    public bool heldItemIsPotionable;

    public override void SetStaticDefaults()
    {
        HitSound = SoundID.Shatter;
        DustType = DustID.Glass;
        AddMapEntry(new Color(219, 226, 225), Language.GetText("Mods.CalamityVanilla.Items.FruitPunchBowl.DisplayName"));
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.Style2x1.Width, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<FruitPunchBowlEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<FruitPunchBowlEntity>().Kill(i, j);
    }

    public override void MouseOver(int i, int j)
    {
        if (TileEntity.TryGet(i, j, out FruitPunchBowlEntity entity))
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;
            bool hasBottleInRecipe = RecipeLoader.FirstRecipeForItem[item.type].HasIngredient(ItemID.BottledWater) || RecipeLoader.FirstRecipeForItem[item.type].HasIngredient(ItemID.Bottle);

            heldItemIsPotionable = item.consumable & hasBottleInRecipe;
            //Main.NewText(itemName);
            entity.potionType = player.HeldItem.netID;
            player.cursorItemIconEnabled = true;
            if (!entity.filled)
            {
                if (!heldItemIsPotionable)
                {
                    player.cursorItemIconID = ItemID.BottledWater;
                }
                else
                {
                    player.cursorItemIconID = entity.potionType;
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
        if (TileEntity.TryGet(i, j, out FruitPunchBowlEntity tileEntity) && !tileEntity.filled)
        {
            if (heldItemIsPotionable)
            {
                //Main.NewText(player.HeldItem.Name);
                Vector2 tileCoords = new Vector2(i, j).ToWorldCoordinates();
                SoundEngine.PlaySound(SoundID.SplashWeak with { MaxInstances = 0 }, tileCoords);
                for (int k = 0; k < Main.rand.Next(3, 7); k++)
                {
                    Dust d = Dust.NewDustDirect(tileCoords - new Vector2(0, 16), 16, 16, DustID.Water, 0, Main.rand.NextFloat(-5f, -1.5f));
                    d.scale = Main.rand.NextFloat(0.5f, 0.75f);
                }
                tileEntity.filled = true;
                return true;
            }
            else
            {
                //Main.NewText("does not has potion");
                return false;
            }
        } else
        {
            if (!heldItemIsPotionable)
            {
                Vector2 tileCoords = new Vector2(i, j).ToWorldCoordinates();
                SoundEngine.PlaySound(SoundID.Item3 with { MaxInstances = 0 }, tileCoords);
                for (int k = 0; k < Main.rand.Next(3, 7); k++)
                {
                    Dust d = Dust.NewDustDirect(tileCoords - new Vector2(0, 16), 16, 16, DustID.Water, 0, Main.rand.NextFloat(-5f, -1.5f));
                    d.scale = Main.rand.NextFloat(0.5f, 0.75f);
                }
                tileEntity.filled = false;
                return true;
            }
        }
        return false;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (TileEntity.TryGet(i, j, out FruitPunchBowlEntity entity) && entity.filled)
        {
            var tex = ModContent.Request<Texture2D>("CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/FruitPunchBowlTile_Juice").Value;
            entity.juiceColor = ItemRarity.GetColor(ContentSamples.ItemsByType[entity.potionType].rare);
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
                    tex, drawCoords, frame, Lighting.GetColor(i, j).MultiplyRGBA(entity.juiceColor), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                );
            spriteBatch.End(out var sbSnapshot);
            spriteBatch.Begin(sbSnapshot);
        }
    }
}

public class FruitPunchBowl : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<FruitPunchBowlTile>());
        Item.value = Item.buyPrice(0, 0, 15, 0);
    }
}


