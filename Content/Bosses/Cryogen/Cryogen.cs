using CalamityVanilla.Common;
using CalamityVanilla.Content.Bosses.Cryogen.Drops;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.HoarfrostBow;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.Icebreaker;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.MagicChisel;
using CalamityVanilla.Content.Bosses.Cryogen.Drops.TheSnowman;
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
    public byte phase = 0;

    private static float _phase2HealthMultiplier = 0.6f;

    const bool ForTheWorthy = false;
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
        if (NPC.life < NPC.lifeMax * _phase2HealthMultiplier)
        {
            NPC.frame.Y = frameHeight;
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

        Color baseColor = Color.Lerp(Color.White, new Color(0.7f,0.7f,0.9f), _snowOverlayOpacity);

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
        BossDownedSystem.DownedCryogen = true;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.EyeofCthulhu);
        NPC.lifeMax = 16000;
        NPC.defense = 30;
        NPC.value = 200000;
        NPC.damage = 70;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        phase = 0;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cryogen");
        NPC.Size = new Vector2(120);
        NPC.noTileCollide = true;

        NPC.HitSound = SoundID.Item50; //ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
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
    public override bool PreAI()
    {
        NPC.localAI[0]--;
        if (NPC.localAI[0] > 0)
        {
            NPC.position -= NPC.velocity;
        }
        NPC.rotation += (NPC.velocity * new Vector2(0.01f, 0.005f)).Length() * NPC.direction;
        return NPC.localAI[0] <= 0;
    }
    public static Color[] AuroraColors = [Color.GreenYellow, Color.MediumSpringGreen, Color.Magenta, Color.MediumSlateBlue, Color.DodgerBlue];
    public static Color GetAuroraColor(int Time)
    {
        int fadeTime = 60;
        int index = (int)((Time / fadeTime) % AuroraColors.Length);
        int nextIndex = (index + 1) % AuroraColors.Length;
        return Color.Lerp(AuroraColors[index], AuroraColors[nextIndex], (Time % fadeTime) / (float)fadeTime);
    }

    private static void SpawnCryoBlockLaserParticle(ParticleOrchestraSettings settings, Color color)
    {
        int num = 30;
        PrettySparkleParticle prettySparkleParticle = CVParticleOrchestrator.RequestPrettySparkleParticle();
        Vector2 movementVector = settings.MovementVector;
        prettySparkleParticle.ColorTint = color;
        prettySparkleParticle.LocalPosition = settings.PositionInWorld;
        prettySparkleParticle.Rotation = movementVector.ToRotation();
        prettySparkleParticle.Scale = new Vector2(6f, 1f);
        prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
        prettySparkleParticle.FadeOutNormalizedTime = 1f;
        prettySparkleParticle.TimeToLive = num;
        prettySparkleParticle.FadeOutEnd = num;
        prettySparkleParticle.FadeInEnd = num / 2;
        prettySparkleParticle.FadeOutStart = num / 2;
        prettySparkleParticle.AdditiveAmount = 0.5f;
        prettySparkleParticle.Velocity = settings.MovementVector + Main.rand.NextVector2Circular(2,2);
        prettySparkleParticle.LocalPosition -= prettySparkleParticle.Velocity * 4f;
        prettySparkleParticle.DrawVerticalAxis = false;
        Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
    }
    private void SpawnBigIceBlock(Point center, int halfwidth, int halfheight)
    {
        Vector2 centerInWorld = center.ToWorldCoordinates();
        for (int i = 0; i < centerInWorld.Distance(NPC.Center) - 40; i += 20)
        {
            ParticleOrchestraSettings settings = new ParticleOrchestraSettings() with { PositionInWorld = NPC.Center + NPC.Center.DirectionTo(centerInWorld) * i, MovementVector = NPC.Center.DirectionTo(centerInWorld) * 5 };
            SpawnCryoBlockLaserParticle(settings, Color.Lerp(new Color(0f,0.2f,1f,0.1f), new Color(0.2f, 0.5f, 1f, 0.1f), MathF.Sin(i * 0.05f)));
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(settings.PositionInWorld, DustID.Frost, settings.MovementVector.RotatedByRandom(0.3f) * 3);
                d.fadeIn = 1.3f;
                d.noGravity = true;
            }
        }
        for(int i = 0; i < 20; i++)
        {
            Dust d = Dust.NewDustPerfect(centerInWorld, DustID.Frost, Main.rand.NextVector2Circular(halfwidth, halfheight) * 2);
            d.noGravity = Main.rand.NextBool();

            ParticleOrchestraSettings settings = new ParticleOrchestraSettings() with { PositionInWorld = centerInWorld, MovementVector = Main.rand.NextVector2Circular(16,16) };
            SpawnCryoBlockLaserParticle(settings, Color.Lerp(new Color(0f, 0.2f, 1f, 0.1f), new Color(0.2f, 0.5f, 1f, 0.1f), Main.rand.NextFloat()));
            settings = new ParticleOrchestraSettings() with { PositionInWorld = NPC.Center, MovementVector = Main.rand.NextVector2Circular(8, 8) + NPC.Center.DirectionTo(centerInWorld) * 5 };
            SpawnCryoBlockLaserParticle(settings, Color.Lerp(new Color(0f, 0.2f, 1f, 0.1f), new Color(0.2f, 0.5f, 1f, 0.1f), Main.rand.NextFloat()));
        }

        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        for (int x = -halfwidth; x <= halfwidth; x++)
        {
            for (int y = -halfheight; y <= halfheight; y++)
            {
                if (!Main.rand.NextBool(16))
                {
                    WorldGen.PlaceTile(center.X + x, center.Y + y, ModContent.TileType<CryogenIceTile>(), plr: Main.myPlayer);
                    CryogenIceBlockSystem.CryogenIceBlocks.Add(new Point(center.X + x, center.Y + y));
                    NetMessage.SendTileSquare(-1, center.X + x, center.Y + y);
                }
            }
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(phase);
        writer.Write((int)NPC.localAI[0]);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        phase = reader.ReadByte();
        NPC.localAI[0] = reader.ReadInt32();
    }
    public override void AI()
    {
        _snowOverlayOpacity *= 0.99f;
        NPC.direction = NPC.velocity.X == 0 ? 1 : Math.Sign(NPC.velocity.X);
        Lighting.AddLight(NPC.Center, new Vector3(0.8f, 1f, 1f));
        if (Main.rand.NextBool(10))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Snow);
            d.scale = 0.8f;
            d.velocity += NPC.velocity;
        }

        if (NPC.localAI[3] == 1)
        {
            NPC.velocity.Y -= 0.1f;
            NPC.velocity.X *= 0.98f;
            return;
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                NPC.localAI[3] = 1;
                return;
            }
        }
        switch (phase)
        {
            case 0:
                ShootIceBlocks_0();
                break;
            case 1:
                DashAndChase_1();
                break;
            case 2:
                Snowflakes_2();
                break;
            case 3:
                Statues_3();
                break;
            case 4:
                SlamAttack_4();
                break;
        }
    }
}