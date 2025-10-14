using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    
    private GutState State {
        get => (GutState)NPC.ai[0];
        set {
            NPC.ai[0] = (int)value;
            NPC.netUpdate = true;
        }
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

    public override void AI()
    {
        
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return true;
    }
}