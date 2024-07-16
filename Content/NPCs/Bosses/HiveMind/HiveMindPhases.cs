using CalamityVanilla.Content.Projectiles;
using CalamityVanilla.Content.Projectiles.Hostile;
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
using Terraria.ModLoader.Config;
using Terraria.Utilities.Terraria.Utilities;

namespace CalamityVanilla.Content.NPCs.Bosses.HiveMind
{
    public partial class HiveMind
    {
        int currentPhase = 0;
        private void Teleport()
        {
            NPC.ai[0]++;

            if (NPC.ai[0] > 0)
            {
                NPC.alpha += 5;
            }
            if (NPC.ai[0] == (255 / 5))
            {
                NPC.TargetClosest();
                Vector2 chosenTile = Vector2.Zero;
                Point targetPos = ((target.Center + (target.velocity * 64)) / 16).ToPoint();
                if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, targetPos.X, targetPos.Y))
                {
                    chosenTile *= 16;
                    NPC.position = new Vector2(chosenTile.X - NPC.width / 2, chosenTile.Y - NPC.height);
                }
                else
                {
                    NPC.Bottom = target.Bottom;
                }
            }
            if (NPC.ai[0] > (255 / 5) && NPC.alpha > 0)
            {
                NPC.alpha -= 10;
            }
            if (NPC.ai[0] == (512 / 5))
            {
                phase = (byte)Main.rand.Next(1, 3);
                //phase = 3;
                NPC.ai[0] = 0;
                NPC.netUpdate = true;
                currentPhase = 1;
            }
        }
        private void ShootSporeBombs()
        {
            NPC.ai[0]++;
            if (NPC.ai[0] > 40 && NPC.ai[1] < 3)
            {
                NPC.ai[0] = 20;
                NPC.ai[1]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.Center.DirectionTo(target.Center).RotatedByRandom(0.1f) * Main.rand.NextFloat(6, 12), ModContent.ProjectileType<SporeBomb>(), 25, 1, -1, Main.rand.NextFloat(-1f, 1f));
            }

            if (NPC.ai[1] == 3 && NPC.ai[0] > 80)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                switch (currentPhase)
                {
                    case 1:
                        currentPhase = 2;
                        phase = (byte)Main.rand.Next(1, 3);
                        break;
                    case 2:
                        currentPhase = 3;
                        phase = 3;
                        break;
                    case 3:
                        currentPhase = 0;
                        phase = 0;
                        break;
                }
            }
        }
        private void VineAttack()
        {
            NPC.ai[0]++;
            if (NPC.ai[0] > 40 && NPC.ai[1] < 3)
            {
                NPC.ai[0] = 20;

                NPC.ai[2] = Main.rand.NextFloatDirection() * 64 * Main.rand.Next(6, 12);
                NPC.netUpdate = true;

                if (NPC.ai[1] == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item8, CVUtils.FindRestingSpot(target.Center) + new Vector2(0, -16));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), CVUtils.FindRestingSpot(target.Center) + new Vector2(0, -16), Vector2.Zero, ModContent.ProjectileType<HiveVine>(), 25, 1, -1, 0, Main.rand.Next(16, 26));
                    }
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item8, CVUtils.FindRestingSpot(target.Center) + new Vector2(NPC.ai[2], -16));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), CVUtils.FindRestingSpot(target.Center) + new Vector2(NPC.ai[2], -16), Vector2.Zero, ModContent.ProjectileType<HiveVine>(), 25, 1, -1, 0, Main.rand.Next(16, 26));
                    }
                }

                NPC.ai[1]++;
            }

            if (NPC.ai[1] == 3 && NPC.ai[0] > 80)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                switch (currentPhase)
                {
                    case 1:
                        currentPhase = 2;
                        phase = (byte)Main.rand.Next(1, 3);
                        break;
                    case 2:
                        currentPhase = 3;
                        phase = 3;
                        break;
                    case 3:
                        currentPhase = 0;
                        phase = 0;
                        break;
                }
            }
        }

        bool minionSummoned = false;
        List<NPC> HiveMindMinions = new List<NPC> {};

        private void SpawnMinions()
        {
            NPC.ai[0]++;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Because we want to spawn minions, and minions are NPCs, we have to do this on the server (or singleplayer, "!= NetmodeID.MultiplayerClient" covers both)
                // This means we also have to sync it after we spawned and set up the minion
                return;
            }

            if (NPC.ai[0] > 20 && !minionSummoned)
            {
                SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

                for (int i = 0; i < 5; i++)
                {
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Corruption);
                    d.velocity = Main.rand.NextVector2Unit((float)-MathHelper.PiOver4, (float)-MathHelper.PiOver2) * Main.rand.NextFloat(2f, 4f);
                }

                int spawnAmount = Main.rand.Next(2,5);
                for (int i = 0; i < spawnAmount; i++)
                {
                    float rotAmount = 1f;
                    Vector2 spawnOffset = new Vector2(0, -1f).RotatedBy((rotAmount / spawnAmount * i) - rotAmount / 2) * Main.rand.NextFloat(3f,7f);
                    
                    if (Main.rand.Next (1,4) != 1)
                    {
                        NPC minnon = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X + (int)spawnOffset.X, (int)NPC.Center.Y + (int)spawnOffset.Y, ModContent.NPCType<HiveMindSwooper>());
                        // Optional parameters allow for specifying a range of rotations. In this example, the start rotation is  MathHelper.Pi / 4 and it can be up to MathHelper.Pi / 2 more than that.
                        minnon.velocity = spawnOffset;
                        minnon.Opacity = 0f;
                        HiveMindMinions.Add(minnon);
                    }
                    else
                    {
                        NPC minnon = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X + (int)spawnOffset.X, (int)NPC.Center.Y + (int)spawnOffset.Y, ModContent.NPCType<HiveMindWeeper>());
                        // Optional parameters allow for specifying a range of rotations. In this example, the start rotation is  MathHelper.Pi / 4 and it can be up to MathHelper.Pi / 2 more than that.
                        minnon.velocity = spawnOffset;
                        minnon.Opacity = 0f;
                        HiveMindMinions.Add(minnon);
                    }
                }
                minionSummoned = true;
                NPC.dontTakeDamage = true;
            }

            for (int i = 0; i < HiveMindMinions.Count(); i++)
            {
                if (!HiveMindMinions[i].active) HiveMindMinions.Remove(HiveMindMinions[i]);
            }

            if (HiveMindMinions.Count() <= 0 && NPC.ai[0] > 30)
            {
                NPC.ai[1]++;
                NPC.dontTakeDamage = false;
            } 

            if (HiveMindMinions.Count() <= 0 && NPC.ai[1] > 60)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                switch (currentPhase)
                {
                    case 1:
                        currentPhase = 2;
                        phase = (byte)Main.rand.Next(1, 3);
                        break;
                    case 2:
                        currentPhase = 3;
                        phase = 3;
                        break;
                    case 3:
                        currentPhase = 0;
                        phase = 0;
                        break;
                }
                minionSummoned = false;
                HiveMindMinions = new List<NPC> { };
            }
        }
    }
}
