using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common
{
    /// <summary>
    /// An abstract clas to facilitate the creation of worm NPCs. By default, this NPC has no movement to it and has to be set in AI() or HeadAI() preferably.
    /// </summary>
    public abstract class WormNPC : ModNPC
    {
        public enum WormSegment
        {
            Head = 0,
            Body = 1,
            Tail = 2
        }

        //NPC.ai[0] is the NPC that this NPC is following
        //NPC.ai[1] is the NPC segmenttype
        public int maxlength = 10;

        /// <summary>
        /// Array with the Y position of each segment in the sprite sheet, used to get the correct segment position in the sprite sheet
        /// </summary>
        public int[] segmentspriteposition = new int[] {0, 34, 66, 94};

        /// <summary>
        /// Array with the size of each segment in the sprite sheet, used to get the correct segment size in the sprite sheet
        /// </summary>
        public int[] segmentsizes = new int[] { 34, 32, 28, 44 };

        /// <summary>
        /// Number of segments in the sprite sheet
        /// </summary>
        public int sheetsegments = 4;

        /// <summary>
        /// If the worm has more segments than segments in the sprite sheet, this array of segments will be repeated instead.
        /// </summary>
        public int[] repeatingsegments = new int[] {0};

        /// <summary>
        /// The inward offset for each segment
        /// </summary>
        public int inwardsegmentoffset = 4;

        /// <summary>
        /// Size for all segment hitboxes, set to -1 to automatically set it (Not recommended for big segments)
        /// </summary>
        public int hitboxsize = -1;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => NPC.ai[1] == (byte)WormSegment.Head;


        public override bool PreAI()
        {
            if (NPC.ai[1] == (byte)WormSegment.Head) HeadAI();
            else BodyAI();

            NPC.spriteDirection = Math.Abs(NPC.rotation) > MathHelper.Pi ? -1 : 1;

            return true;
        }

        /// <summary>
        /// Head AI, It is recommended to use this for worm movement
        /// </summary>
        public void HeadAI()
        {           
            NPC.timeLeft = 90;
        }
        
        /// <summary>
        /// AI that the body uses, this is for attaching segments to each other
        /// </summary>
        public void BodyAI()
        {
            NPC followNPC = Main.npc[(int)NPC.ai[0]];
            if (!followNPC.active) NPC.active = false;

            float rotdifference = Math.Abs(NPC.rotation - followNPC.rotation) / MathHelper.Pi;

            NPC.Center = followNPC.Center + followNPC.Center.DirectionTo(NPC.Center) * (NPC.frame.Height - inwardsegmentoffset);

            NPC.rotation = NPC.DirectionTo(followNPC.Center).ToRotation() + MathHelper.PiOver2;
            NPC.velocity = Vector2.Zero;
            NPC.timeLeft = 90;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            //Create all segments
            if (NPC.ai[1] == (byte)WormSegment.Head)
            {
                NPC.frame.Y = segmentspriteposition[0];
                NPC.frame.Height = segmentsizes[0];
                NPC.Size = hitboxsize == -1 ? new Vector2(segmentsizes[0]) : new Vector2(hitboxsize);
                NPC.realLife = NPC.whoAmI;
                int latestnpc = NPC.whoAmI;
                int currentsegment = 1;

                int maxrepeatingsegment = 0;

                for (int i = 0; i < repeatingsegments.Length; i++)
                {
                    if (repeatingsegments[i] > maxrepeatingsegment) maxrepeatingsegment = repeatingsegments[i];
                }

                for (int i = 0; i < maxlength; i++)
                {
                    NPC spawnedsegment = NPC.NewNPCDirect(source, NPC.position, Type, 0, latestnpc, NPC.whoAmI, (byte)WormSegment.Body);
                    spawnedsegment.frame.Y = segmentspriteposition[currentsegment];
                    spawnedsegment.frame.Height = segmentsizes[currentsegment];
                    spawnedsegment.Size = hitboxsize == -1? new Vector2(segmentsizes[currentsegment]) : new Vector2(hitboxsize);
                    spawnedsegment.realLife = NPC.whoAmI;
                    latestnpc = spawnedsegment.whoAmI;

                    //Set segment sprite
                    if ((sheetsegments - maxrepeatingsegment) > (maxlength - 1 - i))
                    {
                        currentsegment = sheetsegments + 1 - (maxlength - i);
                    }
                    else if (repeatingsegments.Contains(currentsegment))
                    {
                        if (currentsegment < maxrepeatingsegment) currentsegment++;
                        else currentsegment = repeatingsegments[0];
                    }
                    else currentsegment++;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex,
                NPC.Center - Main.screenPosition,
                NPC.frame,
                drawColor,
                NPC.rotation,
                new Vector2(NPC.frame.Width/2, NPC.frame.Height),
                1f,
                SpriteEffects.None,
                0);
            return false;
        }
        
    }
}
