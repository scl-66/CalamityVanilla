using CalamityVanilla.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;

namespace CalamityVanilla.Content.NPCs.Bosses.Perforators
{
    internal partial class Ingestoid : WormNPC
    {
        byte[] chaosnumber = new byte[] { };
        private enum IngestoidPhases
        {
            Idle = 0,
            Chase = 1,
            Wall = 2
        }

        IngestoidPhases phase = IngestoidPhases.Idle;
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
                CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/Ingestoid_Preview",
                //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        
        public override void SetDefaults()
        {
            segmentsizes = new int[] { 44, 28, 32, 34 };
            segmentspriteposition = new int[] { 0, 46, 74, 106 };
            sheetsegments = 4;
            repeatingsegments = new int[] {2};
            maxlength = 15;

            NPC.lifeMax = 100;
            NPC.defense = 30;

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.Size = new Vector2(32);
            NPC.noTileCollide = true;

            NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
            NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
        }

        Player targetplayer = Main.player[0];
        public override void AI()
        {

            NPC.TargetClosest();
            targetplayer = Main.player[NPC.target];

            if (NPC.ai[1] == (byte)WormSegment.Head)
            {
                switch (phase)
                {
                    case IngestoidPhases.Idle: Idle(); break;
                    case IngestoidPhases.Chase: Chase(); break;
                    case IngestoidPhases.Wall: Wall(); break;
                }

                chaosnumber = CVUtils.RepeatableRandom((targetplayer.position + NPC.position).ToString());
            }

            base.AI();
        }
    }
}
