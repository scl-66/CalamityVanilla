using CalamityVanilla.Content.Bosses.HiveMind.Minions;
using CalamityVanilla.Content.Bosses.HiveMind.Projectiles;
using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind;
public partial class HiveMind
{
    public const int ShieldMax = 35;
    private ref float _shieldAmountForPhase2 => ref NPC.ai[3];

    private const float _secondPhaseHealthPercent = 0.35f;
    private ref float _currentAttack => ref NPC.ai[0];
    public override void AI()
    {
        if (_currentAttack > 20)
        {
            Lighting.AddLight(NPC.Center, new Vector3(1.7f,0,2) * NPC.localAI[2]);
            if (_shieldAmountForPhase2 > 0)
            {
                if (NPC.localAI[2] < 1)
                {
                    NPC.localAI[2] += 0.02f;
                }
                NPC.dontTakeDamage = true;
            }
            else if(NPC.dontTakeDamage)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(150, 150), DustID.RainbowMk2);
                    d.velocity = d.position.DirectionTo(NPC.Center) * Main.rand.NextFloat(-20, -2);
                    d.color = Color.Purple;
                    d.noGravity = true;
                    d.fadeIn = Main.rand.NextFloat(2);
                }
                for (int i = 0; i < 10; i++)
                {
                    var p = VanillaParticles.RequestPrettySparkleParticle();
                    p.ColorTint = Color.Purple;
                    p.TimeToLive = Main.rand.Next(20, 50);
                    p.Scale = new Vector2(6, 3);
                    p.DrawHorizontalAxis = false;
                    p.LocalPosition = NPC.Center + Main.rand.NextVector2Circular(150, 150);
                    p.Velocity = p.LocalPosition.DirectionTo(NPC.Center) * Main.rand.NextFloat(-20, -6);
                    p.AccelerationPerFrame = -p.Velocity / p.TimeToLive;
                    p.FadeInEnd = 0.1f;
                    p.FadeOutStart = 0.1f;
                    p.Rotation = p.Velocity.ToRotation() + MathHelper.PiOver2;
                    Main.ParticleSystem_World_OverPlayers.Add(p);
                }
                SoundEngine.PlaySound(SoundID.NPCDeath55, NPC.position);
                NPC.defense = 0;
                NPC.dontTakeDamage = false;
            }
        }
        //Main.NewText("AI0: " + NPC.ai[0] + " AI1: " + NPC.ai[1] + " AI2: " + NPC.ai[2] + " AI3: " + NPC.ai[3],Main.DiscoColor);
        if (!NPC.HasValidTarget || !target.ZoneCorrupt)
        {
            NPC.localAI[0]++;
            NPC.TargetClosest(player => !player.ZoneCorrupt);
            if (NPC.localAI[0] == 60 * 5)
            {
                NPC.netUpdate = true;
                _currentAttack = -1;
            }
        }
        else
        {
            NPC.localAI[0] = 0;
        } 
        if(_currentAttack < 0)
        {
            NPC.alpha += 5;
            if(NPC.alpha > 255)
            {
                NPC.active = false;
            }
            return;
        }
        if (_currentAttack < 13 && _currentAttack % (Main.expertMode ? 3 : 4) == 0)
            Teleport();
        else if (NPC.alpha > 0)
        {
            NPC.alpha -= 255 / 15;
        }
        else
        {
            #region Phase 1
            switch (_currentAttack)
            {
                case 1:
                    Boulders();
                    break;
                case 2:
                    SporeBombs();
                    break;
                case 3:
                    VineSpikes();
                    break;
                case 4:
                    SporeBombs();
                    break;
                case 5:
                    VineSpikes();
                    break;
                case 6:
                    Boulders();
                    break;
                case 7:
                    SporeBombs();
                    break;
                case 8:
                    SporeBombs();
                    break;
                case 9:
                    VineSpikes();
                    break;
                case 10:
                    Boulders();
                    break;
                case 11:
                    VineSpikes();
                    break;
            }
            #endregion Phase 1

            #region Phase 2
            if (NPC.dontTakeDamage)
            {
                int weeper = ModContent.NPCType<HiveMindWeeper>();
                int swooper = ModContent.NPCType<HiveMindSwooper>();
                int weeperCount = NPC.CountNPCS(weeper);
                int swooperCount = NPC.CountNPCS(swooper);
                NPC.localAI[1]++;
                if(Main.netMode != NetmodeID.MultiplayerClient && (NPC.localAI[1] == 300 || swooperCount + weeperCount == 0))
                {
                    NPC.localAI[1] = 0;
                    if (weeperCount + swooperCount < 12)
                    {
                        int rand = Main.rand.Next(2, 5);
                        for (int i = 0; i < rand; i++)
                        {
                            if (weeperCount < 4 && Main.expertMode && Main.rand.NextBool(3))
                            {
                                NPC n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextBool(3) && Main.expertMode ? weeper : swooper, NPC.whoAmI, -60 + (i * -30));
                                n.velocity = Main.rand.NextVector2Circular(2, 2);
                                n.velocity.Y -= Main.rand.Next(4, 8);
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                            }
                            else if (swooperCount < 8)
                            {
                                NPC n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextBool(3) && Main.expertMode ? weeper : swooper, NPC.whoAmI, -60 + (i * -30));
                                n.velocity = Main.rand.NextVector2Circular(2, 2);
                                n.velocity.Y -= Main.rand.Next(4, 8);
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                            }
                        }

                        //if (weeperCount < 4 && Main.expertMode && Main.rand.NextBool(3))
                        //{
                        //    NPC n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextBool(3) && Main.expertMode ? weeper : swooper, NPC.whoAmI);
                        //    n.velocity = Main.rand.NextVector2Circular(2, 2);
                        //    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        //}
                        //else if (swooperCount < 12)
                        //{
                        //    NPC n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextBool(3) && Main.expertMode ? weeper : swooper, NPC.whoAmI);
                        //    n.velocity = Main.rand.NextVector2Circular(2, 2);
                        //    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        //}
                    }
                }
            }

            switch (_currentAttack)
            {
                case 21:
                    VineSpikesP2();
                    break;
                case 22:
                    SporeBombsP2();
                    break;
                case 23:
                    SporeBombsP2();
                    break;
                case 24:
                    BouldersP2();
                    break;
                case 25:
                    VineSpikesP2();
                    break;
                case 26:
                    SporeBombsP2();
                    break;
                case 27:
                    BouldersP2();
                    break;
                case 28:
                    VineSpikesP2();
                    break;
                case 29:
                    BouldersP2();
                    break;
                case 30:
                    SporeBombsP2();
                    break;
            }
            #endregion Phase 2
        }
    }
    private bool FindValidSpotsForBoulder(ref Vector2 chosenTile, int targetTileX, int targetTileY, int rangeFromTargetTile = 20, int telefragPreventionDistanceInTiles = 5, bool teleportInAir = false)
    {
        int solidTileCheckFluff = 1;
        int num = (int)target.Center.X / 16;
        int num2 = (int)target.Center.Y / 16;
        int num3 = 0;
        bool flag = false;
        float num4 = 20f;
        if (Math.Abs(num * 16 - targetTileX * 16) + Math.Abs(num2 * 16 - targetTileY * 16) > 2000)
        {
            num3 = 100;
            flag = false;
        }

        while (!flag && num3 < 100)
        {
            num3++;
            int num5 = Main.rand.Next(targetTileX - rangeFromTargetTile, targetTileX + rangeFromTargetTile + 1);
            for (int i = Main.rand.Next(targetTileY - rangeFromTargetTile, targetTileY + rangeFromTargetTile + 1); i < targetTileY + rangeFromTargetTile; i++)
            {
                if ((i >= num2 - 1 && i <= num2 + 1 && num5 >= num - 1 && num5 <= num + 1) || (!teleportInAir && !Main.tile[num5, i].HasUnactuatedTile))
                    continue;

                bool flag2 = true;

                if (!flag2 || (!teleportInAir && !Main.tileSolid[Main.tile[num5, i].TileType]))
                    continue;

                if (!Collision.SolidTiles(num5 - solidTileCheckFluff, num5 + solidTileCheckFluff, i - solidTileCheckFluff, i + solidTileCheckFluff,false))
                    continue;

                Rectangle rectangle = new Rectangle(num5 * 16, i * 16, 16, 16);
                rectangle.Inflate(telefragPreventionDistanceInTiles * 16, telefragPreventionDistanceInTiles * 16);
                for (int j = 0; j < Main.player.Length; j++)
                {
                    Player player = Main.player[j];
                    if (player != null && player.active && !player.DeadOrGhost)
                    {
                        Rectangle value = player.Hitbox;
                        Rectangle value2 = value.Modified((int)(player.velocity.X * num4), (int)(player.velocity.Y * num4), 0, 0);
                        Rectangle.Union(ref value2, ref value, out value2);
                        if (value2.Intersects(rectangle))
                        {
                            flag2 = false;
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag2)
                {
                    chosenTile = new Vector2(num5, i);
                    flag = true;
                }

                break;
            }
        }

        return flag;
    }
    private Vector2 FindBoulderSpot()
    {
        Vector2 chosenTile = Vector2.Zero;
        Vector2 targetPos = target.Center;
        targetPos /= 16;
        if (FindValidSpotsForBoulder(ref chosenTile, (int)targetPos.X, (int)targetPos.Y,30, 15,false))
        {
            chosenTile *= 16;
            targetPos.X = chosenTile.X;
            targetPos.Y = chosenTile.Y;
        }
        else
        {
            targetPos = CVUtils.FindFloorBelow(target.Center, 32);
            targetPos /= 16;
            if (FindValidSpotsForBoulder(ref chosenTile, (int)targetPos.X, (int)targetPos.Y, 50, 15, true))
            {
                chosenTile *= 16;
                targetPos.X = chosenTile.X;
                targetPos.Y = chosenTile.Y;
            }
            else
            {
                targetPos.X = target.Bottom.X;
                targetPos.Y = target.Bottom.Y;
            }
        }
        return targetPos;
    }
    private void SwitchToPhaseTwo()
    {
        SoundEngine.PlaySound(SoundID.Roar, NPC.position);
        _currentAttack = 21;
        _shieldAmountForPhase2 = ShieldMax;

        for (int i = 0; i < 50; i++)
        {
            Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(150, 150), DustID.RainbowMk2);
            d.velocity = d.position.DirectionTo(NPC.Center) * Main.rand.NextFloat(-20, -2);
            d.color = Color.Purple;
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(2);
        }
        for (int i = 0; i < 10; i++)
        {
            var p = VanillaParticles.RequestPrettySparkleParticle();
            p.ColorTint = Color.Purple;
            p.TimeToLive = Main.rand.Next(20, 50);
            p.Scale = new Vector2(6, 3);
            p.DrawHorizontalAxis = false;
            p.LocalPosition = NPC.Center + Main.rand.NextVector2Circular(150, 150);
            p.Velocity = p.LocalPosition.DirectionTo(NPC.Center) * Main.rand.NextFloat(-20, -6);
            p.AccelerationPerFrame = -p.Velocity / p.TimeToLive;
            p.FadeInEnd = 0.1f;
            p.FadeOutStart = 0.1f;
            p.Rotation = p.Velocity.ToRotation() + MathHelper.PiOver2;
            Main.ParticleSystem_World_OverPlayers.Add(p);
        }
    }
    private void CycleAttack()
    {
        NPC.ai[1] = 0;
        NPC.ai[2] = 0;
        if (NPC.life / (float)NPC.lifeMax > _secondPhaseHealthPercent || _currentAttack > 20)
        {
            if (_currentAttack == 11)
                _currentAttack = 1;
            else if (_currentAttack == 30)
                _currentAttack = 21;
            else
                _currentAttack++;
        }
        else
            _currentAttack = 0;
    }
    private void Teleport()
    {
        NPC.ai[1]++;
        ref float teleportX = ref NPC.ai[2];
        ref float teleportY = ref NPC.ai[3];
        int teleportTime = 120;
        NPC.TargetClosest(target => !target.ZoneCorrupt);
        if (NPC.ai[1] == 1)
        {
            Vector2 chosenTile = Vector2.Zero;
            Vector2 targetPos = target.Center;
            targetPos /= 16;
            if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, (int)targetPos.X, (int)targetPos.Y,40))
            {
                chosenTile *= 16;
                teleportX = chosenTile.X;
                teleportY = chosenTile.Y;
            }
            else
            {
                targetPos = CVUtils.FindFloorBelow(target.Center, 32);
                targetPos /= 16;
                if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, (int)targetPos.X, (int)targetPos.Y,40))
                {
                    chosenTile *= 16;
                    teleportX = chosenTile.X;
                    teleportY = chosenTile.Y;
                }
                else
                {
                    teleportX = target.Bottom.X;
                    teleportY = target.Bottom.Y;
                }
            }
            NPC.netUpdate = true;
        }
        else if (NPC.ai[1] < teleportTime)
        {
            if (NPC.ai[1] % 15 == 0)
            {
                var p = VanillaParticles.RequestFadingParticle();
                p.SetBasicInfo(TextureAssets.Extra[ExtrasID.KeybrandRing], null, Vector2.Zero, new Vector2(teleportX, teleportY - 32));
                p.SetTypeInfo(30);
                p.ColorTint = Color.Purple with { A = 64 };
                p.Scale = Vector2.Zero;
                p.ScaleVelocity = new Vector2(Main.rand.NextFloat(0.8f, 1f), Main.rand.NextFloat(0.8f, 1f)) * 0.075f;
                p.Rotation = Main.rand.NextFloatDirection();
                p.FadeInNormalizedTime = 0.5f;
                p.FadeOutNormalizedTime = 0.5f;
                Main.ParticleSystem_World_OverPlayers.Add(p);
            }
            if (NPC.ai[1] > teleportTime - 15)
            {
                NPC.alpha += 255 / 15;
            }
        }
        else if (NPC.ai[1] == teleportTime)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && _currentAttack != 0 /*&& _currentAttack % (Main.expertMode ? 6 : 8) == 0*/)
            {
                int weeper = ModContent.NPCType<HiveMindWeeper>();
                int swooper = ModContent.NPCType<HiveMindSwooper>();
                int weeperCount = NPC.CountNPCS(weeper);
                int swooperCount = NPC.CountNPCS(swooper);
                if (weeperCount + swooperCount < 8)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (weeperCount < 3 && Main.expertMode && Main.rand.NextBool(3))
                        {
                            NPC n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextBool(3) && Main.expertMode ? weeper : swooper, NPC.whoAmI, -60 + (i * -30));
                            n.velocity = Main.rand.NextVector2Circular(2, 2);
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                        else if (swooperCount < 5)
                        {
                            NPC n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextBool(3) && Main.expertMode ? weeper : swooper, NPC.whoAmI, -60 + (i * -30));
                            n.velocity = Main.rand.NextVector2Circular(2, 2);
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                    }
                }
            }
            Vector2 teleportPos = new Vector2(teleportX - NPC.width / 2, teleportY - NPC.height);
            Vector2 teleportVel = NPC.position.DirectionTo(teleportPos);
            if (teleportVel.HasNaNs())
            {
                teleportVel = Vector2.Zero;
            }
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                d.alpha = 128;

                Dust d2 = Dust.NewDustDirect(teleportPos, NPC.width, NPC.height, DustID.Corruption);
                d2.alpha = 128;
            }
            for (int i = 0; i < 50; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.RainbowMk2);
                d.color = Color.Purple;
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(2);
                d.velocity += teleportVel * Main.rand.NextFloat(24);
                Dust d2 = Dust.NewDustDirect(teleportPos, NPC.width, NPC.height, DustID.RainbowMk2);
                d2.color = Color.Purple;
                d2.fadeIn = Main.rand.NextFloat(2);
                d2.noGravity = true;
                d2.velocity += teleportVel * Main.rand.NextFloat(12);
            }
            NPC.position = teleportPos;
            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
        }
        else if (NPC.ai[1] > teleportTime)
        {
            CycleAttack();
            if(NPC.life / (float)NPC.lifeMax < _secondPhaseHealthPercent)
            {
                SwitchToPhaseTwo();
            }
        }
    }
    private void VineSpikes()
    {
        NPC.ai[1]++;
        if (NPC.ai[1] == 100 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            float spacing = Main.expertMode ? Main.rand.Next(220, 240) : Main.rand.Next(250, 280);
            int type = ModContent.ProjectileType<HiveVineSpawner>();
            for (int i = -15; i <= 15; i++)
            {
                if (i == 0)
                    continue;
                Vector2 place = CVUtils.FindFloorBelowIgnoringSolidTops(new Vector2(NPC.Center.X + i * spacing, NPC.Center.Y - 256), 64);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), place, Vector2.Zero, type, 30, 1, -1, -MathF.Abs(i * 5), Main.rand.Next(20, 30));
            }
        }
        else if (NPC.ai[1] > 230)
        {
            CycleAttack();
        }
    }
    private void SporeBombs()
    {
        NPC.ai[1]++;
        if ((NPC.ai[1] is 100 or 120 or 140) && Main.netMode != NetmodeID.MultiplayerClient)
        {
            int type = ModContent.ProjectileType<SporeBomb>();

            Vector2 adjustedTargetPosition = target.Center + new Vector2(target.velocity.X * 120, 0);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, CVUtils.FindVelocityForGravityAffectedThing(NPC.Center, adjustedTargetPosition + Main.rand.NextVector2Circular(128,128), 0.2f, 120), type, 30, 1, -1, 0, Main.rand.Next(10, 20));
            //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.Center.DirectionTo(target.Center).RotatedByRandom(0.35f) * Main.rand.NextFloat(6, 9), type, 30, 1, -1, 0, Main.rand.Next(10, 20));
        }
        else if (NPC.ai[1] > 160)
        {
            CycleAttack();
        }
    }
    private void Boulders()
    {
        NPC.ai[1]++;
        if (NPC.ai[1] is 100 or 140 or 180 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            //Vector2 place = CVUtils.FindFloorBelowIgnoringSolidTops(new Vector2(target.Center.X + (Main.rand.Next(256,400) * (Main.rand.NextBool()? 1 : -1)), target.Center.Y - 128), 64);
            Vector2 place = FindBoulderSpot();
            Point placePoint = place.ToTileCoordinates();
            int rockType = 0;
            if (Main.tile[placePoint].HasTile)
            {
                switch (Main.tile[placePoint].TileType)
                {
                    case TileID.Ebonstone:
                    case TileID.Stone:
                        rockType = 1;
                        break;
                    case TileID.Mud:
                    case TileID.CorruptJungleGrass:
                        rockType = 2;
                        break;
                    case TileID.SnowBlock:
                    case TileID.CorruptIce:
                    case TileID.IceBlock:
                        rockType = 3;
                        break;
                    case TileID.Ebonsand:
                    case TileID.CorruptSandstone:
                    case TileID.CorruptHardenedSand:
                    case TileID.Sand:
                    case TileID.HardenedSand:
                    case TileID.Sandstone:
                        rockType = 4;
                        break;
                }
            }
            Projectile.NewProjectile(NPC.GetSource_FromThis(), place + new Vector2(0,16), Vector2.Zero, ModContent.ProjectileType<CorruptBoulder>(), 40, 1, -1, NPC.target, ai2: rockType);
        }
        else if (NPC.ai[1] > 240)
        {
            CycleAttack();
        }
    }
    private void SporeBombsP2()
    {
        NPC.ai[1]++;
        if ((NPC.ai[1] is 100 or 110 or 120 or 130 or 140 or 150) && Main.netMode != NetmodeID.MultiplayerClient)
        {
            int type = ModContent.ProjectileType<SporeBomb>();

            Vector2 adjustedTargetPosition = target.Center + new Vector2(target.velocity.X * 120, 0);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, CVUtils.FindVelocityForGravityAffectedThing(NPC.Center, adjustedTargetPosition + Main.rand.NextVector2Circular(128, 128), 0.2f, 120), type, 30, 1, -1, 0, Main.rand.Next(10, 20));
            //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.Center.DirectionTo(target.Center).RotatedByRandom(0.35f) * Main.rand.NextFloat(6, 9), type, 30, 1, -1, 0, Main.rand.Next(10, 20));
        }
        //NPC.ai[1]++;
        //if (((NPC.ai[1] is 100 or 130 or 160) || (!NPC.dontTakeDamage && (NPC.ai[1] is 190 or 220))) && Main.netMode != NetmodeID.MultiplayerClient)
        //{
        //    int type = !NPC.dontTakeDamage && Main.rand.NextBool() ? ModContent.ProjectileType<SporeBomb>() : ModContent.ProjectileType<SporeBombLarge>();
        //    int time = 120;
        //    Vector2 adjustedTargetPosition = target.Center + new Vector2(target.velocity.X * time, 0);
        //    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, CVUtils.FindVelocityForGravityAffectedThing(NPC.Center, adjustedTargetPosition + Main.rand.NextVector2Circular(128, 128), 0.2f, time), type, 30, 1, -1, 0, Main.rand.Next(10, 20));
        //}
        else if (NPC.ai[1] > (NPC.dontTakeDamage ? 250 : 200))
        {
            CycleAttack();
        }
    }
    private void BouldersP2()
    {
        NPC.ai[1]++;
        if (NPC.ai[1] is 100 or 140 or 180 or 220 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            Vector2 place = FindBoulderSpot();
            Point placePoint = place.ToTileCoordinates();
            int rockType = 0;
            if (Main.tile[placePoint].HasTile)
            {
                switch (Main.tile[placePoint].TileType)
                {
                    case TileID.Ebonstone:
                    case TileID.Stone:
                        rockType = 1;
                        break;
                    case TileID.Mud:
                    case TileID.CorruptJungleGrass:
                        rockType = 2;
                        break;
                    case TileID.SnowBlock:
                    case TileID.CorruptIce:
                    case TileID.IceBlock:
                        rockType = 3;
                        break;
                    case TileID.Ebonsand:
                    case TileID.CorruptSandstone:
                    case TileID.CorruptHardenedSand:
                    case TileID.Sand:
                    case TileID.HardenedSand:
                    case TileID.Sandstone:
                        rockType = 4;
                        break;
                }
            }
            Projectile.NewProjectile(NPC.GetSource_FromThis(), place + new Vector2(0, 16), Vector2.Zero, ModContent.ProjectileType<CorruptBoulder>(), 40, 1, -1, NPC.target, ai2: rockType);
        }
        else if (NPC.ai[1] > (NPC.dontTakeDamage ? 360 : 280))
        {
            CycleAttack();
        }
    }
    private void VineSpikesP2(bool offset = false)
    {
        NPC.ai[1]++;
        if (NPC.ai[1] == 100 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            float spacing = Main.expertMode ? Main.rand.Next(260, 280) : Main.rand.Next(320, 340);
            if (!NPC.dontTakeDamage)
                spacing *= 0.75f;
            int type = ModContent.ProjectileType<HiveVineSpawner>();
            for (int i = -15; i <= 15; i++)
            {
                if (i == 0)
                    continue;
                Vector2 place = CVUtils.FindFloorBelowIgnoringSolidTops(new Vector2((NPC.Center.X + i * spacing) + (offset? spacing / 2 : 0), NPC.Center.Y - 256), 64);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), place, Vector2.Zero, type, 30, 1, -1, -MathF.Abs(i * 5), Main.rand.Next(20, 30));
            }
        }
        else if (NPC.ai[1] > (NPC.dontTakeDamage ? 330 : 260))
        {
            CycleAttack();
        }
    }
    //private void VineSpikesP2()
    //{
    //    NPC.ai[1]++;
    //    if (NPC.ai[1] % 10 == 0 && NPC.ai[1] < 200 && Main.netMode != NetmodeID.MultiplayerClient)
    //    {
    //        Vector2 place = CVUtils.FindFloorBelowIgnoringSolidTops(new Vector2(target.Center.X + (Main.rand.Next(256, 300) * (Main.rand.NextBool() ? 1 : -1)), target.Center.Y - 32), 64);
    //        Projectile.NewProjectile(NPC.GetSource_FromThis(), place, Vector2.Zero, ModContent.ProjectileType<HiveVineSpawner>(), 30, 1, -1, 0, Main.rand.Next(20, 30));
    //    }
    //    else if (NPC.ai[1] > 260)
    //    {
    //        CycleAttack();
    //    }
    //}
}