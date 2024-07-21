using CalamityVanilla.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common
{
    public class CVGlobalNPC : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot) //Drop Cryo Key Mold
        {
            globalLoot.Add(ItemDropRule.ByCondition(new Conditions.FrozenKeyCondition(), ModContent.ItemType<CryogenSummonMold>(), 100));
        }
    }
}
