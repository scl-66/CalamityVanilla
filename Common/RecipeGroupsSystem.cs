using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class RecipeGroupsSystem : ModSystem
{
    public override void AddRecipeGroups()
    {
        RecipeGroup ice = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.IceBlock)}", new int[]
        {
            ItemID.IceBlock,
            ItemID.PurpleIceBlock,
            ItemID.RedIceBlock,
            ItemID.PinkIceBlock,
        });
        RecipeGroup.RegisterGroup("CalamityVanillaAnyIceBlock", ice);

        RecipeGroup cursedflame = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CursedFlame)}", ItemID.CursedFlame, ItemID.Ichor);
        RecipeGroup.RegisterGroup("CalamityVanillaAnyCursedFlame", cursedflame);
    }
}