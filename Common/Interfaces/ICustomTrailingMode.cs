using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Interfaces;

public interface ICustomTrailingMode
{
    public void UpdateTrails(NPC npc);
}
internal class CustomUpdateOldPositions : ILoadable
{
    public void Load(Mod mod)
    {
        On_NPC.UpdateNPC_UpdateTrails += On_NPC_UpdateNPC_UpdateTrails;
    }
    private void On_NPC_UpdateNPC_UpdateTrails(On_NPC.orig_UpdateNPC_UpdateTrails orig, NPC self)
    {
        if (self.ModNPC is ICustomTrailingMode i)
            i.UpdateTrails(self);
    }
    public void Unload()
    {
    }
}
