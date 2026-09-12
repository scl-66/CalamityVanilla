using CalamityVanilla.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Emotes;

public class PriestEmote : ModEmoteBubble
{
    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.Town);
    }
    public override bool IsUnlocked()
    {
        return BossDownedSystem.DownedCryogen;
    }
}