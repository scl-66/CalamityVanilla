using CalamityVanilla.Common;
using CalamityVanilla.Content.Projectiles.Hostile;
using CalamityVanilla.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.Cryogen
{
    public partial class Cryogen : ModNPC
    {
        public void Phase2Derping_3()
        {
            NPC.ai[0]++;
            NPC.velocity += NPC.Center.DirectionTo(target.Center + new Vector2(0, 300 + (100 * MathF.Sin(NPC.ai[0] * 0.01f))).RotatedBy(NPC.ai[0] * 0.03f)) * 0.8f;
            NPC.velocity = NPC.velocity.LengthClamp(12);
        }
        public void SpikyIceBarrier_2()
        {
            if (NPC.ai[0] == -30)
            {
                SoundEngine.PlaySound(SoundID.Item160,NPC.Center);
                NPC.rotation += NPC.direction * 0.2f;
            }
            NPC.ai[0]++;
            if (NPC.ai[0] == 19 && NPC.noTileCollide)
            {
                NPC.ai[0] = 19;
            }
            if (NPC.ai[0] > 0)
            {
                NPC.velocity *= 0.9f;
                NPC.rotation += MathF.Sin(NPC.ai[0] * 0.8f) * 0.01f;

                if (NPC.ai[0] >= 60 && NPC.ai[0] % 60 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, 8).RotatedBy(NPC.ai[0] * 0.01f), ModContent.ProjectileType<IceBomb>(), 30, 0, -1, Main.rand.Next(2),8);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, -8).RotatedBy(NPC.ai[0] * 0.01f), ModContent.ProjectileType<IceBomb>(), 30, 0, -1, Main.rand.Next(2),8);
                }
            }
            else
            {
                if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.noTileCollide = false;
                NPC.velocity += NPC.Center.DirectionTo(target.Center) * (Collision.SolidCollision(NPC.position,NPC.width,NPC.height)? 2f : -0.5f);
            }
            if (NPC.ai[0] == 20)
            {
                NPC.behindTiles = true;
                NPC.noTileCollide = false;
                SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, NPC.Center);
                // Make the ice barrier
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Point placePos = NPC.Center.ToTileCoordinates();
                    int squaresize = 16;
                    float sinMultipler = Main.rand.NextFloat(4, 8);
                    for (int x = -squaresize; x <= squaresize; x++)
                    {
                        for (int y = -squaresize; y <= squaresize; y++)
                        {
                            if (new Vector2(placePos.X + x, placePos.Y + y).Distance(NPC.Center * (1 / 16f)) <= Math.Abs(MathF.Sin(new Vector2(placePos.X + x, placePos.Y + y).DirectionTo(NPC.Center * (1 / 16f)).ToRotation() * sinMultipler) * (squaresize - 8)) + 8)
                            {
                                WorldGen.PlaceTile(placePos.X + x, placePos.Y + y, ModContent.TileType<CryogenIceTile>(), plr: Main.myPlayer);
                                CryogenIceBlockSystem.CryogenIceBlocks.Add(new Vector3(placePos.X + x, placePos.Y + y, CryogenIceBlockSystem.DEFAULT_ICE_TIMER * 20));
                            }
                        }
                    }
                    NetMessage.SendTileSquare(-squaresize, placePos.X - squaresize, placePos.Y - 1, (squaresize * 2) + 1, (squaresize * 2) + 1);
                }
            }
        }
        public void FlyAndShoot_0()
        {

            if (NPC.ai[0] <= 0)
            {
                NPC.velocity.Y += 0.3f;
                NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.8f;
                NPC.velocity = NPC.velocity.LengthClamp(6, 0);

                if(NPC.life < NPC.lifeMax * _phase2HealthMultiplier)
                {
                    NPC.ai[0] = -30;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    phase = 2;
                }
            }
            else
            {
                NPC.rotation += NPC.direction * 0.1f;
                NPC.velocity *= 0.99f;
            }
            if (NPC.ai[0] == -31)
            {
                NPC.TargetClosest();
                if (NPC.ai[1] % 4 == 0)
                {
                    NPC.ai[0] = 60;
                }
            }
            if (NPC.ai[0] > -30 && NPC.ai[0] <= 0)
            {
                float multiplier = (30 + NPC.ai[0]) / 30f;
                NPC.rotation += NPC.direction * 0.3f * multiplier;
                NPC.velocity += NPC.Center.DirectionTo(target.Center) * -multiplier * 3;
            }

            if (NPC.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);
                NPC.velocity = NPC.Center.DirectionTo(target.Center) * 24f;
            }
            else if (NPC.ai[0] == 60)
            {
                NPC.ai[1]++;
                NPC.ai[0] = NPC.ai[1] % 3 == 0? -200 : -120;
            }
            NPC.ai[0]++;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center) * 16, ProjectileID.FrostBlastHostile, 24, 0);
                
                switch(NPC.ai[1] % 3)
                {
                    case 0: // Ice blocks or do the slam thing instead
                        if (NPC.ai[0] is -190 or -180 or -170 or -100 or -90 or -80) {

                            if (NPC.ai[0] == -190 && Main.rand.NextBool(3))
                            {
                                NPC.ai[0] = -60;
                                NPC.ai[2] = 0;
                                NPC.ai[1] = -1.5f;
                                phase = 1;
                                NPC.netUpdate = true;
                                return;
                            }
                            Vector2 blockPlacement = (target.Center + target.velocity * 40) + Main.rand.NextVector2Circular(16 * 4, 16 * 4);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(blockPlacement) * 48, ModContent.ProjectileType<CryogenIceBlock>(), 0, 0, ai0: blockPlacement.X, ai1: blockPlacement.Y);
                        }
                        break;
                    case 1: // Statues
                        if (NPC.ai[0] is -110 or -90 or - 70 or -50)
                        {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(16,16), ModContent.ProjectileType<IceStatues>(), 40, 0,-1,target.whoAmI,NPC.whoAmI,-80);
                        }
                        break;
                    case 2: // Bombs
                        if (NPC.ai[0] == -110)
                        {
                            for (int i = -2; i <= 2; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center).RotatedBy(0.2 * i) * (10 - Math.Abs(i * 2)), ModContent.ProjectileType<IceBomb>(), 30, 0, -1, Main.rand.Next(2),15);
                            }
                        }
                        break;
                }
            }
        }
        public void SlamAttack_1()
        {
            NPC.ai[0]++;
            if (NPC.ai[2] == 0)
            {
                if (NPC.ai[0] < 0)
                {
                    NPC.velocity.Y += 0.3f;
                    NPC.velocity += NPC.Center.DirectionTo(target.Center + new Vector2(0, -300)) * 1.4f;
                    NPC.velocity = NPC.velocity.LengthClamp(9, 0);
                }
                else
                {

                    NPC.velocity.X *= 0.96f;
                    NPC.ai[1] += 0.1f;
                    NPC.velocity.Y += NPC.ai[1];
                    NPC.velocity = NPC.velocity.LengthClamp(32, 0);

                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.ai[2] = 1;
                        NPC.ai[0] = 0;
                        NPC.velocity.Y = -20;
                        SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, Main.rand.NextVector2Circular(30,10) - new Vector2(0,15), ModContent.ProjectileType<IceChunks>(), 20, 1);
                            }
                        }
                    }
                }
            }
            else
            {
                NPC.velocity *= 0.99f;
                NPC.ai[0]++;
                if (NPC.ai[0] == 60)
                {
                    phase = 0;
                    NPC.ai[0] = -30;
                    NPC.ai[1] = 1;
                    NPC.ai[2] = 0;
                }
            }
        }
    }
}
