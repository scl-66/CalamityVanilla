using CalamityVanilla.Content.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common
{
    public class CVModPlayer : ModPlayer
    {
        public int GothicToothRegenCounter = 0;
        public override void PostUpdateBuffs()
        {
            GothicToothRegenCounter++;
            if (GothicToothRegenCounter > 120)
            {
                if (!Player.moonLeech)
                {
                    int lifeRegen = 0;
                    foreach(Projectile tooth in Main.ActiveProjectiles)
                    {
                        if(tooth.type == ModContent.ProjectileType<GothicTooth>() && tooth.owner == Player.whoAmI && tooth.ai[0] == 1 && Main.npc[(int)tooth.ai[1]].type != NPCID.TargetDummy)
                        {
                            lifeRegen++;
                        }
                    }
                    if (lifeRegen == 0)
                        return;
                    if (lifeRegen > 30)
                        lifeRegen = 30;
                    Player.statLife += lifeRegen;
                    CombatText.NewText(Player.Hitbox, CombatText.HealLife, lifeRegen);
                    for(int i = 0; i < lifeRegen * 2; i++)
                    {
                        Vector2 rotation = Main.rand.NextVector2Circular(1,1);
                        Dust d = Dust.NewDustPerfect(Player.Center + rotation * 40, DustID.VampireHeal);
                        d.velocity = -rotation * 3 + Player.velocity;
                        d.scale *= 1.3f;
                        d.noGravity = true;
                    }
                }
                GothicToothRegenCounter = 0;
            }
        }
    }
}
