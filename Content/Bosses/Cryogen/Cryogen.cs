using CalamityVanilla.Common;
using CalamityVanilla.Content.Items.Consumable;
using CalamityVanilla.Content.Items.Equipment.Vanity;
using CalamityVanilla.Content.Items.Pets;
using CalamityVanilla.Content.Items.Weapons.Melee;
using CalamityVanilla.Content.Items.Weapons.Ranged;
using CalamityVanilla.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
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

        // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        // Automatically group with other bosses
        NPCID.Sets.BossBestiaryPriority.Add(Type);

        // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

        // Influences how the NPC looks in the Bestiary
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/Cryogen_Preview",
            //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
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
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Asset<Texture2D> tex = TextureAssets.Npc[Type];
        Rectangle bigFlake = new Rectangle(0, 0, 270, 270);
        Rectangle smallFlake = new Rectangle(0, 272, 126, 126);
        //Flakes

        for (int i = 0; i < 4; i++)
        {
            spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition + new Vector2((float)Math.Sin(Main.timeForVisualEffects * 0.03f) * 2).RotatedBy(MathHelper.PiOver2 * i), bigFlake, new Color(0.5f, 1f, 1f, 0f) * 0.2f, NPC.rotation, bigFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);
        }

        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, bigFlake, Color.White * 0.5f, NPC.rotation, bigFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);

        spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, smallFlake, Color.White * 0.7f, -NPC.rotation, smallFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);

        // The Hexagon
        spriteBatch.Draw(tex.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White, phase == 2 ? (float)Math.Sin(Main.timeForVisualEffects * 0.8f) * 0.1f : NPC.velocity.X * 0.03f, NPC.frame.Size() / 2, !ForTheWorthy ? 1f : 0.5f, SpriteEffects.None, 0);
        return false;
    }
    public override void BossLoot(ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

        // The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
        // 1. Trophy
        // 2. Classic Mode ("not expert")
        // 3. Expert Mode (usually just the treasure bag)
        // 4. Master Mode (relic first, pet last, everything else in between)

        // Trophies are spawned with 1/10 chance
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CryogenTrophy>(), 10));

        // All the Classic Mode drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

        // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
        // Boss masks are spawned with 1/7 chance
        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CryogenMask>(), 7));

        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Icebreaker>(), ModContent.ItemType<HoarfrostBow>()));

        notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TheSnowman>(), 5));

        // Finally add the leading rule
        npcLoot.Add(notExpertRule);

        // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CryogenBag>()));

        // ItemDropRule.MasterModeCommonDrop for the relic
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CryogenRelic>()));

        // ItemDropRule.MasterModeDropOnAllPlayers for the pet
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

        NPC.aiStyle = -1;
        NPC.noGravity = true;
        phase = 0;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cryogen");
        NPC.Size = new Vector2(120);
        NPC.noTileCollide = true;

        NPC.HitSound = SoundID.Item50; //ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
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
    public override void AI()
    {
        NPC.direction = NPC.velocity.X == 0 ? 1 : Math.Sign(NPC.velocity.X);
        Lighting.AddLight(NPC.Center, new Vector3(0.8f, 1f, 1f));
        if (Main.rand.NextBool(10))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Snow);
            d.scale = 0.8f;
            d.velocity += NPC.velocity;
        }
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
        }
        switch (phase)
        {
            case 0:
                DashAndShoot_0();
                break;
            case 1:
                SlamAttack_1();
                break;
        }
    }
}