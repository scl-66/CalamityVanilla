using Daybreak.Common.Features.Authorship;
using Daybreak.Common.Features.ModPanel;
using System;
using Terraria.ModLoader;

namespace CalamityVanilla;

public partial class CalamityVanilla : Mod, IHasCustomAuthorMessage
{
    public string GetAuthorText()
    {
        return AuthorText.GetAuthorTooltip(this, headerText: null);
    }
}