using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.TownNPCs.Priest.Items.CheckerBlocks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TileHelper.Common;

namespace CalamityVanilla.Content.TownNPCs.Priest.Items;

public enum Colors
{
    Black,
    White
}
public enum Pieces
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class GenerateChessPieces : ModSystem
{
    public override void Load()
    {
        foreach (Colors c in (Colors[])Enum.GetValues(typeof(Colors)))
        {
            foreach (Pieces p in (Pieces[])Enum.GetValues(typeof(Pieces)))
            {
                string itemName = c.ToString() + p.ToString();
                string tileName = itemName + "Tile";
                Mod.AddContent(new ChessTileTemplate("CalamityVanilla/Assets/Textures/TownNPCs/Priest/Items/ChessPieces/" + tileName, tileName, c, (int)p < 3));
                int chessTile = Mod.Find<ModTile>(tileName).Type;
                Mod.AddContent(new ChessItemTemplate(chessTile, itemName, "CalamityVanilla/Assets/Textures/TownNPCs/Priest/Items/ChessPieces/" + itemName, c, p));
            }
        }
    }
}

public class ChessItemTemplate : ModItem
{
    protected override bool CloneNewInstances => true;

    private int _placedTile;
    private string _nameOverride;
    private string _textureOverride;
    private Colors _color;
    private Pieces _piece;

    public ChessItemTemplate(int placedTile, string nameOverride, string textureOverride, Colors color, Pieces piece)
    {
        _placedTile = placedTile;
        _nameOverride = nameOverride;
        _textureOverride = textureOverride;
        _color = color;
        _piece = piece;
    }

    public override string Texture => _textureOverride;
    public override string Name => _nameOverride;

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(_placedTile);
        Item.SetShopValues(ItemRarityColor.White0, Item.buyPrice(silver: 2));

    }

    public override void AddRecipes()
    {
        if (Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<BlackCheckerBlock>()), out ModItem black) &&
            Helpers.TryGetBlockItem(TileLoader.GetTile(ModContent.TileType<WhiteCheckerBlock>()), out ModItem white))
            CreateRecipe(_piece == Pieces.Pawn ? 4 : _piece < Pieces.Queen ? 2 : 1)
                .AddIngredient(_color == Colors.Black ? black.Type : white.Type)
                .AddTile(TileID.Anvils)
                .Register();
    }
}

public class ChessTileTemplate : ModTile
{
    private string _textureOverride;
    private string _nameOverride;
    private bool _isSmall;
    private Colors _color;
    public ChessTileTemplate(string textureOverride, string nameOverride, Colors color, bool isSmall)
    {
        _textureOverride = textureOverride;
        _nameOverride = nameOverride;
        _isSmall = isSmall;
        _color = color;
    }
    public override string Name => _nameOverride;
    public override string Texture => _textureOverride;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileLavaDeath[Type] = false;
        Main.tileNoAttach[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(_isSmall ? TileObjectData.Style1x2 : TileObjectData.Style1xX);
        TileObjectData.newTile.CoordinateHeights = _isSmall ? [16, 18] : [16, 16, 18];
        TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleWrapLimit = 2;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        DustType = _color == Colors.Black ? ModContent.DustType<BlackCheckerDust>() : ModContent.DustType<WhiteCheckerDust>();
        AddMapEntry(_color == Colors.Black ? new Color(38, 36, 60) : new Color(95, 92, 92), Language.GetText("Mods.CalamityVanilla.Tiles.ChessPieceTile.MapEntry"));
    }
}