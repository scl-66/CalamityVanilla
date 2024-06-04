using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityVanilla.Content.NPCs.Bosses.HiveMind
{
    public partial class HiveMind
    {
        private void Phase0()
        {
            NPC.ai[0]++;

            if (NPC.ai[0] > 200)
            {
                NPC.alpha += 5;
            }
        }
    }
}
