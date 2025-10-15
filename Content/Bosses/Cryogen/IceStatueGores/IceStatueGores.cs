using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen.IceStatueGores;

public class IceStatueGoreGeneric_0 : ModGore
{
    public override Color? GetAlpha(Gore gore, Color lightColor)
    {
        return new Color(0.8f, 0.8f, 0.8f, 0.5f) * (1f - (gore.alpha / 255f));
    }
}
public class IceStatueGoreGeneric_1 : IceStatueGoreGeneric_0;
public class IceStatueGoreGeneric_2 : IceStatueGoreGeneric_0;
public class IceStatueGore0_0 : IceStatueGoreGeneric_0;
public class IceStatueGore0_1 : IceStatueGoreGeneric_0;
public class IceStatueGore0_2 : IceStatueGoreGeneric_0;
public class IceStatueGore0_3 : IceStatueGoreGeneric_0;
public class IceStatueGore1_0 : IceStatueGoreGeneric_0;
public class IceStatueGore1_1 : IceStatueGoreGeneric_0;
public class IceStatueGore1_2 : IceStatueGoreGeneric_0;
public class IceStatueGore1_3 : IceStatueGoreGeneric_0;
public class IceStatueGore1_4 : IceStatueGoreGeneric_0;
public class IceStatueGore1_5 : IceStatueGoreGeneric_0;
public class IceStatueGore2_0 : IceStatueGoreGeneric_0;
public class IceStatueGore2_1 : IceStatueGoreGeneric_0;
public class IceStatueGore2_2 : IceStatueGoreGeneric_0;
public class IceStatueGore2_3 : IceStatueGoreGeneric_0;
public class IceStatueGore3_0 : IceStatueGoreGeneric_0;
public class IceStatueGore3_1 : IceStatueGoreGeneric_0;
public class IceStatueGore3_2 : IceStatueGoreGeneric_0;
public class IceStatueGore3_3 : IceStatueGoreGeneric_0;
public class IceStatueGore3_4 : IceStatueGoreGeneric_0;
public class IceStatueGore3_5 : IceStatueGoreGeneric_0;
public class IceStatueGore4_0 : IceStatueGoreGeneric_0;
public class IceStatueGore4_1 : IceStatueGoreGeneric_0;
public class IceStatueGore4_2 : IceStatueGoreGeneric_0;
public class IceStatueGore4_3 : IceStatueGoreGeneric_0;