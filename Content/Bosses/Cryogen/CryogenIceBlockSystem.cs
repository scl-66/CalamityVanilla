using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;
public struct CryogenIce
{
    public CryogenIce(int x, int y, int timeLeft)
    {
        X = (ushort)x;
        Y = (ushort)y;
        TimeLeft = (ushort)timeLeft;
    }
    public ushort X;
    public ushort Y;
    public ushort TimeLeft;
}
public class CryogenIceBlockSystem : ModSystem
{
    public static List<CryogenIce> CryogenIceBlocks = new List<CryogenIce>();
    private const int MaxCryogenIceBlocks = 800;
    public static bool PlaceIceBlock(int x, int y, int timeLeft = 900)
    {
        for (int i = 0; i < CryogenIceBlocks.Count; i++)
        {
            if (CryogenIceBlocks[i].X == x && CryogenIceBlocks[i].Y == y && CryogenIceBlocks[i].TimeLeft < timeLeft)
            {
                CryogenIceBlocks.RemoveAt(i);
                CryogenIceBlocks.Add(new CryogenIce(x, y, timeLeft));
                NetMessage.SendTileSquare(-1, x, y);
                return true;
            }
        }
        if (WorldGen.PlaceTile(x,y,ModContent.TileType<CryogenIceTile>(), plr: Main.myPlayer))
        {
            CryogenIceBlocks.Add(new CryogenIce(x,y,timeLeft));
            NetMessage.SendTileSquare(-1, x,y);
            return true;
        }
        return false;
    }
    public override void OnWorldUnload()
    {
        CryogenIceBlocks.Clear();
    }
    public override void PostUpdateWorld()
    {
        if(CryogenIceBlocks.Count == 0)
            return;
        bool cryogenIsREAL = false;
        int iceBlockType = ModContent.TileType<CryogenIceTile>();
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
            CryogenIceBlocks[i] = CryogenIceBlocks[i] with { TimeLeft = (ushort)(CryogenIceBlocks[i].TimeLeft - 1) };
            //if (i == 0)
            //    Main.NewText(CryogenIceBlocks[i].TimeLeft);
            if (Main.tile[CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y].TileType != iceBlockType)
            {
                CryogenIceBlocks.RemoveAt(i);
                i--;
                return;
            }
            else if (CryogenIceBlocks[i].TimeLeft == 0 || (CryogenIceBlocks.Count > MaxCryogenIceBlocks && i < MaxCryogenIceBlocks))
            {
                WorldGen.KillTile(CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y, false, false, true);
                NetMessage.SendTileSquare(-1, CryogenIceBlocks[i].X, CryogenIceBlocks[i].Y);
                CryogenIceBlocks.RemoveAt(i);
                i--;
            }
        }
    }
}