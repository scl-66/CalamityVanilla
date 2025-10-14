using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.GutOfCthulhu;

internal enum GutAttack
{
    Idle
}

internal sealed class GutOfCthulhu : ModNPC
{
    public override string Texture => "CalamityVanilla/Content/NPCs/Bosses/GutOfCthulhu/GutOfCthulhu_Body";

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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return true;
    }
}