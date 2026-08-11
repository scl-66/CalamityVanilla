using Microsoft.Xna.Framework;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.Underground.Items.Tomes;

public class GraniteTomeObjectItem : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Granite}";

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<GraniteTomeObject>(), 0);
    }
}

public class GraniteTomeObject : ModTile
{
    public override string Texture => Assets.Textures.Underground.Items.GraniteTome.GraniteTomeObject.KEY;

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ModContent.ItemType<GraniteTome>());
    }
    public override void SetStaticDefaults()
    {
        Main.tileNoAttach[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileShine[Type] = 2000;
        TileID.Sets.BreakableWhenPlacing[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        DustType = DustID.Granite;
        AddMapEntry(new Color(78, 73, 145), ModContent.GetInstance<GraniteTome>().DisplayName);
        // Set other values here
    }
}

public class MarbleTomeObjectItem : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Marble}";

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<MarbleTomeObject>(), 0);
    }
}

public class MarbleTomeObject : ModTile
{
    public override string Texture => Assets.Textures.Underground.Items.MarbleTome.MarbleTomeObject.KEY;

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ModContent.ItemType<MarbleTome>());
    }
    public override void SetStaticDefaults()
    {
        Main.tileNoAttach[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileShine[Type] = 2000;
        TileID.Sets.BreakableWhenPlacing[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        DustType = DustID.Marble;
        AddMapEntry(new Color(189, 200, 223), ModContent.GetInstance<MarbleTome>().DisplayName);
        // Set other values here
    }
}