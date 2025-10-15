using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityVanilla.Content.Bosses.Cryogen;
public partial class Cryogen : ModNPC
{
    private void ShootIceBlocks_0()
    {
        if (NPC.ai[0] > 0 || NPC.Center.Distance(target.Center) < 240)
        {
            NPC.ai[0]++;
            if (NPC.velocity.Length() > 1)
                NPC.velocity *= 0.95f;
        }
        else
        {
            NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.4f;
            NPC.velocity = NPC.velocity.LengthClamp(Math.Max(9,target.velocity.Length() * 1.1f), 0);
        }
        if (NPC.ai[0] > 60 && NPC.ai[0] % 10 == 0 && NPC.ai[0] < 130)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 ShootSpot = Vector2.Zero;
                if ((NPC.ai[3] + 1) % 2 != 0)
                {
                    ShootSpot = target.Center + Main.rand.NextVector2CircularEdge(200, 200) + target.velocity * 10;
                    float distance = ShootSpot.Distance(target.Center);
                    if (distance < 100)
                    {
                        ShootSpot -= ShootSpot.DirectionTo(target.Center) * (100f - distance);
                    }
                }
                else
                {
                    ShootSpot = NPC.Center + Main.rand.NextVector2CircularEdge(500, 200);
                    float distance = ShootSpot.Distance(target.Center);
                    if (distance < 100)
                    {
                        ShootSpot -= ShootSpot.DirectionTo(target.Center) * (100f - distance);
                    }
                }
                if (Main.tileSolid[Main.tile[ShootSpot.ToTileCoordinates()].TileType] && Main.tile[ShootSpot.ToTileCoordinates()].HasTile)
                {
                    continue;
                }
                else
                {
                    SpawnBigIceBlock(ShootSpot.ToTileCoordinates(), Main.rand.Next(1, 4), Main.rand.Next(1, 4));
                    break;
                }
            }
        }
        if (NPC.ai[0] > 130)
        {
            NPC.ai[0] = 0;
            NPC.ai[3]++;
            phase = (byte)Main.rand.Next(1, 5);
            NPC.netUpdate = true;
        }
    }
    private void DashAndChase_1()
    {
        NPC.ai[0]++;
        if (NPC.ai[0] <= 60)
        {
            NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.4f;
            NPC.velocity = NPC.velocity.LengthClamp(4.5f, 0);
        }
        else if (NPC.ai[0] < 120)
        {
            NPC.velocity *= 0.98f;
            NPC.velocity += NPC.Center.DirectionTo(target.Center) * -0.1f;
            NPC.rotation += NPC.direction * (NPC.ai[0] - 60) * 0.01f;
            _snowOverlayOpacity += 1f / 60;
            _snowOverlaySpinDirection = NPC.direction;
        }
        else if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC.rotation += NPC.direction * 0.1f;
            NPC.velocity *= 0.985f;
            bool hitAnything = false;
            for (int x = NPC.Left.ToTileCoordinates().X; x < NPC.Right.ToTileCoordinates().X; x++) // Destroy ice blocks
            {
                for (int y = NPC.Top.ToTileCoordinates().Y; y < NPC.Bottom.ToTileCoordinates().Y; y++)
                {
                    if (Main.tile[x, y].TileType == ModContent.TileType<CryogenIceTile>())
                    {
                        hitAnything = true;
                        WorldGen.KillTile(x, y);
                        NetMessage.SendTileSquare(-1, x, y);
                        //if (Main.rand.NextBool(6) && Main.netMode != NetmodeID.MultiplayerClient)
                        //{
                        //    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Point(x, y).ToWorldCoordinates(), new Point(x, y).ToWorldCoordinates().DirectionTo(target.Center).RotatedByRandom(0.1f) * 17, ModContent.ProjectileType<IceShrapnel>(), 12, 1, -1);
                        //}
                    }
                }
            }
            if (hitAnything)
            {
                NPC.velocity *= 0.9f;
                NPC.localAI[0] = 5;
                NPC.netUpdate = true;
            }
        }
        if (NPC.ai[0] == 120)
        {
            NPC.TargetClosest();
            SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);
            NPC.velocity = NPC.Center.DirectionTo(target.Center) * 34f;
        }
        if (NPC.ai[0] > 120 + 30)
        {
            NPC.ai[0] = 0;
            phase = 0;
        }
    }
    private void Snowflakes_2()
    {
        NPC.ai[0]++;
        NPC.velocity *= 0.98f;

        NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.2f;
        NPC.velocity = NPC.velocity.LengthClamp(4.5f, 0);

        if (NPC.ai[0] > 60 && NPC.ai[0] % 8 == 0 && NPC.ai[0] < 130)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(3f, 5f), ModContent.ProjectileType<CryoFlake1>() + Main.rand.Next(3), 15, 1, -1, NPC.target, 0, Main.rand.NextFloat(0.8f, 1.1f));
            }
        }
        if (NPC.ai[0] > 130)
        {
            NPC.ai[0] = 0;
            phase = 0;
        }
    }
    private void Statues_3()
    {
        NPC.ai[0]++;

        if (NPC.ai[0] < 60)
        {
            NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.2f;
            NPC.velocity = NPC.velocity.LengthClamp(4.5f, 1);
        }
        else
        {
            NPC.velocity *= 0.98f;
        }

        if (NPC.ai[0] > 60 && NPC.ai[0] % 20 == 0 && NPC.ai[0] < 160)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(16, 16), ModContent.ProjectileType<IceStatues>(), 40, 0, -1, target.whoAmI, NPC.whoAmI, -80);
            }
        }
        if (NPC.ai[0] > 240)
        {
            NPC.ai[0] = 0;
            phase = 0;
        }
    }
    private void SlamAttack_4()
    {
        int slamStart = 10;

        if (NPC.ai[1] == 0)
        {
            if(Math.Abs(NPC.Center.X - target.Center.X) < 100)
                NPC.ai[0]++;
            if (NPC.ai[0] < slamStart)
            {
                NPC.velocity += NPC.Center.DirectionTo(target.Center + new Vector2(0, -400)) * 1.4f;
                NPC.velocity = NPC.velocity.LengthClamp(Math.Max(9, target.velocity.Length() * 1.1f), 0);
            }
            else
            {

                NPC.velocity.X *= 0.96f;
                NPC.localAI[1] += 0.08f;
                NPC.velocity.Y += NPC.localAI[1] - 1f;
                NPC.velocity = NPC.velocity.LengthClamp(32, 0);

                if (NPC.velocity.Y > 5)
                {
                    List<Point> tiles = Collision.GetTilesIn(NPC.position, NPC.BottomRight);
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        if (Main.tile[tiles[i]].HasTile && Main.tile[tiles[i]].TileType == ModContent.TileType<CryogenIceTile>())
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                WorldGen.KillTile(tiles[i].X, tiles[i].Y, false, false, true);
                                NetMessage.SendTileSquare(-1, tiles[i].X, tiles[i].Y);
                            }
                        }
                        else if (Main.tile[tiles[i]].HasTile && Main.tileSolid[Main.tile[tiles[i]].TileType] && !Main.tileSolidTop[Main.tile[tiles[i]].TileType])
                        {
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                            NPC.velocity.Y = -20;
                            SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i2 = 0; i2 < 15; i2++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, Main.rand.NextVector2Circular(10, 10) - new Vector2(0, 15), ModContent.ProjectileType<IceChunks>(), 20, 1);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            NPC.velocity *= 0.99f;
            NPC.ai[0]++;
            if (NPC.ai[0] == slamStart + 30)
            {
                phase = 0;
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                NPC.localAI[1] = 0;
            }
        }
    }
}