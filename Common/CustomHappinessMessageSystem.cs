using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class CustomHappinessMessageSystem : ModSystem
{
    public override void Load()
    {
        On_ShopHelper.ApplyNpcRelationshipEffect += On_ShopHelper_ApplyNpcRelationshipEffect;
        On_ShopHelper.ApplyBiomeRelationshipEffect += On_ShopHelper_ApplyBiomeRelationshipEffect;
    }

    private static void On_ShopHelper_ApplyBiomeRelationshipEffect(On_ShopHelper.orig_ApplyBiomeRelationshipEffect orig, ShopHelper self, string biomeNameKey, Terraria.GameContent.Personalities.AffectionLevel affectionLevel)
    {
        if (self._currentNPCBeingTalkedTo.ModNPC?.Mod != ModContent.GetInstance<CalamityVanilla>())
        {
            orig.Invoke(self, biomeNameKey, affectionLevel);
            return;
        }

        // If the current Town NPC is a ModNPC
        if (self._currentNPCBeingTalkedTo.ModNPC is ModNPC modNPC)
        {

            // Check to see if there is a localization entry for the other Town NPC defined in our current Town NPC.
            if (Language.Exists($"{modNPC.GetLocalizationKey("TownNPCMood")}.{affectionLevel}Biome_{biomeNameKey}"))
            {
                // If it does exist, add the happiness report.
                self.AddHappinessReportText($"{affectionLevel}Biome_{biomeNameKey}", new
                {
                    BiomeName = ShopHelper.BiomeNameByKey(biomeNameKey)
                });
            }
            else
            {
                //TEMPORARY UNTIL 1.4.5: Check if biome is Underworld, then set the name to "Underworld"
                //if (biomeNameKey == "Underworld")
                //{
                //    // Otherwise, add the generic affection quote.
                //    self.AddHappinessReportText($"{affectionLevel}Biome", new
                //    {
                //        BiomeName = "the Underworld"
                //    });
                //}

                // Otherwise, add the generic affection quote.
                self.AddHappinessReportText($"{affectionLevel}Biome", new
                {
                    BiomeName = ShopHelper.BiomeNameByKey(biomeNameKey)
                });
            }
        }
        else
        { // Vanilla NPC
            if (Language.Exists($"TownNPCMood_{NPCID.Search.GetName(self._currentNPCBeingTalkedTo.netID)}.{affectionLevel}Biome_{biomeNameKey}"))
            {
                self.AddHappinessReportText($"{affectionLevel}Biome_{biomeNameKey}", new
                {
                    BiomeName = ShopHelper.BiomeNameByKey(biomeNameKey)
                });
            }
            else
            {
                self.AddHappinessReportText($"{affectionLevel}Biome", new
                {
                    BiomeName = ShopHelper.BiomeNameByKey(biomeNameKey)
                });
            }
        }
        self._currentPriceAdjustment *= NPCHappiness.AffectionLevelToPriceMultiplier[affectionLevel];
    }

    private static void On_ShopHelper_ApplyNpcRelationshipEffect(On_ShopHelper.orig_ApplyNpcRelationshipEffect orig, ShopHelper self, int npcType, Terraria.GameContent.Personalities.AffectionLevel affectionLevel)
    {
        if (self._currentNPCBeingTalkedTo.ModNPC?.Mod != ModContent.GetInstance<CalamityVanilla>())
        {
            orig.Invoke(self, npcType, affectionLevel);
            return;
        }
		string otherNPCName = NPCID.Search.GetName(npcType); // Get the name of the other Town NPC.

		// If the current Town NPC is a ModNPC
		if (self._currentNPCBeingTalkedTo.ModNPC is ModNPC modNPC) {

			// Check to see if there is a localization entry for the other Town NPC defined in our current Town NPC.
			if (Language.Exists($"{modNPC.GetLocalizationKey("TownNPCMood")}.{affectionLevel}NPC_{otherNPCName}")) {
				if (otherNPCName == "Princess")
                {
                    return;
                }
                
                // If it does exist, add the happiness report.
				self.AddHappinessReportText($"{affectionLevel}NPC_{otherNPCName}", new {
					NPCName = NPC.GetFullnameByID(npcType)
				});
			}
			else {
				// Otherwise, add the generic affection quote.
				self.AddHappinessReportText($"{affectionLevel}NPC", new {
					NPCName = NPC.GetFullnameByID(npcType)
				});
			}
		}
		else { // Vanilla NPC
			if (Language.Exists($"TownNPCMood_{NPCID.Search.GetName(self._currentNPCBeingTalkedTo.netID)}.{affectionLevel}NPC_{otherNPCName}")) {
                self.AddHappinessReportText($"{affectionLevel}NPC_{otherNPCName}", new {
					NPCName = NPC.GetFullnameByID(npcType)
				});
			}
			else {
                self.AddHappinessReportText($"{affectionLevel}NPC", new {
					NPCName = NPC.GetFullnameByID(npcType)
				});
			}
		}
        self._currentPriceAdjustment *= NPCHappiness.AffectionLevelToPriceMultiplier[affectionLevel];
    }
}
