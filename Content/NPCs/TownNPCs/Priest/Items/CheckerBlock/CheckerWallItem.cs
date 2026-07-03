using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TileHelper.Common;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;

public class CheckerWall : ModWall
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWall";

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
        if ((i + j) % 2 == 0)
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

internal class CheckerWallItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<CheckerWall>());
        Item.value = Item.buyPrice(0, 0, 0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddTile(TileID.WorkBenches)
            .AddIngredient(AutoContent.ItemType<CheckerBlock>())
            .Register();
    }
}

//flat color versions
public class WhiteCheckerWall : ModWall
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWallFlat";

    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(120, 124, 123));
        DustType = ModContent.DustType<WhiteCheckerDust>();
    }
}
public class BlackCheckerWall : ModWall
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWallFlat";

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
internal class WhiteCheckerWallItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<WhiteCheckerWall>());
        Item.value = Item.buyPrice(0, 0, 0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddTile(TileID.WorkBenches)
            .AddIngredient(AutoContent.ItemType<WhiteCheckerBlock>())
            .Register();
    }
}
internal class BlackCheckerWallItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<BlackCheckerWall>());
        Item.value = Item.buyPrice(0, 0, 0, 5);
    }
    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddTile(TileID.WorkBenches)
            .AddIngredient(AutoContent.ItemType<BlackCheckerBlock>())
            .Register();
    }
}