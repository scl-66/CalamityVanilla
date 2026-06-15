using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;

public class CheckerBlockTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;
        AddMapEntry(new Color(219, 226, 225));
        HitSound = SoundID.Tink;
    }

    public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
    {
        // For every even X and Y coordinate, we will offset the tile's horizontal and vertical frame by the size of the sheet so the tile's frame ends up using the alternate version on the sheet, making a pattern that spans 2x2 tiles
        Tile t = Main.tile[i, j];
        if ((i + j) % 2 == 0) { }
        else
            t.TileFrameY += 270;
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        if((i + j) % 2 == 0)
        {
            type = ModContent.DustType<WhiteCheckerDust>();
        }
        else
        {
            type = ModContent.DustType<BlackCheckerDust>();
        }
        return true;
    }
}

//flat color versions
public class WhiteCheckerBlockTile : ModTile
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerBlockTile";

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;
        Main.tileMerge[Type][ModContent.TileType<BlackCheckerBlockTile>()] = true;
        AddMapEntry(new Color(219, 226, 225));
        HitSound = SoundID.Tink;
        DustType = ModContent.DustType<WhiteCheckerDust>();
    }
}
public class BlackCheckerBlockTile : ModTile
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerBlockTile";

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;
        Main.tileMerge[Type][ModContent.TileType<WhiteCheckerBlockTile>()] = true;
        AddMapEntry(new Color(87, 105, 122));
        HitSound = SoundID.Tink;
        DustType = ModContent.DustType<BlackCheckerDust>();
    }

    public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
    {
        Tile t = Main.tile[i, j];
        t.TileFrameY += 270;
    }
}

internal class CheckerBlock : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<CheckerBlockTile>());
        Item.value = Item.buyPrice(0, 0, 0, 20);
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<CheckerWall>(4)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

internal class WhiteCheckerBlock : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<WhiteCheckerBlockTile>());
        Item.value = Item.buyPrice(0, 0, 0, 20);
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<WhiteCheckerWall>()
            .AddTile(TileID.WorkBenches)
            .Register();
        CreateRecipe()
            .AddIngredient<WhiteCheckerPlatform>(2)
            .Register();
    }
}
internal class BlackCheckerBlock : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<BlackCheckerBlockTile>());
        Item.value = Item.buyPrice(0, 0, 0, 20);
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<BlackCheckerWall>()
            .AddTile(TileID.WorkBenches)
            .Register();
        CreateRecipe()
            .AddIngredient<BlackCheckerPlatform>(2)
            .Register();
    }
}