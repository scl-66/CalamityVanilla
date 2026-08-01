using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.Crystaline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.Dungeon.Items.CandleSentry;

public class CandleSentryTile : ModTile
{
    private Asset<Texture2D> flameTexture;
    public override void SetStaticDefaults()
    {
        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.WaterCandle, 0));
        Main.tileNoAttach[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileWaterDeath[Type] = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.DisableSmartInteract[Type] = true;
        Main.tileLighted[Type] = true;

        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.Table, TileObjectData.newTile.Width, 0);
        TileObjectData.addTile(Type);

        DustType = DustID.WaterCandle;
        AddMapEntry(new Color(89, 201, 255), ModContent.GetInstance<CandleSentryItem>().DisplayName);

        flameTexture = ModContent.Request<Texture2D>(Texture + "_Flame");
    }
    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ModContent.ItemType<CandleSentryItem>());
    }
    public override bool RightClick(int i, int j)
    {
        WorldGen.KillTile(i, j);
        return true;
    }
    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<CandleSentryItem>();
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (!visible)
        {
            return;
        }

        if (Main.rand.NextBool(2))
        {
            int d = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 - 4), 4, 4, DustID.DungeonWater, 0f, 0f, 100);
            if (Main.rand.Next(3) == 0)
            {
                Main.dust[d].scale = 0.5f;
            }
            else
            {
                Main.dust[d].scale = 0.9f;
                Main.dust[d].noGravity = true;
            }
            Main.dust[d].velocity *= 0.3f;
            Main.dust[d].velocity.Y -= 1.5f;
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0f;
        g = 0.35f;
        b = 0.8f;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        var tile = Main.tile[i, j];

        if (!TileDrawing.IsVisible(tile))
        {
            return;
        }

        // The following code draws multiple flames on top our placed torch.

        int offsetX = 1;
        int offsetY = -3;

        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

        ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.
        Color color = new Color(100, 100, 100, 0);
        int width = 20;
        int height = 20;
        int frameX = tile.TileFrameX;
        int frameY = tile.TileFrameY;

        for (int k = 0; k < 7; k++)
        {
            float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
            float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

            spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx + offsetX, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
        }
    }
}

public class CandleSentryTileItem : ModItem
{
    public override string Texture => ModContent.GetModTile(ModContent.TileType<CandleSentryTile>()).Texture;

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<CandleSentryTile>(), 0);
        Item.value = 0;
    }
}