using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Daybreak.Common.Features.Hooks.GlobalItemHooks;

namespace CalamityVanilla.Content.TownNPCs.Priest.Items;

public class PriestWafer : ModItem
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.Items.PriestWafer.KEY;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;

        // This is to show the correct frame in the inventory
        // The first argument is for the animation speed, how many ticks to spend on each frame.
        // Since we set NotActuallyAnimating, it will be stuck on frame 1 forever while in the inventory so we pass in -1 since the value doesn't matter.
        // The second argument is the number of frames, which is 3
        // The first frame is the inventory texture, the second frame is the holding texture,
        // and the third frame is the placed texture
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(-1, 3)
        {
            NotActuallyAnimating = true
        });

        ItemID.Sets.FoodParticleColors[Type] = [
                new Color(255, 236, 194),
            ];
        ItemID.Sets.IsFood[Type] = true; // This allows it to be placed on a plate and held correctly
    }
    public override void SetDefaults()
    {
        // This code matches the ApplePie code.

        // DefaultToFood sets all of the food related item defaults such as the buff type, buff duration, use sound, and animation time.
        Item.UseSound = SoundID.Item2;
        Item.useStyle = ItemUseStyleID.EatFood;
        Item.useTurn = true;
        Item.useAnimation = (Item.useTime = 17);
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.healLife = 5;
        Item.width = 26;
        Item.height = 26;
        Item.value = Item.buyPrice(0, 0, 5);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine? line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

        if (line != null)
            tooltips.Remove(line);
    }

    public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
    {
        healValue = Main.rand.Next(1, 5);
    }

    public override bool ApplyPotionDelay(Player player, int potionDelay)
    {
        return false;
    }
}