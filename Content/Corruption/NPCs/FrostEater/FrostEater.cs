using CalamityVanilla.Common;
using CalamityVanilla.Common.AIStyles;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.NPCs.FrostEater;

public class FrostEater : ModNPCWithBanner
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 2;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, NPCID.Sets.NPCBestiaryDrawOffset[NPCID.EaterofSouls]);
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptIce,
            new FlavorTextBestiaryInfoElement($"Mods.CalamityVanilla.NPCs." + Name + ".Bestiary")
        });
    }
    public override void SetDefaults()
    {
        NPC.width = 33;
        NPC.height = 33;
        NPC.aiStyle = -1;
        NPC.damage = 75;
        NPC.defense = 28;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
        NPC.knockBackResist = 0.35f;
        NPC.value = 500f;
        DrawOffsetY = 20;
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter > 8)
        {
            NPC.frame.Y = NPC.frame.Y == frameHeight ? 0 : frameHeight;
            NPC.frameCounter = 0;
        }
    }
    public override void AI()
    {
        if (Main.rand.NextBool((int)Utils.Remap(NPC.velocity.Length(), 4.5f, 7, 15, 2)))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Snow);
            d.noGravity = true;
            d.velocity = NPC.velocity * 0.2f;
            d.alpha = 128;
        }
        if (Main.rand.NextBool(35))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<SnowCorruptGibs>());
            d.velocity *= 0.2f;
            d.velocity += NPC.velocity * 0.2f;
        }

        if (!NPC.HasValidTarget)
            NPC.TargetClosest();
        FlyingAI.AI(NPC, 3.6f, Main.expertMode ? 0.02f : 0.01f, 0.4f, true);

        if (NPC.wet)
        {
            if (NPC.velocity.Y > 0f)
                NPC.velocity.Y *= 0.95f;

            NPC.velocity.Y -= 0.3f;
            if (NPC.velocity.Y < -2f)
                NPC.velocity.Y = -2f;
        }

        if (NPC.HasValidTarget)
        {
            Vector2 vector = NPC.Center;
            float num4 = Main.player[NPC.target].Center.X;
            float num5 = Main.player[NPC.target].Center.Y;
            num4 = (int)(num4 / 8f) * 8;
            num5 = (int)(num5 / 8f) * 8;
            vector.X = (int)(vector.X / 8f) * 8;
            vector.Y = (int)(vector.Y / 8f) * 8;
            num4 -= vector.X;
            num5 -= vector.Y;
            NPC.rotation = (float)Math.Atan2(num5, num4) - MathHelper.PiOver2;
        }
        else
            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        int type = ModContent.DustType<SnowCorruptGibs>();
        if (NPC.life > 0)
        {
            for (int i = 0; i < hit.Damage * 0.1f; i++)
            {
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, type, hit.HitDirection, -2f, NPC.alpha, NPC.color, NPC.scale);
            }
            return;
        }
        for (int i = 0; i < 50; i++)
        {
            Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, type, hit.HitDirection, -2f, NPC.alpha, NPC.color, NPC.scale);
        }
        for (int i = 0; i < 3; i++)
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(Name + "Gore_" + i).Type, NPC.scale);
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        target.AddBuff(BuffID.Chilled, 60 * 10);
        if (Main.rand.NextBool(5))
            target.AddBuff(BuffID.Frostburn, 60 * 10);
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.EaterofSouls, false);
        foreach (var dR in dropRules)
        {
            npcLoot.Add(dR);
        }
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.hardMode && spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneCorrupt ? 0.2f : 0f;
    }
}