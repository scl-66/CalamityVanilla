using CalamityVanilla.Common.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.GutOfCthulhu.Worms;

internal sealed class Hematode : CustomWormNPC {
    internal enum HematodePhases {
        Idle = 0,
        Chase = 1,
    }
    
    public ref float CurrentPhaseAsFloat => ref NPC.ai[1]; 
    private HematodePhases CurrentPhase {
        get => (HematodePhases)(int)CurrentPhaseAsFloat;
        set => CurrentPhaseAsFloat = (float)(int)value;
    }
    
    void ChangeState(HematodePhases state) {
        CurrentPhase = state;
        _stateTimer = 0;
        NPC.netUpdate = true;
    }

    private float _stateTimer = 0;

    private Player _targetPlayer;

    private int _rechargeTimer;
    private int _shotCooldownTimer;

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
        NPC.friendly = false;
        NPC.damage = 20;

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
        
        if (Kind == PartKind.Head) ChangeState(HematodePhases.Idle);
        
        _rechargeTimer = 0;
        _shotCooldownTimer = Main.rand.Next(300, 600);
    }
    
    public override void DrawBehind(int index) {
        Main.instance.DrawCacheNPCsOverPlayers.Add(index);  
    }

    public override Rectangle GetSegmentFrame(int segmentIndex, PartKind kind) {
        const int frameWidth = 66;
        int segmentsFromTail = (MaxSegments + 1) - segmentIndex; 

        switch (kind) {
            case PartKind.Head:
                return new Rectangle(0, 0, frameWidth, 48);
            case PartKind.Tail:
                if (_rechargeTimer > 0) {
                    return new Rectangle(66, 154, frameWidth, 30);
                }
                return new Rectangle(0, 154, frameWidth, 30);
            
            case PartKind.Body:
                if (segmentsFromTail == 5) {
                    if (_rechargeTimer > 0) {
                        return new Rectangle(66, 74, frameWidth, 18);
                    }
                    
                    return new Rectangle(0, 74, frameWidth, 18);
                }
                if (segmentsFromTail == 4) {
                    if (_rechargeTimer > 0) {
                        return new Rectangle(66, 94, frameWidth, 18);
                    }
                    return new Rectangle(0, 94, frameWidth, 18);
                }
                if (segmentsFromTail == 3) {
                    if (_rechargeTimer > 0) {
                        return new Rectangle(66, 114, frameWidth, 18);
                    }
                    return new Rectangle(0, 114, frameWidth, 18);
                }
                if (segmentsFromTail == 2) {
                    if (_rechargeTimer > 0) {
                        return new Rectangle(66, 134, frameWidth, 18);
                    }
                    return new Rectangle(0, 134, frameWidth, 18);
                }

                if (_rechargeTimer > 0) {
                    return new Rectangle(66, 50, frameWidth, 22);
                }

                return new Rectangle(0, 50, frameWidth, 22);
            default:
                return new Rectangle(0, 0, 0, 0);
        }
    }

    public override void FollowAI() {
        base.FollowAI();
        if (Kind == PartKind.Body || Kind == PartKind.Tail) {
            _rechargeTimer--;
            
            if (_rechargeTimer < 0) {
                _rechargeTimer = 0;
            }
            
            if (_rechargeTimer == 0) {
                _shotCooldownTimer--;
                if (_shotCooldownTimer <= 0) {
                    NPC.TargetClosest(false); 
                    _targetPlayer = Main.player[NPC.target];

                    if (_targetPlayer.active && !_targetPlayer.dead) {
                        var projectileVelocity = NPC.DirectionTo(_targetPlayer.Center) * 8f;

                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            projectileVelocity,
                            ModContent.ProjectileType<HematodeShot>(),
                            NPC.damage / 2,
                            0.5f,
                            Main.myPlayer 
                        );
                        
                        _rechargeTimer = 60 * 2;
                        _shotCooldownTimer = Main.rand.Next(60, 180);
                        NPC.netUpdate = true;
                    }
                }
            }
        }
    }

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

        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.DirectionTo(_targetPlayer.Center) * 3f * distmult, 0.05f);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);

        writer.Write(_rechargeTimer);
        writer.Write(_shotCooldownTimer);
        writer.Write((byte)CurrentPhase);
        writer.Write(_stateTimer);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);

        _rechargeTimer = reader.ReadInt32();
        _shotCooldownTimer = reader.ReadInt32();
        CurrentPhase = (HematodePhases)reader.ReadByte();
        _stateTimer = reader.ReadSingle();
    }
}

public class HematodeShot : ModProjectile {
    public override string Texture => ModContent.GetModNPC(ModContent.NPCType<Hematode>()).Texture + "_Shot";

    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = false; 
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        AIType = ProjectileID.IceSpike;
    }
}