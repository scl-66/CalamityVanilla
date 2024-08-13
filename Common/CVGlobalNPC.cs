using CalamityVanilla.Content.Items;
using CalamityVanilla.Content.Items.Weapons.Ranged;
using Terraria;
using Terraria.GameContent.ItemDropRules;
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
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if(npc.type == NPCID.Vampire)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheGothic>(), 100));
            }
        }
    }
}
