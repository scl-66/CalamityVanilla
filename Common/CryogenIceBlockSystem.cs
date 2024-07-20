using CalamityVanilla.Content.Projectiles.Hostile;
using CalamityVanilla.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common
{
    public class CryogenIceBlockSystem : ModSystem
    {
        public const int DEFAULT_ICE_TIMER = 600;
        public static List<Vector3> CryogenIceBlocks = new List<Vector3>();
        public override void OnWorldLoad()
        {
            CryogenIceBlocks.Clear();
        }
        public override void OnWorldUnload()
        {
            CryogenIceBlocks.Clear();
        }
        public override void PostUpdateWorld()
        {
            for(int i = 0; i < CryogenIceBlocks.Count; i++)
            {
                // x and y are the tile coords, z is the timer before it explodes !!!
                CryogenIceBlocks[i] -= new Vector3(0, 0, 1);
                if (Main.tile[(int)CryogenIceBlocks[i].X, (int)CryogenIceBlocks[i].Y].TileType != ModContent.TileType<CryogenIceTile>())
                {
                    CryogenIceBlocks.RemoveAt(i);
                    return;
                }
                if (CryogenIceBlocks[i].Z < 0)
                {
                    WorldGen.KillTile((int)CryogenIceBlocks[i].X, (int)CryogenIceBlocks[i].Y,false,false,true);
                    NetMessage.SendTileSquare(-1, (int)CryogenIceBlocks[i].X, (int)CryogenIceBlocks[i].Y);
                    CryogenIceBlocks.RemoveAt(i);
                }
            }
        }
    }
}
