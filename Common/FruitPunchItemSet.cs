using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public sealed class FruitPunchItemSet : ModSystem
{
    public static bool[] PunchAccepts = ItemID.Sets.Factory.CreateNamedSet("CalamityVanilla:PunchBowl").RegisterBoolSet();

    static IReadOnlyDictionary<int, Color?[]> potionIdsToColors = new Dictionary<int, Color?[]>()
    {
        { ItemID.BottledWater, [new Color(48, 108, 247)] },
        { ItemID.BottledHoney, [new Color(248, 195, 11)] },
        { ItemID.LesserHealingPotion, [new Color(230, 10, 57)] },
        { ItemID.HealingPotion, [new Color(230, 10, 57)] },
        { ItemID.GreaterHealingPotion, [new Color(230, 10, 57)] },
        { ItemID.SuperHealingPotion, [new Color(230, 10, 57)] },
        { ItemID.RestorationPotion, [new Color(252, 83, 234)] },
        { ItemID.LesserManaPotion, [new Color(7, 64, 236)] },
        { ItemID.ManaPotion, [new Color(7, 64, 236)] },
        { ItemID.GreaterManaPotion, [new Color(7, 64, 236)] },
        { ItemID.SuperManaPotion, [new Color(7, 64, 236)] },
        { ItemID.AmmoReservationPotion, [new Color(248, 195, 11), new Color(197, 197, 197)] },
        { ItemID.ArcheryPotion, [new Color(243, 154, 33)] },
        { ItemID.BattlePotion, [new Color(98, 74, 165)] },
        { ItemID.BiomeSightPotion, [new Color(253, 99, 147), new Color(168, 103, 233)] },
        { ItemID.BuilderPotion, [new Color(169, 107, 80)] },
        { ItemID.CalmingPotion, [new Color(99, 106, 228)] },
        { ItemID.CratePotion, [new Color(212, 136, 60)] },
        { ItemID.TrapsightPotion, [new Color(250, 78, 3)] },
        { ItemID.EndurancePotion, [new Color(126, 126, 126)] },
        { ItemID.FeatherfallPotion, [new Color(103, 213, 253)] },
        { ItemID.FishingPotion, [new Color(33, 208, 98)] },
        { ItemID.FlipperPotion, [new Color(14, 162, 236)] },
        { ItemID.GenderChangePotion, [new Color(84, 196, 253), new Color(253, 99, 147)] },
        { ItemID.GillsPotion, [new Color(8, 114, 231)] },
        { ItemID.GravitationPotion, [new Color(118, 21, 238)] },
        { ItemID.HeartreachPotion, [new Color(200, 21, 149)] },
        { ItemID.HunterPotion, [new Color(201, 80, 6)] },
        { ItemID.InfernoPotion, [new Color(232, 45, 1), new Color(251, 217, 2)] },
        { ItemID.InvisibilityPotion, [new Color(20, 158, 168)] },
        { ItemID.IronskinPotion, [new Color(251, 217, 2)] },
        { ItemID.LifeforcePotion, [new Color(174, 10, 2), new Color(151, 196, 197)] },
        { ItemID.LovePotion, [new Color(174, 10, 2)] },
        { ItemID.LuckPotionLesser, [new Color(171, 194, 216)] },
        { ItemID.LuckPotion, [new Color(36, 37, 38)] },
        { ItemID.LuckPotionGreater, [new Color(226, 21, 149)] },
        { ItemID.MagicPowerPotion, [new Color(118, 21, 238)] },
        { ItemID.ManaRegenerationPotion, [new Color(226, 21, 149)] },
        { ItemID.MiningPotion, [new Color(83, 142, 156)] },
        { ItemID.NightOwlPotion, [new Color(200, 255, 86)] },
        { ItemID.ObsidianSkinPotion, [new Color(98, 74, 165)] },
        { ItemID.RagePotion, [new Color(244, 17, 7), new Color(249, 235, 115)] },
        { ItemID.RecallPotion, [new Color(24, 217, 244)] },
        { ItemID.RegenerationPotion, [new Color(226, 21, 149)] },
        { ItemID.PotionOfReturn, [new Color(203, 148, 212), new Color(67, 66, 211)] },
        { ItemID.ShinePotion, [new Color(220, 242, 9)] },
        { ItemID.SonarPotion, [new Color(117, 179, 10), new Color(218, 254, 157)] },
        { ItemID.SpelunkerPotion, [new Color(248, 204, 78)] },
        { ItemID.StinkPotion, [new Color(101, 121, 21)] },
        { ItemID.SummoningPotion, [new Color(149, 179, 31)] },
        { ItemID.SwiftnessPotion, [new Color(82, 233, 33)] },
        { ItemID.TeleportationPotion, [new Color(118, 21, 238), new Color(252, 83, 234)] },
        { ItemID.ThornsPotion, [new Color(215, 241, 109)] },
        { ItemID.TitanPotion, [new Color(153, 229, 100)] },
        { ItemID.WarmthPotion, [new Color(244, 182, 9), new Color(252, 251, 179)] },
        { ItemID.WaterWalkingPotion, [new Color(8, 114, 231)] },
        { ItemID.WrathPotion, [new Color(199, 82, 71)] },
        { ItemID.FlaskofFire, [new Color(232, 45, 1), new Color(248, 204, 78)] },
        { ItemID.FlaskofIchor, [new Color(149, 179, 31), new Color(248, 204, 78)] },
        { ItemID.FlaskofGold, [new Color(191, 163, 20), new Color(249, 235, 115)] },
        { ItemID.FlaskofCursedFlames, [new Color(20, 158, 168), new Color(220, 242, 9)] },
        { ItemID.FlaskofPoison, [new Color(83, 142, 156), new Color(115, 228, 151)] },
        { ItemID.FlaskofNanites, [new Color(120, 107, 177), new Color(84, 196, 253)] },
        { ItemID.FlaskofVenom, [new Color(98, 74, 165), new Color(178, 133, 204)] },
        { ItemID.FlaskofParty, [new Color(226, 21, 149), new Color(252, 83, 234)] },
    };

    public static void GetPotionColor(int potionID, out Color? color1, out Color? color2)
    {
        color1 = potionIdsToColors[potionID][0];
        color2 = color1;
        if (potionIdsToColors[potionID].Length == 2)
        {
            color2 = potionIdsToColors[potionID][1];
        }
        //Main.NewText(color1);
        //Main.NewText(color2);
    }

    public override void PostSetupRecipes()
    {
        int[] ingredientList = {
            ItemID.BottledWater, ItemID.Bottle,
            ItemID.LesserHealingPotion, ItemID.LesserManaPotion,
            ItemID.HealingPotion, ItemID.ManaPotion,
            ItemID.GreaterHealingPotion, ItemID.GreaterManaPotion,
            ItemID.SuperHealingPotion, ItemID.SuperManaPotion,
            ItemID.RecallPotion,
        };
        foreach (var recipe in Main.recipe)
        {
            var type = recipe.createItem.type;
            bool inIngredientList = false;

            foreach (var item in ingredientList)
            {
                if (recipe.HasIngredient(item))
                {
                    inIngredientList = true;
                    break;
                }
            }
            if (inIngredientList && type > ItemID.None && recipe.createItem.consumable)
                PunchAccepts[type] = true;
        }
        PunchAccepts[ItemID.WormholePotion] = false;
        PunchAccepts[ItemID.StinkPotion] = false;
        PunchAccepts[ItemID.LovePotion] = false;
        PunchAccepts[ItemID.LesserManaPotion] = true;
        PunchAccepts[ItemID.GreaterManaPotion] = true;
    }
}