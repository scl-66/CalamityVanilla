using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Dusts;

public abstract class OreDust : ModDust
{
    public override void SetStaticDefaults()
    {
        UpdateType = DustID.LunarOre;
    }
}

public class UraniumDust : OreDust { }
public class PlutoniumDust : OreDust { }
public class BismuthDust : OreDust { }
public class ExoditeDust : OreDust { }
public class VibraniteDust : OreDust { }
public class UnobtaniumDust : OreDust { }
