using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.HiveMind
{
    [AutoloadBossHead]
    public partial class HiveMind : ModNPC
    {
        public byte phase = 0;
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
                for(int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(6, 6), Mod.Find<ModGore>("HiveMind" + $"{i}").Type);
                }
                for (int i = 0; i < 100; i++)
                {
                    Dust d = Dust.NewDustDirect(NPC.position,NPC.width,NPC.height,DustID.CorruptGibs);
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

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/HiveMind_Preview",
                //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 180;

            NPC.frameCounter++;
            if(NPC.frameCounter > 7)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if(NPC.frame.Y > frameHeight * 3)
                { 
                    NPC.frame.Y = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Need to make the squish not look janky on platforms
            Asset<Texture2D> tex = TextureAssets.Npc[Type];
            int move = (int)MathHelper.SmoothStep(tex.Height() / 4f,0,NPC.Opacity);
            spriteBatch.Draw(tex.Value,NPC.Center - Main.screenPosition + new Vector2(0,-9 + (move * MathHelper.SmoothStep(1.2f,1,NPC.Opacity))),new Rectangle(NPC.frame.X,NPC.frame.Y,NPC.frame.Width,NPC.frame.Height - move),Color.Lerp(Color.Black,drawColor,NPC.Opacity) * NPC.Opacity,NPC.rotation,NPC.frame.Size() / 2, Vector2.SmoothStep(new Vector2(0.2f,1.6f), new Vector2(1),NPC.Opacity), SpriteEffects.None,0);
            return false;
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EyeofCthulhu);

            NPC.lifeMax = 16000;
            NPC.defense = 30;

            NPC.aiStyle = -1;
            NPC.behindTiles = true;
            NPC.noGravity = false;
            phase = 0;
            Music = MusicID.Boss3;
            NPC.Size = new Vector2(150);
            NPC.noTileCollide = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("A despicable shroom-like behemoth responsible for the vile fungi decorating the Corruption soil. It releases aggressive spores when damaged.")
            });
        }
        public override void AI()
        {
            switch (phase)
            {
                case 0:
                    Teleport();
                    break;
                case 1:
                    ShootSporeBombs();
                    break;
            }
        }
    }
}
