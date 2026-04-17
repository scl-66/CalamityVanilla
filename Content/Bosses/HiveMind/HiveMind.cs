using CalamityVanilla.Common;
using CalamityVanilla.Content.Bosses.HiveMind.Drops;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.FilthyGrip;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.FungiDish;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.MyceliumStaff;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.PerfectDark;
using CalamityVanilla.Content.Vanity.BossMasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind;

[AutoloadBossHead]
public partial class HiveMind : ModNPC
{
    public Player target
    { get { return Main.player[NPC.target]; } }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        if (NPC.alpha > 64)
            return false;
        return base.CanHitPlayer(target, ref cooldownSlot);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 5; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(6, 6), Mod.Find<ModGore>("HiveMind" + $"{i}").Type);
            }
            for (int i = 0; i < 100; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.scale = Main.rand.NextFloat(1, 2);
                d.noGravity = !Main.rand.NextBool(3);
            }
            for (int i = 0; i < 100; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.scale = Main.rand.NextFloat(1, 2);
                d.noGravity = Main.rand.NextBool();
            }
        }
    }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 4;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = Texture + "_Bestiary",
            PortraitPositionYOverride = 0f,
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Width = 180;

        NPC.frameCounter++;
        if (NPC.frameCounter > 7)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y > frameHeight * 3)
            {
                NPC.frame.Y = 0;
            }
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D tex = TextureAssets.Npc[NPC.type].Value;
        spriteBatch.Draw(tex,NPC.Bottom - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor) * NPC.Opacity, NPC.rotation, new Vector2(NPC.frame.Width / 2,NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
        return false;
    }

    public override void BossLoot(ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HiveMindTrophy>(), 10));

        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<HiveMindMask>(), 7));
        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FilthyGrip>(), 3));
        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<MyceliumStaff>(), ModContent.ItemType<PerfectDark>()));

        npcLoot.Add(notExpertRule);
        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<HiveMindBag>()));
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<HiveMindRelic>()));
        npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<FungiDish>(), 4));
    }

    public override void OnKill()
    {
        BossDownedSystem.DownedHiveMind = true;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }

    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.EyeofCthulhu);

        NPC.lifeMax = 16000;
        NPC.defense = 30;

        NPC.aiStyle = -1;
        NPC.behindTiles = true;
        NPC.noGravity = false;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HiveMind");
        NPC.Size = new Vector2(150);
        NPC.noTileCollide = false;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement($"Mods.CalamityVanilla.NPCs.HiveMind.Bestiary")

        });
    }
}