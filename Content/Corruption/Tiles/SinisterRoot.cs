using CalamityVanilla.Content.Bosses.HiveMind;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.Corruption.Tiles;

public class SinisterRoot : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileCut[Type] = true;
        Main.tileNoFail[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileID.Sets.IgnoredInHouseScore[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(124, 87, 149), name);

        TileObjectData.newTile.CoordinateHeights = [20];
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.RandomStyleRange = 3;

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
        TileObjectData.newTile.AnchorValidTiles = [
            TileID.CorruptGrass,
            TileID.Ebonstone,
            TileID.Ebonsand,
            TileID.CorruptSandstone,
            TileID.CorruptHardenedSand,
            TileID.CorruptJungleGrass,
            TileID.CorruptIce,
        ];
        TileObjectData.newTile.AnchorAlternateTiles = [
            TileID.ClayPot,
            TileID.PlanterBox
        ];
        TileObjectData.addTile(Type);

        HitSound = SoundID.Grass;
        DustType = DustID.Corruption;
        RegisterItemDrop(ModContent.ItemType<HiveMindSummon>());
    }
    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        offsetY = -2;
    }
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, DustID.Corruption);
            d.velocity.X *= 0.1f;
            d.velocity.Y = Main.rand.NextFloat(-1f, 0f);
            d.alpha = 128;
        }
        if (Main.rand.NextBool(45))
        {
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, Main.rand.NextBool() ? DustID.CursedTorch : DustID.CorruptTorch);
            d.noGravity = true;
            d.fadeIn = 1;
            d.scale = 0.1f;
        }
    }
}
public class SinisterRootSpawner : GlobalTile
{
    public override void RandomUpdate(int i, int j, int type)
    {
        int tileType = ModContent.TileType<SinisterRoot>();
        if ((type != TileID.CorruptGrass && type != TileID.Ebonstone && type != TileID.Ebonsand && type != TileID.CorruptSandstone && type != TileID.CorruptHardenedSand && type != TileID.CorruptJungleGrass && type != TileID.CorruptIce) ||
            Main.tile[i, j - 1].TileType == tileType || !Main.hardMode || !Main.rand.NextBool(1000))
            return;
        for (int x = -48; x < 48; x++)
        {
            for (int y = -32; y < 32; y++)
            {
                if (Main.tile[Math.Clamp(i + x, 0, Main.maxTilesX), Math.Clamp(j + y, 0, Main.maxTilesY)].TileType == tileType)
                    return;
            }
        }
        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SinisterRoot>(), style: Main.rand.Next(3));
    }
}