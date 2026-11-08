using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.World;
using CalamityVanilla.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using TileHelper.Common;

namespace CalamityVanilla.Content.Tundra.Cryonic.Ore;

public class CryonicOre : ModTile, ILoadItem
{
    public void SetItemDefaults(ModItem modItem)
    {
        modItem.Item.ResearchUnlockCount = 100;
        modItem.Item.rare = ItemRarityID.Green;
        modItem.Item.value = Item.sellPrice(0, 0, 5, 0);
    }
    public override void Load()
    {
        On_Player.PickTile += On_Player_PickTile;
    }

    private void On_Player_PickTile(On_Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
    {
        if (pickPower < 100 && Main.tile[x, y].TileType == ModContent.TileType<CryonicOre>())
        {
            int buffType = ModContent.BuffType<FrozenTool>();
            int debuffTime = 90;
            int index = self.FindBuffIndex(buffType);
            if(index != -1)
            {
                if(self.buffTime[index] > FrozenTool.MaxTime * 0.5f)
                {
                    self.AddBuff(BuffID.Chilled, self.buffTime[index] * 2);
                    if (self.buffTime[index] >= FrozenTool.MaxTime)
                    {
                        //self.DelBuff(index);
                        self.AddBuff(BuffID.Frozen, debuffTime);
                        orig(self, x, y, pickPower);
                        return;
                    }
                }
                self.AddBuff(buffType, debuffTime + self.buffTime[index]) ;
            }
            else
            {
                self.AddBuff(buffType, debuffTime);
            }
        }
        orig(self, x, y, pickPower);
    }
    public override void SetStaticDefaults()
    {
        TileID.Sets.Ore[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        Main.tileSpelunker[Type] = true;
        Main.tileOreFinderPriority[Type] = 450;
        Main.tileShine[Type] = 2000;
        Main.tileLighted[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(130, 239, 255), name);
        VanillaFallbackOnModDeletion = TileID.IceBlock;
        HitSound = SoundID.Tink;
        MineResist = 2f;
        MinPick = 100;
        TileID.Sets.ChecksForMerge[Type] = true;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.FleshIce][Type] = true;
        Main.tileMerge[TileID.CorruptIce][Type] = true;
        Main.tileMerge[TileID.HallowedIce][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
    }
    public override bool CanExplode(int i, int j)
    {
        return false;
    }
    public override bool IsTileDangerous(int i, int j, Player player)
    {
        return player.iceSkate;
    }
    public override void FloorVisuals(Player player)
    {
        if (!player.iceSkate)
        {
            player.slippy2 = true;
            player.AddBuff(BuffID.Chilled, 60 * 5);
        }
    }
    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (!TileDrawing.IsVisible(Main.tile[i, j]))
            return;
        CVUtils.DrawTileGlow(Color.White with { A = 64 } * (float)(Math.Sin(Main.timeForVisualEffects * 0.01f) * 0.15f + 0.35f), AssetReferences.Content.Tundra.Cryonic.Ore.CryonicOre_Glow.Asset.Value, i, j);
    }
    public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
    {
        WorldGen.TileMergeAttempt(-2, TileID.Sets.IcesSnow, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r += 0.35f / 2;
        g += 0.4f / 2;
        b += 0.5f / 2;
    }
    public override bool CreateDust(int i, int j, ref int type)
    {
        Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<CryonicSmokeDust>());
        d.fadeIn = 5;
        d.noGravity = true;
        d.customData = 0.02f;
        d.scale += Main.rand.NextFloat();
        d.color.A = 0;

        Dust d2 = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<SimpleColorableGlowyDust>());
        d2.color = new Color(0.35f, 0.4f, 0.5f, 0f);
        d2.noGravity = true;
        return false;
    }
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.NextBool(4) && WorldGen.TileIsExposedToAir(i,j))
        {
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<CryonicSmokeDust>());
            d.fadeIn = 5;
            d.noGravity = false;
            d.customData = 0.03f;
            d.velocity *= 0.1f;
            d.scale += Main.rand.NextFloat();
            d.velocity.Y -= Main.rand.NextFloat();
            d.color *= Main.rand.NextFloat();
            d.color.A = 0;
        }
    }
}
internal class CryonicGen : CVGenPass
{
    public override string Name => base.Name;
    public override string Parent => "Ice";
    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = ProgressMessage.Value;

        int Veins = (int)(30 * (Main.maxTilesX / 4200f));
        for (int i = 0; i < Veins; i++)
        {
            int y = WorldGen.genRand.Next((int)Main.rockLayer, GenVars.snowBottom);
            int x = WorldGen.genRand.Next(GenVars.snowMinX[y], GenVars.snowMaxX[y]);
            if (TileID.Sets.IcesSnow[Main.tile[x,y].TileType])
                WorldGen.TileRunner(x,y, WorldGen.genRand.Next(4,9), WorldGen.genRand.Next(10, 30), (ushort)ModContent.TileType<CryonicOre>());
        }
    }
}