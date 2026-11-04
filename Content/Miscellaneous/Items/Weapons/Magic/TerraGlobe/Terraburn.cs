using CalamityVanilla.Common.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.TerraGlobe;

public class Terraburn : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Terra);
        d.noGravity = true;
        d.velocity *= 0.5f;
        d.velocity.Y -= 1;
        d.fadeIn = Main.rand.NextFloat(1.5f);
        npc.AddDOT(50, 25, new Color(0.45f,0.8f,0.55f,0.85f));
    }
}
