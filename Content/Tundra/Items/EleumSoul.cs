using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Items;

public class EleumSoul : ModItem // thanks example mod lmao
{
    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 255, 255, 100);
    }
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        ItemID.Sets.ItemIconPulse[Item.type] = true;
        ItemID.Sets.ItemNoGravity[Item.type] = true;
        Item.ResearchUnlockCount = 25;
    }
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.SoulofNight);
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
    }
}