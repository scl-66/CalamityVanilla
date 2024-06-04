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
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.HiveMind
{
    public partial class HiveMind : ModNPC
    {
        public byte phase = 0;
        public Player target 
        { get { return Main.player[NPC.target]; } }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 180;

            NPC.frameCounter++;
            if(NPC.frameCounter > 7)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if(NPC.frame.Y > frameHeight * 3)
                { 
                    NPC.frame.Y = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> tex = TextureAssets.Npc[Type];
            int move = (int)MathHelper.SmoothStep(tex.Height() / 4f,0,NPC.Opacity);
            spriteBatch.Draw(tex.Value,NPC.Center - Main.screenPosition + new Vector2(0,-9 + (move * MathHelper.SmoothStep(1.2f,1,NPC.Opacity))),new Rectangle(NPC.frame.X,NPC.frame.Y,NPC.frame.Width,NPC.frame.Height - move),Color.Lerp(Color.Black,drawColor,NPC.Opacity) * NPC.Opacity,NPC.rotation,NPC.frame.Size() / 2, Vector2.SmoothStep(new Vector2(0.2f,1.6f), new Vector2(1),NPC.Opacity), SpriteEffects.None,0);
            return false;
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EyeofCthulhu);
            NPC.aiStyle = -1;
            NPC.behindTiles = true;
            NPC.noGravity = false;
            phase = 0;
            Music = MusicID.Boss3;
            NPC.Size = new Vector2(150);
            NPC.noTileCollide = false;
        }
        public override void AI()
        {
            switch (phase)
            {
                case 0:
                    Phase0();
                    break;
            }
        }
    }
}
