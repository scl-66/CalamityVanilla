using CalamityVanilla.Common.DataSets;
using CalamityVanilla.Common.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Buffs;

public class ChilledCV : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffSets.SpeedChangeBuff[Type] = true;
        BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Frostburn);
    }
    public override string Texture => $"Terraria/Images/Buff_{BuffID.Chilled}";

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.ChangeSpeed(0.66f);
        var stats = npc.NPCStats;
        stats.EffectColor = stats.EffectColor.MultiplyRGB(new Color(0.8f,0.9f,1));
        if (Main.rand.NextBool(15))
        {
            Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Snow);
            d.noGravity = true;
            d.velocity *= 0.1f;
        }
    }
}
