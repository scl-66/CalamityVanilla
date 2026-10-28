using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.DataSets;

[ReinitializeDuringResizeArrays]
public class BuffSets
{
    public static bool[] SpeedChangeBuff = BuffID.Sets.Factory.CreateNamedSet("SpeedChangeBuff")
    .Description("for buffs that change the npc's speed")
    .RegisterBoolSet();
}
