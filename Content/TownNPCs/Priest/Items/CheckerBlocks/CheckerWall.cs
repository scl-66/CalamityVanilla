using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TileHelper.Common;

namespace CalamityVanilla.Content.TownNPCs.Priest.Items.CheckerBlocks;

public class CheckerWall : ModWall, ILoadItem
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.Items.CheckerBlocks.CheckerWall.KEY;

    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 5);
    public void AddItemRecipes(ModItem item)
    {
        item.CreateRecipe(4).AddIngredient(AutoContent.ItemType<CheckerBlock>()).AddTile(TileID.WorkBenches).Register();

        //Allow wall items to be crafted back into base materials
        Recipe.Create(AutoContent.ItemType<CheckerBlock>()).AddIngredient(item.Type, 4).AddTile(TileID.WorkBenches).Register();
    }

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

//flat color versions
public class WhiteCheckerWall : ModWall
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.Items.CheckerBlocks.CheckerWallFlat.KEY;

    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(120, 124, 123));
        DustType = ModContent.DustType<WhiteCheckerDust>();
    }
}
public class BlackCheckerWall : ModWall
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.Items.CheckerBlocks.CheckerWallFlat.KEY;

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
    public override string Texture => Assets.Textures.TownNPCs.Priest.Items.CheckerBlocks.WhiteCheckerWallItem.KEY;

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
        Recipe.Create(AutoContent.ItemType<WhiteCheckerBlock>()).AddIngredient(Type, 4).Register(); //Allow wall items to be crafted back into base materials
    }
}
internal class BlackCheckerWallItem : ModItem
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.Items.CheckerBlocks.BlackCheckerWallItem.KEY;

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
        Recipe.Create(AutoContent.ItemType<BlackCheckerBlock>()).AddIngredient(Type, 4).Register(); //Allow wall items to be crafted back into base materials
    }
}