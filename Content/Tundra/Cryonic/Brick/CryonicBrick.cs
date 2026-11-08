using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Items;
using CalamityVanilla.Content.Tundra.Cryonic.Ore;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TileHelper.Common;

namespace CalamityVanilla.Content.Tundra.Cryonic.Brick;

public class CryonicBrick : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem)
    {
        modItem.Item.ResearchUnlockCount = 100;
        modItem.Item.rare = ItemRarityID.White;
    }
    public void AddItemRecipes(ModItem modItem)
    {
        modItem.CreateRecipe(5).AddTile(TileID.Furnaces).AddIngredient(AutoContent.ItemType<CryonicOre>(),5).AddIngredient(ItemID.StoneBlock).Register();
    }
    public override void SetStaticDefaults()
    {
        Main.tileBrick[Type] = true;
        TileID.Sets.GemsparkFramingTypes[Type] = Type;
        TileID.Sets.ForcedDirtMerging[Type] = true;

        //Main.tileLighted[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(130, 239, 255));
        VanillaFallbackOnModDeletion = TileID.IceBrick;
        HitSound = SoundID.Tink;
        MineResist = 2f;
        DustType = DustID.Ice;
    }
    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
        return false;
    }
    //public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    //{
    //    if (!TileDrawing.IsVisible(Main.tile[i, j]))
    //        return;
    //    CVUtils.DrawTileGlow(Color.White with { A = 64 } * (float)(Math.Sin(Main.timeForVisualEffects * 0.01f) * 0.25f + 0.75f), AssetReferences.Content.Tundra.Cryonic.Brick.CryonicBrickGlow.Asset.Value, i, j);
    //}
    //public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    //{
    //    r += 0.35f / 2;
    //    g += 0.4f / 2;
    //    b += 0.5f / 2;
    //}
    //public override bool CreateDust(int i, int j, ref int type)
    //{
    //    Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<CryonicSmokeDust>());
    //    d.fadeIn = 5;
    //    d.noGravity = true;
    //    d.customData = 0.02f;
    //    d.scale += Main.rand.NextFloat();
    //    d.color.A = 0;

    //    Dust d2 = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<SimpleColorableGlowyDust>());
    //    d2.color = new Color(0.35f, 0.4f, 0.5f, 0f);
    //    d2.noGravity = true;
    //    return false;
    //}
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.NextBool(30) && WorldGen.TileIsExposedToAir(i, j))
        {
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<CryonicSmokeDust>());
            d.fadeIn = 5;
            d.noGravity = false;
            d.customData = 0.03f;
            d.velocity *= 0.1f;
            d.scale += Main.rand.NextFloat();
            d.velocity.Y -= Main.rand.NextFloat();
            d.color *= Main.rand.NextFloat(0.25f);
            d.color.A = 0;
        }
    }
}
