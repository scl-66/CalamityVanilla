using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Tundra.Cryonic.Ore;
using CalamityVanilla.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TileHelper.Common;

namespace CalamityVanilla.Content.Tundra.Cryonic.Bar;

public class CryonicBar : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem)
    {
        modItem.Item.ResearchUnlockCount = 100;
        modItem.Item.rare = ItemRarityID.Green;
        modItem.Item.value = Item.sellPrice(0, 0, 40, 0);
    }
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe().AddTile(TileID.Hellforge).AddIngredient(AutoContent.ItemType<CryonicOre>(),4).Register();
    }
    public override void SetStaticDefaults()
    {
        Main.tileShine[Type] = 1100;
        Main.tileSolid[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        VanillaFallbackOnModDeletion = TileID.MetalBars;
        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.MetalBar"));
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        if (!WorldGen.SolidTileAllowBottomSlope(i, j + 1))
        {
            WorldGen.KillTile(i, j);
        }
        return true;
    }
    public override bool CreateDust(int i, int j, ref int type)
    {
        Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<CryonicSmokeDust>());
        d.fadeIn = 10;
        d.noGravity = true;
        d.customData = 0.01f;
        d.color.A = 0;

        Dust d2 = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<SimpleColorableGlowyDust>());
        d2.color = new Color(0.35f, 0.4f, 0.5f, 0f);
        d2.noGravity = true;
        return false;
    }
}
