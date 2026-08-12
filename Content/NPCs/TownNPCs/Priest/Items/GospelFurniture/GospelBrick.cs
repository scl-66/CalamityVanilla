using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.CheckerBlock;
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
using TileHelper.Common;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.GospelFurniture;

public class GospelBrick : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 50);

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        // These relate to what tiles this tile will merge with
        Main.tileBrick[Type] = true;
        TileID.Sets.GemsparkFramingTypes[Type] = Type;
        TileID.Sets.ForcedDirtMerging[Type] = true;
        // This is necessary to avoid visual issues with half-blocks.
        TileID.Sets.AllBlocksWithSmoothBordersToResolveHalfBlockIssue[Type] = true;
        AddMapEntry(new Color(49, 54, 77));
        HitSound = SoundID.Tink;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
        return false;
    }

    public override bool CreateDust(int i, int j, ref int type)
    {
        if (Main.rand.NextBool())
            type = ModContent.DustType<BlackCheckerDust>();
        else
            type = DustID.Gold;
        return true;
    }
}
public class GospelBrickWall : ModWall, ILoadItem
{
    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 12);
    public void AddItemRecipes(ModItem item)
    {
        item.CreateRecipe(4).AddIngredient(AutoContent.ItemType<GospelBrick>()).AddTile(TileID.WorkBenches).Register();

        //Allow wall items to be crafted back into base materials
        Recipe.Create(AutoContent.ItemType<GospelBrick>()).AddIngredient(item.Type, 4)
            .AddTile(TileID.WorkBenches).Register();
    }
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = true;
        AddMapEntry(new Color(27, 29, 42));
        DustType = ModContent.DustType<BlackCheckerDust>();
    }
}

public class GospelPlatform : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 25);
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe(2)
                .AddIngredient(AutoContent.ItemType<GospelBrick>())
                .Register();
    }

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
        AddMapEntry(new Color(203, 179, 73));

        DustType = DustID.Gold;
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