using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;
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
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<CheckerBlock.CheckerBlock>()), out ModItem item))
        {
            string gospelName = typeof(GospelSet).Namespace + ".Gospel";
            TileHelper.ArgumentCollection args = AllArgs(DustID.Gold, Color.Orange.ToVector3())
                - new BarrelTile()
                - new BenchTile();

            LoadFurnitureSet(gospelName, args, item.Type);
        }

        GospelClothWorkBench gospelClothWorkBench = ModContent.GetInstance<GospelClothWorkBench>();
        GospelClothTable gospelClothTable = ModContent.GetInstance<GospelClothTable>();
        GospelOrgan gospelOrgan = ModContent.GetInstance<GospelOrgan>();

        List<FurnitureTile> OtherFurniture = [
            gospelClothWorkBench, gospelClothTable, gospelOrgan
        ];
        foreach (var tile in OtherFurniture)
        {
            TileTypes.Add(tile.FurnitureName, tile.Type);
        }
    }
    public void Unload() { }
}

public class GospelClothWorkBench : WorkBenchTile, ILoadItem
{
    public void AddItemRecipes(ModItem modItem)
    {
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<CheckerBlock.CheckerBlock>()), out ModItem item))
        {
            modItem.CreateRecipe()
                .AddIngredient(item, 10)
                .AddIngredient(ItemID.Silk, 3) //placeholder until we add the priest cloth block
                .Register();
        }
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
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<CheckerBlock.CheckerBlock>()), out ModItem item))
        {
            modItem.CreateRecipe()
                .AddIngredient(item, 8)
                .AddIngredient(ItemID.Silk, 3) //placeholder until we add the priest cloth block
                .Register();
        }
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
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<CheckerBlock.CheckerBlock>()), out ModItem item))
        {
            modItem.CreateRecipe()
                .AddIngredient(item, 25)
                .AddIngredient(ItemID.Bone, 6)
                .AddIngredient(ItemID.Book)
                .Register();
        }
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
