using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.Perforators
{
    [AutoloadBossHead]
    public partial class PerforatorBigWorm : ModNPC
    {
        private enum PerforatorBigWormPhase
        {
            idle = 0,
            follow = 1,
        }

        private PerforatorBigWormPhase phase = PerforatorBigWormPhase.idle;
        public Player target
        { get { return Main.player[NPC.target]; } }

        public Asset<Texture2D> TentacleTexture;

        public override void SetStaticDefaults()
        {
            TentacleTexture = ModContent.Request<Texture2D>("CalamityVanilla/Content/NPCs/Bosses/Perforators/PerforatorBigWormTentacle");

            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/HiveMind_Preview",
                //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EyeofCthulhu);

            NPC.lifeMax = 16000;
            NPC.defense = 30;

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            phase = PerforatorBigWormPhase.follow;
            Music = MusicID.Boss3;
            NPC.Size = new Vector2(50);
            NPC.noTileCollide = true;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void AI()
        {
            switch (phase)
            {
                case PerforatorBigWormPhase.idle: Idle(); break;
                case PerforatorBigWormPhase.follow: Follow(); break;
            }

            NPC.ai[0]++;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            DrawTentacle(NPC.Center - new Vector2(-16, 34), target.Center, drawColor);
            DrawTentacle(NPC.Center - new Vector2(16, 34), target.Center, drawColor);

            DrawTentacle(NPC.Center - new Vector2(-28, -4), target.Center, drawColor);
            DrawTentacle(NPC.Center - new Vector2(28, -4), target.Center, drawColor);

            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, TextureAssets.Npc[Type].Size() / 2 + new Vector2(0, 20), 1f, SpriteEffects.None);
            return false;
        }
        public void DrawTentacle(Vector2 startpoint, Vector2 endpoint, Color drawColor)
        {
            TentacleTexture = ModContent.Request<Texture2D>("CalamityVanilla/Content/NPCs/Bosses/Perforators/PerforatorBigWormTentacle");

            Vector2 movedpoint = NPC.Center + (startpoint - NPC.Center).RotatedBy(NPC.rotation);
            int midsections = (int)((movedpoint.Distance(endpoint) - 28 - 24) / 12);

            Main.EntitySpriteDraw(TentacleTexture.Value, movedpoint - Main.screenPosition, new Rectangle(0,0,28,22), drawColor, movedpoint.DirectionTo(endpoint).ToRotation(), new Vector2(0,12), 1f, SpriteEffects.None);

            for (int i = 0; i < midsections; i++)
            {
                Main.EntitySpriteDraw(TentacleTexture.Value, movedpoint - Main.screenPosition, new Rectangle(30, 0, 12, 22), drawColor, movedpoint.DirectionTo(endpoint).ToRotation(), new Vector2(0, 12) - new Vector2(28 + 12 * i,0) , 1f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(TentacleTexture.Value, movedpoint - Main.screenPosition, new Rectangle(44, 0, 24, 22), drawColor, movedpoint.DirectionTo(endpoint).ToRotation(), new Vector2(0, 12) - new Vector2(movedpoint.Distance(endpoint) - 38, 0), 1f, SpriteEffects.None);
        }
    }
}
