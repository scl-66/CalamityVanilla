using CalamityVanilla.Common.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;

namespace CalamityVanilla.Content.Bosses.GutOfCthulhu.Worms;

internal sealed class Hematode : CustomWormNPC {
    internal enum HematodePhases {
        Idle = 0,
        Chase = 1,
    }
    
    public HematodePhases CurrentPhase => Unsafe.BitCast<float, HematodePhases>(NPC.ai[3]);
    
    void ChangeState(HematodePhases state) {
        NPC.ai[0] = Unsafe.BitCast<HematodePhases, float>(state);
        _stateTimer = 0;
        NPC.netUpdate = true;
    }

    private float _stateTimer = 0;

    private Player _targetPlayer;

    public override void SetStaticDefaults() {
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
            CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/Hematode_Preview",
            PortraitPositionYOverride = 0f,
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void WormDefaults() {
        (NPC.width, NPC.height) = (66, 48);
        NPC.lifeMax = 100;
        NPC.defense = 30;

        NPC.noTileCollide = true;
        NPC.noGravity = true;

        NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;

        int segments = 0;
        
        switch (Main.GameMode) {
            case GameModeID.Normal: segments = 10; break;
            case GameModeID.Expert: segments = 20; break;
            case GameModeID.Master: segments = 30; break;
        }

        MaxSegments = segments;
        InwardSegmentOffset = 2;
        
        if (Kind == PartKind.Head)
        {
            ChangeState(HematodePhases.Idle);
        }
    }

    public override Rectangle GetSegmentFrame(int segmentIndex, PartKind kind) {
        const int frameWidth = 66;
        
        //calc which frame to use, idx starts at 0 and the first body segment after is 1, and the tail is maxsegments + 1. so to get the 3rd to tail part, its segmentsFromTrail = 3
        int segmentsFromTail = (MaxSegments + 1) - segmentIndex; 

        switch (kind) {
            case PartKind.Head:
                return new Rectangle(0, 0, frameWidth, 48);
            case PartKind.Tail:
                return new Rectangle(0, 154, frameWidth, 30);
            case PartKind.Body:
                if (segmentsFromTail == 5) {
                    return new Rectangle(0, 74, frameWidth, 18);
                }
                if (segmentsFromTail == 4) {
                    return new Rectangle(0, 94, frameWidth, 18);
                }
                if (segmentsFromTail == 3) {
                    return new Rectangle(0, 114, frameWidth, 18);
                }
                if (segmentsFromTail == 2) {
                    return new Rectangle(0, 134, frameWidth, 18);
                }
                return new Rectangle(0, 50, frameWidth, 22);
            default:
                return new Rectangle(0, 0, 1, 1);
        }
    }

    public override void FollowAI() { base.FollowAI(); }

    public override void HeadAI() {
        NPC.TargetClosest();
        _targetPlayer = Main.player[NPC.target];

        switch (CurrentPhase) {
            case HematodePhases.Idle:
                IdleLogic();
                break;
        }

        base.HeadAI(); 
    }

    private void IdleLogic() {
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.Center.DirectionTo(_targetPlayer.Center).ToRotation() + MathHelper.PiOver2, 0.05f);

        float distmult = Math.Clamp(_targetPlayer.velocity.Length() / 2f, 1f, 5f);

        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.DirectionTo(_targetPlayer.Center) * 2f * distmult, 0.05f);
    }
}