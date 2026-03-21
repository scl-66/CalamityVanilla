using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.MushroomBomber;

public class MushroomBomberInfection : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<MushroomBomberInfectionNPC>().Active = true;
    }
}
public class MushroomBomberInfectionNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public bool Active = false;

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if(Active)
        {
            int stacks = 0;
            int type = ModContent.ProjectileType<MushroomBomberShotMedium>();
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == type && p.ai[1] == npc.whoAmI)
                    stacks++;
            }
            npc.lifeRegen -= 15 * stacks;
            damage = Math.Max(damage, 5 * stacks);
        }
    }
}
