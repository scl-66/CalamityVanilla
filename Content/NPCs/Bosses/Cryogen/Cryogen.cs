using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.Cryogen
{
    [AutoloadBossHead]
    public partial class Cryogen : ModNPC
    {
        public byte phase = 0;

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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(6, 6), Mod.Find<ModGore>("Cryogen" + $"{i}").Type);
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
            if (NPC.life < NPC.lifeMax / 3)
                NPC.frame.Y = frameHeight * 2;
            else if (NPC.life < NPC.lifeMax / 3 * 2)
                NPC.frame.Y = frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> tex = TextureAssets.Npc[Type];
            Rectangle bigFlake = new Rectangle(0,0,270,270);
            Rectangle smallFlake = new Rectangle(0, 272, 126, 126);
            //Flakes
            spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, bigFlake, Color.White * 0.5f, NPC.rotation, bigFlake.Size() / 2, !ForTheWorthy? 1f : 2f, SpriteEffects.None, 0);
            spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, smallFlake, Color.White * 0.7f, -NPC.rotation, smallFlake.Size() / 2, !ForTheWorthy ? 1f : 2f, SpriteEffects.None, 0);

            // The Hexagon
            spriteBatch.Draw(tex.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.velocity.X * 0.03f, NPC.frame.Size() / 2, !ForTheWorthy ? 1f : 0.5f, SpriteEffects.None, 0);
            return false;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EyeofCthulhu);
            NPC.lifeMax = 16000;
            NPC.defense = 30;

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            phase = 0;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cryogen");
            NPC.Size = new Vector2(120);
            NPC.noTileCollide = true;

            NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
            NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;

            if (ForTheWorthy)
            {
                NPC.Size = new Vector2(300);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                new FlavorTextBestiaryInfoElement("An incredible feat of magical architecture, this intricate fortress of ice and steel serves to trap a lonely soul inside...")
            });
        }

        public override void AI()
        {
            NPC.direction = Math.Sign(NPC.velocity.X);
            Lighting.AddLight(NPC.Center, new Vector3(0.8f,1f,1f));
            NPC.rotation += NPC.velocity.Length() * 0.01f * NPC.direction;

            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Snow);
                d.scale = 0.8f;
                d.velocity += NPC.velocity;
            }
            
            switch (phase)
            {
                case 0:
                    FlyAndShoot();
                    break;
            }
        }
    }
}
