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
using TileHelper.Common;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items.GospelFurniture;

public class GospelBrick : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem) => modItem.Item.value = Item.buyPrice(0, 0, 0, 50);
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe()
                .AddIngredient<CheckerWallItem>(4) //todo: add wall
                .Register();
    }

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Dirt] = true;
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
