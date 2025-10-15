using CalamityVanilla.Content.Bosses.GutOfCthulhu.Worms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.GutOfCthulhu;

internal partial class GutOfCthulhu : ModNPC
{
    internal enum GutState : sbyte
    {
        Idle
    }

    public override string Texture => "CalamityVanilla/Content/Bosses/GutOfCthulhu/GutOfCthulhu_Body";

    public GutState CurrentState => Unsafe.BitCast<float, GutState>(NPC.ai[0]);
    public ref float Timer => ref NPC.ai[1];

    private int[] _eyeNPCs = new int[3] { -1, -1, -1 };
    public int EyesKilledCount = 0;

    void ChangeState(GutState state)
    {
        NPC.ai[0] = Unsafe.BitCast<GutState, float>(state);
        Timer = 0;
        NPC.netUpdate = true;
    }

    public override void SetDefaults()
    {
        (NPC.width, NPC.height) = (326, 332);

        NPC.lifeMax = 16000;
        NPC.defense = 30;

        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;

        NPC.dontTakeDamage = true;

        NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
        //NPC.hide = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) return;

        EyesKilledCount = 0;
        
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = Vector2.One;
            switch (i)
            {
                case 0: pos = new Vector2(0, -50); break;
                case 1: pos = new Vector2(0, -70); break;
                case 2: pos = new Vector2(0, -90); break;
            }

            int eyeWhoAmI = NPC.NewNPCDirect(
                source,
                pos,
                ModContent.NPCType<GutOfCthulhuEye>(),
                ai0: NPC.whoAmI,
                ai1: i
            ).whoAmI;

            _eyeNPCs[i] = eyeWhoAmI;
        }
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheProjsBehindNPCs.Add(index);
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player targetPlayer = Main.player[NPC.target];
        
        //NPC.rotation += 0.02f;

        for (int i = 0; i < _eyeNPCs.Length; i++)
        {   
            int eyeWhoAmI = _eyeNPCs[i];

            if (eyeWhoAmI != -1 && Main.npc[eyeWhoAmI].active && Main.npc[eyeWhoAmI].type == ModContent.NPCType<GutOfCthulhuEye>())
            {
                NPC eyeNPC = Main.npc[eyeWhoAmI];

                Vector2 localOffset;
                switch (i)
                {
                    case 0: localOffset = new Vector2(20, -150); break;
                    case 1: localOffset = new Vector2(20, -90); break;
                    case 2: localOffset = new Vector2(20, -30); break;
                    default: localOffset = Vector2.Zero; break;
                }
                Vector2 targetEyeCenter = NPC.Center + localOffset.RotatedBy(NPC.rotation);

                eyeNPC.Center = targetEyeCenter;
                eyeNPC.velocity = Vector2.Zero;
                eyeNPC.rotation = NPC.rotation;
            }
            else
            {
                if (_eyeNPCs[i] != -1)
                {
                    EyesKilledCount++;
                    NPC.netUpdate = true;
                }
                _eyeNPCs[i] = -1;
            }

            Main.Achievements.ClearAll();
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);

        for (int i = 0; i < _eyeNPCs.Length; i++)
        {
            writer.Write(_eyeNPCs[i]);
        }
        writer.Write(EyesKilledCount);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);

        for (int i = 0; i < _eyeNPCs.Length; i++)
        {
            _eyeNPCs[i] = reader.ReadInt32();
        }
        EyesKilledCount = reader.ReadInt32();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return true;
    }
}

internal class GutOfCthulhuEye : ModNPC
{
    public override string Texture => "CalamityVanilla/Content/Bosses/GutOfCthulhu/GutOfCthulhu_Eye";

    public ref float ParentWhoAmI => ref NPC.ai[0];
    public ref float EyeIndex => ref NPC.ai[1];

    public override void SetDefaults()
    {
        (NPC.width, NPC.height) = (56, 56);

        NPC.lifeMax = 10;
        NPC.defense = 30;

        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;

        NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCProjectiles.Add(index);
    }

    public override void OnKill()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) return;
        
        NPC parentNPC = Main.npc[(int)ParentWhoAmI];
        
        if (parentNPC.ModNPC is not GutOfCthulhu parentGut) {
            return;
        }

        int spawnOrder = parentGut.EyesKilledCount;

        int npcToSpawnType;
        switch (spawnOrder) {
            case 0: npcToSpawnType = ModContent.NPCType<Ingestoid>(); break;
            case 1: npcToSpawnType = ModContent.NPCType<Hematode>(); break;
            case 2: npcToSpawnType = ModContent.NPCType<Malarasite>(); break;
            default: npcToSpawnType = NPCID.Pinky; break;
        }

        IEntitySource source = NPC.GetSource_FromThis();
        NPC.NewNPCDirect(source, NPC.Center, npcToSpawnType);
    }

    public override void AI()
    {
        if (!Main.npc[(int)ParentWhoAmI].active || Main.npc[(int)ParentWhoAmI].type != ModContent.NPCType<GutOfCthulhu>())
        {
            NPC.active = false;
            return;
        }
    }
}