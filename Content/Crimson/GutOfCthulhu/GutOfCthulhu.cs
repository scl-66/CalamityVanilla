using CalamityVanilla.Common.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.GutOfCthulhu;

public partial class GutOfCthulhu : ModNPC, ICustomTrailingMode
{
    public Player target
    { get { return Main.player[NPC.target]; } }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
        NPCID.Sets.TrailCacheLength[Type] = 8;
    }
    public override void SetDefaults()
    {
        (NPC.width, NPC.height) = (216, 216);

        NPC.lifeMax = 16000;
        NPC.defense = 30;
        NPC.knockBackResist = 0;

        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.behindTiles = true;

        NPC.boss = true;
        Music = MusicID.Boss2;

        NPC.HitSound = SoundID.NPCHit18;
        NPC.DeathSound = SoundID.NPCDeath11;

        _afterImageActive = false;
        _afterImageOpacity = 1;
        _wormsSummoned = 0;
    }
}