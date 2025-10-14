using CalamityVanilla.Common;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Furniture.MusicBoxes;

public class CryogenMusicBox : BaseMusicBox.MusicBoxItem
{
    public override void SetStaticDefaults()
    {
        MusicLoader.AddMusicBox(
        Mod,
        MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cryogen"),
        Type,
        TileType);

        base.SetStaticDefaults();
    }
    public override int TileType => ModContent.TileType<CryogenMusicBoxTile>();
}

public class CryogenMusicBoxTile : BaseMusicBox.MusicBoxTile { }