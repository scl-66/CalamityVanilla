using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TileHelper.Common;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;

public class CheckerWall : ModWall, ILoadItem
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/CheckerWall";

    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 5);
    public void ItemRecipes(ModItem modItem)
    {
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<CheckerBlock>()), out ModItem tileItem))
        {
            modItem.CreateRecipe(4)
                .AddIngredient(tileItem)
                .Register();
        }
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
public class WhiteCheckerWall : ModWall, ILoadItem
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/WhiteCheckerWall";

    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 5);
    public void ItemRecipes(ModItem modItem)
    {
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<WhiteCheckerBlock>()), out ModItem tileItem))
        {
            modItem.CreateRecipe(4)
                .AddIngredient(tileItem)
                .Register();
        }
    }

    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(120, 124, 123));
        DustType = ModContent.DustType<WhiteCheckerDust>();
    }
}
public class BlackCheckerWall : ModWall, ILoadItem
{
    public override string Texture => "CalamityVanilla/Content/NPCs/TownNPCs/Priest/Items/CheckerBlock/BlackCheckerWall";

    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 5);
    public void ItemRecipes(ModItem modItem)
    {
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<BlackCheckerBlock>()), out ModItem tileItem))
        {
            modItem.CreateRecipe(4)
                .AddIngredient(tileItem)
                .Register();
        }
    }

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