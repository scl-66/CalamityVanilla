using CalamityVanilla.Common.ItemDropRules.DropConditions;
using CalamityVanilla.Content.Emotes;
using CalamityVanilla.Content.Miscellaneous.Items;
using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.BouncingEyeball;
using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TheGothic;
using CalamityVanilla.Content.Tundra.Items;
using CalamityVanilla.Content.Underworld.Items;
using CalamityVanilla.Content.Underworld.Items.WallOfFleshDrops;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class CVGlobalNPC : GlobalNPC
{
    public override void ModifyGlobalLoot(GlobalLoot globalLoot) //Drop Cryo Key Mold
    {
        globalLoot.Add(ItemDropRule.ByCondition(new Conditions.FrozenKeyCondition(), ModContent.ItemType<CryogenSummonMold>(), 100));
        globalLoot.Add(ItemDropRule.ByCondition(new EleumSoulDropCondition(), ModContent.ItemType<EleumSoul>(), 5));
        globalLoot.Add(ItemDropRule.ByCondition(new HavocSoulDropCondition(), ModContent.ItemType<HavocSoul>(), 5));
    }
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        switch (npc.type)
        {
            case NPCID.GoblinArcher:
            case NPCID.GoblinPeon:
            case NPCID.GoblinSorcerer:
            case NPCID.GoblinThief:
            case NPCID.GoblinWarrior:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoblinScrap>(), 4, 1, 2));
                break;

            case NPCID.Vampire:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheGothic>(), 50));
                break;

            case NPCID.Drippler:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BouncingEyeball>(), 4, 15, 35));
                break;

            case NPCID.WallofFlesh:
                LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                npcLoot.Add(notExpertRule);

                IItemDropRule wofLoot = ItemDropRule.OneFromOptions(1, ModContent.ItemType<FleshLauncher>(), ModContent.ItemType<PyrobatStaff>(), ModContent.ItemType<FighterJetRemote>());
                notExpertRule.OnSuccess(wofLoot);
                break;
        }

        if (NPCID.Sets.DemonEyes[npc.type])
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsBloodMoonAndNotFromStatue(), ModContent.ItemType<BouncingEyeball>(), 10, 5, 15));
        }
    }

    public override int? PickEmote(NPC npc, Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
    {
        if (Main.rand.NextBool(3) && BossDownedSystem.DownedGutOfCthulhu)
            emoteList.Add(ModContent.EmoteBubbleType<GutofCthulhuEmote>());
        if (Main.rand.NextBool(3) && BossDownedSystem.DownedHiveMind)
            emoteList.Add(ModContent.EmoteBubbleType<HiveMindEmote>());
        if (Main.rand.NextBool(3) && BossDownedSystem.DownedCryogen)
            emoteList.Add(ModContent.EmoteBubbleType<CryogenEmote>());
        return base.PickEmote(npc, closestPlayer, emoteList, otherAnchor);
    }
}