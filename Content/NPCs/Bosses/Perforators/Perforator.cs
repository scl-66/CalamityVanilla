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

namespace CalamityVanilla.Content.NPCs.Bosses.Perforators
{
    [AutoloadBossHead]
    public partial class Perforator : ModNPC
    {

        public byte phase = 0;
        public Player target
        { get { return Main.player[NPC.target]; } }
        public override string Texture => "CalamityVanilla/Content/NPCs/Bosses/Perforators/PerforatorBigWorm";

        public override void SetStaticDefaults()
        {

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/HiveMind_Preview",
                //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
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
                new FlavorTextBestiaryInfoElement("Heart of Cthulhu hehe.")
            });
        }
        public override void AI()
        {

        }
    }
}
