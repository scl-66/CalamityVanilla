using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
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
    
    void ChangeState(GutState state) {
        NPC.ai[0] = Unsafe.BitCast<GutState, float>(state);
        Timer = 0;
        NPC.netUpdate = true;
    }

    public override void SetDefaults()
    {
        (NPC.width, NPC.height) = (200, 200);
        
        NPC.lifeMax = 100;
        NPC.defense = 30;

        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;

        NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
    }

    public override void AI()
    {
        
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return true;
    }
}

internal class GutOfCthulhuEye : ModNPC
{
    public override string Texture => "CalamityVanilla/Content/Bosses/GutOfCthulhu/GutOfCthulhu_Eye";
    
    public override void SetDefaults()
    {
        (NPC.width, NPC.height) = (56, 56);
        
        NPC.lifeMax = 100;
        NPC.defense = 30;

        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;

        NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
        NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
    }
}