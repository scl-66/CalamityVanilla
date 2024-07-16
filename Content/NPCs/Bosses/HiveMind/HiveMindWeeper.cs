using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.HiveMind
{
    public class HiveMindWeeper : ModNPC
    {
        public Player target
        { get { return Main.player[NPC.target]; } }

        // This is a neat trick that uses the fact that NPCs have all NPC.ai[] values set to 0f on spawn (if not otherwise changed).
        // We set ParentIndex to a number in the body after spawning it. If we set ParentIndex to 3, NPC.ai[0] will be 4. If NPC.ai[0] is 0, ParentIndex will be -1.
        // Now combine both facts, and the conclusion is that if this NPC spawns by other means (not from the body), ParentIndex will be -1, allowing us to distinguish
        // between a proper spawn and an invalid/"cheated" spawn
        public int ParentIndex
        {
            get => (int)NPC.ai[0] - 1;
            set => NPC.ai[0] = value + 1;
        }

        public bool HasParent => ParentIndex > -1;

        // Helper method to determine the body type
        public static int BodyType()
        {
            return ModContent.NPCType<HiveMind>();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = -1; i <= 1; i += 2) // larger smoke puffs
                {
                    for (int j = Main.rand.Next(-1, 1); j <= 1; j += 2)
                    {
                        int smoke = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y), default, Main.rand.Next(61, 64));
                        Main.gore[smoke].velocity *= 0.4f;
                        Main.gore[smoke].velocity.X += i;
                        Main.gore[smoke].velocity.Y += j;
                        Main.gore[smoke].scale = Main.rand.NextFloat(0.5f, 0.75f);
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs);
                    d.velocity = Main.rand.NextVector2Circular(3, 3);
                    d.scale = Main.rand.NextFloat(0.5f, 1f);
                    //d.noGravity = !Main.rand.NextBool(3);
                }
                for (int i = 0; i < 15; i++)
                {
                    Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                    d.velocity = Main.rand.NextVector2Circular(6, 6);
                    //d.scale = Main.rand.NextFloat(0.5f, 1f);
                    d.noGravity = true;
                }
            }
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;

            // By default enemies gain health and attack if hardmode is reached. this NPC should not be affected by that
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            // Enemies can pick up coins, let's prevent it for this NPC
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionYOverride = -24f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void FindFrame(int frameHeight)
        {
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
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.AngryNimbus);

            NPC.lifeMax = 120;
            NPC.defense = 25;

            NPC.width = 60;
            NPC.height = 38;

            NPC.value = 0;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
        }

        public override void AI()
        {
            NPC.ai[0]++;
            NPC.Opacity += 0.1f;

            NPC.rotation = NPC.velocity.X / 5f;
            //if (Despawn())
            //{
            //    return;
            //}

            if (NPC.ai[0] % 10 == 0)
            {
                Dust d = Dust.NewDustDirect(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.Corruption, Scale: 0.75f);
                d.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
            }
        }

        private bool Despawn()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && (!HasParent || !Main.npc[ParentIndex].active || Main.npc[ParentIndex].type != BodyType()))
            {
                // * Not spawned by the boss body (didn't assign a position and parent) or
                // * Parent isn't active or
                // * Parent isn't the body
                // => invalid, kill itself without dropping any items
                NPC.active = false;
                NPC.life = 0;
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return true;
            }
            return false;
        }

        public override void OnKill()
        {
            // Boss minions typically have a chance to drop an additional heart item in addition to the default chance
            Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

            if (Main.rand.NextBool(2) && closestPlayer.statLife < closestPlayer.statLifeMax2)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Makes it so whenever you beat the boss associated with it, it will also get unlocked immediately
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("These malevolent spores are summoned by the Hive, with their only goal being to rain hell on whoever their master deems a threat.")
            });
        }
    }
}
