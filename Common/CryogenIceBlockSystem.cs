using CalamityVanilla.Content.Bosses.Cryogen;
using CalamityVanilla.Content.Tiles;
using CalamityVanilla.Content.Tundra.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityVanilla.Common;

public struct CryogenIceData
{
    public ushort X;
    public ushort Y;
    public ushort Time;
    public CryogenIceData(int x, int y, int time)
    {
        X = (ushort)x;
        Y = (ushort)y;
        Time = (ushort)time;
    }
}
public class CryogenIceBlockSystem : ModSystem
{
    public const int DEFAULT_ICE_TIMER = 2400;
    public const int CrogenIceDataChunkSize = 3;
    public static List<CryogenIceData> CryogenIceBlocks = new List<CryogenIceData>();
    public override void OnWorldUnload()
    {
        CryogenIceBlocks.Clear();
    }
    public static void AddTime(int x, int y, int Time)
    {
        for (int i = 0; i < CryogenIceBlocks.Count; ++i)
        {
            if (CryogenIceBlocks[i].X == x && CryogenIceBlocks[i].Y == y)
            {
                CryogenIceBlocks[i] = new CryogenIceData(x, y, Time);
                return;
            }
        }
        CryogenIceBlocks.Add(new CryogenIceData(x, y, Time));
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
        for (int i = 0; i < CryogenIceBlocks.Count; i++)
        {
            CryogenIceBlocks[i] = new CryogenIceData(CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y, (ushort)(CryogenIceBlocks[i].Time - 1));
            if (Main.tile[CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y].TileType != ModContent.TileType<CryogenIceTile>())
            {
                CryogenIceBlocks.RemoveAt(i);
                return;
            }
            if (CryogenIceBlocks[i].Time <= 0 || (!cryogenIsREAL && Main.rand.NextBool(10)))
            {
                WorldGen.KillTile(CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y, false, false, true);
                NetMessage.SendTileSquare(-1, CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y);
                CryogenIceBlocks.RemoveAt(i);
            }
        }
    }

    // If there's a way to get all the ice blocks to go away before the world's saved and unloaded that'd be better.

    //public override void SaveWorldData(TagCompound tag)
    //{
    //    tag["CalamityVanilla:IceBlocks"] = CryogenIceBlocks;
    //}
    //public override void LoadWorldData(TagCompound tag)
    //{
    //    if (tag.ContainsKey("CalamityVanilla:IceBlocks"))
    //    {
    //        CryogenIceBlocks = tag.Get<List<CryogenIceData>>("CalamityVanilla:IceBlocks");
    //    }
    //}
}