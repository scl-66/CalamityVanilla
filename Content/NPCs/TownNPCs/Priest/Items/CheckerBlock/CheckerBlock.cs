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
        AddMapEntry(new Color(219, 226, 225));
        HitSound = SoundID.Tink;
    }

    public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
    {
        // For every even X and Y coordinate, we will offset the tile's horizontal and vertical frame by the size of the sheet so the tile's frame ends up using the alternate version on the sheet, making a pattern that spans 2x2 tiles
        Tile t = Main.tile[i, j];
        if ((i + j) % 2 == 0) { }
        else
            t.TileFrameY += 90;
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
public class InvertedCheckerBlockTile : ModTile
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerBlockTile";

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new Color(87, 105, 122));
        HitSound = SoundID.Tink;
    }

    public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
    {
        // For every even X and Y coordinate, we will offset the tile's horizontal and vertical frame by the size of the sheet so the tile's frame ends up using the alternate version on the sheet, making a pattern that spans 2x2 tiles
        Tile t = Main.tile[i, j];
        if ((i + j) % 2 == 0)
            t.TileFrameY += 90;
    }
    public override bool CreateDust(int i, int j, ref int type)
    {
        if ((i + j) % 2 == 0)
        {
            type = ModContent.DustType<BlackCheckerDust>();
        }
        else
        {
            type = ModContent.DustType<WhiteCheckerDust>();
        }
        return true;
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
            .AddIngredient<InvertedCheckerBlock>()
            .Register();
    }
}
internal class InvertedCheckerBlock : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<InvertedCheckerBlockTile>());
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<CheckerBlock>()
            .Register();
    }
}
