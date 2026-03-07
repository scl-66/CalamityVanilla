using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Players;

public class SkinBoilHatPlayer : ModPlayer
{
    public bool skinBoilHat = false;
    public override void ResetEffects()
    {
        skinBoilHat = false;
    }
}
