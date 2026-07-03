using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using ReLogic.Localization.IME;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TileHelper.Common;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;

public class CheckerBlock : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 20);
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
                .AddIngredient<CheckerWallItem>(4)
                .Register();
    }

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
public class WhiteCheckerBlock : ModTile, ILoadItem
{
    //public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerBlock";

    public void SetDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 20);
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
                .AddIngredient<WhiteCheckerWallItem>(4)
                .Register();
        modItem.CreateRecipe()
                .AddIngredient(AutoContent.ItemType<WhiteCheckerPlatform>(), 2)
                .Register();
    }

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;
        Main.tileMerge[Type][ModContent.TileType<BlackCheckerBlock>()] = true;
        AddMapEntry(new Color(219, 226, 225));
        HitSound = SoundID.Tink;
        DustType = ModContent.DustType<WhiteCheckerDust>();
    }
}
public class BlackCheckerBlock : ModTile, ILoadItem
{
    //public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerBlock";

    public void SetDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 20);
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
                .AddIngredient<BlackCheckerWallItem>(4)
                .Register();
        modItem.CreateRecipe()
                .AddIngredient(AutoContent.ItemType<BlackCheckerPlatform>(), 2)
                .Register();
    }

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;
        Main.tileMerge[Type][ModContent.TileType<WhiteCheckerBlock>()] = true;
        AddMapEntry(new Color(87, 105, 122));
        HitSound = SoundID.Tink;
        DustType = ModContent.DustType<BlackCheckerDust>();
    }
}