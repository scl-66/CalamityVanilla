using CalamityVanilla.Common;
using CalamityVanilla.Common.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.GutOfCthulhu.Worms;

partial class Hematode : WormNPC
{
    byte[] chaosnumber = new byte[] { };
    private enum HematodePhases
    {
        Idle = 0,
        Chase = 1,
        Wall = 2
    }

    HematodePhases phase = HematodePhases.Idle;
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
        repeatingsegments = new int[] { 1, 2 };
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

    Player targetplayer = Main.player[0];
    public override void AI()
    {

        NPC.TargetClosest();
        targetplayer = Main.player[NPC.target];

        if (NPC.ai[1] == (byte)WormSegment.Head)
        {
            chaosnumber = CVUtils.RepeatableRandom((targetplayer.position + NPC.position).ToString());

            switch (phase)
            {
                case HematodePhases.Idle: Idle(); break;
                case HematodePhases.Chase: Idle(); break;
                case HematodePhases.Wall: Idle(); break;
            }
        }

        base.AI();
    }
    
    private void Idle()
    {
        NPC.TargetClosest();
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.Center.DirectionTo(targetplayer.Center).ToRotation() + MathHelper.PiOver2, 0.05f);

        float distmult = Math.Clamp(targetplayer.velocity.Length() / 2f, 1f, 5f);

        if (NPC.Center.Distance(targetplayer.Center) > 450f ||
            CVUtils.AngleDifference(NPC.rotation - MathHelper.PiOver2, NPC.velocity.ToRotation()) > 0.5f) NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.DirectionTo(targetplayer.Center) * 2f * distmult, 0.05f);
        else NPC.velocity *= 0.97f;
    }
}