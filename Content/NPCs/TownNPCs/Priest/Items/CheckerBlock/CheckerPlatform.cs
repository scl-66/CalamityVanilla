using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;

public class WhiteCheckerPlatformTile : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = true;
        Main.tileLavaDeath[Type] = false;
        TileID.Sets.Platforms[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
        AddMapEntry(new Color(197, 200, 199));

        DustType = ModContent.DustType<WhiteCheckerDust>();
        AdjTiles = [TileID.Platforms];
        VanillaFallbackOnModDeletion = TileID.Platforms;

        // Placement
        TileObjectData.newTile.CoordinateHeights = [16];
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleMultiplier = 27;
        TileObjectData.newTile.StyleWrapLimit = 27;
        TileObjectData.newTile.UsesCustomCanPlace = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
    }

    public override void PostSetDefaults() => Main.tileNoSunLight[Type] = false;

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}
public class BlackCheckerPlatformTile : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = true;
        Main.tileLavaDeath[Type] = false;
        TileID.Sets.Platforms[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
        AddMapEntry(new Color(97, 105, 122));

        DustType = ModContent.DustType<BlackCheckerDust>();
        AdjTiles = [TileID.Platforms];
        VanillaFallbackOnModDeletion = TileID.Platforms;

        // Placement
        TileObjectData.newTile.CoordinateHeights = [16];
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleMultiplier = 27;
        TileObjectData.newTile.StyleWrapLimit = 27;
        TileObjectData.newTile.UsesCustomCanPlace = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
    }

    public override void PostSetDefaults() => Main.tileNoSunLight[Type] = false;

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}

internal class WhiteCheckerPlatform : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<WhiteCheckerPlatformTile>());
        Item.value = Item.buyPrice(0, 0, 0, 10);
    }
    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient<WhiteCheckerBlock>()
            .Register();
    }
}
internal class BlackCheckerPlatform : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<BlackCheckerPlatformTile>());
        Item.value = Item.buyPrice(0, 0, 0, 10);
    }
    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient<BlackCheckerBlock>()
            .Register();
    }
}