using CalamityVanilla.Common;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Emotes;

public class HiveMindEmote : ModEmoteBubble
{
    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.Dangers);
    }
    public override bool IsUnlocked()
    {
        return BossDownedSystem.DownedHiveMind;
    }
}
public class GutofCthulhuEmote : ModEmoteBubble
{
    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.Dangers);
    }
    public override bool IsUnlocked()
    {
        return BossDownedSystem.DownedGutOfCthulhu;
    }
}
public class CryogenEmote : ModEmoteBubble
{
    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.Dangers);
    }
    public override bool IsUnlocked()
    {
        return BossDownedSystem.DownedCryogen;
    }
}