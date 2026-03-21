using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.AIStyles;

public static class FighterAI
{
    /*
     * List of things that need to be sorted out eventually:
     * Wide npcs can't open doors
     * hasWideHitbox could be made automatic
     * figure out what crab does
     * add jump height
     */
    public static void AI(NPC n, float moveSpeed, out bool JustJumped, bool despawnDiscouraged = true, bool hasWideHitbox = false, bool canOpenDoors = false, bool undead = false, bool crab = true, int wallForm = -1)
    {
        JustJumped = false;
        if (Main.player[n.target].position.Y + (float)Main.player[n.target].height == n.position.Y + (float)n.height)
        {
            n.directionY = -1;
        }
        bool flag = false;
        bool flag23 = false;
        bool flag24 = false;
        if (n.velocity.X == 0f)
        {
            flag24 = true;
        }
        if (n.justHit)
        {
            flag24 = false;
        }
        int num141 = 60;
        bool flag25 = false;
        bool flag27 = false;
        bool flag2 = true;
        if (!flag27 && flag2)
        {
            if (n.velocity.Y == 0f && ((n.velocity.X > 0f && n.direction < 0) || (n.velocity.X < 0f && n.direction > 0)))
            {
                flag25 = true;
            }
            if (n.position.X == n.oldPosition.X || n.ai[3] >= (float)num141 || flag25)
            {
                n.ai[3] += 1f;
            }
            else if ((double)Math.Abs(n.velocity.X) > 0.9 && n.ai[3] > 0f)
            {
                n.ai[3] -= 1f;
            }
            if (n.ai[3] > (float)(num141 * 10))
            {
                n.ai[3] = 0f;
            }
            if (n.justHit)
            {
                n.ai[3] = 0f;
            }
            if (n.ai[3] == (float)num141)
            {
                n.netUpdate = true;
            }
            if (Main.player[n.target].Hitbox.Intersects(n.Hitbox))
            {
                n.ai[3] = 0f;
            }
        }
        if (n.ai[3] < (float)num141 && despawnDiscouraged)
        {
            n.TargetClosest();
            if (n.directionY > 0 && Main.player[n.target].Center.Y <= n.Bottom.Y)
            {
                n.directionY = -1;
            }
        }
        else if (!(n.ai[2] > 0f) || !despawnDiscouraged)
        {
            if (Main.dayTime && (double)(n.position.Y / 16f) < Main.worldSurface)
            {
                n.EncourageDespawn(10);
            }
            if (n.velocity.X == 0f)
            {
                if (n.velocity.Y == 0f)
                {
                    n.ai[0] += 1f;
                    if (n.ai[0] >= 2f)
                    {
                        n.direction *= -1;
                        n.spriteDirection = n.direction;
                        n.ai[0] = 0f;
                    }
                }
            }
            else
            {
                n.ai[0] = 0f;
            }
            if (n.direction == 0)
            {
                n.direction = 1;
            }
        }

        if (n.velocity.X < 0f - moveSpeed || n.velocity.X > moveSpeed)
        {
            if (n.velocity.Y == 0f)
            {
                n.velocity *= 0.8f;
            }
        }
        else if (n.velocity.X < moveSpeed && n.direction == 1)
        {
            n.velocity.X += 0.07f;
            if (n.velocity.X > moveSpeed)
            {
                n.velocity.X = moveSpeed;
            }
        }
        else if (n.velocity.X > 0f - moveSpeed && n.direction == -1)
        {
            n.velocity.X -= 0.07f;
            if (n.velocity.X < 0f - moveSpeed)
            {
                n.velocity.X = 0f - moveSpeed;
            }
        }

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (n.velocity.Y == 0f)
            {
                if (wallForm != -1 && n.NPCCanStickToWalls())
                {
                    n.Transform(wallForm);
                }
            }
        }
        if (n.velocity.Y == 0f || flag)
        {
            int num72 = (int)(n.position.Y + (float)n.height + 7f) / 16;
            int num73 = (int)(n.position.Y - 9f) / 16;
            int num74 = (int)n.position.X / 16;
            int num75 = (int)(n.position.X + (float)n.width) / 16;
            int num196 = (int)(n.position.X + 8f) / 16;
            int num76 = (int)(n.position.X + (float)n.width - 8f) / 16;
            bool flag13 = false;
            for (int num78 = num196; num78 <= num76; num78++)
            {
                if (num78 >= num74 && num78 <= num75 && Main.tile[num78, num72] == null)
                {
                    flag13 = true;
                    continue;
                }
                if (Main.tile[num78, num73] != null && Main.tile[num78, num73].HasTile && Main.tileSolid[Main.tile[num78, num73].TileType])
                {
                    flag23 = false;
                    break;
                }
                if (!flag13 && num78 >= num74 && num78 <= num75 && Main.tile[num78, num72].HasTile && Main.tileSolid[Main.tile[num78, num72].TileType])
                {
                    flag23 = true;
                }
            }
            if (!flag23 && n.velocity.Y < 0f)
            {
                n.velocity.Y = 0f;
            }
            if (flag13)
            {
                return;
            }
        }

        if (n.velocity.Y >= 0f && n.directionY != 1)
		{
			int num79 = 0;
			if (n.velocity.X < 0f)
			{
				num79 = -1;
			}
			if (n.velocity.X > 0f)
			{
				num79 = 1;
			}
			Vector2 position3 = n.position;
			position3.X += n.velocity.X;
			int num80 = (int)((position3.X + (float)(n.width / 2) + (float)((n.width / 2 + 1) * num79)) / 16f);
			int num81 = (int)((position3.Y + (float)n.height - 1f) / 16f);
			if (WorldGen.InWorld(num80, num81, 4))
			{
				//if (Main.tile[num80, num81] == null)
				//{
				//	Main.tile[num80, num81] = default(Tile);
				//}
				//if (Main.tile[num80, num81 - 1] == null)
				//{
				//	Main.tile[num80, num81 - 1] = default(Tile);
				//}
				//if (Main.tile[num80, num81 - 2] == null)
				//{
				//	Main.tile[num80, num81 - 2] = default(Tile);
				//}
				//if (Main.tile[num80, num81 - 3] == null)
				//{
				//	Main.tile[num80, num81 - 3] = default(Tile);
				//}
				//if (Main.tile[num80, num81 + 1] == null)
				//{
				//	Main.tile[num80, num81 + 1] = default(Tile);
				//}
				//if (Main.tile[num80 - num79, num81 - 3] == null)
				//{
				//	Main.tile[num80 - num79, num81 - 3] = default(Tile);
				//}
                // torment nexus line what the hell
				if ((float)(num80 * 16) < position3.X + (float)n.width && (float)(num80 * 16 + 16) > position3.X && ((Main.tile[num80, num81].HasTile && !Main.tile[num80, num81].TopSlope && !Main.tile[num80, num81 - 1].TopSlope && Main.tileSolid[Main.tile[num80, num81].TileType] && !Main.tileSolidTop[Main.tile[num80, num81].TileType]) || (Main.tile[num80, num81 - 1].IsHalfBlock && Main.tile[num80, num81 - 1].HasTile)) && (!Main.tile[num80, num81 - 1].HasTile || !Main.tileSolid[Main.tile[num80, num81 - 1].TileType] || Main.tileSolidTop[Main.tile[num80, num81 - 1].TileType] || (Main.tile[num80, num81 - 1].IsHalfBlock && (!Main.tile[num80, num81 - 4].HasTile || !Main.tileSolid[Main.tile[num80, num81 - 4].TileType] || Main.tileSolidTop[Main.tile[num80, num81 - 4].TileType]))) && (!Main.tile[num80, num81 - 2].HasTile || !Main.tileSolid[Main.tile[num80, num81 - 2].TileType] || Main.tileSolidTop[Main.tile[num80, num81 - 2].TileType]) && (!Main.tile[num80, num81 - 3].HasTile || !Main.tileSolid[Main.tile[num80, num81 - 3].TileType] || Main.tileSolidTop[Main.tile[num80, num81 - 3].TileType]) && (!Main.tile[num80 - num79, num81 - 3].HasTile || !Main.tileSolid[Main.tile[num80 - num79, num81 - 3].TileType]))
				{
					float num82 = num81 * 16;
					if (Main.tile[num80, num81].IsHalfBlock)
					{
						num82 += 8f;
					}
					if (Main.tile[num80, num81 - 1].IsHalfBlock)
					{
						num82 -= 8f;
					}
					if (num82 < position3.Y + (float)n.height)
					{
						float num83 = position3.Y + (float)n.height - num82;
						float num84 = 16.1f;
						if (num83 <= num84)
						{
							n.gfxOffY += n.position.Y + (float)n.height - num82;
							n.position.Y = num82 - (float)n.height;
							if (num83 < 9f)
							{
								n.stepSpeed = 1f;
							}
							else
							{
								n.stepSpeed = 2f;
							}
						}
					}
				}
			}
		}
        if (flag23)
        {
            int num85 = (int)((n.position.X + (float)(n.width / 2) + (float)(15 * n.direction)) / 16f);
            int num86 = (int)((n.position.Y + (float)n.height - 15f) / 16f);
            if (hasWideHitbox)
            {
                num85 = (int)((n.position.X + (float)(n.width / 2) + (float)((n.width / 2 + 16) * n.direction)) / 16f);
            }
            //if (Main.tile[num85, num86] == null)
            //{
            //    Main.tile[num85, num86] = default(Tile);
            //}
            //if (Main.tile[num85, num86 - 1] == null)
            //{
            //    Main.tile[num85, num86 - 1] = default(Tile);
            //}
            //if (Main.tile[num85, num86 - 2] == null)
            //{
            //    Main.tile[num85, num86 - 2] = default(Tile);
            //}
            //if (Main.tile[num85, num86 - 3] == null)
            //{
            //    Main.tile[num85, num86 - 3] = default(Tile);
            //}
            //if (Main.tile[num85, num86 + 1] == null)
            //{
            //    Main.tile[num85, num86 + 1] = default(Tile);
            //}
            //if (Main.tile[num85 + n.direction, num86 - 1] == null)
            //{
            //    Main.tile[num85 + n.direction, num86 - 1] = default(Tile);
            //}
            //if (Main.tile[num85 + n.direction, num86 + 1] == null)
            //{
            //    Main.tile[num85 + n.direction, num86 + 1] = default(Tile);
            //}
            //if (Main.tile[num85 - n.direction, num86 + 1] == null)
            //{
            //    Main.tile[num85 - n.direction, num86 + 1] = default(Tile);
            //}
            if (Main.tile[num85, num86 - 1].HasTile && (TileLoader.IsClosedDoor(Main.tile[num85, num86 - 1]) || Main.tile[num85, num86 - 1].TileType == TileID.TallGateClosed) && canOpenDoors)
            {
                n.ai[2] += 1f;
                n.ai[3] = 0f;
                if (n.ai[2] >= 60f)
                {
                    
                    bool flag15 = Main.player[n.target].ZoneGraveyard && Main.rand.Next(60) == 0;
                    if ((!Main.bloodMoon || Main.getGoodWorld) && !flag15 && undead)
                    {
                        n.ai[1] = 0f;
                    }
                    n.velocity.X = 0.5f * (float)(-n.direction);
                    int num87 = 5;
                    if (Main.tile[num85, num86 - 1].TileType == TileID.TallGateClosed)
                    {
                        num87 = 2;
                    }
                    n.ai[1] += num87;
                    //if (n.type == 27)
                    //{
                    //    n.ai[1] += 1f;
                    //}
                    //if (n.type == 31 || n.type == 294 || n.type == 295 || n.type == 296)
                    //{
                    //    n.ai[1] += 6f;
                    //}
                    n.ai[2] = 0f;
                    //bool flag16 = false;
                    if (n.ai[1] >= 10f)
                    {
                        //flag16 = true;
                        n.ai[1] = 10f;
                    }
                    WorldGen.KillTile(num85, num86 - 1, fail: true);
                    {
                        //if (n.type == 26)
                        //{
                        //    WorldGen.KillTile(num85, num86 - 1);
                        //    if (Main.netMode == 2)
                        //    {
                        //        NetMessage.SendData(17, -1, -1, null, 0, num85, num86 - 1);
                        //    }
                        //}
                        //else
                        {
                            if (TileLoader.OpenDoorID(Main.tile[num85, num86 - 1]) >= 0)
                            {
                                bool flag17 = WorldGen.OpenDoor(num85, num86 - 1, n.direction);
                                if (!flag17)
                                {
                                    n.ai[3] = num141;
                                    n.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server && flag17)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, num85, num86 - 1, n.direction);
                                }
                            }
                            if (Main.tile[num85, num86 - 1].TileType == TileID.TallGateClosed)
                            {
                                bool flag18 = WorldGen.ShiftTallGate(num85, num86 - 1, closing: false);
                                if (!flag18)
                                {
                                    n.ai[3] = num141;
                                    n.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server && flag18)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, num85, num86 - 1);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                int num89 = n.spriteDirection;
                if ((n.velocity.X < 0f && num89 == -1) || (n.velocity.X > 0f && num89 == 1))
                {
                    if (n.height >= 32 && Main.tile[num85, num86 - 2].HasTile && Main.tileSolid[Main.tile[num85, num86 - 2].TileType])
                    {
                        if (Main.tile[num85, num86 - 3].HasTile && Main.tileSolid[Main.tile[num85, num86 - 3].TileType])
                        {
                            n.velocity.Y = -8f;
                            JustJumped = true;
                            n.netUpdate = true;
                        }
                        else
                        {
                            n.velocity.Y = -7f;
                            JustJumped = true;
                            n.netUpdate = true;
                        }
                    }
                    else if (Main.tile[num85, num86 - 1].HasTile && Main.tileSolid[Main.tile[num85, num86 - 1].TileType])
                    {
                        n.velocity.Y = -6f;
                        JustJumped = true;
                        n.netUpdate = true;
                    }
                    else if (n.position.Y + (float)n.height - (float)(num86 * 16) > 20f && Main.tile[num85, num86].HasTile && !Main.tile[num85, num86].TopSlope && Main.tileSolid[Main.tile[num85, num86].TileType])
                    {
                        n.velocity.Y = -5f;
                        JustJumped = true;
                        n.netUpdate = true;
                    }
                    else if (n.directionY < 0 && crab && (!Main.tile[num85, num86 + 1].HasTile || !Main.tileSolid[Main.tile[num85, num86 + 1].TileType]) && (!Main.tile[num85 + n.direction, num86 + 1].HasTile || !Main.tileSolid[Main.tile[num85 + n.direction, num86 + 1].TileType]))
                    {
                        n.velocity.Y = -8f;
                        JustJumped = true;
                        n.velocity.X *= 1.5f;
                        n.netUpdate = true;
                    }
                    else if (canOpenDoors)
                    {
                        n.ai[1] = 0f;
                        n.ai[2] = 0f;
                    }
                    if (n.velocity.Y == 0f && flag24 && n.ai[3] == 1f)
                    {
                        n.velocity.Y = -5f;
                        JustJumped = true;
                    }
                    if (n.velocity.Y == 0f && (Main.expertMode/* || n.type == 586*/) && Main.player[n.target].Bottom.Y < n.Top.Y && Math.Abs(n.Center.X - Main.player[n.target].Center.X) < (float)(Main.player[n.target].width * 3) && Collision.CanHit(n, Main.player[n.target]))
                    {
                        //if (n.type == 586)
                        //{
                        //    int num91 = (int)((n.Bottom.Y - 16f - Main.player[n.target].Bottom.Y) / 16f);
                        //    if (num91 < 14 && Collision.CanHit(n, Main.player[n.target]))
                        //    {
                        //        if (num91 < 7)
                        //        {
                        //            n.velocity.Y = -8.8f;
                        //        }
                        //        else if (num91 < 8)
                        //        {
                        //            n.velocity.Y = -9.2f;
                        //        }
                        //        else if (num91 < 9)
                        //        {
                        //            n.velocity.Y = -9.7f;
                        //        }
                        //        else if (num91 < 10)
                        //        {
                        //            n.velocity.Y = -10.3f;
                        //        }
                        //        else if (num91 < 11)
                        //        {
                        //            n.velocity.Y = -10.6f;
                        //        }
                        //        else
                        //        {
                        //            n.velocity.Y = -11f;
                        //        }
                        //    }
                        //}
                        if (n.velocity.Y == 0f)
                        {
                            int num92 = 6;
                            if (Main.player[n.target].Bottom.Y > n.Top.Y - (float)(num92 * 16))
                            {
                                n.velocity.Y = -7.9f;
                                JustJumped = true;
                            }
                            else
                            {
                                int num93 = (int)(n.Center.X / 16f);
                                int num94 = (int)(n.Bottom.Y / 16f) - 1;
                                for (int num95 = num94; num95 > num94 - num92; num95--)
                                {
                                    if (Main.tile[num93, num95].HasTile && TileID.Sets.Platforms[Main.tile[num93, num95].TileType])
                                    {
                                        n.velocity.Y = -7.9f;
                                        JustJumped = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (canOpenDoors)
        {
            n.ai[1] = 0f;
            n.ai[2] = 0f;
        }
    }
}
