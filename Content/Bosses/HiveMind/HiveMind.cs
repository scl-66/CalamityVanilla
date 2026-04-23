using CalamityVanilla.Common;
using CalamityVanilla.Content.Bosses.HiveMind.Drops;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.AdrianHelmet;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.FilthyGrip;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.FungiDish;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.MushroomBomber;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.MyceliumStaff;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.PerfectDark;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.SinisterIncubator;
using CalamityVanilla.Content.Bosses.HiveMind.Minions;
using CalamityVanilla.Content.Vanity.BossMasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind;

public class HiveBossBar : ModBossBar
{
    public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
    {
        return ModContent.Request<Texture2D>(ModContent.GetInstance<HiveMind>().BossHeadTexture);
    }
    public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
    {
        if (Main.npc[info.npcIndexToAimAt].dontTakeDamage)
        {
            shield = Main.npc[info.npcIndexToAimAt].ai[3];
            shieldMax = HiveMind.ShieldMax;
        }
        else
        {
            shield = 0;
            shieldMax = 0;
        }
        return base.ModifyInfo(ref info, ref life, ref lifeMax, ref shield, ref shieldMax);
    }
}

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
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        //NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        //{
        //    CustomTexturePath = Texture + "_Bestiary",
        //    PortraitPositionYOverride = 0f,
        //};
        //NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Width = 180;
        if (_currentAttack > 20)
        {
            NPC.frame.X = 180;
            if (!NPC.dontTakeDamage)
                NPC.frameCounter++;
        }
        else
        {
            NPC.frame.X = 0;
        }
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
        if (NPC.IsABestiaryIconDummy)
        {
            spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(0,6), NPC.frame, Color.White, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        spriteBatch.Draw(tex, NPC.Bottom - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor) * NPC.Opacity, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
        if (NPC.Opacity != 1)
        {
            //Color glowColor = NPC.GetNPCColorTintedByBuffs(Color.Lerp(drawColor, Color.Purple with { A = 0 }, 1f - NPC.Opacity)) * NPC.Opacity * NPC.Opacity;
            //for (int i = 0; i < 4; i++)
            //{
            //    spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(0,NPC.alpha * 0.1f).RotatedBy(i * MathHelper.PiOver2), NPC.frame, glowColor, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
            //    spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(0, NPC.alpha * 0.15f).RotatedBy(i * MathHelper.PiOver2 + MathHelper.PiOver4), NPC.frame, glowColor * 0.5f, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
            //}
            Color glowColor = NPC.GetNPCColorTintedByBuffs(drawColor) * NPC.Opacity * NPC.Opacity;
            for (int i = -1; i <= 1; i++)
            {
                spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(NPC.alpha * 0.3f * i, 0), NPC.frame, glowColor, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(NPC.alpha * 0.6f * i, 0), NPC.frame, glowColor, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
            }
        }
        if (NPC.dontTakeDamage)
        {
            //int swooper = ModContent.NPCType<HiveMindSwooper>();
            //int weeper = ModContent.NPCType<HiveMindWeeper>();
            //foreach (NPC n in Main.ActiveNPCs)
            //{
            //    if (n.type != weeper && n.type != swooper)
            //        continue;
            //    Utils.DrawLine(spriteBatch, NPC.Center, n.Center, Color.Transparent, Color.Purple with { A = 0 } * 0.25f, 4);
            //}

            float interval = 160;
            float amount = (float)(Main.timeForVisualEffects % interval) / interval;
            Color c = Color.Purple with { A = 0 } * amount * NPC.Opacity * (1f - amount);
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(0, 16 * amount).RotatedBy((i * MathHelper.PiOver2) + MathHelper.PiOver4), NPC.frame, c, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
            }
            amount = (float)((Main.timeForVisualEffects + (interval / 2)) % interval) / interval;
            c = Color.Purple with { A = 0 } * amount * NPC.Opacity * (1f - amount);
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(tex, NPC.Bottom - screenPos + new Vector2(0, 16 * amount).RotatedBy(i * MathHelper.PiOver2), NPC.frame, c, NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height - 34), NPC.scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            DrawData value91 = new DrawData(Main.Assets.Request<Texture2D>("Images/Misc/noise").Value, NPC.Center - screenPos, new Rectangle(0, 0, 500, 340), Color.Purple with { A = 0 }, 0, new Vector2(500, 340) / 2, new Vector2(1f + (float)Math.Sin(Main.timeForVisualEffects * 0.015f) * 0.05f, 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.02f) * 0.05f) * NPC.Opacity, SpriteEffects.None, 0);
            GameShaders.Misc["ForceField"].UseColor(new Vector3((_shieldAmountForPhase2 / (float)ShieldMax) + 1f) * NPC.localAI[2]);
            GameShaders.Misc["ForceField"].Apply(value91);
            value91.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
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
        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
            ModContent.ItemType<PerfectDark>(),
            ModContent.ItemType<MushroomBomber>(),
            ModContent.ItemType<MyceliumStaff>(),
            ModContent.ItemType<SinisterIncubator>()));
        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AdrianHelmet>(), 50));
        notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.SoulofNight, 1, 7, 12));

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
    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment * (2 / 3f));
    }
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.EyeofCthulhu);

        NPC.lifeMax = 28800;
        NPC.defense = 30;

        NPC.aiStyle = -1;
        NPC.behindTiles = true;
        NPC.noGravity = false;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HiveMind");
        NPC.Size = new Vector2(150);
        NPC.noTileCollide = false;
        NPC.BossBar = ModContent.GetInstance<HiveBossBar>();
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