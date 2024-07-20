using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.Cryogen
{
    //[AutoloadBossHead]
    public partial class Cryogen : ModNPC
    {
        public byte phase = 0;
        public Player target
        { get { return Main.player[NPC.target]; } }

        private static Asset<Texture2D> backTexture;
        public override void HitEffect(NPC.HitInfo hit)
        {
        }
        public override void SetStaticDefaults()
        {
            backTexture = ModContent.Request<Texture2D>(Texture + "Flake");

            Main.npcFrameCount[Type] = 3;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "CalamityVanilla/Assets/Textures/Bestiary/HiveMind_Preview",
                //PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.life < NPC.lifeMax / 3)
                NPC.frame.Y = frameHeight * 2;
            else if (NPC.life < NPC.lifeMax / 3 * 2)
                NPC.frame.Y = frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> tex = TextureAssets.Npc[Type];
            Rectangle bigFlake = new Rectangle(0,0,270,270);
            Rectangle smallFlake = new Rectangle(0, 272, 126, 126);
            spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, bigFlake, Color.White * 0.5f, NPC.rotation, bigFlake.Size() / 2, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(backTexture.Value, NPC.Center - Main.screenPosition, smallFlake, Color.White * 0.7f, -NPC.rotation, smallFlake.Size() / 2, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex.Value,NPC.Center - Main.screenPosition, NPC.frame,Color.White,NPC.velocity.X * 0.03f,NPC.frame.Size() / 2, 1f, SpriteEffects.None,0);
            return false;
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EyeofCthulhu);

            NPC.lifeMax = 16000;
            NPC.defense = 30;

            NPC.aiStyle = -1;
            NPC.noGravity = true;
            phase = 0;
            Music = MusicID.Boss1;
            NPC.Size = new Vector2(78);
            NPC.noTileCollide = true;

            NPC.HitSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].HitSound;
            NPC.DeathSound = ContentSamples.NpcsByNetId[NPCID.IceElemental].DeathSound;
        }
        public override void AI()
        {
            NPC.direction = Math.Sign(NPC.velocity.X);
            Lighting.AddLight(NPC.Center, new Vector3(0.8f,1f,1f));
            NPC.rotation += NPC.velocity.Length() * 0.01f * NPC.direction;
            

            
            switch (phase)
            {
                case 0:
                    FlyAndShoot();
                    break;
            }
        }
    }
}
