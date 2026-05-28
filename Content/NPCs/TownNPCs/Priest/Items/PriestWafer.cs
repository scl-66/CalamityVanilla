using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items;

public class PriestWafer : ModItem
{
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
        Item.DefaultToFood(22, 22, 2, -1); // 57600 is 16 minutes: 16 * 60 * 60
        Item.value = Item.buyPrice(0, 0, 5);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void OnConsumeItem(Player player)
    {
        player.Heal(Main.rand.Next(1, 5));
    }
}
