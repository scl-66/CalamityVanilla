using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;

public class CheckerWallTile : ModWall
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWallTileAlt";

    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(120, 124, 123));
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Main.wallFrame[Type] = (byte)((i + j) % 2);

        return true;
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

internal class CheckerWall : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<CheckerWallTile>());
        Item.value = Item.buyPrice(0, 0, 0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<CheckerBlock>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

//flat color versions
public class WhiteCheckerWallTile : ModWall
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWallTile";

    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(120, 124, 123));
        DustType = ModContent.DustType<WhiteCheckerDust>();
    }
}
public class BlackCheckerWallTile : ModWall
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWallTile";

    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(47, 57, 67));
        DustType = ModContent.DustType<BlackCheckerDust>();
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Main.wallFrame[Type] = 1;
        return true;
    }
}
internal class WhiteCheckerWall : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<WhiteCheckerWallTile>());
        Item.value = Item.buyPrice(0, 0, 0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<WhiteCheckerBlock>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
internal class BlackCheckerWall : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<BlackCheckerWallTile>());
        Item.value = Item.buyPrice(0, 0, 0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<BlackCheckerBlock>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}