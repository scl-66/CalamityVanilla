using CalamityVanilla.Content.Bosses.Cryogen;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public class CryogenIceBlockSystem : ModSystem
{
    public const int DEFAULT_ICE_TIMER = 2400;
    public static List<Point> CryogenIceBlocks = new List<Point>();
    private const int MaxCryogenIceBlocks = 800;
    public override void OnWorldUnload()
    {
        CryogenIceBlocks.Clear();
    }
    public override void PostUpdateWorld()
    {
        bool cryogenIsREAL = false;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.type == ModContent.NPCType<Cryogen>())
            {
                cryogenIsREAL = true;
                break;
            }
        }
        if (!cryogenIsREAL)
        {
            for (int i = 0; i < CryogenIceBlocks.Count; i++)
            {
                WorldGen.KillTile(CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y, false, false, true);
                NetMessage.SendTileSquare(-1, CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y);
            }
            CryogenIceBlocks.Clear();
            return;
        }
        for (int i = 0; i < CryogenIceBlocks.Count; i++)
        {
            if (Main.tile[CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y].TileType != ModContent.TileType<CryogenIceTile>())
            {
                CryogenIceBlocks.RemoveAt(i);
                return;
            }
            else if (CryogenIceBlocks.Count > MaxCryogenIceBlocks)
            {
                if (i < MaxCryogenIceBlocks)
                {
                    WorldGen.KillTile(CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y, false, false, true);
                    NetMessage.SendTileSquare(-1, CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y);
                    CryogenIceBlocks.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}