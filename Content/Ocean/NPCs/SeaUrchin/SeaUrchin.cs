using CalamityVanilla.Common;
using CalamityVanilla.Common.AIStyles;
using CalamityVanilla.Content.Ocean.Items.UrchinSpine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Ocean.NPCs.SeaUrchin;

public class SeaUrchin : ModNPCWithBanner
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 5;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Velocity = 1f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
            new FlavorTextBestiaryInfoElement($"Mods.CalamityVanilla.NPCs.SeaUrchin.Bestiary")
        });
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.aiStyle = -1;
        NPC.width = 42;
        NPC.height = 42;
        NPC.lifeMax = 240;
        NPC.damage = 50;
        NPC.knockBackResist = 0.7f;
        NPC.value = 1000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.GravityIgnoresLiquid = true;
        NPC.waterMovementSpeed = 1f;
    }
    public override void AI()
    {
        FighterAI.AI(NPC, NPC.collideY ? 1 : 6, out bool JustJumped, hasWideHitbox: true);
        NPC.spriteDirection = NPC.direction;
        if (JustJumped)
        {
            NPC.localAI[2] = 1;
            NPC.velocity.X += NPC.direction * 2;
            NPC.collideY = false;
        }
        if (NPC.collideY || NPC.justHit)
        {
            NPC.localAI[2] = 0;
        }
        if (NPC.localAI[2] == 1)
        {
            NPC.rotation += MathHelper.Clamp(NPC.velocity.X * 0.1f, -0.1f, 0.1f);
        }
        else
        {
            NPC.rotation = 0;
        }
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < hit.Damage; i++)
        {
            //Uses stone dust with water's frame because I don't want water behavior.
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Stone);
            d.frame.X = 10 * DustID.Water;
            d.velocity += NPC.velocity;
            d.alpha = 180;
        }
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity + Main.rand.NextVector2Square(0, 2), Mod.Find<ModGore>(Name + "Gore" + i).Type);
            }
        }
    }
    public override void FindFrame(int frameHeight)
    {
        if (NPC.collideY || NPC.IsABestiaryIconDummy)
            NPC.frameCounter += Math.Abs(NPC.velocity.X);
        if (NPC.frameCounter > 3)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y >= frameHeight * 5)
                NPC.frame.Y = 0;
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        if (Main.rand.NextBool())
            target.AddBuff(BuffID.Venom, 60 * 10);
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(new CommonDrop(ItemID.ShrimpPoBoy, 50));
        npcLoot.Add(new CommonDrop(ModContent.ItemType<UrchinSpine>(), 1, 33, 75));
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.ZoneBeach && Main.hardMode ? 0.1f : 0;
    }
}