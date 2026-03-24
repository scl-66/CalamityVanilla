using CalamityVanilla.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode;

public abstract class PostHardmodeBarTile : ModTile
{
    public virtual int dustType => DustID.Stone;
    public virtual bool glows => false;
    public virtual int sparkleChance => 1100;
    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = glows;
        Main.tileShine[Type] = sparkleChance;
        Main.tileSolid[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        DustType = dustType;

        VanillaFallbackOnModDeletion = TileID.MetalBars;

        AddMapEntry(new Color(224, 194, 101), Language.GetText("MapObject.MetalBar")); // localized text for "Metal Bar"
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        // This check will destroy this tile if the tile below has become sloped such that it doesn't have a solid top side.
        // This is necessary in this case because Bar tiles can be placed on top of each other but can also be hammered to be half bricks despite being tileSolidTop.
        if (!WorldGen.SolidTileAllowBottomSlope(i, j + 1))
        {
            WorldGen.KillTile(i, j);
        }
        return true;
    }
}
