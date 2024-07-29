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

namespace CalamityVanilla.Content.NPCs.Bosses.Perforators
{
    internal class Hematode : WormNPC
    {
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
                CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/Hematode_Preview",
                //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        
        public override void SetDefaults()
        {
            segmentsizes = new int[] { 64, 28, 28, 36 };
            segmentspriteposition = new int[] { 0, 64, 92, 120 };
            sheetsegments = 4;
            repeatingsegments = new int[] {1,2};
            inwardsegmentoffset = 8;
            maxlength = 12;

            NPC.lifeMax = 100;
            NPC.defense = 30;

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.Size = new Vector2(32);
            NPC.noTileCollide = true;

            NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
            NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
        }
    }
}
