using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

[ReinitializeDuringResizeArrays]
public static class CVItemSets
{
    public static bool?[] CanBeReflected = ProjectileID.Sets.Factory.CreateNamedSet("CanBeReflected")
        .Description("Makes projectiles with the ID reflectable by Frost Guard Staff.")
        .RegisterCustomSet<bool?>(false
            );
}
