using CalamityVanilla.Common;
using CalamityVanilla.Content.Bosses.Cryogen.Drops;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.HoarfrostBow;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.Icebreaker;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.MagicChisel;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.TheSnowman;
using CalamityVanilla.Content.NPCs.TownNPCs.Priest;
using CalamityVanilla.Content.Particles;
using CalamityVanilla.Content.Vanity.BossMasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;

[AutoloadBossHead]
public partial class Cryogen : ModNPC
{

    const bool ForTheWorthy = false;
    private bool IsInPhase2()
    {
        return NPC.life <= (float)NPC.lifeMax * 0.5f;
    }
    public Player target
    { get { return Main.player[NPC.target]; } }

    private static Asset<Texture2D> backTexture;
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 7; i++)
            {
                int g = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(6, 6), Mod.Find<ModGore>("Cryogen" + $"{i}").Type);
            }
            for (int i = 0; i < 100; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Ice);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.scale = Main.rand.NextFloat(1, 2);
                d.noGravity = !Main.rand.NextBool(3);
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Ice);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.scale = Main.rand.NextFloat(1, 2);
                d.noGravity = !Main.rand.NextBool(3);
            }
        }
    }
    public override void SetStaticDefaults()
    {   
        if (!Main.dedServ)
            backTexture = ModContent.Request<Texture2D>(Texture + "Flake");
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/Cryogen_Preview",
            PortraitPositionYOverride = 0f,
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }
    public override void FindFrame(int frameHeight)
    {
        if (_currentAttack > 19)
        {
            NPC.frame.Y = frameHeight;
            if (BossDownedSystem.DownedCryogen)
            {
                NPC.frame.Y += frameHeight;
            }
        }
    }
    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
    private float _snowOverlayOpacity = 0f;
    private int _snowOverlaySpinDirection = 1;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Asset<Texture2D> tex = TextureAssets.Npc[Type];
        Rectangle bigFlake = new Rectangle(0, 0, 270, 270);
        Rectangle smallFlake = new Rectangle(0, 272, 126, 126);
        Rectangle SnowOverlay = new Rectangle(272, 0, 212, 208);
        //Flakes

        Color baseColor = Color.Lerp(Color.White, new Color(0.7f, 0.7f, 0.9f), _snowOverlayOpacity);

        // snow behind
        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, SnowOverlay, Color.White * _snowOverlayOpacity * 0.75f, (float)Main.timeForVisualEffects * -0.075f * _snowOverlaySpinDirection, SnowOverlay.Size() / 2, 1.4f, SpriteEffects.None, 0);

        for (int i = 0; i < 4; i++)
        {
            spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition + new Vector2((float)Math.Sin(Main.timeForVisualEffects * 0.03f) * 8).RotatedBy(MathHelper.PiOver2 * i), bigFlake, baseColor * 0.2f, NPC.rotation, bigFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);
        }

        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, bigFlake, baseColor * 0.5f, NPC.rotation, bigFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);

        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, smallFlake, baseColor * 0.7f, -NPC.rotation, smallFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);

        // The Hexagon
        spriteBatch.Draw(tex.Value, NPC.Center - Main.screenPosition, NPC.frame, baseColor, NPC.velocity.X * 0.03f, NPC.frame.Size() / 2, !ForTheWorthy ? 1f : 0.5f, SpriteEffects.None, 0);

        // snow overlay
        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, SnowOverlay, Color.White * _snowOverlayOpacity, (float)Main.timeForVisualEffects * -0.1f * _snowOverlaySpinDirection, SnowOverlay.Size() / 2, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, SnowOverlay, Color.White * _snowOverlayOpacity, (float)Main.timeForVisualEffects * -0.05f * _snowOverlaySpinDirection, SnowOverlay.Size() / 2, 1.2f, SpriteEffects.None, 0);
        return false;
    }
    public override void BossLoot(ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CryogenTrophy>(), 10));
        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CryogenMask>(), 7));
        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Icebreaker>(), ModContent.ItemType<HoarfrostBow>()));
        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TheSnowman>(), 5));
        npcLoot.Add(notExpertRule);
        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CryogenBag>()));
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CryogenRelic>()));
        npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MagicChisel>(), 4));
    }
    public override void OnKill()
    {
        if (!BossDownedSystem.DownedCryogen && Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Priest>());
        }
        BossDownedSystem.DownedCryogen = true;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.EyeofCthulhu);
        NPC.lifeMax = 20000;
        NPC.defense = 30;
        NPC.value = 200000;
        NPC.damage = 70;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cryogen");
        NPC.Size = new Vector2(120);
        NPC.noTileCollide = true;

        NPC.HitSound = SoundID.Item50;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
        _snowOverlayOpacity = 0f;
        _snowOverlaySpinDirection = 1;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
            new FlavorTextBestiaryInfoElement($"Mods.CalamityVanilla.NPCs.Cryogen.Bestiary")
        });
    }
    public static Color[] AuroraColors = [Color.GreenYellow, Color.MediumSpringGreen, Color.Magenta, Color.MediumSlateBlue, Color.DodgerBlue];
    public static Color GetAuroraColor(int Time)
    {
        int fadeTime = 60;
        int index = (int)((Time / fadeTime) % AuroraColors.Length);
        int nextIndex = (index + 1) % AuroraColors.Length;
        return Color.Lerp(AuroraColors[index], AuroraColors[nextIndex], (Time % fadeTime) / (float)fadeTime);
    }
}