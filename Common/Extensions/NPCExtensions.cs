using CalamityVanilla.Common.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Extensions;

public static class NPCExtensions
{
    extension(NPC npc)
    {
        /// <summary>
        /// This checks if the npc is immune to slows, so you can safely put this on a debuff that does things other than slowing and it'll be fine.
        /// </summary>
        public void ChangeSpeed(float speed)
        {
            if (NPCStats.ImmuneToSlows(npc.type))
                return;
            var n = npc.NPCStats;
            n.Speed *= speed;
        }
        public NPCStats NPCStats => npc.GetGlobalNPC<NPCStats>();
        public void AddDOT(int dps, int damage)
        {
            var instance = npc.NPCStats;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= dps * 2;
            instance.DOTDamage = Math.Max(damage, instance.DOTDamage);
        }
        public void AddDOT(int dps, int damage, Color color)
        {
            var instance = npc.NPCStats;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= dps * 2;
            instance.DOTDamage = Math.Max(damage, instance.DOTDamage);
            instance.EffectColor = instance.EffectColor.MultiplyRGBA(color);
        }
        public Color GetColorTint(Color color)
        {
            color = npc.GetNPCColorTintedByBuffs(color);
            NPCLoader.DrawEffects(npc, ref color);
            return color;
        }
    }
}
