using CalamityVanilla.Common.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TileHelper.Common;
using TileHelper.Content.Tiles;
using static TileHelper.Autoloader;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.GospelFurniture;

public class GospelSet : ILoadable
{
    public static Dictionary<string, int> TileTypes { get; } = [];
    public void Load(Mod mod) => ILoadItem.PostAutoloadItems += LoadGospelFurniture;

    private static void LoadGospelFurniture()
    {
        string gospelName = typeof(GospelSet).Namespace + ".Gospel";
        TileHelper.ArgumentCollection args = AllArgs(DustID.Gold, Color.Orange.ToVector3())
            - new BarrelTile()
            - new CandleTile()
            - new BenchTile();

        LoadFurnitureSet(gospelName, args, AutoContent.ItemType<GospelBrick>());

        GospelCandle gospelCandle = ModContent.GetInstance<GospelCandle>();
        GospelClothWorkBench gospelClothWorkBench = ModContent.GetInstance<GospelClothWorkBench>();
        GospelClothTable gospelClothTable = ModContent.GetInstance<GospelClothTable>();
        GospelOrgan gospelOrgan = ModContent.GetInstance<GospelOrgan>();

        List<FurnitureTile> OtherFurniture = [
            gospelCandle,
            gospelClothWorkBench,
            gospelClothTable,
            gospelOrgan
        ];
        foreach (var tile in OtherFurniture)
            TileTypes.Add(tile.FurnitureName, tile.Type);
    }
    public void Unload() { }
}

public class GospelCandle : CandleTile, ILoadItem
{

    public void AddItemRecipes(ModItem modItem) => DataStructures.Recipes[FurnitureName]?.Invoke(modItem, AutoContent.ItemType<GospelBrick>());

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
        TileObjectData.newTile.CoordinateHeights = [22];
        TileObjectData.newTile.DrawYOffset = -6;

        TileHelperSets.TileGlowmask[Type] = Helpers.RequestGlowmask(this);
        AdjTiles = [33];
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        AddMapEntry(MapColor, Language.GetText("ItemName.Candle"));

        AdjTiles = [TileID.Candles];
        DustType = -1;
        Light = Color.Orange.ToVector3();
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            (r, g, b) = (Light.X, Light.Y, Light.Z);
    }
}
public class GospelClothWorkBench : WorkBenchTile, ILoadItem
{
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
            .AddIngredient(AutoContent.ItemType<GospelBrick>(), 10)
            .AddIngredient(ItemID.Silk, 3) //placeholder until we add the priest cloth block
            .Register();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DustType = -1;
    }
}
public class GospelClothTable : TableTile, ILoadItem
{
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
            .AddTile(TileID.WorkBenches)
            .AddIngredient(AutoContent.ItemType<GospelBrick>(), 8)
            .AddIngredient(ItemID.Silk, 3) //placeholder until we add the priest cloth block
            .Register();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DustType = -1;
    }
}
public class GospelOrgan : PianoTile, ILoadItem
{
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
            .AddTile(TileID.Sawmill)
            .AddIngredient(AutoContent.ItemType<GospelBrick>(), 25)
            .AddIngredient(ItemID.Bone, 6)
            .AddIngredient(ItemID.Book)
            .Register();
    }

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.Origin = new Point16(2, 3);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.newTile.CoordinateWidth = 16;

        TileObjectData.newTile.Origin = new Point16(3, 4);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
        TileObjectData.addTile(Type);
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
        AddMapEntry(MapColor, Language.GetText("ItemName.Piano"));
        DustType = -1;
    }
}