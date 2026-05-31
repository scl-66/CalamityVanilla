using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class GameTitles : ILoadable
{
    public void Load(Mod mod)
    {
        On_Lang.GetRandomGameTitle += On_Lang_GetRandomGameTitle;
    }

    private string On_Lang_GetRandomGameTitle(On_Lang.orig_GetRandomGameTitle orig)
    {
        // I would not be surprised if there's a better more mod compatible way to do this
        List<LocalizedText> titles =
        [
            .. Language.FindAll(Lang.CreateDialogFilter(ModContent.GetInstance<CalamityVanilla>().GetLocalizationKey("GameTitles."))),
            .. Language.FindAll(Lang.CreateDialogFilter("GameTitle")),
        ];
        return titles[Main.rand.Next(titles.Count)].Value;
    }

    public void Unload()
    {
    }
}
