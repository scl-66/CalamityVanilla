using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.TownNPCs.Priest;

public class PriestAttackDebuff : ModBuff
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.PriestAttackDebuff.KEY;

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<PriestAttackDebuffNPC>().ShouldApply = true;
    }
}

public class PriestAttackDebuffNPC : GlobalNPC
{
    public bool ShouldApply { get; set; }

    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc)
    {
        ShouldApply = false;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!ShouldApply)
            return;

        if (npc.lifeRegen > 0)
        {
            npc.lifeRegen = 0;
        }
        npc.lifeRegen -= 100;
    }
}

public class PriestAttackController : ModProjectile
{
    public override string Texture => Assets.Textures.TownNPCs.Priest.PriestAttackController.KEY;

    public int SpawnNPCWhoAmI
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public ref float Timer => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;

        Projectile.alpha = 255;

        Projectile.friendly = true;
        Projectile.penetrate = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_Parent parentSource)
        {
            SpawnNPCWhoAmI = parentSource.Entity.whoAmI;
        }
    }

    public override void AI()
    {
        if (!Main.npc[SpawnNPCWhoAmI].active || Main.npc[SpawnNPCWhoAmI].type != ModContent.NPCType<Priest>() || Main.npc[SpawnNPCWhoAmI].ai[0] != 14f)
        {
            Projectile.Kill();
            return;
        }

        Timer++;

        var effectDistance = 300;

        var debuffId = ModContent.BuffType<PriestAttackDebuff>();

        if (Main.netMode != NetmodeID.Server)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.active && !player.dead && Projectile.Distance(player.Center) <= effectDistance && player.FindBuffIndex(BuffID.Regeneration) == -1)
            {
                //player.AddBuff(BuffID.Regeneration, 120);
            }
        }

        if (Timer % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type != NPCID.TargetDummy && npc.active && Projectile.Distance(npc.Center) <= effectDistance)
                {
                    if (npc.townNPC && (npc.FindBuffIndex(BuffID.Regeneration) == -1 || npc.buffTime[npc.FindBuffIndex(BuffID.Regeneration)] <= 20))
                    {
                        //npc.AddBuff(165, 120);
                    }
                    if (!npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage && (npc.FindBuffIndex(debuffId) == -1 || npc.buffTime[npc.FindBuffIndex(debuffId)] <= 20))
                    {
                        npc.AddBuff(debuffId, 120);
                    }
                }
            }
        }

        if (Timer >= 570f)
        {
            Projectile.Kill();
        }
    }
}