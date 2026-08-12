using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underworld.Items;

public class HavocSoul : ModItem // thanks example mod lmao
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
        Item.useAnimation = 1;
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.55f * Main.essScale);
    }
}