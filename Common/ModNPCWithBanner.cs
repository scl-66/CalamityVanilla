using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public abstract class ModNPCWithBanner : ModNPC
{
    public override void Load()
    {
        Mod.AddContent(new BannerTileTemplate(Texture + "_BannerTile", NPC.ModNPC.Name + "_BannerTile"));
        int bannerTile = Mod.Find<ModTile>(NPC.ModNPC.Name + "_BannerTile").Type;
        Mod.AddContent(new BannerItemTemplate(bannerTile, NPC.ModNPC.Name + "Banner", Texture + "_BannerItem"));
    }
    public override void SetDefaults()
    {
        Banner = Type;
        BannerItem = Mod.Find<ModItem>(NPC.ModNPC.Name + "Banner").Type;
    }
}
public class BannerItemTemplate : ModItem
{
    protected override bool CloneNewInstances => true;

    private int _placedTile;
    private string _nameOverride;
    private string _textureOverride;

    public BannerItemTemplate(int placedTile, string nameOverride, string textureOverride)
    {
        _placedTile = placedTile;
        _nameOverride = nameOverride;
        _textureOverride = textureOverride;
    }

    public override string Texture => _textureOverride;
    public override string Name => _nameOverride;

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(_placedTile);
        Item.height = 24;
        Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
    }
}
public class BannerTileTemplate : ModBannerTile
{
    private string _textureOverride;
    private string _nameOverride;
    public BannerTileTemplate(string textureOverride, string nameOverride)
    {
        _textureOverride = textureOverride;
        _nameOverride = nameOverride;
    }
    public override string Name => _nameOverride;
    public override string Texture => _textureOverride;

}