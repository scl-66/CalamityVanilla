using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.NPCs;

// ReSharper disable CompareOfFloatsByEqualityOperator

//todo maybe use less ai slots, reserve for custom use
internal abstract class CustomWormNPC : ModNPC
{
    internal enum PartKind
    {
        Head,
        Body,
        Tail
    }

    internal struct PartData
    {
        public PartKind Kind;
        public int FollowingNPC;
        public int IndexInWorm;
    }

    public ref float FollowingWhoAmI => ref NPC.ai[0];

    public PartKind Kind
    {
        get => (PartKind)(int)NPC.ai[1];
        set => NPC.ai[1] = (int)value;
    }

    public ref float SegmentIndex => ref NPC.ai[2];

    public virtual int MaxSegments { get; set; } = 10;
    public virtual int InwardSegmentOffset { get; set; } = 4;

    public virtual Rectangle GetSegmentFrame(int segmentIndex, PartKind kind) => new Rectangle(0, 0, NPC.width, NPC.height);

    public virtual int GetSegmentDrawHeight(int segmentIndex, PartKind kind) => GetSegmentFrame(segmentIndex, kind).Height;

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => Kind == PartKind.Head;

    public override void SetDefaults()
    {
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;

        FollowingWhoAmI = -1;
        Kind = PartKind.Head;
        SegmentIndex = 0;

        WormDefaults();
    }

    public virtual void WormDefaults() { }

    public override void OnSpawn(IEntitySource source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) return;

        if (Kind == PartKind.Head)
        {
            NPC.realLife = NPC.whoAmI;

            int currentSegmentNpc = NPC.whoAmI;
            int headWhoAmI = NPC.whoAmI;

            for (int i = 1; i <= MaxSegments; i++)
            {
                var kind = PartKind.Body;
                if (i == MaxSegments)
                {
                    kind = PartKind.Tail;
                }

                var newSegment = NPC.NewNPCDirect(
                    source,
                    NPC.position,
                    Type,
                    0,
                    currentSegmentNpc,
                    (int)kind,
                    i
                );

                newSegment.velocity = Vector2.Zero;
                newSegment.realLife = headWhoAmI;

                currentSegmentNpc = newSegment.whoAmI;
            }
        }
    }


    public override bool PreAI()
    {
        if (!Main.npc[(int)FollowingWhoAmI].active && FollowingWhoAmI != -1 && Kind != PartKind.Head)
        {
            NPC.active = false;
            return false;
        }

        switch (Kind)
        {
            case PartKind.Head:
                HeadAI();
                break;
            case PartKind.Body:
            case PartKind.Tail:
                FollowAI();
                break;
        }

        NPC.spriteDirection = Math.Abs(NPC.rotation) > MathHelper.PiOver2 ? -1 : 1;
        return true;
    }

    public virtual void HeadAI()
    {
        NPC.timeLeft = 90;
    }


    public virtual void FollowAI()
    {
        NPC precedingNPC = Main.npc[(int)FollowingWhoAmI];

        if (!precedingNPC.active || precedingNPC.whoAmI == NPC.whoAmI)
        {
            NPC.active = false;
            return;
        }

        Vector2 directionToPreceding = precedingNPC.Center - NPC.Center;
        float distanceToPreceding = directionToPreceding.Length();

        float thisSegmentHeight = GetSegmentFrame((int)SegmentIndex, Kind).Height;
        float precedingSegmentHeight = GetSegmentFrame((int)precedingNPC.ai[2], (PartKind)(int)precedingNPC.ai[1]).Height;

        float idealCenterDistance = (precedingSegmentHeight / 2f) + (thisSegmentHeight / 2f) - InwardSegmentOffset;

        if (idealCenterDistance <= 1f) idealCenterDistance = 1f;

        if (distanceToPreceding > idealCenterDistance)
        {
            NPC.velocity = directionToPreceding.SafeNormalize(Vector2.Zero) * (distanceToPreceding - idealCenterDistance);
        }
        else
        {
            NPC.Center = precedingNPC.Center - directionToPreceding.SafeNormalize(Vector2.Zero) * idealCenterDistance;
            NPC.velocity = Vector2.Zero;
        }

        NPC.rotation = directionToPreceding.ToRotation() + MathHelper.PiOver2;

        NPC.timeLeft = 90;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var tex = ModContent.Request<Texture2D>(Texture).Value;
        var frame = GetSegmentFrame((int)SegmentIndex, Kind);

        Main.EntitySpriteDraw(tex,
            NPC.Center - screenPos,
            frame,
            drawColor,
            NPC.rotation,
            frame.Size() / 2f,
            1f,
            NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );

        return false;
    }
}