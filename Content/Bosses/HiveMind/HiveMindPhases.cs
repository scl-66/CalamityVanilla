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
    private ref float _currentAttack => ref NPC.ai[0];
    public override void AI()
    {
        //Main.NewText("AI0: " + NPC.ai[0] + " AI1: " + NPC.ai[1] + " AI2: " + NPC.ai[2] + " AI3: " + NPC.ai[3],Main.DiscoColor);
        if (!NPC.HasValidTarget || !target.ZoneCorrupt)
        {
            NPC.TargetClosest(player => !player.ZoneCorrupt);
            if (!NPC.HasValidTarget || !target.ZoneCorrupt)
            {
                _currentAttack = -1;
            }
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
        if (_currentAttack % (Main.expertMode ? 3 : 5) == 0)
            Teleport();
        else
        {
            int whichAttack = (int)_currentAttack % 9;
            //whichAttack = 1;
            switch (whichAttack)
            {
                case 0:
                    VineSpikes();
                    break;
                case 1:
                    Boulders();
                    break;
                case 2:
                    SporeBombs();
                    break;
                case 3:
                    Boulders();
                    break;
                case 4:
                    SporeBombs();
                    break;
                case 5:
                    VineSpikes();
                    break;
                case 6:
                    SporeBombs();
                    break;
                case 7:
                    SporeBombs();
                    break;
                case 8:
                    VineSpikes();
                    break;
                case 9:
                    Boulders();
                    break;
                case 10:
                    VineSpikes();
                    break;
            }
        }
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
            if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, (int)targetPos.X, (int)targetPos.Y))
            {
                chosenTile *= 16;
                teleportX = chosenTile.X;
                teleportY = chosenTile.Y;
            }
            else
            {
                targetPos = CVUtils.FindFloorBelow(target.Center, 32);
                targetPos /= 16;
                if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, (int)targetPos.X, (int)targetPos.Y))
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
            if (NPC.ai[1] > teleportTime - 5)
            {
                NPC.alpha += 255 / 5;
            }
        }
        else if (NPC.ai[1] == teleportTime)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && _currentAttack != 0 && _currentAttack % (Main.expertMode ? 6 : 10) == 0)
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
                d.velocity += teleportVel * Main.rand.NextFloat(12);
                Dust d2 = Dust.NewDustDirect(teleportPos, NPC.width, NPC.height, DustID.RainbowMk2);
                d2.color = Color.Purple;
                d2.fadeIn = Main.rand.NextFloat(2);
                d2.noGravity = true;
                d2.velocity += teleportVel * Main.rand.NextFloat(12);
            }
            NPC.position = teleportPos;
            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
        }
        else if (NPC.ai[1] > teleportTime && NPC.alpha <= 0)
        {
            NPC.alpha = 0;
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            _currentAttack++;
        }
        else
        {
            NPC.alpha -= 255 / 5;
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
            NPC.alpha = 0;
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            _currentAttack++;
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
            NPC.alpha = 0;
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            _currentAttack++;
        }
    }
    private void Boulders()
    {
        NPC.ai[1]++;
        if (NPC.ai[1] is 100 or 140 or 180 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            Vector2 place = CVUtils.FindFloorBelowIgnoringSolidTops(new Vector2(target.Center.X + (Main.rand.Next(256,400) * (Main.rand.NextBool()? 1 : -1)), target.Center.Y - 32), 32);
            Point placePoint = place.ToTileCoordinates();
            int rockType = 0;
            if (Main.tile[placePoint].HasTile)
            {
                switch (Main.tile[placePoint].TileType)
                {
                    case TileID.Stone:
                    case TileID.Ebonstone:
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
            Projectile.NewProjectile(NPC.GetSource_FromThis(), place, new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-6, -3)), ModContent.ProjectileType<CorruptBoulder>(), 40, 1, -1, NPC.target, ai2: rockType);
        }
        else if (NPC.ai[1] > 180)
        {
            NPC.alpha = 0;
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            _currentAttack++;
        }
    }
}