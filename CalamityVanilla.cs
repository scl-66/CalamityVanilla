using Daybreak.Common.Features.Authorship;
using Daybreak.Common.Features.ModPanel;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla;

public partial class CalamityVanilla : Mod, IHasCustomAuthorMessage
{
    public const string AssetPath = "CalamityVanilla/Assets/";
    public string GetAuthorText()
    {
        return AuthorText.GetAuthorTooltip(this, headerText: null);
    }

    public override void Load()
    {
        TileHelper.Autoloader.Load(this);
    }
}